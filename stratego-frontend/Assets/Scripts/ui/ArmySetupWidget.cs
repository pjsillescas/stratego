using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ArmySetupWidget : MonoBehaviour
{
	[SerializeField]
	private List<SetupRow> Setup;

	[SerializeField]
	private List<ToolUnitItem> ToolUnitItems;

	[SerializeField]
	private Button UseSetupButton;

	[SerializeField]
	private WaitForSetupsWidget WaitForSetupWidget;

	private Action<GameStateDTO> onGameStart;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
	}

	public void Initialize(Action<GameStateDTO> onGameStart)
	{
		gameObject.SetActive(true);
		UseSetupButton.enabled = false;
		UseSetupButton.onClick.AddListener(UseSetupButtonClick);
		ToolUnitItem.OnNumItemsChanged += ToolUnitItem_OnNumItemsChanged;
		this.onGameStart = onGameStart;
	}

	private void ToolUnitItem_OnNumItemsChanged(object sender, System.EventArgs e)
	{
		var numItems = ToolUnitItems.Select(item => item.GetNumUnits()).Sum();

		if (numItems <= 0)
		{
			UseSetupButton.enabled = true;
		}
	}

	private void OnDestroy()
	{
		ToolUnitItem.OnNumItemsChanged -= ToolUnitItem_OnNumItemsChanged;
	}

	private void UseSetupButtonClick()
	{
		var commData = CommData.GetInstance();
		var gameId = commData.GetGameId();
		var token = commData.GetToken();
		List<List<Rank>> setup = Setup.Select(row => row.GetPositions().Select(pos => pos.GetRank()).ToList()).ToList();
		StartCoroutine(BackendService.GetInstance().AddSetup(gameId, token, setup, OnSetupAdded, OnError));
	}

	private void OnSetupAdded(GameStateDTO gameStateDto)
	{
		WaitForSetupWidget.gameObject.SetActive(true);
		WaitForSetupWidget.Initialize(() => {
			onGameStart?.Invoke(gameStateDto);
			gameObject.SetActive(false);
		});
	}

	private void OnError(StrategoErrorDTO error)
	{
		Debug.Log($"Error {error.message}");
	}
}
