package com.pdrosoft.stratego.dto;

import java.util.List;

import com.pdrosoft.matchmaking.dto.PlayerDTO;
import com.pdrosoft.stratego.enums.GamePhase;
import com.pdrosoft.stratego.enums.Rank;

import lombok.Builder;
import lombok.Data;

@Data
@Builder
public class GameStateDTO {
	private PlayerDTO currentPlayer;
	private Long gameId;

	private StrategoMovementDTO movement;
	private GamePhase phase;

	private List<List<Rank>> board;
}
