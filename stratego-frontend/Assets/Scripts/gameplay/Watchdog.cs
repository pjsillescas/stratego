using System.Collections;
using UnityEngine;

public class Watchdog : MonoBehaviour
{
	private const float WAIT_TIME_SECONDS_TURN_CHANGE = 0.6f;
	private const float WAIT_TIME_SECONDS_END_GAME = 5f;


	private GameManager gameManager;
	private bool hasTurnChanged;
	private GameStateDTO lastGameStateDto;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		lastGameStateDto = null;
		gameManager = FindFirstObjectByType<GameManager>();
	}


	private void OnEnable()
	{
		GameManager.OnGameStateUpdated += OnGameStateUpdated;
	}

	private void OnDisable()
	{
		GameManager.OnGameStateUpdated -= OnGameStateUpdated;
	}

	private void OnGameStateUpdated(object sender, GameStateDTO gameStateDto)
	{
		if (gameStateDto.myTurn)
		{
			StartCoroutine(CheckForEndGame());
		}
		else
		{
			StartCoroutine(WaitForMyTurn());
		}
	}

	private IEnumerator WaitForMyTurn()
	{
		var waitForSeconds = new WaitForSeconds(WAIT_TIME_SECONDS_TURN_CHANGE);

		var commData = CommData.GetInstance();
		var gameId = commData.GetGameId();
		var token = commData.GetToken();

		hasTurnChanged = false;
		while (!hasTurnChanged)
		{
			StartCoroutine(BackendService.GetInstance().GetStatus(gameId, token, OnGameStateGot, OnError));
			yield return waitForSeconds;
		}

		yield return null;
	}

	private bool IsTheLastMovement(GameStateDTO gameStateDto)
	{
		var newMovement = gameStateDto?.movement;
		var lastMovement = lastGameStateDto?.movement;
		return newMovement != null && lastMovement != null //
			&& (newMovement.rowInitial == lastMovement.rowInitial && newMovement.colInitial == lastMovement.colInitial
			&& newMovement.rowFinal == lastMovement.rowFinal && newMovement.colFinal == lastMovement.colFinal
			&& gameStateDto.currentPlayer.id == lastGameStateDto.currentPlayer.id);
	}

	private void OnGameStateGot(GameStateDTO gameStateDto)
	{
		OnGameStateGotEndGame(gameStateDto);

		// Check that it is our turn and the samee gameState does not come for a second time
		if (gameStateDto.myTurn && !IsTheLastMovement(gameStateDto))
		{
			lastGameStateDto = gameStateDto;
			hasTurnChanged = true;
			gameManager.OnMovementAdded(gameStateDto);
		}
	}

	private IEnumerator CheckForEndGame()
	{
		var waitForSeconds = new WaitForSeconds(WAIT_TIME_SECONDS_END_GAME);

		var commData = CommData.GetInstance();
		var gameId = commData.GetGameId();
		var token = commData.GetToken();

		hasTurnChanged = false;
		while (!hasTurnChanged)
		{
			StartCoroutine(BackendService.GetInstance().GetStatus(gameId, token, OnGameStateGotEndGame, OnError));
			yield return waitForSeconds;
		}

		yield return null;
	}

	private void OnGameStateGotEndGame(GameStateDTO gameStateDto)
	{
		if (gameStateDto.guestPlayerId == 0 || gameStateDto.hostPlayerId == 0 || gameStateDto.phase == GamePhase.ABORTED)
		{
			// The other player quit the game
			hasTurnChanged = true;
			gameManager.LeaveGame();
		}
	}

	private void OnError(StrategoErrorDTO error)
	{
		Debug.Log(error.message);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
