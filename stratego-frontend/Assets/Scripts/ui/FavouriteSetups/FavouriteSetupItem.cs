using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class FavouriteSetupItem : MonoBehaviour
{
	public static event EventHandler<FavouriteSetupDTO> OnSetupSelected;
	[SerializeField]
	private TextMeshProUGUI DescriptionText;

	private Button button;
	private FavouriteSetupDTO setup;
	//private FavouriteSetupPreviewWidget previewWidget;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		button = GetComponent<Button>();
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(ButtonClick);

		//previewWidget = FindFirstObjectByType<FavouriteSetupPreviewWidget>();

		ResetItem();
	}

	private void ButtonClick()
	{
		OnSetupSelected?.Invoke(this, setup);
		//previewWidget.LoadSetup(setup);
	}

	public void InitializeSetup(FavouriteSetupDTO setup)
	{
		this.setup = setup;
		DescriptionText.text = setup.description;
	}

	public FavouriteSetupDTO GetSetup() => setup;

	public void ResetItem()
	{
		DescriptionText.text = "---";
		setup = null;
	}
}
