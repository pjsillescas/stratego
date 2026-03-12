using System.Collections;
using UnityEngine;

public class Watchdog : MonoBehaviour
{
	private GameManager gameManager;
	private GameStateDTO lastGameStateDto;

	private string token;
	private int gameId;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		lastGameStateDto = null;
		gameManager = FindFirstObjectByType<GameManager>();

		token = CommData.GetInstance().GetToken();
		gameId = CommData.GetInstance().GetGameId();
	}

	private void OnEnable()
	{
		NotificationService.OnNotificationReceived += OnNotificationReceived;
	}

	private void OnDisable()
	{
		NotificationService.OnNotificationReceived -= OnNotificationReceived;
	}

	private void OnNotificationReceived(object sender, NotificationDTO notification)
	{
		if (notification.message == "Add movement")
		{
			StartCoroutine(GetDelayedStatus());
		}
		else
		{
			Debug.Log($"otra cosa '{notification.message}'");
		}
	}

	private IEnumerator GetDelayedStatus()
	{
		yield return new WaitForSeconds(0.5f);
		yield return BackendService.GetInstance().GetStatus(gameId, token, OnGameStateGot, OnError);
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
		//var json = JsonUtility.ToJson(gameStateDto);
		var movement = gameStateDto.movement;
		Debug.Log($"Got state from notification {gameStateDto.movement.rank} from ({movement.rowInitial},{movement.colInitial}) to ({movement.rowFinal},{movement.colFinal})");
		OnGameStateGotEndGame(gameStateDto);

		// Check that it is our turn and the same gameState does not come for a second time
		if (!IsTheLastMovement(gameStateDto))
		{
			lastGameStateDto = gameStateDto;
			gameManager.OnMovementAdded(gameStateDto);
		}
	}

	private void OnGameStateGotEndGame(GameStateDTO gameStateDto)
	{
		if (gameStateDto.guestPlayerId == 0 || gameStateDto.hostPlayerId == 0 || gameStateDto.phase == GamePhase.ABORTED)
		{
			// The other player quit the game
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
		;
	}

}
