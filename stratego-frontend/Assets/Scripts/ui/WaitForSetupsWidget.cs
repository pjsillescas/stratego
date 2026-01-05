using UnityEngine;
using System;
using System.Collections;

public class WaitForSetupsWidget : MonoBehaviour
{
	private BackendService backendService;
	private int gameId;
	private string token;
	private bool isSetupArrived;
	private Action OnSetupGot;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		;
	}

	public void Initialize(int gameId, string token, BackendService backendService, Action OnSetupGot)
	{
		this.gameId = gameId;
		this.token = token;
		this.backendService = backendService;
		this.OnSetupGot = OnSetupGot;

		StartCoroutine(CheckForSetupCoroutine());
	}

	private IEnumerator CheckForSetupCoroutine()
	{
		Debug.Log("check for setup");

		var waitForSeconds = new WaitForSeconds(10);

		isSetupArrived = false;
		while (!isSetupArrived)
		{
			StartCoroutine(backendService.GetStatus(gameId, token, OnSetupReceived, OnError));
			yield return waitForSeconds;
		}

		OnSetupGot?.Invoke();
		gameObject.SetActive(false);
		yield return null;
	}

	private void OnSetupReceived(GameStateDTO gameStateDto)
	{
		if (gameStateDto.phase == GamePhase.PLAYING)
		{
			isSetupArrived = true;
		}
	}

	private void OnError(StrategoErrorDTO error)
	{
		Debug.Log(error.message);
	}

}
