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
		mainCamera = Camera.main;
		isHost = PlayerPrefsManager.GetIsHost();
		isHost = true;

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
			/*
			if(selectedPiece != null)
			{
				selectedPiece.Deselect();
				selectedPiece = null;
			}
			*/

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
	private bool IsValidTileForHighlight(Tile tile)
	{
		return true;
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
