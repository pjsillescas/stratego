using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	[SerializeField]
	private ArmySetupWidget SetupWidget;
	[SerializeField]
	private GameObject GameWidget;
	[SerializeField]
	private Board Board;
	[SerializeField]
	private GameObject PiecePrefab;

	private BackendService backendService;
	private List<Piece> hostPieces;
	private List<Piece> guestPieces;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		hostPieces = new();
		guestPieces = new();

		backendService = FindFirstObjectByType<BackendService>();
		//SetupWidget.Initialize(backendService, OnGameStarted);
		OnGameStarted(GetTestGameState());
	}

	private GameStateDTO GetTestGameState()
	{
		var gameState = new GameStateDTO();
		gameState.phase = GamePhase.PLAYING;
		gameState.currentPlayer = new PlayerDTO() { id = 1, username = "host" };
		gameState.movement = null;
		gameState.isMyTurn = true;
		gameState.gameId = 1;
		gameState.board = new () { 
			// Blue player (host)
			new () {
				new BoardTileDTO() { isHostOwner = true, rank = Rank.FLAG },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.GENERAL},
				new BoardTileDTO() { isHostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.GENERAL },
			},
			new () {
				new BoardTileDTO() { isHostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.GENERAL},
				new BoardTileDTO() { isHostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.GENERAL },
			},
			new () {
				new BoardTileDTO() { isHostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.GENERAL},
				new BoardTileDTO() { isHostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.GENERAL },
			},
			new () {
				new BoardTileDTO() { isHostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.GENERAL},
				new BoardTileDTO() { isHostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.GENERAL },
			},

			// Central zone
			new () {
				null,
				null,
				new BoardTileDTO() { isHostOwner = true, rank = Rank.DISABLED },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.DISABLED },
				null,
				null,
				new BoardTileDTO() { isHostOwner = true, rank = Rank.DISABLED },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.DISABLED},
				null,
				null,
			},
			new () {
				null,
				null,
				new BoardTileDTO() { isHostOwner = true, rank = Rank.DISABLED },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.DISABLED },
				null,
				null,
				new BoardTileDTO() { isHostOwner = true, rank = Rank.DISABLED },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.DISABLED},
				null,
				null,
			},

			// Red player (guest)
			new () {
				new BoardTileDTO() { isHostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.GENERAL},
				new BoardTileDTO() { isHostOwner = false, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.GENERAL },
			},
			new () {
				new BoardTileDTO() { isHostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.GENERAL},
				new BoardTileDTO() { isHostOwner = false, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.GENERAL },
			},
			new () {
				new BoardTileDTO() { isHostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.MINER },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.GENERAL},
				new BoardTileDTO() { isHostOwner = false, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.GENERAL },
			},
			new () {
				new BoardTileDTO() { isHostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = false, rank = Rank.GENERAL},
				new BoardTileDTO() { isHostOwner = false, rank = Rank.SPY},
				new BoardTileDTO() { isHostOwner = false, rank = Rank.FLAG },
			},

		};
		return gameState;
	}

	private void OnGameStarted(GameStateDTO gameStateDto)
	{
		var isHost = PlayerPrefsManager.GetIsHost();
		//var isHost = true;
		var board = gameStateDto.board;
		if (!isHost)
		{
			board = new(board);
			board.Reverse();
		}

		int irow = -1;
		foreach(var row in board)
		{
			irow++;
			int icol = -1;
			foreach(var tileDto in row)
			{
				icol++;

				if (tileDto != null)
				{
					if (tileDto.rank == Rank.DISABLED)
					{
						;
					}
					else
					{
						var piece = Instantiate(PiecePrefab).GetComponent<Piece>();
						piece.transform.position = Board.GetWorldPosition(irow, icol);
						piece.Initialize(tileDto.rank, tileDto.isHostOwner);
						piece.SetCoordinates(irow, icol);
						if (tileDto.isHostOwner)
						{
							hostPieces.Add(piece);
						}
						else
						{
							guestPieces.Add(piece);
						}
					}
				}
			}
		}
	}

	// Update is called once per frame
	void Update()
	{

	}
}
