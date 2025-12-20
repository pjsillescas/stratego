package com.pdrosoft.matchmaking.stratego.service;

import static org.assertj.core.api.Assertions.assertThat;
import static org.assertj.core.api.Assertions.assertThatThrownBy;

import java.time.Instant;
import java.util.List;
import java.util.Optional;
import java.util.stream.Stream;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.junit.jupiter.api.extension.ExtensionContext;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.Arguments;
import org.junit.jupiter.params.provider.ArgumentsProvider;
import org.junit.jupiter.params.provider.ArgumentsSource;
import org.junit.jupiter.params.provider.EnumSource;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.Mockito;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.security.crypto.password.PasswordEncoder;

import com.pdrosoft.matchmaking.dao.GameDAO;
import com.pdrosoft.matchmaking.dao.PlayerDAO;
import com.pdrosoft.matchmaking.exception.MatchmakingValidationException;
import com.pdrosoft.matchmaking.model.Game;
import com.pdrosoft.matchmaking.model.Player;
import com.pdrosoft.matchmaking.model.StrategoMovement;
import com.pdrosoft.matchmaking.model.StrategoStatus;
import com.pdrosoft.matchmaking.repository.GameRepository;
import com.pdrosoft.matchmaking.repository.PlayerRepository;
import com.pdrosoft.matchmaking.repository.StrategoMovementRepository;
import com.pdrosoft.matchmaking.repository.StrategoStatusRepository;
import com.pdrosoft.matchmaking.stratego.dto.BoardTileDTO;
import com.pdrosoft.matchmaking.stratego.dto.GameStateDTO;
import com.pdrosoft.matchmaking.stratego.dto.StrategoMovementDTO;
import com.pdrosoft.matchmaking.stratego.enums.GamePhase;
import com.pdrosoft.matchmaking.stratego.enums.Rank;

@ExtendWith(MockitoExtension.class)
public class StrategoServiceTest {

	private static final Integer PLAYER_ID = 1;
	private static final String PLAYER_USERNAME = "player";
	private static final Integer HOST_ID = 2;
	private static final Integer GUEST_ID = 3;
	private static final Long GAME_ID = 10L;

	@Mock
	private GameDAO gameDao;
	@Mock
	private GameRepository gameRepository;
	@Mock
	private PlayerRepository playerRepository;
	@Mock
	private PlayerDAO playerDao;
	@Mock
	private PasswordEncoder passwordEncoder;
	@Mock
	private StrategoStatusRepository strategoStatusRepository;
	@Mock
	private StrategoMovementRepository strategoMovementRepository;

	@InjectMocks
	private StrategoServiceImpl strategoService;

	@Test
	void testGetStatusNoGame() {
		var player = getTestPlayer();

		Mockito.when(gameRepository.findById(GAME_ID)).thenReturn(Optional.empty());

		assertThatThrownBy(() -> strategoService.getStatus(GAME_ID, player))
				.isInstanceOf(MatchmakingValidationException.class).hasMessage("Game does not exist");
	}

	@ParameterizedTest
	@EnumSource(value = GamePhase.class, names = { "WAITING_FOR_SETUP_2_PLAYERS", "WAITING_FOR_SETUP_1_PLAYER",
			"FINISHED" })
	void testGetStatusWrongPlayerTurn(GamePhase wrongPhase) {
		var player = getTestPlayer();
		var game = getTestGame(player, player);
		game.setPhase(wrongPhase);

		Mockito.when(gameRepository.findById(GAME_ID)).thenReturn(Optional.of(game));

		assertThatThrownBy(() -> strategoService.getStatus(GAME_ID, player))
				.isInstanceOf(MatchmakingValidationException.class).hasMessage("Game not in playing state");
	}

	@Test
	void testGetStatusNoStatus() {
		var player = getTestPlayer();
		var game = getTestGame(player, player);

		Mockito.when(gameRepository.findById(GAME_ID)).thenReturn(Optional.of(game));

		Mockito.when(strategoStatusRepository.findByGameId(GAME_ID)).thenReturn(Optional.empty());

		assertThatThrownBy(() -> strategoService.getStatus(GAME_ID, player))
				.isInstanceOf(MatchmakingValidationException.class).hasMessage("Game has not been started");
	}

	private static class MovementArgs implements ArgumentsProvider {
		@Override
		public Stream<? extends Arguments> provideArguments(ExtensionContext context) {
			return Stream.of(Arguments.of(List.of()), Arguments.of(List.of(getTestMovement())));
		}
	}

