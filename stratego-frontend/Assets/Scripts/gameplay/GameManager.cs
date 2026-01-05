using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static event EventHandler<GameStateDTO> OnGameStateUpdated;

	[SerializeField]
	private ArmySetupWidget SetupWidget;
	[SerializeField]
	private GameObject GameWidget;
	[SerializeField]
	private Board Board;
	[SerializeField]
	private GameObject PiecePrefab;

	private List<Piece> hostPieces;
	private List<Piece> guestPieces;
	private bool isHost;
	private GameStateDTO currentGameState;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		hostPieces = new();
		guestPieces = new();
		
		var commData = CommData.GetInstance();
		isHost = commData.GetIsHost();

		SetupWidget.Initialize(OnGameStarted);
		//OnGameStarted(GetTestGameState());
	}

	public bool GetIsHost() => isHost;

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
				new BoardTileDTO() { isHostOwner = true, rank = Rank.SCOUT },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { isHostOwner = true, rank = Rank.FLAG},
				new BoardTileDTO() { isHostOwner = true, rank = Rank.BOMB },
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
		currentGameState = gameStateDto;
		var board = gameStateDto.board;
		if (!isHost)
		{
			board = new(board);
			board.Reverse();
			board.ForEach(row => row.Reverse());
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
						var tile = Board.GetTile(irow, icol);
						piece.SetTile(tile);
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

		OnGameStateUpdated?.Invoke(this, gameStateDto);
	}

	public Piece GetPieceAtCoordinates(int row, int col)
	{
		var piece = GetPieceAtCoordinates(row, col, hostPieces);

		if (piece == null)
		{
			piece = GetPieceAtCoordinates(row, col, guestPieces);
		}

		return piece;
	}

	private Piece GetPieceAtCoordinates(int row, int col, List<Piece> pieces)
	{
		return pieces.Where(piece => piece.GetTile().HasCoordinates(row, col)).FirstOrDefault();
	}

	public void SendMovement(StrategoMovementDTO movement)
	{
		var commData = CommData.GetInstance();
		var gameId = commData.GetGameId();
		var token = commData.GetToken();
		StartCoroutine(BackendService.GetInstance().AddMovement(gameId, token, movement, OnMovementAdded, OnError));
		/*
		var status = new GameStateDTO() {
			board = null,
			isMyTurn = !currentGameState.isMyTurn,
			movement = movement,
			currentPlayer = currentGameState.currentPlayer,
			gameId = currentGameState.gameId,
			phase = currentGameState.phase,
		};
		OnMovementAdded(status);
		*/
	}

	private void OnMovementAdded(GameStateDTO gameStateDto)
	{
		currentGameState = gameStateDto;
		OnGameStateUpdated?.Invoke(this, gameStateDto);

		//Board.MoveTile(gameStateDto.movement.rowInitial, gameStateDto.movement.colInitial, gameStateDto.movement.rowFinal, gameStateDto.movement.colFinal);
		var piece = GetPieceAtCoordinates(gameStateDto.movement.rowInitial, gameStateDto.movement.colInitial);
		var targetRow = gameStateDto.movement.rowFinal;
		var targetCol = gameStateDto.movement.colFinal;
		piece.transform.position = Board.GetWorldPosition(targetRow, targetCol);
		var tile = Board.GetTile(targetRow, targetCol);
		piece.SetTile(tile);

	}

	private void OnError(StrategoErrorDTO error)
	{
		Debug.Log(error);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
