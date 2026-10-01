using System;

[Serializable]
public class GameInputDTO
{
	public string joinCode;
	public int gameTemplateId;

	public GameInputDTO(string joinCode, int gameTemplateId)
	{
		this.joinCode = joinCode;
		this.gameTemplateId = gameTemplateId;
	}
}
