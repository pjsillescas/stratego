using System.Collections.Generic;
using UnityEngine;
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
	private Button StopButton;
	[SerializeField]
	private Button UpdateGameListButton;
	[SerializeField]
	private Button ManageFavouriteSetupsButton;

	[SerializeField]
	private WaitForGuestPlayerWidget WaitWidget;

	[SerializeField]
	private ArmySetupWidget ArmySetupWidget;

	private void OnJoinedGame(GameExtendedDTO gameExtendedDto)
	{
		var commData = CommData.GetInstance();

		commData.SetGameId(gameExtendedDto.id);

		var host = gameExtendedDto.host;
		var guest = gameExtendedDto.guest;
		//commData.SetOpponentUsername((guest.username == commData.GetMyUsername()) ? host.username : guest.username);
		commData.SetOpponentUsername(host.username);
		
		ActivateButtons();
		
		LoadGameplayScene(false);
	}
	private void OnCreatedGame(GameDTO gameDto)
	{
		var commData = CommData.GetInstance();
		commData.SetGameId(gameDto.id);
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

		ActivateButtons();

		SceneManager.LoadScene("Gameplay");
	}

	private string GetToken() => CommData.GetInstance().GetToken();

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		WaitWidget.gameObject.SetActive(false);

		CreateGameButton.onClick.RemoveAllListeners();
		CreateGameButton.onClick.AddListener(CreateGameButtonClick);

		UpdateGameListButton.onClick.RemoveAllListeners();
		UpdateGameListButton.onClick.AddListener(UpdateGameListButtonClick);

		ManageFavouriteSetupsButton.onClick.RemoveAllListeners();
		ManageFavouriteSetupsButton.onClick.AddListener(ManageFavouriteSetupsButtonClick);

		StopButton.onClick.RemoveAllListeners();
		StopButton.onClick.AddListener(StopWaiting);
		StopButton.gameObject.SetActive(false);

		ArmySetupWidget.gameObject.SetActive(false);

		UpdateGameListButtonClick();
		ActivateButtons();
	}

	private void ClearList()
	{
		var oldGames = new List<GameItemWidget>(GameListContent.GetComponentsInChildren<GameItemWidget>());
		oldGames.ForEach(widget => Destroy(widget.gameObject));
	}

	private void OnGamesGot(List<GameDTO> games)
	{
		ClearList();

		foreach(var game in games)
		{
			var item = Instantiate(GameItemPrefab, GameListContent.transform);
			var gameItem = item.GetComponent<GameItemWidget>();
			gameItem.SetGameData(game, JoinGame);
		}

		ActivateButtons();
	}

	private void ManageFavouriteSetupsButtonClick()
	{
		ArmySetupWidget.gameObject.SetActive(true);
		ArmySetupWidget.SetMode(ArmySetupWidget.TSetupWidgetMode.EDITOR_SETUP);
		ArmySetupWidget.Initialize(null);

	}

	private void UpdateGameListButtonClick()
	{
		DeactivateButtons();
		var token = GetToken();
		StartCoroutine(BackendService.GetInstance().GetGameList(token, OnGamesGot, OnError));
	}

	private void StopWaiting()
	{
		ActivateButtons();
	}

	private void CreateGameButtonClick()
	{
		DeactivateButtons();
		var token = GetToken();
		StartCoroutine(BackendService.GetInstance().CreateGame(token, OnCreatedGame, OnError));
	}
	private void JoinGame(int gameId)
	{
		DeactivateButtons();
		var token = GetToken();
		StartCoroutine(BackendService.GetInstance().JoinGame(gameId, token, OnJoinedGame, OnError));
	}

	private void ActivateButtons()
	{
		CreateGameButton.interactable = true;
		UpdateGameListButton.interactable = true;
		ManageFavouriteSetupsButton.interactable = true;
		StopButton.interactable = false;
	}

	private void DeactivateButtons()
	{
		CreateGameButton.interactable = false;
		UpdateGameListButton.interactable = false;
		ManageFavouriteSetupsButton.interactable = false;
		StopButton.interactable = true;
	}

}
