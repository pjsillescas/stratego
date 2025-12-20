package com.pdrosoft.matchmaking.stratego.service;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Objects;
import java.util.Optional;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.pdrosoft.matchmaking.dao.GameDAO;
import com.pdrosoft.matchmaking.dao.PlayerDAO;
import com.pdrosoft.matchmaking.dto.PlayerDTO;
import com.pdrosoft.matchmaking.exception.MatchmakingValidationException;
import com.pdrosoft.matchmaking.model.Game;
import com.pdrosoft.matchmaking.model.Player;
import com.pdrosoft.matchmaking.model.StrategoMovement;
import com.pdrosoft.matchmaking.model.StrategoStatus;
import com.pdrosoft.matchmaking.repository.GameRepository;
import com.pdrosoft.matchmaking.repository.PlayerRepository;
import com.pdrosoft.matchmaking.repository.StrategoMovementRepository;
import com.pdrosoft.matchmaking.repository.StrategoStatusRepository;
import com.pdrosoft.matchmaking.stratego.dto.ArmySetupDTO;
import com.pdrosoft.matchmaking.stratego.dto.BoardTileDTO;
import com.pdrosoft.matchmaking.stratego.dto.GameStateDTO;
import com.pdrosoft.matchmaking.stratego.dto.StrategoMovementDTO;
import com.pdrosoft.matchmaking.stratego.enums.GamePhase;
import com.pdrosoft.matchmaking.stratego.enums.Rank;

import jakarta.validation.Valid;
import lombok.NonNull;

@Service
public class StrategoServiceImpl implements StrategoService {

	@NonNull
	private final GameDAO gameDao;
	@NonNull
	private final GameRepository gameRepository;
	@NonNull
	private final PlayerRepository playerRepository;
	@NonNull
	private final PlayerDAO playerDao;
	@NonNull
	private final PasswordEncoder passwordEncoder;
	@NonNull
	private final StrategoStatusRepository strategoStatusRepository;
	@NonNull
	private final StrategoMovementRepository strategoMovementRepository;

	public StrategoServiceImpl(@Autowired GameDAO gameDao, @Autowired PlayerRepository playerRepository,
			PlayerDAO playerDao, PasswordEncoder passwordEncoder, StrategoStatusRepository strategoStatusRepository,
			GameRepository gameRepository, StrategoMovementRepository strategoMovementRepository) {
		this.gameDao = gameDao;
		this.playerRepository = playerRepository;
		this.playerDao = playerDao;
		this.passwordEncoder = passwordEncoder;
		this.strategoStatusRepository = strategoStatusRepository;
		this.gameRepository = gameRepository;
		this.strategoMovementRepository = strategoMovementRepository;
	}

	private PlayerDTO toPlayerDTO(Player player) {
		return PlayerDTO.builder().id(player.getId()).username(player.getUserName()).build();

	}

	private List<List<BoardTileDTO>> getEmptyBoard() {
		var disa = BoardTileDTO.builder().rank(Rank.DISABLED).build();
		var row1 = new ArrayList<BoardTileDTO>(
				Arrays.asList(null, null, null, null, null, null, null, null, null, null));
		var row2 = new ArrayList<BoardTileDTO>(
				Arrays.asList(null, null, null, null, null, null, null, null, null, null));
		var row3 = new ArrayList<BoardTileDTO>(
				Arrays.asList(null, null, null, null, null, null, null, null, null, null));
		var row4 = new ArrayList<BoardTileDTO>(
				Arrays.asList(null, null, null, null, null, null, null, null, null, null));
		var row5 = new ArrayList<BoardTileDTO>(
				Arrays.asList(null, null, disa, disa, null, null, disa, disa, null, null));
		var row6 = new ArrayList<BoardTileDTO>(
				Arrays.asList(null, null, disa, disa, null, null, disa, disa, null, null));
		var row7 = new ArrayList<BoardTileDTO>(
				Arrays.asList(null, null, null, null, null, null, null, null, null, null));
		var row8 = new ArrayList<BoardTileDTO>(
				Arrays.asList(null, null, null, null, null, null, null, null, null, null));
		var row9 = new ArrayList<BoardTileDTO>(
				Arrays.asList(null, null, null, null, null, null, null, null, null, null));
		var row10 = new ArrayList<BoardTileDTO>(
				Arrays.asList(null, null, null, null, null, null, null, null, null, null));

		var board = new ArrayList<List<BoardTileDTO>>(
				Arrays.asList(row1, row2, row3, row4, row5, row6, row7, row8, row9, row10));

		return board;
	}

