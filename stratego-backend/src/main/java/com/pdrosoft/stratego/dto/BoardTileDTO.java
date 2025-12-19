package com.pdrosoft.stratego.dto;

import java.io.Serializable;

import com.pdrosoft.stratego.enums.Rank;

import lombok.Builder;
import lombok.Data;

@Data
@Builder
public class BoardTileDTO implements Serializable {
	private static final long serialVersionUID = -5850120129376886855L;

	private Rank rank;
	private boolean isHostOwner;
}
