using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class FavouriteSetupItem : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI DescriptionText;

	private Button button;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		button = GetComponent<Button>();
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(ButtonClick);

		ResetItem();
	}

	private void ButtonClick()
	{
		;
	}

	public void InitializeSetup()
	{
		;
	}

	public void ResetItem()
	{
		DescriptionText.text = "---";
	}
}
