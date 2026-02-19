using System;

[Serializable]
public class FavouriteSetupDTO
{
	public int id;

	public string description;

	public ArmySetupDTO armySetupDTO;

	public FavouriteSetupDTO(int id, string description, ArmySetupDTO armySetupDTO)
	{
		this.id = id;
		this.description = description;
		this.armySetupDTO = armySetupDTO;
	}
}
