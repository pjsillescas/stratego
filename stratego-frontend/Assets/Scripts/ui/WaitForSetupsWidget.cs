using UnityEngine;
using System;
using System.Collections;

public class WaitForSetupsWidget : MonoBehaviour
{
	private bool isSetupArrived;
	private Action OnSetupGot;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		;
	}

	public void Initialize(Action OnSetupGot)
	{
		this.OnSetupGot = OnSetupGot;

		StartCoroutine(CheckForSetupCoroutine());
	}

	private IEnumerator CheckForSetupCoroutine()
	{
		Debug.Log("check for setup");

		var waitForSeconds = new WaitForSeconds(10);

		var commData = CommData.GetInstance();
		var gameId = commData.GetGameId();
		var token = commData.GetToken();
		isSetupArrived = false;
		while (!isSetupArrived)
		{
			StartCoroutine(BackendService.GetInstance().GetStatus(gameId, token, OnSetupReceived, OnError));
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
