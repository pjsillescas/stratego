using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSetupPosition : MonoBehaviour, IDropHandler
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

	public void OnDrop(PointerEventData eventData)
	{
		var unitImage = eventData.pointerDrag.GetComponent<UnitImage>();
		var toolUnitItem = unitImage.GetToolUnitItem();
		if (toolUnitItem.DecrementNumUnits())
		{
			if (previousToolUnitItem != null)
			{
				previousToolUnitItem.IncrementNumUnits();
			}
			
			previousToolUnitItem = toolUnitItem;
			data = unitImage.GetData();
			//Debug.Log($"ondrop setupposition {rank}");
			ShowImage();
			NameText.text = unitImage.GetName();
			RankImage.texture = data.texture;
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
		previousToolUnitItem = null;
		HideImage();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