	private void copySetup(ArmySetupDTO setupDto, List<List<BoardTileDTO>> board, int offset, boolean isHost) {
		for (int iRow = 3; iRow >= 0; iRow--) {
			var setupRow = setupDto.getArmy().get(iRow);
			var boardRow = board.get(offset + iRow);

			for (int iCol = 0; iCol < 10; iCol++) {
				var tile = BoardTileDTO.builder().rank(setupRow.get(iCol)).isHostOwner(isHost).build();
				boardRow.set(iCol, tile);
			}
		}
	}

	private StrategoStatus getNewStrategoGame(Game game) {
		var status = new StrategoStatus();

		status.setBoard(getEmptyBoard());
		status.setGame(game);
		status.setIsGuestTurn(false);
		status.setIsHostInitialized(false);
		status.setIsGuestInitialized(false);

		return strategoStatusRepository.save(status);
	}

	private boolean isPlayerId(Integer playerId, Player player2) {
		return Optional.ofNullable(player2).map(Player::getId).filter(id -> id == playerId).isPresent();
	}

	@Override
	@Transactional(rollbackFor = Exception.class)
	public GameStateDTO addSetup(Long gameId, Player player, @Valid ArmySetupDTO setupDto) {

		var game = gameRepository.findById(gameId)
				.orElseThrow(() -> new MatchmakingValidationException("Game does not exist"));

		List<List<BoardTileDTO>> board = null;
		var status = strategoStatusRepository.findByGameId(gameId).orElseGet(() -> getNewStrategoGame(game));
		board = status.getBoard();

		var isHost = isPlayerId(player.getId(), game.getHost());
		var isGuest = isPlayerId(player.getId(), game.getGuest());

		if (isHost && !status.getIsHostInitialized()) {
			status.setIsHostInitialized(true);
			copySetup(setupDto, board, 0, true);
		} else if (isGuest && !status.getIsGuestInitialized()) {
			status.setIsGuestInitialized(true);
			copySetup(setupDto, board, 7, false);
		} else {
			throw new MatchmakingValidationException("Invalid player setup");
		}

		status.setBoard(board);

		if (GamePhase.WAITING_FOR_SETUP_1_PLAYER.equals(game.getPhase())) {
			game.setPhase(GamePhase.PLAYING);
		} else if (GamePhase.WAITING_FOR_SETUP_2_PLAYERS.equals(game.getPhase())) {
			game.setPhase(GamePhase.WAITING_FOR_SETUP_1_PLAYER);
		} else {
			throw new MatchmakingValidationException("Game not in setup state");
		}

		strategoStatusRepository.save(status);
		return GameStateDTO.builder() //
				.currentPlayer(toPlayerDTO(player)) //
				.gameId(gameId) //
				.phase(game.getPhase()) //
				.movement(null) //
				.board(board) //
				.isMyTurn(isHost) //
				.build();
	}

	private boolean getIsMyTurn(Integer playerId, Game game, StrategoStatus status) {
		var isHost = isPlayerId(playerId, game.getHost());
		var isGuest = isPlayerId(playerId, game.getGuest());

		return isHost && !status.getIsGuestTurn() || isGuest && status.getIsGuestTurn();
	}

	private void checkValidMovement(StrategoMovementDTO movementDto, List<List<BoardTileDTO>> board, Game game,
			StrategoStatus status, Integer playerId) {

		var isMyTurn = getIsMyTurn(playerId, game, status);
		if (!isMyTurn) {
			throw new MatchmakingValidationException("Invalid movement");
		}

		var isHost = isPlayerId(playerId, game.getHost());
		var initialTile = board.get(movementDto.getRowInitial()).get(movementDto.getColInitial());
		if (initialTile == null || initialTile.isHostOwner() != isHost || Rank.DISABLED.equals(initialTile.getRank())) {
			throw new MatchmakingValidationException("Invalid chosen square");
		}

		if (isInmobileRank(initialTile.getRank())) {
			throw new MatchmakingValidationException("This square cannot move");
		}

		var finalTile = board.get(movementDto.getRowFinal()).get(movementDto.getColFinal());
		if (finalTile != null && (finalTile.isHostOwner() == isHost || Rank.DISABLED.equals(finalTile.getRank()))) {
			throw new MatchmakingValidationException("Invalid destination square");
		}

		if (finalTile != null) {
			var result = compareRanks(initialTile.getRank(), finalTile.getRank());

			if (result < 0) {
				// player lost, destination tile stays
				setBoardPosition(board, movementDto.getRowInitial(), movementDto.getColInitial(), null);
				;
			} else if (result == 0) {
				// Tie, both squares are deleted
				setBoardPosition(board, movementDto.getRowInitial(), movementDto.getColInitial(), null);
				setBoardPosition(board, movementDto.getRowFinal(), movementDto.getColFinal(), null);
			} else { // result > 0
				// player won
				setBoardPosition(board, movementDto.getRowInitial(), movementDto.getColInitial(), null);
				setBoardPosition(board, movementDto.getRowFinal(), movementDto.getColFinal(), initialTile);
			}
		}
	}

