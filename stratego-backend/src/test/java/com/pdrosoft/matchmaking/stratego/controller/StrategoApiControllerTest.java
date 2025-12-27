package com.pdrosoft.matchmaking.stratego.controller;

import static org.assertj.core.api.Assertions.assertThat;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.post;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.put;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

import java.util.List;

import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.boot.webmvc.test.autoconfigure.AutoConfigureMockMvc;
import org.springframework.http.MediaType;
import org.springframework.test.annotation.DirtiesContext;
import org.springframework.test.annotation.DirtiesContext.ClassMode;
import org.springframework.test.context.ActiveProfiles;
import org.springframework.test.context.jdbc.Sql;
import org.springframework.test.web.servlet.MockMvc;

import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.SerializationFeature;
import com.fasterxml.jackson.datatype.jsr310.JavaTimeModule;
import com.pdrosoft.matchmaking.dto.ErrorResultDTO;
import com.pdrosoft.matchmaking.dto.LoginResultDTO;
import com.pdrosoft.matchmaking.dto.UserAuthDTO;
import com.pdrosoft.matchmaking.stratego.dto.ArmySetupDTO;
import com.pdrosoft.matchmaking.stratego.dto.BoardTileDTO;
import com.pdrosoft.matchmaking.stratego.dto.GameStateDTO;
import com.pdrosoft.matchmaking.stratego.enums.GamePhase;
import com.pdrosoft.matchmaking.stratego.enums.Rank;

@ActiveProfiles("test")
@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
@Sql(scripts = "classpath:test-gameplay-data.sql", executionPhase = Sql.ExecutionPhase.BEFORE_TEST_CLASS)
@DirtiesContext(classMode = ClassMode.AFTER_CLASS)
@AutoConfigureMockMvc
public class StrategoApiControllerTest {

	private static final Long GAME_ID = 5L;

	@Autowired
	private MockMvc mockMvc;

	private ObjectMapper mapper = null;

	private ObjectMapper getObjectMapper() {
		if (mapper == null) {
			mapper = new ObjectMapper();
			mapper.configure(SerializationFeature.WRAP_ROOT_VALUE, false);
			mapper.configure(SerializationFeature.INDENT_OUTPUT, false);
			// support Java 8 date time apis
			mapper.registerModule(new JavaTimeModule());
		}

		return mapper;
	}

	private String getToken(String user, String password) throws Exception {
		var authData = UserAuthDTO.builder().username(user).password(password).build();

		var json = getObjectMapper().writeValueAsString(authData);
		var result = mockMvc.perform(post("/api/auth/login")//
				.contentType(MediaType.APPLICATION_JSON)//
				.content(json))//
				.andExpect(status().isOk()).andReturn();

		var authDTO = getObjectMapper().readValue(result.getResponse().getContentAsString(), LoginResultDTO.class);

		return authDTO.getToken();
	}

	@Test
	void testAddSetupInvalidSetup() throws Exception {

		var setupDto = ArmySetupDTO.builder().army(List.of()).build();
		var json = getObjectMapper().writeValueAsString(setupDto);

		var token1 = getToken("testuser1", "password1");
		var result = mockMvc.perform(put("/api/stratego/%d/setup".formatted(GAME_ID)) //
				.header("Authorization", "Bearer %s".formatted(token1)) //
				.contentType(MediaType.APPLICATION_JSON) //
				.content(json) //
		).andExpect(status().isBadRequest()).andReturn();

		var resultDto = getObjectMapper().readValue(result.getResponse().getContentAsString(), ErrorResultDTO.class);
		assertThat(resultDto.getMessage()).contains("'army': rejected value").contains("Invalid army setup");
	}

