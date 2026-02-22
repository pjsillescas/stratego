using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitSetupPreviewPosition : MonoBehaviour
{
	[SerializeField]
	private RawImage UnitImage;
	[SerializeField]
	private TextMeshProUGUI NameText;
	[SerializeField]
	private RawImage RankImage;

	private ToolUnitItem previousToolUnitItem;
	private PieceData data;

	public Rank GetRank() => data.rank;

	public PieceData GetData() => data;

	public ToolUnitItem GetPreviousToolUnitItem() => previousToolUnitItem;

	public string GetName() => NameText.text;

	public void Init(ToolUnitItem toolUnitItem, PieceData data, string nameText)
	{
		this.data = data;
		previousToolUnitItem = toolUnitItem;
		ShowImage();
		NameText.text = nameText;
		RankImage.texture = data.texture;

	}

	public void SetUnitImage(UnitImage unitImage)
	{
		var toolUnitItem = unitImage.GetToolUnitItem();
		if (toolUnitItem.DecrementNumUnits())
		{
			if (previousToolUnitItem != null)
			{
				previousToolUnitItem.IncrementNumUnits();
			}

			Init(toolUnitItem, unitImage.GetData(), unitImage.GetName());
		}
	}

	public void SetUnitSetupPosition(UnitSetupPosition unitSetupPosition)
	{
		if (previousToolUnitItem == null)
		{
			Init(unitSetupPosition.GetPreviousToolUnitItem(), unitSetupPosition.GetData(), unitSetupPosition.GetName());
			unitSetupPosition.ResetData();
		}
		else
		{
			var thisToolUnitItem = previousToolUnitItem;
			var thisData = data;
			var thisName = NameText.text;

			// copy dragged data to this
			Init(unitSetupPosition.GetPreviousToolUnitItem(), unitSetupPosition.GetData(), unitSetupPosition.GetName());
			// Transfer previous data from this position to the dragged one
			unitSetupPosition.Init(thisToolUnitItem, thisData, thisName);
		}
	}

	private void ShowImage()
	{
		UnitImage.gameObject.SetActive(true);
	}

	private void HideImage()
	{
		UnitImage.gameObject.SetActive(false);
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		ResetData();
	}

	public void ResetData()
	{
		previousToolUnitItem = null;
		data = null;
		HideImage();
	}


	// Update is called once per frame
	void Update()
	{

	}
}
