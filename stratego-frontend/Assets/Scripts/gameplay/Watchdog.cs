using NativeWebSocket;
using System.Collections;
using UnityEngine;

public class Watchdog : MonoBehaviour
{
	private GameManager gameManager;
	private GameStateDTO lastGameStateDto;
	private WebSocket websocket;

	private string token;
	private string roomId;
	private int gameId;
	private bool isReconnecting = false;
	private readonly float reconnectDelay = 3f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		lastGameStateDto = null;
		gameManager = FindFirstObjectByType<GameManager>();

		token = CommData.GetInstance().GetToken();
		gameId = CommData.GetInstance().GetGameId();
		roomId = gameId.ToString();
		//websocket = BackendService.GetInstance().BuildNotificationWebSocket(token, roomId, OnMessageReceived, OnReconnect);
		Connect();
	}

	private void OnMessageReceived(string message)
	{
		Debug.Log($"received message '{message}'");
		var notification = JsonUtility.FromJson<NotificationDTO>(message);
		
		if (notification == null)
		{
			Debug.Log($"bad notification '{message}'");
			return;
		}

		if (notification.message == "Add movement")
		{
			StartCoroutine(BackendService.GetInstance().GetStatus(gameId, token, OnGameStateGot, OnError));
		}
		else
		{
			Debug.Log($"otra cosa '{notification.message}'");
		}
	}

	private void OnReconnect()
	{
		if (isReconnecting) return;

		isReconnecting = true;
		StartCoroutine(ReconnectRoutine());
	}

	IEnumerator ReconnectRoutine()
	{
		while (isReconnecting)
		{
			Debug.Log("Trying reconnect...");
			Connect();
			yield return new WaitForSeconds(reconnectDelay);
		}
	}

	async void Connect()
	{
		if (websocket != null && (websocket.State == WebSocketState.Open || websocket.State == WebSocketState.Connecting))
		{
			return;
		}

		websocket = BackendService.GetInstance().BuildNotificationWebSocket(token, roomId, OnMessageReceived, OnReconnect);

		await websocket.Connect();
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

	}
}
