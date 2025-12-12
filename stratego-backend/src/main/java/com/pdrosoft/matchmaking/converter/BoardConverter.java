package com.pdrosoft.matchmaking.converter;

import java.io.IOException;
import java.util.List;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.pdrosoft.stratego.enums.Rank;

import jakarta.persistence.AttributeConverter;

public class BoardConverter implements AttributeConverter<List<List<Rank>>, String> {

	private static ObjectMapper objectMapper = null;

	private static ObjectMapper getObjectMapper() {
		if (objectMapper == null) {
			objectMapper = new ObjectMapper();
		}

		return objectMapper;
	}

	@Override
	public String convertToDatabaseColumn(List<List<Rank>> board) {
		String customerInfoJson = null;
		try {
			customerInfoJson = getObjectMapper().writeValueAsString(board);
		} catch (final JsonProcessingException e) {
			// logger.error("JSON writing error", e);
		}

		return customerInfoJson;
	}

	@Override
	public List<List<Rank>> convertToEntityAttribute(String json) {
		List<List<Rank>> customerInfo = null;
		try {
			customerInfo = getObjectMapper().readValue(json, //
					new TypeReference<List<List<Rank>>>() {
					});
		} catch (final IOException e) {
			// logger.error("JSON reading error", e);
		}

		return customerInfo;
	}

}
