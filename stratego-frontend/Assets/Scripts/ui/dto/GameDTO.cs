using System;

[Serializable]
public class GameDTO
{
	public int id;

	public string name;

	public string creationDate;

	public PlayerDTO host;
	public PlayerDTO guest;

	public GamePhase phase;
}
