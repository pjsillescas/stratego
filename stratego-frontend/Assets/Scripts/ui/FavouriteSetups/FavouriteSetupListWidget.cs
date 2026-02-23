using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FavouriteSetupListWidget : MonoBehaviour
{
	[SerializeField]
	private List<FavouriteSetupItem> SetupItems;
	[SerializeField]
	private TMP_InputField DescriptionInput;
	[SerializeField]
	private Button AddSetupButton;
	[SerializeField]
	private Button UpdateSetupButton;
	[SerializeField]
	private Button DeleteSetupButton;
	[SerializeField]
	private Button AcceptSetupButton;
	[SerializeField]
	private Button ExitButton;

	private FavouriteSetupPreviewWidget previewWidget;
	private BackendService backendService;
	private string token;
	private List<List<Rank>> currentSetup;
	private FavouriteSetupDTO currentSetupDto;
	private Action<FavouriteSetupDTO> OnSetupAccepted;

	private void OnEnable()
	{
		FavouriteSetupItem.OnSetupSelected += OnSetupSelected;
	}

	private void OnDisable()
	{
		FavouriteSetupItem.OnSetupSelected -= OnSetupSelected;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		previewWidget = GetComponentInChildren<FavouriteSetupPreviewWidget>();
		backendService = FindFirstObjectByType<BackendService>();

		AddSetupButton.onClick.RemoveAllListeners();
		AddSetupButton.onClick.AddListener(AddSetupButtonClick);

		UpdateSetupButton.onClick.RemoveAllListeners();
		UpdateSetupButton.onClick.AddListener(UpdateSetupButtonClick);

		DeleteSetupButton.onClick.RemoveAllListeners();
		DeleteSetupButton.onClick.AddListener(DeleteSetupButtonClick);

		AcceptSetupButton.onClick.RemoveAllListeners();
		AcceptSetupButton.onClick.AddListener(AcceptSetupButtonClick);
		
		ExitButton.onClick.RemoveAllListeners();
		ExitButton.onClick.AddListener(ExitButtonClick);

		token = CommData.GetInstance().GetToken();
		RefreshSetups();
	}

	private void OnSetupSelected(object sender, FavouriteSetupDTO setupDto)
	{
		currentSetupDto = setupDto;
		previewWidget.LoadSetup(currentSetupDto);
	}

	private void AcceptSetupButtonClick()
	{
		OnSetupAccepted?.Invoke(currentSetupDto);
	}

	private void ExitButtonClick()
	{
		Deactivate();
	}

	private void RefreshSetups()
	{
		StartCoroutine(backendService.GetFavouriteSetupList(token, OnFavouriteSetupListGot, OnError));
	}

	private FavouriteSetupDTO GetSetupDTO()
	{
		return new FavouriteSetupDTO(0, DescriptionInput.text, new ArmySetupDTO(currentSetup));
	}

	private void AddSetupButtonClick()
	{
		var setupDto = GetSetupDTO();
		StartCoroutine(backendService.AddFavouriteSetup(setupDto, token, OnFavouriteSetupAdded, OnError));
	}

	private void OnFavouriteSetupAdded(FavouriteSetupDTO setup)
	{
		RefreshSetups();
	}

	private void UpdateSetupButtonClick()
	{
		if (currentSetupDto != null)
		{
			var id = currentSetupDto.id;
			var setupDto = GetSetupDTO();
			StartCoroutine(backendService.UpdateFavouriteSetup(id, setupDto, token, OnFavouriteSetupUpdated, OnError));
		}
	}
	private void OnFavouriteSetupUpdated(FavouriteSetupDTO setup)
	{
		RefreshSetups();
	}

	private void DeleteSetupButtonClick()
	{
		if (currentSetupDto != null)
		{
			var id = currentSetupDto.id;
			StartCoroutine(backendService.DeleteFavouriteSetup(id, token, OnFavouriteSetupDeleted, OnError));
		}
	}

	private void OnFavouriteSetupDeleted()
	{
		RefreshSetups();
	}

	public void Activate(List<List<Rank>> setup, Action<FavouriteSetupDTO> OnSetupAccepted)
	{
		gameObject.SetActive(true);
		currentSetup = setup;
		this.OnSetupAccepted = OnSetupAccepted;
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);
		currentSetup = null;
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
