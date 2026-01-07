using UnityEngine;
using UnityEngine.UI;

public class ConfirmMovementWidget : MonoBehaviour
{
	[SerializeField]
	private Button ConfirmMovementButton;

	private GameManager gameManager;
	private Tile selectedTile;
	private Piece selectedPiece;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		gameManager = FindFirstObjectByType<GameManager>();
		ConfirmMovementButton.enabled = false;
		ConfirmMovementButton.onClick.AddListener(ConfirmMovementClick);
	}

	private void OnEnable()
	{
		InputManager.OnPieceSelected += OnPieceSelected;
		InputManager.OnTileSelected += OnTileSelected;
	}

	private void OnPieceSelected(object sender, Piece piece)
	{
		ConfirmMovementButton.enabled = false;
		selectedPiece = piece;
	}

	private void OnTileSelected(object sender, Tile tile)
	{
		ConfirmMovementButton.enabled = true;
		selectedTile = tile;
	}

	private void ConfirmMovementClick()
	{
		var movement = new StrategoMovementDTO() {
			rowInitial = selectedPiece.GetTile().GetRow(),
			colInitial = selectedPiece.GetTile().GetCol(),
			rank = selectedPiece.GetRank(),

			rowFinal = selectedTile.GetRow(),
			colFinal = selectedTile.GetCol(),
		};

		gameManager.SendMovement(movement);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
