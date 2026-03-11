using NativeWebSocket;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatWidget : MonoBehaviour
{
	public static event EventHandler<string> OnMessageReceived;

	[SerializeField]
	private TMP_InputField InputField;
	[SerializeField]
	private Button SendButton;
	[SerializeField]
	private GameObject ChatTextItemPrefab;
	[SerializeField]
	private Transform MessageBoard;

	private string token;
	private string roomId;

	private WebSocket websocket;
	private bool isReconnecting = false;
	private readonly float reconnectDelay = 3f;
	private BackendService backendService;


	void Start()
	{
		SendButton.onClick.RemoveAllListeners();
		SendButton.onClick.AddListener(SendClick);
		InputField.onSubmit.RemoveAllListeners();
		InputField.onSubmit.AddListener(InputSubmit);
		backendService = FindFirstObjectByType<BackendService>();

		var commData = CommData.GetInstance();
		token = commData.GetToken() ?? "token";
		roomId = commData.GetGameId().ToString() ?? "noroom";
		websocket = null;


		Connect();
	}

	private void InputSubmit(string text)
	{
		SendClick();
	}

	private void SendClick()
	{
		SendMessage();
	}

	public async void SendMessage()
	{
		Debug.Log("send message " + InputField.text);
		if (websocket.State == WebSocketState.Open)
		{
			await websocket.SendText(InputField.text);
			InputField.text = "";
		}

		InputField.Select();
		InputField.ActivateInputField();
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


	public void Initialize(string token, string roomId)
	{
		this.token = token;
		this.roomId = roomId;
		Connect();
	}

	async void Connect()
	{
		if (websocket != null && (websocket.State == WebSocketState.Open || websocket.State == WebSocketState.Connecting))
		{
			return;
		}
		websocket = backendService.BuildChatWebSocket(token, roomId, MessageReceived, OnReconnect);

		await websocket.Connect();
	}

	private void MessageReceived(string message)
	{
		OnMessageReceived?.Invoke(this, message);

		Debug.Log($"received '{message}'");
		var messageDto = JsonUtility.FromJson<ChatMessageDTO>(message);
		var textItem = Instantiate(ChatTextItemPrefab, MessageBoard).GetComponent<ChatItemWidget>();

		var commData = CommData.GetInstance();
		Color textColor;
		if (commData.GetMyUsername() == messageDto.player)
		{
			textColor = commData.GetIsHost() ? Color.blue : Color.red;
		}
		else
		{
			textColor = commData.GetIsHost() ? Color.red : Color.blue;
		}
		textItem.Init(messageDto.player, messageDto.message, textColor);
		//chatOutput.text += "\n" + message;
	}

	// Reconnect coroutine
	void OnReconnect()
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

}
