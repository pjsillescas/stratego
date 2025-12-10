package com.pdrosoft.stratego.dto;

import java.util.List;

import com.pdrosoft.stratego.enums.Rank;
import com.pdrosoft.stratego.validation.ArmySetupValidation;

import lombok.Builder;
import lombok.Data;

@Data
@Builder
public class ArmySetupDTO {
	@ArmySetupValidation
	private List<List<Rank>> army;
}
