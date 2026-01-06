using UnityEngine;
using System;
using System.Collections;

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

	public void Initialize(Action<GameStateDTO> OnSetupGot)
	{
		this.OnSetupGot = OnSetupGot;

		StartCoroutine(CheckForSetupCoroutine());
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

	private void OnSetupReceived(GameStateDTO gameStateDto)
	{
		if (gameStateDto.phase == GamePhase.PLAYING)
		{
			this.gameStateDto = gameStateDto;
		}
	}

	private void OnError(StrategoErrorDTO error)
	{
		Debug.Log(error.message);
	}

}
