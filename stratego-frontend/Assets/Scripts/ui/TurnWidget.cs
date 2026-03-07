using TMPro;
using UnityEngine;

public class TurnWidget : MonoBehaviour
{
	private const string YOUR_TURN = "Your turn";
	private const string OPPONENT_TURN = "Opponent's turn";
	private const string WAIT_FOR_OPPONENT = "Wait for your opponent";

	[SerializeField]
	private TextMeshProUGUI TurnText;
	[SerializeField]
	private WaitWidget WaitWidget;

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
		/*
		if (gameStateDTO.myTurn)
		{
			WaitWidget.Deactivate();
		}
		else
		{
			WaitWidget.Activate(WAIT_FOR_OPPONENT);
		}
		*/
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		WaitWidget.Deactivate();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
