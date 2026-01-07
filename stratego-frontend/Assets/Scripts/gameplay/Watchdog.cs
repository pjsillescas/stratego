using System.Collections;
using UnityEngine;

public class Watchdog : MonoBehaviour
{
	private const float WAIT_TIME_SECONDS = 2f;


	private GameManager gameManager;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		gameManager = FindFirstObjectByType<GameManager>();
	}


	private void OnEnable()
	{
		GameManager.OnGameStateUpdated += OnGameStateUpdated;
	}

	private void OnDisable()
	{
		GameManager.OnGameStateUpdated -= OnGameStateUpdated;
	}

	private void OnGameStateUpdated(object sender, GameStateDTO gameStateDto)
	{
		if (gameStateDto.myTurn)
		{
			;
		}
		else
		{
			StartCoroutine(WaitForMyTurn());
		}
	}

	private IEnumerator WaitForMyTurn()
	{
		var waitForSeconds = new WaitForSeconds(WAIT_TIME_SECONDS);

		var commData = CommData.GetInstance();
		var gameId = commData.GetGameId();
		var token = commData.GetToken();
		
		var isGuestArrived = false;
		while (!isGuestArrived)
		{
			StartCoroutine(BackendService.GetInstance().GetStatus(gameId, token, OnGameStateGot, OnError));
			yield return waitForSeconds;
		}

		yield return null;
	}

	private void OnGameStateGot(GameStateDTO gameStateDto)
	{
		gameManager.OnMovementAdded(gameStateDto);
	}

	private void OnError(StrategoErrorDTO error)
	{
		Debug.Log(error.message);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