	List<List<Rank>> getValidSetup() {
		return List.of(
				List.of(Rank.BOMB, Rank.BOMB, Rank.BOMB, Rank.BOMB, Rank.BOMB, Rank.BOMB, Rank.SPY, Rank.FLAG,
						Rank.MARSHAL, Rank.GENERAL),
				List.of(Rank.COLONEL, Rank.COLONEL, Rank.MAJOR, Rank.MAJOR, Rank.MAJOR, Rank.CAPTAIN, Rank.CAPTAIN,
						Rank.CAPTAIN, Rank.CAPTAIN, Rank.LIEUTENANT),
				List.of(Rank.LIEUTENANT, Rank.LIEUTENANT, Rank.LIEUTENANT, Rank.SERGEANT, Rank.SERGEANT, Rank.SERGEANT,
						Rank.SERGEANT, Rank.MINER, Rank.MINER, Rank.MINER),
				List.of(Rank.MINER, Rank.MINER, Rank.SCOUT, Rank.SCOUT, Rank.SCOUT, Rank.SCOUT, Rank.SCOUT, Rank.SCOUT,
						Rank.SCOUT, Rank.SCOUT));
	}

	@Test
	void testAddSetupSuccess() throws Exception {

		var board = getValidSetup();
		var setupDto = ArmySetupDTO.builder().army(board).build();
		var json = getObjectMapper().writeValueAsString(setupDto);

		// Host
		var token1 = getToken("testuser1", "password1");
		var resultHost = mockMvc.perform(put("/api/stratego/%d/setup".formatted(GAME_ID)) //
				.header("Authorization", "Bearer %s".formatted(token1)) //
				.contentType(MediaType.APPLICATION_JSON) //
				.content(json) //
		).andExpect(status().isOk()).andReturn();

		GameStateDTO gameState = getObjectMapper().readValue(resultHost.getResponse().getContentAsString(),
				new TypeReference<GameStateDTO>() {
				});
		assertThat(gameState).isNotNull();
		assertThat(gameState.getGameId()).isEqualTo(GAME_ID);
		assertThat(gameState.getMovement()).isNull();
		assertThat(gameState.getPhase()).isEqualTo(GamePhase.WAITING_FOR_SETUP_1_PLAYER);

		checkHostBoard(gameState.getBoard(), setupDto);

		// Guest
		var token2 = getToken("testuser2", "password2");
		var resultGuest = mockMvc.perform(put("/api/stratego/%d/setup".formatted(GAME_ID)) //
				.header("Authorization", "Bearer %s".formatted(token2)) //
				.contentType(MediaType.APPLICATION_JSON) //
				.content(json) //
		).andExpect(status().isOk()).andReturn();

		GameStateDTO gameState2 = getObjectMapper().readValue(resultGuest.getResponse().getContentAsString(),
				new TypeReference<GameStateDTO>() {
				});
		assertThat(gameState2).isNotNull();
		assertThat(gameState2.getGameId()).isEqualTo(GAME_ID);
		assertThat(gameState2.getMovement()).isNull();
		assertThat(gameState2.getPhase()).isEqualTo(GamePhase.PLAYING);

		checkHostBoard(gameState2.getBoard(), setupDto);
		checkGuestBoard(gameState2.getBoard(), setupDto);
	}

	private void checkHostBoard(List<List<BoardTileDTO>> board, ArmySetupDTO setup) {
		var ranks = setup.getArmy();
		for (int row = 0; row < 3; row++) {
			for (int col = 0; col < 10; col++) {
				assertThat(board.get(row).get(col).getRank()).isEqualTo(ranks.get(row).get(col));
			}
		}
	}

