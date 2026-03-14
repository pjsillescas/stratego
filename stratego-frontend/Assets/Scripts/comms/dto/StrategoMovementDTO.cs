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

	public virtual bool Equals(StrategoMovementDTO movement)
	{
		if (movement == null)
		{
			return false;
		}

		return rank == movement.rank //
			&& rowInitial == movement.rowInitial //
			&& colInitial == movement.colInitial //
			&& rowFinal == movement.rowFinal //
			&& colFinal == movement.colFinal //
		;
	}
}
