using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
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

	public Piece GetSelectedPiece() => selectedPiece;
	public Tile GetSelectedTile() => selectedTile;

	private void Awake()
	{
		actions = new InputActions();
	}

	private void OnEnable()
	{
		actions.Enable();
	}

	private void OnDisable()
	{
		actions.Disable();
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		gameManager = FindFirstObjectByType<GameManager>();
		mainCamera = Camera.main;
		isHost = gameManager.GetIsHost();

		highlightedPiece = null;
		selectedPiece = null;
	}

	private const float MAX_DISTANCE = 20f;

	// Update is called once per frame
	void Update()
	{
		var mousePosition = Mouse.current.position.ReadValue();
		var mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

		var ray = new Ray(mouseWorldPosition, Vector3.down);

		var interact = actions.Player.Interact.WasPressedThisFrame();

		if (interact)
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
	}

	private bool ProcessSelectedTile(Ray ray)
	{
		var result = Physics.Raycast(ray, out RaycastHit hitInfo, MAX_DISTANCE, TileLayer);

		if (result && hitInfo.collider.gameObject.TryGetComponent(out Tile tile) && IsValidTile(tile))
		{
			if (selectedTile != null && !tile.Equals(selectedTile))
			{
				selectedTile.Deselect();
			}
			selectedTile = tile;
			selectedTile.Select();

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
			}
			else
			{
				;
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
		return piece.IsHost() == isHost;
	}

	private bool IsValidTile(Tile tile)
	{
		return selectedPiece != null && IsValidTileForHighlight(tile);
	}

	private int GetDistance(Tile tile1, Tile tile2)
	{
		var d = Mathf.Abs(tile1.GetRow() - tile2.GetRow()) + Mathf.Abs(tile1.GetCol() - tile2.GetCol());
		Debug.Log($"distance from {tile1} to {tile2} is {d}");
		return Mathf.Abs(tile1.GetRow() - tile2.GetRow()) + Mathf.Abs(tile1.GetCol() - tile2.GetCol());
	}

	private bool IsStraightLine(Tile tileOrigin, Tile tileTarget)
	{
		if (tileOrigin.GetCol() == tileTarget.GetCol())
		{
			var minRow = tileOrigin.GetRow() < tileTarget.GetRow() ? tileOrigin.GetRow() : tileTarget.GetRow();
			var maxRow = tileOrigin.GetRow() > tileTarget.GetRow() ? tileOrigin.GetRow() : tileTarget.GetRow();
			var col = tileTarget.GetCol();
			for (int row = minRow + 1; row < maxRow - 1; row++)
			{
				if (gameManager.GetPieceAtCoordinates(row, col) != null)
				{
					return false;
				}
			}

			return true;
		}
		else if(tileOrigin.GetRow() == tileTarget.GetRow())
		{
			var minCol = tileOrigin.GetCol() < tileTarget.GetCol() ? tileOrigin.GetCol() : tileTarget.GetCol();
			var maxCol = tileOrigin.GetCol() > tileTarget.GetCol() ? tileOrigin.GetCol() : tileTarget.GetCol();
			var row = tileTarget.GetRow();
			for (int col = minCol + 1; row < maxCol - 1; col++)
			{
				if (gameManager.GetPieceAtCoordinates(row, col) != null)
				{
					return false;
				}
			}

			return true;
		}

		return false;
	}

	private bool IsValidTileForHighlight(Tile tile)
	{
		if (selectedPiece != null)
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
		return piece == null || piece.GetIsHost() != isHost;
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
}
