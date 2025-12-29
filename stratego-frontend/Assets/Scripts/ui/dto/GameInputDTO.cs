using System;

[Serializable]
public class GameInputDTO
{
	public string joinCode;

	public GameInputDTO(string joinCode)
	{
		this.joinCode = joinCode;
	}
}
