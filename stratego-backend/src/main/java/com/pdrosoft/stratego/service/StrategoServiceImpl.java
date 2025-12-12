package com.pdrosoft.stratego.service;

import java.time.Instant;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Optional;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.pdrosoft.matchmaking.dao.GameDAO;
import com.pdrosoft.matchmaking.dao.PlayerDAO;
import com.pdrosoft.matchmaking.dto.GameDTO;
import com.pdrosoft.matchmaking.dto.GameExtendedDTO;
import com.pdrosoft.matchmaking.dto.GameInputDTO;
import com.pdrosoft.matchmaking.dto.PlayerDTO;
import com.pdrosoft.matchmaking.exception.PlayerExistsException;
import com.pdrosoft.matchmaking.model.Player;
import com.pdrosoft.matchmaking.repository.PlayerRepository;
import com.pdrosoft.matchmaking.repository.StrategoStatusRepository;
import com.pdrosoft.matchmaking.security.payload.MatchmakingUserDetails;
import com.pdrosoft.stratego.dto.ArmySetupDTO;
import com.pdrosoft.stratego.dto.GameStateDTO;
import com.pdrosoft.stratego.dto.StrategoMovementDTO;
import com.pdrosoft.stratego.enums.Rank;

import jakarta.validation.Valid;
import lombok.NonNull;

@Service
public class StrategoServiceImpl implements StrategoService {

	@NonNull
	private final GameDAO gameDao;
	@NonNull
	private final PlayerRepository playerRepository;
	@NonNull
	private final PlayerDAO playerDao;
	@NonNull
	private final PasswordEncoder passwordEncoder;
	@NonNull
	private final StrategoStatusRepository strategoStatusRepository;

	public StrategoServiceImpl(@Autowired GameDAO gameDao, @Autowired PlayerRepository playerRepository,
			PlayerDAO playerDao, PasswordEncoder passwordEncoder, StrategoStatusRepository strategoStatusRepository) {
		this.gameDao = gameDao;
		this.playerRepository = playerRepository;
		this.playerDao = playerDao;
		this.passwordEncoder = passwordEncoder;
		this.strategoStatusRepository = strategoStatusRepository;
	}

	private PlayerDTO toPlayerDTO(Player player) {
		return PlayerDTO.builder().id(player.getId()).username(player.getUserName()).build();

	}

	@Override
	@Transactional(rollbackFor = Exception.class)
	public PlayerDTO addPlayer(String name, String password) {
		var playerOpt = playerDao.findPlayersByName(name);

		if (playerOpt.isPresent()) {
			throw new PlayerExistsException("player already exists '%s'".formatted(name));
		}

		var player = new Player();
		player.setUserName(name);
		player.setPassword(passwordEncoder.encode(password));

		return Optional.ofNullable(playerRepository.save(player)).map(this::toPlayerDTO).orElseThrow();
	}

	private List<List<Rank>> getEmptyBoard() {
		var disa = Rank.DISABLED;
		var row1 =  new ArrayList<Rank>(Arrays.asList(null, null, null, null, null, null, null, null, null, null));
		var row2 =  new ArrayList<Rank>(Arrays.asList(null, null, null, null, null, null, null, null, null, null));
		var row3 =  new ArrayList<Rank>(Arrays.asList(null, null, null, null, null, null, null, null, null, null));
		var row4 =  new ArrayList<Rank>(Arrays.asList(null, null, null, null, null, null, null, null, null, null));
		var row5 =  new ArrayList<Rank>(Arrays.asList(null, null, disa, disa, null, null, disa, disa, null, null));
		var row6 =  new ArrayList<Rank>(Arrays.asList(null, null, disa, disa, null, null, disa, disa, null, null));
		var row7 =  new ArrayList<Rank>(Arrays.asList(null, null, null, null, null, null, null, null, null, null));
		var row8 =  new ArrayList<Rank>(Arrays.asList(null, null, null, null, null, null, null, null, null, null));
		var row9 =  new ArrayList<Rank>(Arrays.asList(null, null, null, null, null, null, null, null, null, null));
		var row10 = new ArrayList<Rank>(Arrays.asList(null, null, null, null, null, null, null, null, null, null));
		
		var board = new ArrayList<List<Rank>>(Arrays.asList(row1, row2, row3, row4, row5, row6, row7, row8, row9, row10));
		
		return board;
	}

	@Override
	@Transactional(rollbackFor = Exception.class)
	public GameStateDTO addSetup(Long gameId, MatchmakingUserDetails userDetails, @Valid ArmySetupDTO setupDto) {
		var player = userDetails.getPlayer();

		var game = gameDao.getGameById(gameId);

		var board = getEmptyBoard();
		if (game.getHost() != null && game.getHost().getId() == player.getId()) {
			;
		} else if (game.getGuest() != null && game.getGuest().getId() == player.getId()) {
			var status = strategoStatusRepository.findByGameId(gameId);
		}

		return GameStateDTO.builder() //
				.currentPlayer(toPlayerDTO(player)) //
				.gameId(gameId) //
				.phase(game.getPhase()) //
				.movement(null) //
				.board(board) //
				.build();
	}

	@Override
	@Transactional(rollbackFor = Exception.class)
	public GameStateDTO addMovement(Player player, @Valid StrategoMovementDTO movementDto) {
		return null;
	}

}
