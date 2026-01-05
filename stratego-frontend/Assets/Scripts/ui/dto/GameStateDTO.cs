using System;
using System.Collections.Generic;

[Serializable]
public class GameStateDTO
{
	public PlayerDTO currentPlayer;
	public int hostPlayerId;
	public int guestPlayerId;
	public int gameId;

	public StrategoMovementDTO movement;
	public GamePhase phase;

	public List<List<BoardTileDTO>> board;

	public bool isMyTurn;

}
