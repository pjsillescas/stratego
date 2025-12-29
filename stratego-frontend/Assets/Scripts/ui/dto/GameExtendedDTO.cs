using System;

[Serializable]
public class GameExtendedDTO
{
	public int id;

	public string name;

	public DateTime creationDate;

	public PlayerDTO host;
	public PlayerDTO guest;

	public string joinCode;

	public GamePhase phase;

}
