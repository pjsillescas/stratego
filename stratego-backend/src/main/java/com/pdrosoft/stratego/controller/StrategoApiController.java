package com.pdrosoft.stratego.controller;

import java.time.Duration;
import java.time.Instant;
import java.util.List;
import java.util.Optional;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import com.pdrosoft.matchmaking.dto.GameDTO;
import com.pdrosoft.matchmaking.dto.GameExtendedDTO;
import com.pdrosoft.matchmaking.dto.GameInputDTO;
import com.pdrosoft.matchmaking.security.payload.MatchmakingUserDetails;
import com.pdrosoft.stratego.dto.ArmySetupDTO;
import com.pdrosoft.stratego.dto.GameStateDTO;
import com.pdrosoft.stratego.dto.StrategoMovementDTO;
import com.pdrosoft.stratego.service.StrategoService;

import jakarta.validation.Valid;
import lombok.NonNull;

@RestController
@RequestMapping("/api/stratego/{gameId:[0-9]+}")
public class StrategoApiController {

	@NonNull
	private final StrategoService strategoService;

	public StrategoApiController(@Autowired StrategoService strategoService) {
		this.strategoService = strategoService;
	}

	@PutMapping(path = "/setup", produces = { "application/json" })
	public GameStateDTO addSetup(@AuthenticationPrincipal MatchmakingUserDetails userDetails,
			@PathVariable("gameId") Long gameId, @RequestBody @Valid ArmySetupDTO setupDto) {
		return strategoService.addSetup(gameId, userDetails, setupDto);
	}

	@PutMapping(path = "/movement", produces = { "application/json" })
	public GameStateDTO addMovement(@AuthenticationPrincipal MatchmakingUserDetails userDetails,
			@PathVariable("gameId") Long gameId, @Valid @RequestBody StrategoMovementDTO movementDto) {
		return strategoService.addMovement(userDetails.getPlayer(), movementDto);
	}
}
