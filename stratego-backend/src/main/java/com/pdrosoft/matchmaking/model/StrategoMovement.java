package com.pdrosoft.matchmaking.model;

import com.pdrosoft.matchmaking.stratego.enums.Rank;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.FetchType;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.OneToOne;
import lombok.Data;

@Entity
@Data
public class StrategoMovement {

	@Id
	@GeneratedValue(strategy = GenerationType.IDENTITY)
	private Integer id;

	@OneToOne(fetch = FetchType.LAZY)
	@JoinColumn(name = "game_id", nullable = false)
	private Game game;
	
	@Column(name = "is_guest_turn", nullable = false)
	private Boolean isGuestTurn;

	@Column(nullable=false)
	private Rank rank;
	@Column(name = "row_initial", nullable=false)
	private int rowInitial;
	@Column(name = "col_initial", nullable=false)
	private int colInitial;
	@Column(name = "row_final", nullable=false)
	private int rowFinal;
	@Column(name = "col_final", nullable=false)
	private int colFinal;

}