	private boolean isInmobileRank(Rank rank) {
		var inmobileRanks = List.of(Rank.BOMB, Rank.DISABLED, Rank.FLAG);
		return inmobileRanks.contains(rank);
	}

	private int compareRanks(Rank rankAttacker, Rank rankDefender) {
		if (Objects.equals(rankAttacker, rankDefender)) {
			return 0;
		}

		List<Rank> upperRanks = List.of();
		switch (rankDefender) {
		case FLAG:
			upperRanks = Arrays.asList(Rank.values());
			break;
		case BOMB:
			upperRanks = List.of(Rank.MINER);
			break;
		case SPY:
			upperRanks = List.of();
			break;
		case MARSHAL:
			upperRanks = List.of(Rank.SPY);
			break;
		case GENERAL:
			upperRanks = List.of(Rank.MARSHAL);
			break;
		case COLONEL:
			upperRanks = List.of(Rank.MARSHAL, Rank.GENERAL);
			break;
		case MAJOR:
			upperRanks = List.of(Rank.MARSHAL, Rank.GENERAL, Rank.COLONEL);
			break;
		case CAPTAIN:
			upperRanks = List.of(Rank.MARSHAL, Rank.GENERAL, Rank.COLONEL, Rank.MAJOR);
			break;
		case LIEUTENANT:
			upperRanks = List.of(Rank.MARSHAL, Rank.GENERAL, Rank.COLONEL, Rank.MAJOR, Rank.CAPTAIN);
			break;
		case SERGEANT:
			upperRanks = List.of(Rank.MARSHAL, Rank.GENERAL, Rank.COLONEL, Rank.MAJOR, Rank.CAPTAIN, Rank.LIEUTENANT);
			break;
		case MINER:
			upperRanks = List.of(Rank.MARSHAL, Rank.GENERAL, Rank.COLONEL, Rank.MAJOR, Rank.CAPTAIN, Rank.LIEUTENANT,
					Rank.SERGEANT);
			break;
		case SCOUT:
			upperRanks = List.of(Rank.MARSHAL, Rank.GENERAL, Rank.COLONEL, Rank.MAJOR, Rank.CAPTAIN, Rank.LIEUTENANT,
					Rank.SERGEANT, Rank.MINER);
			break;
		default:
			throw new MatchmakingValidationException("Invalid Ranks compared");
		}

		return upperRanks.contains(rankAttacker) ? 1 : -1;
	}

	private void setBoardPosition(List<List<BoardTileDTO>> board, Integer row, Integer col, BoardTileDTO tile) {
		board.get(row).set(col, tile);
	}

	private void applyMovement(StrategoMovementDTO movement, List<List<BoardTileDTO>> board, boolean isHost) {
		setBoardPosition(board, movement.getRowInitial(), movement.getColInitial(), null);
		var tile = BoardTileDTO.builder().rank(movement.getRank()).isHostOwner(isHost).build();
		setBoardPosition(board, movement.getRowFinal(), movement.getColFinal(), tile);
	}

