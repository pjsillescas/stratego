using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RandomSetupGenerator))]
public class ArmySetupWidget : MonoBehaviour
{
	public enum TSetupWidgetMode { GAME_SETUP, EDITOR_SETUP }

	[SerializeField]
	private List<SetupRow> Setup;

	[SerializeField]
	private List<ToolUnitItem> ToolUnitItems;

	[SerializeField]
	private Button UseSetupButton;
	[SerializeField]
	private Button UseRandomSetupButton;
	[SerializeField]
	private Button FavouriteSetupsButton;
	[SerializeField]
	private Button CloseButton;
	[SerializeField]
	private Button ResetButton;

	[SerializeField]
	private WaitForSetupsWidget WaitForSetupWidget;
	[SerializeField]
	private FavouriteSetupListWidget FavouriteSetupListWidget;

	private Action<GameStateDTO> onGameStart;
	private RandomSetupGenerator randomSetupGenerator;

	public void SetMode(TSetupWidgetMode mode)
	{
		Debug.Log($"set mode {mode}");
		CloseButton.gameObject.SetActive(mode == TSetupWidgetMode.EDITOR_SETUP);
		UseSetupButton.gameObject.SetActive(mode == TSetupWidgetMode.GAME_SETUP);
	}

	private void Awake()
	{
		SetMode(TSetupWidgetMode.GAME_SETUP);
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
	}

	public void ResetWidget()
	{
		ToolUnitItems.ForEach(item => item.ResetItem());
		Setup.ForEach(row => row.GetPositions().ForEach(position => position.ResetData()));
	}

	public void Initialize(Action<GameStateDTO> onGameStart)
	{
		gameObject.SetActive(true);

		randomSetupGenerator = GetComponent<RandomSetupGenerator>();

		UseSetupButton.interactable = false;
		UseSetupButton.onClick.RemoveAllListeners();
		UseSetupButton.onClick.AddListener(UseSetupButtonClick);

		UseRandomSetupButton.enabled = true;
		UseRandomSetupButton.onClick.RemoveAllListeners();
		UseRandomSetupButton.onClick.AddListener(UseRandomSetupButtonClick);

		FavouriteSetupsButton.enabled = true;
		FavouriteSetupsButton.onClick.RemoveAllListeners();
		FavouriteSetupsButton.onClick.AddListener(FavouriteSetupsButtonClick);

		CloseButton.enabled = true;
		CloseButton.onClick.RemoveAllListeners();
		CloseButton.onClick.AddListener(CloseButtonClick);
		
		ResetButton.enabled = true;
		ResetButton.onClick.RemoveAllListeners();
		ResetButton.onClick.AddListener(ResetWidget);

		FavouriteSetupListWidget.Deactivate();

		ToolUnitItem.OnNumItemsChanged += ToolUnitItem_OnNumItemsChanged;
		this.onGameStart = onGameStart;
	}

	private void UseRandomSetupButtonClick()
	{
		randomSetupGenerator.UseRandomSetup(Setup, ToolUnitItems);
	}

	private void CloseButtonClick()
	{
		Debug.Log("close click");
		gameObject.SetActive(false);
	}

	private void FavouriteSetupsButtonClick()
	{
		FavouriteSetupListWidget.Activate(GetSetup, LoadSetup);
	}

	private void LoadSetup(FavouriteSetupDTO setupDto)
	{
		ResetWidget();
		LoadSetup(Setup, ToolUnitItems, setupDto);
	}

	private void LoadSetup(List<SetupRow> setup, List<ToolUnitItem> toolUnitItems, FavouriteSetupDTO setupDto)
	{
		Debug.Log("setting setup");

		var toolUnitItemsList = new List<ToolUnitItem>(toolUnitItems);

		int nRows = setup.Count;
		int nCols = setup[0].GetPositions().Count;

		for(int row = 0; row < nRows; row++)
		{
			for(int col = 0; col < nCols; col++)
			{
				var position = setup[row].GetPositions()[col];
				var rank = setupDto.armySetupDTO.army[row][col];
				var toolItem = toolUnitItems.Find(item => item.GetUnitImage().GetData().rank == rank);

				if(toolItem != null)
				{
					position.SetUnitImage(toolItem.GetUnitImage());
				}
			}
		}

		Debug.Log("setting setup finished");
	}

	private void ToolUnitItem_OnNumItemsChanged(object sender, EventArgs e)
	{
		var numItems = ToolUnitItems.Sum(item => item.GetNumUnits());
		if (numItems <= 0)
		{
			UseSetupButton.interactable = true;
		}
	}

	private void OnDestroy()
	{
		ToolUnitItem.OnNumItemsChanged -= ToolUnitItem_OnNumItemsChanged;
	}

	private List<List<Rank>> GetSetup()
	{
		return Setup.Select(row => row.GetPositions().Select(pos => pos.GetRank()).ToList()).ToList();
	}

	private void UseSetupButtonClick()
	{
		var commData = CommData.GetInstance();
		var gameId = commData.GetGameId();
		var token = commData.GetToken();
		List<List<Rank>> setup = GetSetup();
		StartCoroutine(BackendService.GetInstance().AddSetup(gameId, token, setup, OnSetupAdded, OnError));
	}

	private void OnSetupAdded(GameStateDTO gameStateDto)
	{
		WaitForSetupWidget.gameObject.SetActive(true);
		WaitForSetupWidget.Initialize((gameState) =>
		{
			onGameStart?.Invoke(gameState);
			gameObject.SetActive(false);
		});
	}

	private void OnError(StrategoErrorDTO error)
	{
		Debug.Log($"Error {error.message}");
	}
}
