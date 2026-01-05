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

	private void OnJoinedGame(GameExtendedDTO gameExtendedDto)
	{
		LoadGameplayScene(false);
	}
	private void OnCreatedGame(GameDTO gameDto)
	{
		var commData = CommData.GetInstance();
		commData.SetGameId(gameDto.id);
		Debug.Log(gameDto.id);
		WaitWidget.gameObject.SetActive(true);
		WaitWidget.Initialize(() => LoadGameplayScene(true));
	}

	private void OnError(StrategoErrorDTO error)
	{
		Debug.Log(error.message);
	}

	private void LoadGameplayScene(bool isHost)
	{
		var commData = CommData.GetInstance();
		commData.SetIsHost(isHost);
		SceneManager.LoadScene("Gameplay");
	}

	private string GetToken() => CommData.GetInstance().GetToken();

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		WaitWidget.gameObject.SetActive(false);

		CreateGameButton.onClick.AddListener(CreateGameButtonClick);

		var token = GetToken();
		StartCoroutine(BackendService.GetInstance().GetGameList(token, OnGamesGot, OnError));

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
		StartCoroutine(BackendService.GetInstance().CreateGame(token, OnCreatedGame, OnError));
	}
	private void JoinGame(int gameId)
	{
		var token = GetToken();
		StartCoroutine(BackendService.GetInstance().JoinGame(gameId, token, OnJoinedGame, OnError));
	}
}
