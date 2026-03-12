using System;
using System.Collections;
using UnityEngine;

public class WaitForSetupsWidget : MonoBehaviour
{
	private const float WAIT_TIME_SECONDS = 2f;

	private GameStateDTO gameStateDto;
	private Action<GameStateDTO> OnSetupGot;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		;
	}

	private void OnEnable()
	{
		NotificationService.OnNotificationReceived += OnNotificationReceived;
	}
	
	private void OnDisable()
	{
		NotificationService.OnNotificationReceived -= OnNotificationReceived;
	}

	public void Initialize(Action<GameStateDTO> OnSetupGot)
	{
		this.OnSetupGot = OnSetupGot;

		//StartCoroutine(CheckForSetupCoroutine());
	}

	private IEnumerator CheckForSetupCoroutine()
	{
		var waitForSeconds = new WaitForSeconds(WAIT_TIME_SECONDS);

		var commData = CommData.GetInstance();
		var gameId = commData.GetGameId();
		var token = commData.GetToken();
		gameStateDto = null;
		while (gameStateDto == null)
		{
			StartCoroutine(BackendService.GetInstance().GetStatus(gameId, token, OnSetupReceived, OnError));
			yield return waitForSeconds;
		}

		OnSetupGot?.Invoke(gameStateDto);
		gameObject.SetActive(false);
		yield return null;
	}

	private void OnNotificationReceived(object sender, NotificationDTO notification)
	{
		if (notification.message == "Add setup" && notification.gamePhase == GamePhase.PLAYING)
		{
			StartCoroutine(GetDelayedStatusCoroutine());
		}
	}

	private IEnumerator GetDelayedStatusCoroutine()
	{
		yield return new WaitForSeconds(0.5f);
		var commData = CommData.GetInstance();
		var gameId = commData.GetGameId();
		var token = commData.GetToken();
		yield return BackendService.GetInstance().GetStatus(gameId, token, OnSetupReceived, OnError);
	}

	private void OnSetupReceived(GameStateDTO gameStateDto)
	{
		Debug.Log($"status received in setup widget '{gameStateDto.phase}'");

		if (gameStateDto.phase == GamePhase.PLAYING)
		{
			this.gameStateDto = gameStateDto;

			OnSetupGot?.Invoke(gameStateDto);
			gameObject.SetActive(false);
		}
	}

	private void OnError(StrategoErrorDTO error)
	{
		Debug.Log(error.message);
	}

}
