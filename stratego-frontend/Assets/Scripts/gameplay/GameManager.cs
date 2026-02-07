using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static event EventHandler<GameStateDTO> OnGameStateUpdated;
	public static event EventHandler<Piece> OnPieceCaptured;
	public static event EventHandler OnEndMovementAnimation;

	[SerializeField]
	private ArmySetupWidget SetupWidget;
	[SerializeField]
	private GameObject GameWidget;
	[SerializeField]
	private Board Board;
	[SerializeField]
	private GameObject PiecePrefab;
	[SerializeField]
	private GameObject DisablePrefab;
	[SerializeField]
	private Transform HostPieceShowPosition;
	[SerializeField]
	private Transform HostPieceLastPosition;
	[SerializeField]
	private Transform GuestPieceShowPosition;
	[SerializeField]
	private Transform GuestPieceLastPosition;

	[SerializeField]
	private PiecesSet pieceSet;

	private List<Piece> hostPieces;
	private List<Piece> guestPieces;
	private List<Piece> disabledPieces;
	private bool isHost;
	Coroutine movementCoroutine = null;

	//private GameStateDTO currentGameState;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		hostPieces = new();
		guestPieces = new();
		disabledPieces = new();

		if (GameWidget != null)
		{
			var commData = CommData.GetInstance();
			isHost = commData.GetIsHost();

			GameWidget.SetActive(false);
			SetupWidget.Initialize(OnGameStarted);
			//OnGameStarted(GetTestGameState());

			var myusername = commData.GetMyUsername();
			var opponentusername = commData.GetOpponentUsername();
			Debug.Log($"{myusername} vs {opponentusername}");
		}
	}

	public void DisableGame()
	{
		FindFirstObjectByType<Watchdog>().gameObject.SetActive(false);
		FindFirstObjectByType<InputManager>().gameObject.SetActive(false);
	}

	public bool GetIsHost() => isHost;

	private GameStateDTO GetTestGameState()
	{
		var gameState = new GameStateDTO();
		gameState.phase = GamePhase.PLAYING;
		gameState.currentPlayer = new PlayerDTO() { id = 1, username = "host" };
		gameState.movement = null;
		gameState.myTurn = true;
		gameState.gameId = 1;
		gameState.board = new() { 
			// Blue player (host)
			new () {
				new BoardTileDTO() { hostOwner = true, rank = Rank.FLAG },
				new BoardTileDTO() { hostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { hostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { hostOwner = true, rank = Rank.GENERAL},
				new BoardTileDTO() { hostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { hostOwner = true, rank = Rank.GENERAL },
			},
			new () {
				new BoardTileDTO() { hostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { hostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { hostOwner = true, rank = Rank.GENERAL},
				new BoardTileDTO() { hostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { hostOwner = true, rank = Rank.GENERAL },
			},
			new () {
				new BoardTileDTO() { hostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { hostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { hostOwner = true, rank = Rank.GENERAL},
				new BoardTileDTO() { hostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { hostOwner = true, rank = Rank.GENERAL },
			},
			new () {
				new BoardTileDTO() { hostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = true, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = true, rank = Rank.SCOUT },
				new BoardTileDTO() { hostOwner = true, rank = Rank.GENERAL },
				new BoardTileDTO() { hostOwner = true, rank = Rank.FLAG},
				new BoardTileDTO() { hostOwner = true, rank = Rank.BOMB },
				new BoardTileDTO() { hostOwner = true, rank = Rank.GENERAL },
			},

			// Central zone
			new () {
				null,
				null,
				new BoardTileDTO() { hostOwner = true, rank = Rank.DISABLED },
				new BoardTileDTO() { hostOwner = true, rank = Rank.DISABLED },
				null,
				null,
				new BoardTileDTO() { hostOwner = true, rank = Rank.DISABLED },
				new BoardTileDTO() { hostOwner = true, rank = Rank.DISABLED},
				null,
				null,
			},
			new () {
				null,
				null,
				new BoardTileDTO() { hostOwner = true, rank = Rank.DISABLED },
				new BoardTileDTO() { hostOwner = true, rank = Rank.DISABLED },
				null,
				null,
				new BoardTileDTO() { hostOwner = true, rank = Rank.DISABLED },
				new BoardTileDTO() { hostOwner = true, rank = Rank.DISABLED},
				null,
				null,
			},

			// Red player (guest)
			new () {
				new BoardTileDTO() { hostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = false, rank = Rank.GENERAL },
				new BoardTileDTO() { hostOwner = false, rank = Rank.GENERAL },
				new BoardTileDTO() { hostOwner = false, rank = Rank.GENERAL},
				new BoardTileDTO() { hostOwner = false, rank = Rank.GENERAL },
				new BoardTileDTO() { hostOwner = false, rank = Rank.GENERAL },
			},
			new () {
				new BoardTileDTO() { hostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = false, rank = Rank.GENERAL },
				new BoardTileDTO() { hostOwner = false, rank = Rank.GENERAL },
				new BoardTileDTO() { hostOwner = false, rank = Rank.GENERAL},
				new BoardTileDTO() { hostOwner = false, rank = Rank.GENERAL },
				new BoardTileDTO() { hostOwner = false, rank = Rank.GENERAL },
			},
			new () {
				new BoardTileDTO() { hostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = false, rank = Rank.MINER },
				new BoardTileDTO() { hostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = false, rank = Rank.GENERAL },
				new BoardTileDTO() { hostOwner = false, rank = Rank.GENERAL },
				new BoardTileDTO() { hostOwner = false, rank = Rank.GENERAL},
				new BoardTileDTO() { hostOwner = false, rank = Rank.GENERAL },
				new BoardTileDTO() { hostOwner = false, rank = Rank.GENERAL },
			},
			new () {
				new BoardTileDTO() { hostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = false, rank = Rank.CAPTAIN },
				new BoardTileDTO() { hostOwner = false, rank = Rank.GENERAL },
				new BoardTileDTO() { hostOwner = false, rank = Rank.GENERAL },
				new BoardTileDTO() { hostOwner = false, rank = Rank.GENERAL},
				new BoardTileDTO() { hostOwner = false, rank = Rank.SPY},
				new BoardTileDTO() { hostOwner = false, rank = Rank.FLAG },
			},

		};
		return gameState;
	}

	private void OnGameStarted(GameStateDTO gameStateDto)
	{
		GameWidget.SetActive(true);

		//currentGameState = gameStateDto;
		var board = gameStateDto.board;

		if (!isHost)
		{
			/*
			board = new(board);
			board.Reverse();
			board.ForEach(row => row.Reverse());
			*/
			Camera.main.transform.Rotate(Vector3.right, -90f);
			Camera.main.transform.Rotate(Vector3.up, 180f);
			Camera.main.transform.Rotate(Vector3.right, 90f);
		}

		int irow = -1;
		foreach (var row in board)
		{
			irow++;
			int icol = -1;
			foreach (var tileDto in row)
			{
				icol++;

				if (tileDto != null)
				{
					var tile = Board.GetTile(irow, icol);

					if (tileDto.rank == Rank.DISABLED)
					{
						var disabledPiece = Instantiate(DisablePrefab).GetComponent<Piece>();
						disabledPiece.transform.position = Board.GetWorldPosition(irow, icol);
						disabledPiece.SetTile(tile);
						disabledPieces.Add(disabledPiece);
					}
					else
					{
						var piece = Instantiate(PiecePrefab).GetComponent<Piece>();
						piece.transform.position = Board.GetWorldPosition(irow, icol);
						//piece.Initialize(tileDto.rank, tileDto.hostOwner);
						piece.Initialize(GetRankData(tileDto.rank), tileDto.hostOwner, tileDto.hostOwner != isHost);
						piece.SetTile(tile);

						if (!isHost)
						{
							piece.transform.Rotate(Vector3.up, 180f);
						}

						if (tileDto.hostOwner)
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

	private PieceData GetRankData(Rank rank)
	{
		return pieceSet.pieces.Where(piece => piece.rank == rank).First();
	}

	public Piece GetPieceAtCoordinates(int row, int col)
	{
		var piece = GetPieceAtCoordinates(row, col, hostPieces);

		if (piece == null)
		{
			piece = GetPieceAtCoordinates(row, col, guestPieces);
		}

		if (piece == null)
		{
			piece = GetPieceAtCoordinates(row, col, disabledPieces);
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

	private IEnumerator WaitForGameState(GameStateDTO gameStateDto)
	{
		var waitForNextFrame = new WaitForSeconds(0.1f);
		while(movementCoroutine != null)
		{
			yield return null;
		}

		movementCoroutine = StartCoroutine(AnimateMovement(gameStateDto));
	}

	private int ClashPieces(Piece attacker, Piece defender)
	{
		var rankAttacker = attacker.GetRank();
		var rankDefender = defender.GetRank();

		var immobileRanks = new List<Rank>() { Rank.BOMB, Rank.FLAG, Rank.DISABLED };
		if (immobileRanks.Contains(rankAttacker) || Rank.DISABLED == rankDefender)
		{
			//throw new MatchmakingValidationException("Invalid ranks compared");
		}

		if (rankAttacker == rankDefender)
		{
			return 0;
		}

		if (rankDefender == Rank.FLAG)
		{
			return 1;
		}

		List<Rank> upperRanks;
		switch (rankDefender)
		{
			case Rank.BOMB:
				upperRanks = new() { Rank.MINER };
				break;
			case Rank.MARSHAL:
				upperRanks = new() { Rank.SPY };
				break;
			case Rank.GENERAL:
				upperRanks = new() { Rank.MARSHAL };
				break;
			case Rank.COLONEL:
				upperRanks = new() { Rank.MARSHAL, Rank.GENERAL };
				break;
			case Rank.MAJOR:
				upperRanks = new() { Rank.MARSHAL, Rank.GENERAL, Rank.COLONEL };
				break;
			case Rank.CAPTAIN:
				upperRanks = new() { Rank.MARSHAL, Rank.GENERAL, Rank.COLONEL, Rank.MAJOR };
				break;
			case Rank.LIEUTENANT:
				upperRanks = new() { Rank.MARSHAL, Rank.GENERAL, Rank.COLONEL, Rank.MAJOR, Rank.CAPTAIN };
				break;
			case Rank.SERGEANT:
				upperRanks = new() { Rank.MARSHAL, Rank.GENERAL, Rank.COLONEL, Rank.MAJOR, Rank.CAPTAIN, Rank.LIEUTENANT };
				break;
			case Rank.MINER:
				upperRanks = new() { Rank.MARSHAL, Rank.GENERAL, Rank.COLONEL, Rank.MAJOR, Rank.CAPTAIN, Rank.LIEUTENANT,
						Rank.SERGEANT };
				break;
			case Rank.SCOUT:
				upperRanks = new() { Rank.MARSHAL, Rank.GENERAL, Rank.COLONEL, Rank.MAJOR, Rank.CAPTAIN, Rank.LIEUTENANT,
						Rank.SERGEANT, Rank.MINER };
				break;
			case Rank.SPY:
			default:
				upperRanks = new() { Rank.MARSHAL, Rank.GENERAL, Rank.COLONEL, Rank.MAJOR, Rank.CAPTAIN, Rank.LIEUTENANT,
						Rank.SERGEANT, Rank.MINER, Rank.SCOUT };
				break;
		}

		return upperRanks.Contains(rankAttacker) ? 1 : -1;
	}

	public void OnMovementAdded(GameStateDTO gameStateDto)
	{
		//currentGameState = gameStateDto;
		OnGameStateUpdated?.Invoke(this, gameStateDto);

		StartCoroutine(WaitForGameState(gameStateDto));
	}

	private IEnumerator AnimateMovement(GameStateDTO gameStateDto)
	{
		//Board.MoveTile(gameStateDto.movement.rowInitial, gameStateDto.movement.colInitial, gameStateDto.movement.rowFinal, gameStateDto.movement.colFinal);
		var piece = GetPieceAtCoordinates(gameStateDto.movement.rowInitial, gameStateDto.movement.colInitial);
		var pieceTarget = GetPieceAtCoordinates(gameStateDto.movement.rowFinal, gameStateDto.movement.colFinal);

		int clashResult = 1;
		var showData = pieceTarget != null && (isHost != piece.IsHost());
		if (showData)
		{
			piece.ShowData();
		}

		if (pieceTarget != null)
		{
			if (isHost != pieceTarget.IsHost())
			{
				pieceTarget.ShowData();
			}

			clashResult = ClashPieces(piece, pieceTarget);
		}

		var targetRow = gameStateDto.movement.rowFinal;
		var targetCol = gameStateDto.movement.colFinal;

		var initialPosition = piece.transform.position;
		var targetPosition = Board.GetWorldPosition(targetRow, targetCol);
		var direction = (targetPosition - initialPosition).normalized;
		var threshold = 0.1f;

		var speed = 1f / 0.5f;
		while ((piece.transform.position - targetPosition).sqrMagnitude > threshold)
		{
			piece.transform.position = piece.transform.position + direction * speed * Time.deltaTime;
			yield return null;
		}


		piece.transform.position = targetPosition;
		var tile = Board.GetTile(targetRow, targetCol);
		piece.SetTile(tile);


		if (pieceTarget != null)
		{
			if (clashResult == 1 || clashResult == 0)
			{
				yield return StartCoroutine(DestroyPiece(pieceTarget));
			}
			else if (isHost != pieceTarget.IsHost())
			{
				pieceTarget.HideData();
			}
		}

		if (clashResult == -1 || clashResult == 0)
		{
			yield return StartCoroutine(DestroyPiece(piece));
		}
		else if (showData)
		{
			piece.HideData();
		}

		OnEndMovementAnimation?.Invoke(this, EventArgs.Empty);

		movementCoroutine = null;
		yield return null;
	}

	private IEnumerator DestroyPiece(Piece piece)
	{
		Transform showPiecePosition = piece.GetIsHost() ? HostPieceShowPosition : GuestPieceShowPosition;
		Transform lastPiecePosition = piece.GetIsHost() ? HostPieceLastPosition : GuestPieceLastPosition;

		var threshold = 0.1f;
		var speed = 1f / 0.25f;
		var direction = (showPiecePosition.position - piece.transform.position).normalized; 
		while ((piece.transform.position - showPiecePosition.position).sqrMagnitude > threshold)
		{
			piece.transform.position = piece.transform.position + speed * Time.deltaTime * direction;
			yield return null;
		}

		yield return new WaitForSeconds(0.5f);

		speed = 1f / 0.25f;
		direction = (lastPiecePosition.position - piece.transform.position).normalized;
		while ((piece.transform.position - lastPiecePosition.position).sqrMagnitude > threshold)
		{
			piece.transform.position = piece.transform.position + speed * Time.deltaTime * direction;
			yield return null;
		}

		OnPieceCaptured?.Invoke(this, piece);
		Destroy(piece.gameObject, 1f);
		
		yield return null;
	}

	private void OnError(StrategoErrorDTO error)
	{
		Debug.Log(error.message);
	}

	public void LeaveGame()
	{
		var commData = CommData.GetInstance();

		if (commData.GetGameId() != 0)
		{
			var backendService = FindFirstObjectByType<BackendService>();
			var gameId = commData.GetGameId();
			var token = commData.GetToken();
			StartCoroutine(backendService.LeaveGame(gameId, token, OnLeftGame, OnError));
		}
		else
		{
			OnLeftGame(null);
		}
	}
	private void OnLeftGame(GameDTO gameDTO)
	{
		CommData.GetInstance().ResetData();
		SceneManager.LoadScene("Login", LoadSceneMode.Single);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