	private void checkGuestBoard(List<List<BoardTileDTO>> board, ArmySetupDTO setup) {
		var ranks = setup.getArmy();
		for (int row = 0; row < 3; row++) {
			for (int col = 0; col < 10; col++) {
				assertThat(board.get(6 + row).get(col).getRank()).isEqualTo(ranks.get(row).get(col));
			}
		}
	}

	
	/*
	@Test
	void testGameListWithNoToken() throws Exception {
		mockMvc.perform(get("/api/game")).andExpect(status().isForbidden());
	}

	@Test
	void testGameListWithInvalidToken() throws Exception {
		var token = "invalid.token";

		mockMvc.perform(get("/api/game").header("Authorization", "Bearer %s".formatted(token)))//
				.andExpect(status().isForbidden());
	}

	@Test
	void testCreateGameWithInvalidToken() throws Exception {
		var token = "invalid.token";

		mockMvc.perform(put("/api/game").header("Authorization", "Bearer %s".formatted(token)))//
				.andExpect(status().isForbidden());
	}

	@Test
	void testCreateJoinLeaveGuestFirstGameSuccess() throws Exception {
		var tokenHost = getToken("testuser1", "password1");
		var tokenGuest = getToken("testuser2", "password2");

		var gameInputDto = GameInputDTO.builder().joinCode("test-code").build();
		var json = getObjectMapper().writeValueAsString(gameInputDto);
		var result = mockMvc.perform(put("/api/game") //
				.header("Authorization", "Bearer %s".formatted(tokenHost)) //
				.contentType(MediaType.APPLICATION_JSON)//
				.content(json))//
				.andExpect(status().isOk()).andReturn();

		var game = getObjectMapper().readValue(result.getResponse().getContentAsString(), GameDTO.class);
		var newGameId = game.getId();
		assertThat(game.getCreationDate()).isBetween(Instant.now().minus(Duration.ofSeconds(2)),
				Instant.now().plus(Duration.ofSeconds(2)));
		assertThat(game.getName()).isEqualTo("testuser1's game");
		assertThat(game.getHost().getUsername()).isEqualTo("testuser1");
		assertThat(game.getGuest()).isNull();

		var resultJoin = mockMvc.perform(post("/api/game/{gameId}/join", Integer.toString(newGameId)) //
				.header("Authorization", "Bearer %s".formatted(tokenGuest)))//
				.andExpect(status().isOk()).andReturn();

		var gameJoined = getObjectMapper().readValue(resultJoin.getResponse().getContentAsString(),
				GameExtendedDTO.class);
		assertThat(gameJoined.getId()).isEqualTo(game.getId());
		assertThat(gameJoined.getCreationDate()).isNotNull();
		assertThat(gameJoined.getJoinCode()).isEqualTo(gameInputDto.getJoinCode());
		assertThat(gameJoined.getName()).isEqualTo(game.getName());
		assertThat(gameJoined.getHost().getUsername()).isEqualTo("testuser1");
		assertThat(gameJoined.getGuest().getUsername()).isEqualTo("testuser2");

		var resultLeaveGuest = mockMvc.perform(post("/api/game/{gameId}/leave", Integer.toString(newGameId)) //
				.header("Authorization", "Bearer %s".formatted(tokenGuest)))//
				.andExpect(status().isOk()).andReturn();

		var gameLeft = getObjectMapper().readValue(resultLeaveGuest.getResponse().getContentAsString(), GameDTO.class);
		assertThat(gameLeft.getId()).isEqualTo(newGameId);
		assertThat(gameLeft.getCreationDate()).isNotNull();
		assertThat(gameLeft.getName()).isEqualTo("testuser1's game");
		assertThat(gameLeft.getHost().getUsername()).isEqualTo("testuser1");
		assertThat(gameLeft.getGuest()).isNull();

		var resultLeaveHost = mockMvc.perform(post("/api/game/{gameId}/leave", Integer.toString(newGameId)) //
				.header("Authorization", "Bearer %s".formatted(tokenHost)))//
				.andExpect(status().isOk()).andReturn();

		assertThat(resultLeaveHost.getResponse().getContentAsString()).isNullOrEmpty();

	}

	@Test
	void testCreateJoinLeaveHostFirstGameSuccess() throws Exception {
		final var testCode = "test-code";

		var tokenHost = getToken("testuser1", "password1");
		var tokenGuest = getToken("testuser2", "password2");

		var gameInputDto = GameInputDTO.builder().joinCode(testCode).build();
		var json = getObjectMapper().writeValueAsString(gameInputDto);
		var result = mockMvc.perform(put("/api/game") //
				.header("Authorization", "Bearer %s".formatted(tokenHost)) //
				.contentType(MediaType.APPLICATION_JSON)//
				.content(json))//
				.andExpect(status().isOk()).andReturn();

		var game = getObjectMapper().readValue(result.getResponse().getContentAsString(), GameDTO.class);
		var newGameId = game.getId();
		assertThat(game.getCreationDate()).isBetween(Instant.now().minus(Duration.ofSeconds(2)),
				Instant.now().plus(Duration.ofSeconds(2)));
		assertThat(game.getName()).isEqualTo("testuser1's game");
		assertThat(game.getHost().getUsername()).isEqualTo("testuser1");
		assertThat(game.getGuest()).isNull();

		var resultJoin = mockMvc.perform(post("/api/game/{gameId}/join", Integer.toString(newGameId)) //
				.header("Authorization", "Bearer %s".formatted(tokenGuest)))//
				.andExpect(status().isOk()).andReturn();

		var gameJoined = getObjectMapper().readValue(resultJoin.getResponse().getContentAsString(),
				GameExtendedDTO.class);
		assertThat(gameJoined.getId()).isEqualTo(game.getId());
		assertThat(gameJoined.getCreationDate()).isNotNull();
		assertThat(gameJoined.getName()).isEqualTo(game.getName());
		assertThat(gameJoined.getJoinCode()).isEqualTo(gameInputDto.getJoinCode());
		assertThat(gameJoined.getHost().getUsername()).isEqualTo("testuser1");
		assertThat(gameJoined.getGuest().getUsername()).isEqualTo("testuser2");

		var resultLeaveHost = mockMvc.perform(post("/api/game/{gameId}/leave", Integer.toString(newGameId)) //
				.header("Authorization", "Bearer %s".formatted(tokenHost)))//
				.andExpect(status().isOk()).andReturn();

		assertThat(resultLeaveHost.getResponse().getContentAsString()).isNullOrEmpty();

		var resultLeaveGuest = mockMvc.perform(post("/api/game/{gameId}/leave", Integer.toString(newGameId)) //
				.header("Authorization", "Bearer %s".formatted(tokenGuest)))//
				.andExpect(status().isOk()).andReturn();

		assertThat(resultLeaveGuest.getResponse().getContentAsString()).isNullOrEmpty();
	}

	@Test
	void testCreateWithEmptyJoinCode() throws Exception {
		final var testCode = "";

		var tokenHost = getToken("testuser1", "password1");

		var gameInputDto = GameInputDTO.builder().joinCode(testCode).build();
		var json = getObjectMapper().writeValueAsString(gameInputDto);
		var result = mockMvc.perform(put("/api/game") //
				.header("Authorization", "Bearer %s".formatted(tokenHost)) //
				.contentType(MediaType.APPLICATION_JSON)//
				.content(json))//
				.andExpect(status().isBadRequest()).andReturn();

		var resultDto = getObjectMapper().readValue(result.getResponse().getContentAsString(), ErrorResultDTO.class);
		assertThat(resultDto.getMessage()).contains("joinCode").contains("must not be blank");
	}

	@Test
	void testLeaveInexistentGame() throws Exception {
		var token = getToken("testuser1", "password1");
		var inexistentGameId = 1200;

		var resultLeaveGuest = mockMvc.perform(post("/api/game/{gameId}/leave", Integer.toString(inexistentGameId)) //
				.header("Authorization", "Bearer %s".formatted(token)))//
				.andExpect(status().isOk()).andReturn();

		assertThat(resultLeaveGuest.getResponse().getContentAsString()).isNullOrEmpty();
	}

	@Test
	void testJoinInexistentGame() throws Exception {
		var token = getToken("testuser1", "password1");
		var inexistentGameId = 1200;

		mockMvc.perform(post("/api/game/{gameId}/join", Integer.toString(inexistentGameId)) //
				.header("Authorization", "Bearer %s".formatted(token)))//
				.andExpect(status().isNotFound());
	}
	*/

}