	@Override
	@Transactional(rollbackFor = Exception.class)
	public GameStateDTO addMovement(Long gameId, Player player, @Valid StrategoMovementDTO movementDto) {

		var game = gameRepository.findById(gameId)
				.orElseThrow(() -> new MatchmakingValidationException("Game does not exist"));

		if (!GamePhase.PLAYING.equals(game.getPhase())) {
			throw new MatchmakingValidationException("Game not in playing state");
		}

		List<List<BoardTileDTO>> board = null;
		var status = strategoStatusRepository.findByGameId(gameId)
				.orElseThrow(() -> new MatchmakingValidationException("Game has not been started"));
		board = status.getBoard();

		checkValidMovement(movementDto, board, game, status, player.getId());

		// var isHost = isPlayerId(player.getId(), game.getHost());
		// applyMovement(movementDto, board, isHost);

		status.setBoard(board);
		var isGuestTurn = status.getIsGuestTurn();
		status.setIsGuestTurn(!isGuestTurn);
		strategoStatusRepository.save(status);

		var move = new StrategoMovement();
		move.setGame(game);
		move.setRank(movementDto.getRank());
		move.setRowInitial(movementDto.getRowInitial());
		move.setColInitial(movementDto.getColInitial());
		move.setRowFinal(movementDto.getRowFinal());
		move.setColFinal(movementDto.getColFinal());
		move.setIsGuestTurn(isGuestTurn);

		strategoMovementRepository.save(move);

		return GameStateDTO.builder() //
				.currentPlayer(toPlayerDTO(player)) //
				.gameId(gameId) //
				.phase(game.getPhase()) //
				.movement(movementDto) //
				.board(board) //
				.isMyTurn(false) //
				.build();
	}

	@Override
	@Transactional(rollbackFor = Exception.class)
	public GameStateDTO checkRank(Long gameId, Player player, @Valid StrategoMovementDTO movementDto) {

		var game = gameRepository.findById(gameId)
				.orElseThrow(() -> new MatchmakingValidationException("Game does not exist"));

		if (!GamePhase.PLAYING.equals(game.getPhase())) {
			throw new MatchmakingValidationException("Game not in playing state");
		}

		List<List<BoardTileDTO>> board = null;
		var status = strategoStatusRepository.findByGameId(gameId)
				.orElseThrow(() -> new MatchmakingValidationException("Game has not been started"));
		board = status.getBoard();

		checkValidMovement(movementDto, board, game, status, player.getId());

		var isHost = isPlayerId(player.getId(), game.getHost());
		applyMovement(movementDto, board, isHost);

		status.setBoard(board);
		var isGuestTurn = status.getIsGuestTurn();
		status.setIsGuestTurn(!isGuestTurn);
		strategoStatusRepository.save(status);

		var move = new StrategoMovement();
		move.setGame(game);
		move.setRank(movementDto.getRank());
		move.setRowInitial(movementDto.getRowInitial());
		move.setColInitial(movementDto.getColInitial());
		move.setRowFinal(movementDto.getRowFinal());
		move.setColFinal(movementDto.getColFinal());
		move.setIsGuestTurn(isGuestTurn);

		strategoMovementRepository.save(move);

		return GameStateDTO.builder() //
				.currentPlayer(toPlayerDTO(player)) //
				.gameId(gameId) //
				.phase(game.getPhase()) //
				.movement(movementDto) //
				.board(board) //
				.isMyTurn(false) //
				.build();
	}

	private StrategoMovementDTO toMovementDTO(StrategoMovement movement) {
		return StrategoMovementDTO.builder() //
				.rank(movement.getRank()) //
				.rowInitial(movement.getRowInitial()) //
				.rowFinal(movement.getRowFinal()) //
				.colInitial(movement.getColInitial()) //
				.colFinal(movement.getColFinal()) //
				.build();
	}

	@Override
	public GameStateDTO getStatus(Long gameId, Player player) {
		var game = gameRepository.findById(gameId)
				.orElseThrow(() -> new MatchmakingValidationException("Game does not exist"));

		if (!GamePhase.PLAYING.equals(game.getPhase())) {
			throw new MatchmakingValidationException("Game not in playing state");
		}

		List<List<BoardTileDTO>> board = null;
		var status = strategoStatusRepository.findByGameId(gameId)
				.orElseThrow(() -> new MatchmakingValidationException("Game has not been started"));
		board = status.getBoard();

		// var movement = strategoMovementRepository.findAllByGameId(gameId).getLast();
		var allMovements = strategoMovementRepository.findAllByGameId(gameId);
		var movement = Optional.ofNullable((allMovements.size() == 0) ? null : allMovements.getLast());

		return GameStateDTO.builder() //
				.currentPlayer(toPlayerDTO(player)) //
				.gameId(gameId) //
				.phase(game.getPhase()) //
				.movement(movement.map(this::toMovementDTO).orElse(null)) //
				.board(board) //
				.isMyTurn(false) //
				.build();
	}

}
