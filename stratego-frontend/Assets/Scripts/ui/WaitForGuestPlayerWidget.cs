using System;
using System.Collections;
using UnityEngine;

public class WaitForGuestPlayerWidget : MonoBehaviour
{
	private const float WAIT_TIME_SECONDS = 2f;

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

	public void Initialize(Action OnGuestGot)
	{
		this.OnGuestGot = OnGuestGot;
		
		StartCoroutine(CheckForGuestPlayerCoroutine());
	}

	private IEnumerator CheckForGuestPlayerCoroutine()
	{
		Debug.Log("check for guest");

		var waitForSeconds = new WaitForSeconds(WAIT_TIME_SECONDS);

		isGuestArrived = false;
		while (!isGuestArrived)
		{
			var commData = CommData.GetInstance();
			var gameId = commData.GetGameId();
			var token = commData.GetToken();
			StartCoroutine(BackendService.GetInstance().GetGame(gameId, token, OnGameGot, OnError));
			yield return waitForSeconds;
		}

		OnGuestGot?.Invoke();
		yield return null;
	}

	private void OnGameGot(GameExtendedDTO gameExtendedDto)
	{
		if (gameExtendedDto.guest != null)
		{
			var commData = CommData.GetInstance();
			var host = gameExtendedDto.host;
			var guest = gameExtendedDto.guest;
			//commData.SetOpponentUsername((guest.username == commData.GetMyUsername()) ? host.username : guest.username);
			commData.SetOpponentUsername(guest.username);
			Debug.Log("opponent " + commData.GetOpponentUsername());

			isGuestArrived = true;
		}
	}

	private void OnError(StrategoErrorDTO error)
	{
		Debug.Log(error.message);
	}

}
