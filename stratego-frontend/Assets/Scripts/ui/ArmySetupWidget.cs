using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class ArmySetupWidget : MonoBehaviour
{
	[SerializeField]
	private List<SetupRow> Setup;

	[SerializeField]
	private GameObject GameWidget;

	[SerializeField]
	private List<ToolUnitItem> ToolUnitItems;

	[SerializeField]
	private Button UseSetupButton;

	BackendService backendService;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		backendService = FindFirstObjectByType<BackendService>();
		UseSetupButton.enabled = false;
		UseSetupButton.onClick.AddListener(UseSetupButtonClick);
		GameWidget.SetActive(false);
		ToolUnitItem.OnNumItemsChanged += ToolUnitItem_OnNumItemsChanged;
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
		var gameId = PlayerPrefsManager.GetGameId();
		var token = PlayerPrefsManager.GetToken();
		List<List<Rank>> setup = Setup.Select(row => row.GetPositions().Select(pos => pos.GetRank()).ToList()).ToList();
		StartCoroutine(backendService.AddSetup(gameId, token, setup, OnSetupAdded, OnError));
	}

	private void OnSetupAdded(GameStateDTO gameStateDto)
	{
		GameWidget.SetActive(true);
		;
	}

	private void OnError(StrategoErrorDTO error)
	{
		Debug.Log($"Error {error.message}");
	}
}
