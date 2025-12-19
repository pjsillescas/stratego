package com.pdrosoft.stratego.service;

import com.pdrosoft.matchmaking.model.Player;
import com.pdrosoft.stratego.dto.ArmySetupDTO;
import com.pdrosoft.stratego.dto.GameStateDTO;
import com.pdrosoft.stratego.dto.StrategoMovementDTO;

public interface StrategoService {

	GameStateDTO addSetup(Long gameId, Player player, ArmySetupDTO setupDto);

	GameStateDTO addMovement(Long gameId, Player player, StrategoMovementDTO movementDto);
	GameStateDTO checkRank(Long gameId, Player player, StrategoMovementDTO movementDto);

	GameStateDTO getStatus(Long gameId, Player player);

}
