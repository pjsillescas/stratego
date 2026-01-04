using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameListWidget : MonoBehaviour
{
	[SerializeField]
	private GameObject GameItemPrefab;

	[SerializeField]
	private GameObject GameListContent;

	[SerializeField]
	private Button CreateGameButton;
	
	[SerializeField]
	private WaitForGuestPlayerWidget WaitWidget;

	private BackendService backendService;

	private void OnJoinedGame(GameExtendedDTO gameExtendedDto)
	{
		LoadGameplayScene(gameExtendedDto.id, false);
	}
	private void OnCreatedGame(GameDTO gameDto)
	{
		var token = PlayerPrefsManager.GetToken();
		WaitWidget.gameObject.SetActive(true);
		WaitWidget.Initialize(gameDto.id, token, backendService, () => LoadGameplayScene(gameDto.id, true));
	}

	private void OnError(StrategoErrorDTO error)
	{
		Debug.Log(error.message);
	}

	private void LoadGameplayScene(int gameId, bool isHost)
	{
		PlayerPrefsManager.SetGameId(gameId);
		PlayerPrefsManager.SetIsHost(isHost);
		SceneManager.LoadScene("Gameplay");
	}

	private string GetToken() => PlayerPrefsManager.GetToken();

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		backendService = FindFirstObjectByType<BackendService>();
		WaitWidget.gameObject.SetActive(false);

		CreateGameButton.onClick.AddListener(CreateGameButtonClick);

		var token = GetToken();
		StartCoroutine(backendService.GetGameList(token, OnGamesGot, OnError));

		/*
		List<GameDTO> testGames = new() {
			GetGame("fulano's game"),
			GetGame("mengano's game"),
			GetGame("zutano's game"),
			GetGame("zetano's game"),
			GetGame("setano's game"),
		};

		OnGamesGot(testGames);
		*/
	}

	/*
	private GameDTO GetGame(string name) 
	{
		var game = new GameDTO
		{
			host = new PlayerDTO()
			{
				id = 1,
				username = "fulano",
			},
			guest = null,
			creationDate = DateTime.Now,
			name = name,
			phase = GamePhase.WAITING_FOR_SETUP_2_PLAYERS,
		};

		return game;
	}
	*/

	private void OnGamesGot(List<GameDTO> games)
	{
		foreach(var game in games)
		{
			var item = Instantiate(GameItemPrefab, GameListContent.transform);
			var gameItem = item.GetComponent<GameItemWidget>();
			gameItem.SetGameData(game, JoinGame);
		}
	}

	private void CreateGameButtonClick()
	{
		var token = GetToken();
		StartCoroutine(backendService.CreateGame(token, OnCreatedGame, OnError));
	}
	private void JoinGame(int gameId)
	{
		var token = GetToken();
		StartCoroutine(backendService.JoinGame(gameId, token, OnJoinedGame, OnError));
	}
}
