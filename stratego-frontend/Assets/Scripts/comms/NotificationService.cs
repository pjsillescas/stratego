using NativeWebSocket;
using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;

public class NotificationService : MonoBehaviour
{
	public static event EventHandler<NotificationDTO> OnNotificationReceived;

	private WebSocket websocket;

	private string token;
	private string roomId;
	private int gameId;
	private bool isReconnecting = false;
	private readonly float reconnectDelay = 3f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		token = CommData.GetInstance().GetToken();
		gameId = CommData.GetInstance().GetGameId();
		roomId = gameId.ToString();
		Connect();
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

	private void OnMessageReceived(string message)
	{
		Debug.Log($"received message '{message}'");
		//var notification = JsonUtility.FromJson<NotificationDTO>(message);
		var notification = JsonConvert.DeserializeObject<NotificationDTO>(message);

		if (notification == null)
		{
			Debug.Log($"bad notification '{message}'");
			return;
		}

		OnNotificationReceived?.Invoke(this, notification);
	}

	// Update is called once per frame
	void Update()
	{
#if !UNITY_WEBGL || UNITY_EDITOR
		websocket.DispatchMessageQueue();
#endif
	}

	private async void OnApplicationQuit()
	{
		await websocket.Close();
	}
}
