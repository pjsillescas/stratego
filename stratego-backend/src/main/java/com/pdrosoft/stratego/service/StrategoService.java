package com.pdrosoft.stratego.service;

import com.pdrosoft.matchmaking.model.Player;
import com.pdrosoft.matchmaking.security.payload.MatchmakingUserDetails;
import com.pdrosoft.stratego.dto.ArmySetupDTO;
import com.pdrosoft.stratego.dto.GameStateDTO;
import com.pdrosoft.stratego.dto.StrategoMovementDTO;

import jakarta.validation.Valid;

public interface StrategoService {

	GameStateDTO addSetup(Long gameId, MatchmakingUserDetails userDetails, @Valid ArmySetupDTO setupDto);

	GameStateDTO addMovement(Player player, @Valid StrategoMovementDTO movementDto);

}
