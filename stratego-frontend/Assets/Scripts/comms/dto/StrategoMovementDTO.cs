using System;
using System.Collections.Generic;

[Serializable]
public class StrategoMovementDTO
{
	public Rank rank;

	public int rowInitial;
	public int colInitial;

	public int rowFinal;
	public int colFinal;

	public List<StrategoMovementResultDTO> result;
}
