using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FavouriteSetupListWidget : MonoBehaviour
{
	[SerializeField]
	private List<FavouriteSetupItem> SetupItems;

	private FavouriteSetupPreviewWidget previewWidget;
	private BackendService backendService;
	private string token;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		previewWidget = GetComponentInChildren<FavouriteSetupPreviewWidget>();
		backendService = FindFirstObjectByType<BackendService>();

		token = CommData.GetInstance().GetToken();
		StartCoroutine(backendService.GetFavouriteSetupList(token, OnFavouriteSetupListGot, OnError));
	}

	private void OnError(StrategoErrorDTO error)
	{
		Debug.Log(error.message);
	}

	private FavouriteSetupItem GetFreeSetupItem()
	{
		return SetupItems.FirstOrDefault(setup => setup.GetSetup() == null);
	}

	private void OnFavouriteSetupListGot(List<FavouriteSetupDTO> favouriteSetupDtoList)
	{
		SetupItems.ForEach(item => item.ResetItem());

		favouriteSetupDtoList.ForEach(setup => GetFreeSetupItem().InitializeSetup(setup));
	}

	// Update is called once per frame
	void Update()
	{

	}
}
