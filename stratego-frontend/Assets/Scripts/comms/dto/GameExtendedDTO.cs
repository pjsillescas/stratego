using System;
using System.Collections.Generic;

[Serializable]
public class GameExtendedDTO
{
	public int id;

	public string name;

	public string creationDate;

	public PlayerDTO host;
	public PlayerDTO guest;

	public List<PlayerDTO> players;

	public GameTemplateDTO gameTemplate;

	public string joinCode;

	public GamePhase phase;

}
