using TMPro;
using UnityEngine;

public class TurnWidget : MonoBehaviour
{
	private const string YOUR_TURN = "Your turn";
	private const string OPPONENT_TURN = "Opponent's turn";

	[SerializeField]
	private TextMeshProUGUI TurnText;

	private void OnEnable()
	{
		GameManager.OnGameStateUpdated += OnGameStateUpdated;
	}

	private void OnDisable()
	{
		GameManager.OnGameStateUpdated -= OnGameStateUpdated;
	}

	private void OnGameStateUpdated(object sender, GameStateDTO gameStateDTO)
	{
		TurnText.text = gameStateDTO.myTurn ? YOUR_TURN : OPPONENT_TURN;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}
