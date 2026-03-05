using NativeWebSocket;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// https://github.com/endel/NativeWebSocket.git#upm

public class ChatDebugClient : MonoBehaviour
{
	WebSocket websocket;

	public TMP_InputField inputField;
	public TextMeshProUGUI chatOutput;
	public Button sendButton;

	private string jwtToken;
	private string roomId;

	private bool isReconnecting = false;
	private readonly float reconnectDelay = 3f;

	

	async void Start()
	{
		sendButton.onClick.RemoveAllListeners();
		sendButton.onClick.AddListener(SendClick);

		var jwt = CommData.GetInstance().GetToken() ?? "token";
		websocket = new WebSocket("ws://localhost:8080/ws?roomId=123", new Dictionary<string, string> { { "Authorization", $"Bearer {jwt}" } });

		websocket.OnOpen += () =>
		{
			Debug.Log("Connected to server");
		};

		websocket.OnMessage += (bytes) =>
		{
			string msg = System.Text.Encoding.UTF8.GetString(bytes);
			Debug.Log($"received '{msg}'");
			chatOutput.text += "\n" + msg;
		};

		websocket.OnError += (e) =>
		{
			Debug.Log("Error: " + e);
		};

		websocket.OnClose += (e) =>
		{
			Debug.Log("Connection closed");
		};

		await websocket.Connect();
	}

	private void SendClick()
	{
		SendMessage();
	}

	public async void SendMessage()
	{
		Debug.Log("send message " + inputField.text);
		if (websocket.State == WebSocketState.Open)
		{
			await websocket.SendText(inputField.text);
			inputField.text = "";
		}
	}

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


	public void Initialize(string token, string room)
	{
		jwtToken = token;
		roomId = room;
		Connect();
	}

	async void Connect()
	{
		websocket = new WebSocket(
			$"ws://localhost:8080/ws?roomId={roomId}",
			new Dictionary<string, string>
			{
				{ "Authorization", "Bearer " + jwtToken }
			}
		);

		websocket.OnOpen += () =>
		{
			Debug.Log("Connected");
			isReconnecting = false;

			//SendSyncRequest();
		};

		websocket.OnClose += (code) =>
		{
			Debug.Log("Disconnected");
			AttemptReconnect();
		};

		websocket.OnError += (e) =>
		{
			Debug.Log("Error: " + e);
			AttemptReconnect();
		};

		websocket.OnMessage += (bytes) =>
		{
			string msg = System.Text.Encoding.UTF8.GetString(bytes);
			Debug.Log("Received: " + msg);
		};

		await websocket.Connect();
	}

	// Reconnect coroutine
	void AttemptReconnect()
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

	// sync request after reconnect
	async void SendSyncRequest()
	{
		if (websocket.State == WebSocketState.Open)
		{
			var msg = JsonUtility.ToJson(new SyncRequest());
			await websocket.SendText(msg);
		}
	}

	[System.Serializable]
	public class SyncRequest
	{
		public string type = "sync";
	}
}