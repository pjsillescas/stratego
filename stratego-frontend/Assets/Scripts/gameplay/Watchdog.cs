using System.Collections;
using UnityEngine;

public class Watchdog : MonoBehaviour
{
	private const float WAIT_TIME_SECONDS = 2f;


	private GameManager gameManager;
	private bool hasTurnChanged;

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

		Debug.Log("watchdog coroutine ending");
		var commData = CommData.GetInstance();
		var gameId = commData.GetGameId();
		var token = commData.GetToken();
		
		hasTurnChanged = false;
		while (!hasTurnChanged)
		{
			Debug.Log("watchdog turn");
			StartCoroutine(BackendService.GetInstance().GetStatus(gameId, token, OnGameStateGot, OnError));
			yield return waitForSeconds;
		}

		Debug.Log("watchdog coroutine ending");
		yield return null;
	}

	private void OnGameStateGot(GameStateDTO gameStateDto)
	{
		if (gameStateDto.myTurn)
		{
			gameManager.OnMovementAdded(gameStateDto);
			hasTurnChanged = true;
		}
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
