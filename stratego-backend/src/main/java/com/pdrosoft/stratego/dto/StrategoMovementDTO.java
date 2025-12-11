package com.pdrosoft.stratego.dto;

import com.pdrosoft.stratego.enums.Rank;

import jakarta.validation.constraints.Max;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotNull;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@Builder
@NoArgsConstructor
@AllArgsConstructor
public class StrategoMovementDTO {

	@NotNull
	private Rank rank;

	@NotNull
	@Min(1)
	@Max(4)
	private int xInitial;
	@NotNull
	@Min(1)
	@Max(10)
	private int yInitial;

	@NotNull
	@Min(1)
	@Max(4)
	private int xFinal;
	@NotNull
	@Min(1)
	@Max(10)
	private int yFinal;
}