	@SuppressWarnings("unchecked")
	@ParameterizedTest
	@ArgumentsSource(value = MovementArgs.class)
	void testGetStatus(List<StrategoMovement> movements) {
		var player = getTestPlayer();
		var game = getTestGame(player, player);

		Mockito.when(gameRepository.findById(GAME_ID)).thenReturn(Optional.of(game));

		var board = (List<List<BoardTileDTO>>) Mockito.mock(List.class);
		var status = Mockito.mock(StrategoStatus.class);
		Mockito.when(strategoStatusRepository.findByGameId(GAME_ID)).thenReturn(Optional.of(status));
		Mockito.when(status.getBoard()).thenReturn(board);

		Mockito.when(strategoMovementRepository.findAllByGameId(GAME_ID)).thenReturn(movements);

		var statusDto = strategoService.getStatus(GAME_ID, player);

		// .currentPlayer(toPlayerDTO(player)) //

		assertThat(statusDto.getCurrentPlayer().getId()).isEqualTo(PLAYER_ID);
		assertThat(statusDto.getCurrentPlayer().getUsername()).isEqualTo(PLAYER_USERNAME);
		assertThat(statusDto.getGameId()).isEqualTo(GAME_ID);
		assertThat(statusDto.getPhase()).isEqualTo(GamePhase.PLAYING);
		assertThat(statusDto.getBoard()).isEqualTo(board);
		assertThat(statusDto.isMyTurn()).isFalse();

		if (movements.size() == 0) {
			assertThat(statusDto.getMovement()).isNull();
		} else {
			var movement = statusDto.getMovement();
			assertThat(movement.getRank()).isEqualTo(Rank.BOMB);
			assertThat(movement.getRowInitial()).isEqualTo(1);
			assertThat(movement.getColInitial()).isEqualTo(2);
			assertThat(movement.getRowFinal()).isEqualTo(3);
			assertThat(movement.getColFinal()).isEqualTo(4);
		}
	}

	@Test
	void testCheckRankNoGame() {
		var player = getTestPlayer();
		var movementDto = getTestMovementDto();

		Mockito.when(gameRepository.findById(GAME_ID)).thenReturn(Optional.empty());

		assertThatThrownBy(() -> strategoService.checkRank(GAME_ID, player, movementDto))
				.isInstanceOf(MatchmakingValidationException.class).hasMessage("Game does not exist");
	}

	@ParameterizedTest
	@EnumSource(value = GamePhase.class, names = { "WAITING_FOR_SETUP_2_PLAYERS", "WAITING_FOR_SETUP_1_PLAYER",
			"FINISHED" })
	void testCheckRankWrongPlayerTurn(GamePhase wrongPhase) {
		var player = getTestPlayer();
		var game = getTestGame(player, player);
		game.setPhase(wrongPhase);
		var movementDto = getTestMovementDto();

		Mockito.when(gameRepository.findById(GAME_ID)).thenReturn(Optional.of(game));

		assertThatThrownBy(() -> strategoService.checkRank(GAME_ID, player, movementDto))
				.isInstanceOf(MatchmakingValidationException.class).hasMessage("Game not in playing state");
	}

	@Test
	void testCheckRankNoStatus() {
		var player = getTestPlayer();
		var game = getTestGame(player, player);
		var movementDto = getTestMovementDto();

		Mockito.when(gameRepository.findById(GAME_ID)).thenReturn(Optional.of(game));

		Mockito.when(strategoStatusRepository.findByGameId(GAME_ID)).thenReturn(Optional.empty());

		assertThatThrownBy(() -> strategoService.checkRank(GAME_ID, player, movementDto))
				.isInstanceOf(MatchmakingValidationException.class).hasMessage("Game has not been started");
	}

	private static StrategoMovement getTestMovement() {
		var host = getTestPlayer();
		host.setId(HOST_ID);
		var guest = getTestPlayer();
		guest.setId(GUEST_ID);
		var movement = new StrategoMovement();
		movement.setId(1);
		movement.setIsGuestTurn(true);
		movement.setGame(getTestGame(host, guest));
		movement.setRank(Rank.BOMB);
		movement.setRowInitial(1);
		movement.setColInitial(2);
		movement.setRowFinal(3);
		movement.setColFinal(4);
		return movement;
	}

	private StrategoMovementDTO getTestMovementDto() {
		return StrategoMovementDTO.builder() //
				.rank(Rank.BOMB) //
				.rowInitial(1) //
				.colInitial(2) //
				.rowFinal(3) //
				.colFinal(4) //
				.build();
	}

	private static Game getTestGame(Player host, Player guest) {
		var game = new Game();
		game.setId(GAME_ID.intValue());
		game.setCreationDate(Instant.now());
		game.setHost(host);
		game.setGuest(guest);
		game.setPhase(GamePhase.PLAYING);
		return game;
	}

	private static Player getTestPlayer() {
		var player = new Player();
		player.setId(PLAYER_ID);
		player.setUserName(PLAYER_USERNAME);
		return player;
	}
}
