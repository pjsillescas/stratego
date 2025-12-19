package com.pdrosoft.stratego.dto;

import java.io.Serializable;
import java.util.List;

import com.pdrosoft.matchmaking.dto.PlayerDTO;
import com.pdrosoft.stratego.enums.GamePhase;

import lombok.Builder;
import lombok.Data;

@Data
@Builder
public class GameStateDTO implements Serializable {

	private static final long serialVersionUID = 3924386161467219822L;

	private PlayerDTO currentPlayer;
	private Long gameId;

	private StrategoMovementDTO movement;
	private GamePhase phase;

	private List<List<BoardTileDTO>> board;
	
	private boolean isMyTurn;
}
