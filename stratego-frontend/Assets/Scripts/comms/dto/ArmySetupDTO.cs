using System;
using System.Collections.Generic;

[Serializable]
public class ArmySetupDTO
{
	public List<List<Rank>> army;

	public ArmySetupDTO(List<List<Rank>> army)
	{
		this.army = army;
	}
}
