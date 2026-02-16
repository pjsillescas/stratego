using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConsoleLog : MonoBehaviour
{
	private const int MESSAGE_THRESHOLD = 200;

	[SerializeField]
	private GameObject LogItemPrefab;
	[SerializeField]
	private Transform ContentTransform;

	private List<ConsoleLogItem> messages;
	private bool isHost;

	public void AddMessage(string message)
	{
		var item = Instantiate(LogItemPrefab, ContentTransform).GetComponent<ConsoleLogItem>();
		item.transform.SetAsFirstSibling();
		item.AddMessage(message);
		messages.Add(item);

		if (messages.Count > MESSAGE_THRESHOLD)
		{
			var lastElement = messages.LastOrDefault();
			messages.Remove(lastElement);
			Destroy(lastElement.gameObject);
		}
	}

	void Awake()
	{
		messages = new();
		Application.logMessageReceived += LogHandler;
	}

	private void Start()
	{
		isHost = CommData.GetInstance().GetIsHost();

		InputManager.OnDebugConsoleToggle += ToggleConsole;
		GameManager.OnGameStateUpdated += OnGameStateUpdated;
		GameManager.OnPieceCaptured += OnPieceCaptured;
		
		gameObject.SetActive(false);
	}

	private void ToggleConsole(object sender, EventArgs args)
	{
		gameObject.SetActive(!gameObject.activeInHierarchy);
	}

	private void OnDestroy()
	{
		Application.logMessageReceived -= LogHandler;
		InputManager.OnDebugConsoleToggle -= ToggleConsole;
		GameManager.OnGameStateUpdated -= OnGameStateUpdated;
		GameManager.OnPieceCaptured -= OnPieceCaptured;
	}

	private void OnPieceCaptured(object sender, Piece piece)
	{
		var player = (piece.GetIsHost() == isHost) ? "You" : "Opponent";
		var rank = piece.GetRank();
		var captureMessage = $"{player} captured {rank}";
		AddMessage(captureMessage);
	}

	private string GetMovementMessage(GameStateDTO gameState)
	{
		var player = (!gameState.myTurn) ? "You" : "Opponent";
		var rank = gameState.movement.rank;
		var rowInit = gameState.movement.rowInitial;
		var colInit = gameState.movement.colInitial;
		var rowEnd = gameState.movement.rowFinal;
		var colEnd = gameState.movement.colFinal;
		var dead = gameState.movement.result == null ? "" : 
			gameState.movement.result.Select(res => $"{res.rank} ({res.host})").Aggregate("", (acc, val) => $"{val},{acc}");
		
		return $"{player} moved {rank} from ({rowInit},{colInit}) to ({rowEnd},{colEnd}) [{dead}]";
	}
	
	private void OnGameStateUpdated(object sender, GameStateDTO gameState)
	{
		if (gameState != null && gameState.movement != null)
		{
			AddMessage(GetMovementMessage(gameState));
		}
	}

	private void LogHandler(string condition, string stackTrace, LogType type)
	{
		AddMessage(condition);
	}

}
