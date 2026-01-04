using System;
using System.Collections;
using UnityEngine;

public class WaitForGuestPlayerWidget : MonoBehaviour
{
	private BackendService backendService;
	private int gameId;
	private string token;
	private bool isGuestArrived;
	private Action OnGuestGot;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		/*
		gameId = PlayerPrefsManager.GetGameId();
		token = PlayerPrefsManager.GetToken();
		backendService = FindFirstObjectByType<BackendService>();
		StartCoroutine(CheckForGuestPlayerCoroutine());
		*/
	}

	public void Initialize(int gameId, string token, BackendService backendService, Action OnGuestGot)
	{
		this.gameId = gameId;
		this.token = token;
		this.backendService = backendService;
		this.OnGuestGot = OnGuestGot;
		
		StartCoroutine(CheckForGuestPlayerCoroutine());
	}

	private IEnumerator CheckForGuestPlayerCoroutine()
	{
		Debug.Log("check for guest");

		var waitForSeconds = new WaitForSeconds(10);

		isGuestArrived = false;
		while (!isGuestArrived)
		{
			StartCoroutine(backendService.GetGame(gameId, token, OnGameGot, OnError));
			yield return waitForSeconds;
		}

		OnGuestGot?.Invoke();
		yield return null;
	}

	private void OnGameGot(GameDTO gameDto)
	{
		if (gameDto.guest != null)
		{
			isGuestArrived = true;
		}
	}

	private void OnError(StrategoErrorDTO error)
	{
		Debug.Log(error.message);
	}

}
