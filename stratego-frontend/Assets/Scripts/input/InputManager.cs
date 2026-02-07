using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
	public static event EventHandler<Piece> OnPieceSelected;
	public static event EventHandler<Tile> OnTileSelected;

	public static event EventHandler OnDebugConsoleToggle;
	
	[SerializeField]
	private LayerMask PieceLayer;
	[SerializeField]
	private LayerMask TileLayer;

	private InputActions actions;
	private Camera mainCamera;
	private bool isHost;

	private Piece selectedPiece;
	private Piece highlightedPiece;
	private Tile selectedTile;
	private Tile highlightedTile;
	private GameManager gameManager;
	private bool isMyTurn;

	public Piece GetSelectedPiece() => selectedPiece;
	public Tile GetSelectedTile() => selectedTile;

	private void Awake()
	{
		actions = new InputActions();
	}

	private void OnEnable()
	{
		actions.Enable();
		GameManager.OnGameStateUpdated += OnGameStateUpdated;
		GameManager.OnEndMovementAnimation += ResetSelections;
	}

	private void OnDisable()
	{
		actions.Disable();
		GameManager.OnGameStateUpdated -= OnGameStateUpdated;
		GameManager.OnEndMovementAnimation -= ResetSelections;
	}

	private void OnGameStateUpdated(object sender, GameStateDTO gameStateDTO)
	{
		isMyTurn = gameStateDTO.myTurn;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		gameManager = FindFirstObjectByType<GameManager>();
		mainCamera = Camera.main;
		isHost = gameManager.GetIsHost();

		ResetSelections(this, EventArgs.Empty);
	}

	private const float MAX_DISTANCE = 20f;

	// Update is called once per frame
	void Update()
	{
		var mousePosition = Mouse.current.position.ReadValue();
		var mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

		var ray = new Ray(mouseWorldPosition, Vector3.down);

		var interact = actions.Player.Interact.WasPressedThisFrame();

		if (interact && isMyTurn)
		{
			if (!ProcessSelectedTile(ray))
			{
				ProcessSelectedPiece(ray);
			}
		}
		else
		{
			ProcessHighlightTile(ray);
			ProcessHighlightedPiece(ray);
		}

		if (actions.Player.ToggleConsole.WasPressedThisFrame())
		{
			Debug.Log("toggle console input");
			OnDebugConsoleToggle?.Invoke(this, EventArgs.Empty);
		}
	}

	private bool ProcessSelectedTile(Ray ray)
	{
		var result = Physics.Raycast(ray, out RaycastHit hitInfo, MAX_DISTANCE, TileLayer);

		if (result && hitInfo.collider.gameObject.TryGetComponent(out Tile tile) && IsValidTile(tile))
		{
			if (selectedTile != null)
			{
				if (!tile.Equals(selectedTile))
				{
					selectedTile.Deselect();

					selectedTile = tile;
					selectedTile.Select();
					OnTileSelected?.Invoke(this, tile);
				}
				else
				{
					ConfirmMovement();
				}
			}
			else
			{
				selectedTile = tile;
				selectedTile.Select();
				OnTileSelected?.Invoke(this, tile);
			}

			return true;
		}

		if (selectedTile != null)
		{
			selectedTile.Deselect();
			selectedTile = null;
		}
		return false;
	}

	private void ProcessSelectedPiece(Ray ray)
	{
		var resultPiece = Physics.Raycast(ray, out RaycastHit hitInfoPiece, MAX_DISTANCE, PieceLayer);

		if (resultPiece && hitInfoPiece.collider.gameObject.TryGetComponent(out Piece piece))
		{
			if (IsValidPiece(piece))
			{
				if (selectedPiece != null && !piece.Equals(selectedPiece))
				{
					selectedPiece.Deselect();
				}
				selectedPiece = piece;
				selectedPiece.Select();
				OnPieceSelected?.Invoke(this, piece);
			}
		}
		else
		{
			if (selectedPiece != null)
			{
				selectedPiece.Deselect();
				selectedPiece = null;
			}
		}
	}

	private bool IsValidPiece(Piece piece)
	{
		return piece.IsHost() == isHost && piece.GetRank() != Rank.DISABLED;
	}

	private bool IsValidTile(Tile tile)
	{
		return selectedPiece != null && IsValidTileForHighlight(tile);
	}

	private int GetDistance(Tile tile1, Tile tile2)
	{
		return Mathf.Abs(tile1.GetRow() - tile2.GetRow()) + Mathf.Abs(tile1.GetCol() - tile2.GetCol());
	}

	private bool IsStraightLine(Tile tileOrigin, Tile tileTarget)
	{
		if (tileOrigin.GetCol() == tileTarget.GetCol())
		{
			/*
			var minRow = tileOrigin.GetRow() < tileTarget.GetRow() ? tileOrigin.GetRow() : tileTarget.GetRow();
			var maxRow = tileOrigin.GetRow() > tileTarget.GetRow() ? tileOrigin.GetRow() : tileTarget.GetRow();
			var col = tileTarget.GetCol();
			for (int row = minRow + 1; row < maxRow; row++)
			{
				if (gameManager.GetPieceAtCoordinates(row, col) != null)
				{
					return false;
				}
			}

			return true;
			*/
			
			var col = tileTarget.GetCol();
			
			var originRow = tileOrigin.GetRow();
			var targetRow = tileTarget.GetRow();
			if (originRow < targetRow)
			{
				for (int row = originRow + 1; row <= targetRow; row++)
				{
					if (gameManager.GetPieceAtCoordinates(row, col) != null && row < targetRow)
					{
						return false;
					}
				}
			}
			else
			{
				for (int row = originRow - 1; row >= targetRow; row--)
				{
					if (gameManager.GetPieceAtCoordinates(row, col) != null && row < targetRow)
					{
						return false;
					}
				}
			}

			return true;

		}
		else if (tileOrigin.GetRow() == tileTarget.GetRow())
		{
			/*
			var minCol = tileOrigin.GetCol() < tileTarget.GetCol() ? tileOrigin.GetCol() : tileTarget.GetCol();
			var maxCol = tileOrigin.GetCol() > tileTarget.GetCol() ? tileOrigin.GetCol() : tileTarget.GetCol();
			var row = tileTarget.GetRow();
			for (int col = minCol + 1; row < maxCol; col++)
			{
				if (gameManager.GetPieceAtCoordinates(row, col) != null)
				{
					return false;
				}
			}

			return true;
			*/

			var row = tileTarget.GetRow();
			
			var originCol = tileOrigin.GetCol();
			var targetCol = tileTarget.GetCol();

			if (originCol < targetCol)
			{
				for (int col = originCol + 1; col <= targetCol; col++)
				{
					if (gameManager.GetPieceAtCoordinates(row, col) != null && col < targetCol)
					{
						return false;
					}
				}
			}
			else
			{
				for (int col = originCol - 1; col >= targetCol; col--)
				{
					if (gameManager.GetPieceAtCoordinates(row, col) != null && col > targetCol)
					{
						return false;
					}
				}
			}

			return true;

		}

		return false;
	}

	private bool IsDisabledTile(Tile tile)
	{
		var piece = gameManager.GetPieceAtCoordinates(tile.GetRow(), tile.GetCol());
		return piece != null && piece.GetRank() == Rank.DISABLED;
	}

	private bool IsValidTileForHighlight(Tile tile)
	{
		if (selectedPiece != null && !IsDisabledTile(tile))
		{
			var immobileRanks = new List<Rank>() { Rank.BOMB, Rank.FLAG };
			var selectedPieceTile = selectedPiece.GetTile();
			var rank = selectedPiece.GetRank();
			if (immobileRanks.Contains(rank))
			{
				return false;
			}
			var distance = GetDistance(tile, selectedPieceTile);
			return ((rank == Rank.SCOUT) ? IsStraightLine(selectedPieceTile, tile) : distance == 1) &&
				IsValidTargetPieceInTile(tile);
		}

		return false;
	}

	private bool IsValidTargetPieceInTile(Tile tile)
	{
		var piece = gameManager.GetPieceAtCoordinates(tile.GetRow(), tile.GetCol());
		return piece == null || piece.GetIsHost() != isHost && piece.GetRank() != Rank.DISABLED;
	}

	private void ProcessHighlightTile(Ray ray)
	{
		var result = Physics.Raycast(ray, out RaycastHit hitInfo, MAX_DISTANCE, TileLayer);

		if (result && hitInfo.collider.gameObject.TryGetComponent(out Tile tile) && IsValidTileForHighlight(tile))
		{
			if (highlightedTile != null && !tile.Equals(highlightedTile))
			{
				highlightedTile.Darken();
			}
			highlightedTile = tile;
			highlightedTile.Highlight();

			if (highlightedPiece != null)
			{
				highlightedPiece.Darken();
				highlightedPiece = null;
			}

		}
	}

	private void ProcessHighlightedPiece(Ray ray)
	{
		var result = Physics.Raycast(ray, out RaycastHit hitInfo, MAX_DISTANCE, PieceLayer);

		if (result && hitInfo.collider.gameObject.TryGetComponent(out Piece piece) && IsValidPiece(piece))
		{
			if (highlightedPiece != null && !piece.Equals(highlightedPiece))
			{
				highlightedPiece.Darken();
			}
			highlightedPiece = piece;
			highlightedPiece.Highlight();
		}
	}

	private void ConfirmMovement()
	{
		var movement = new StrategoMovementDTO()
		{
			rowInitial = selectedPiece.GetTile().GetRow(),
			colInitial = selectedPiece.GetTile().GetCol(),
			rank = selectedPiece.GetRank(),

			rowFinal = selectedTile.GetRow(),
			colFinal = selectedTile.GetCol(),
		};

		gameManager.SendMovement(movement);
	}

	private void ResetSelections(object sender, EventArgs args)
	{
		if (selectedPiece != null)
		{
			selectedPiece.Deselect();
		}
		selectedPiece = null;

		if (highlightedPiece != null)
		{
			highlightedPiece.Deselect();
		}
		highlightedPiece = null;

		if (selectedTile != null)
		{
			selectedTile.Deselect();
		}
		selectedTile = null;

		if (highlightedTile != null)
		{
			highlightedTile.Deselect();
		}
		highlightedTile = null;
	}
}
