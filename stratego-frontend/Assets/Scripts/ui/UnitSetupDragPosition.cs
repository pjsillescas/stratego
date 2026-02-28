using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class UnitSetupDragPosition : MonoBehaviour
{
	[SerializeField]
	private RawImage UnitImage;
	[SerializeField]
	private TextMeshProUGUI NameText;
	[SerializeField]
	private RawImage RankImage;

	private RectTransform rectTransform;
	private CanvasGroup canvasGroup;
	private UnitSetupPosition originalUnitSetupPosition;

	private ToolUnitItem unitItem;
	private PieceData data;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();

		canvasGroup.alpha = 0.6f;
		canvasGroup.blocksRaycasts = false;
	}

	public void Init(UnitSetupPosition unitSetupPosition)
	{
		gameObject.SetActive(true);
		originalUnitSetupPosition = unitSetupPosition;
		ShowImage();
		NameText.text = unitSetupPosition.GetName();
		RankImage.texture = unitSetupPosition.GetData().texture;
		data = unitSetupPosition.GetData();
		unitItem = unitSetupPosition.GetPreviousToolUnitItem();
	}

	public void AddAnchoredPosition(Vector2 offset)
	{
		rectTransform.anchoredPosition += offset;
	}

	public void SetPosition(Vector3 position)
	{
		transform.localPosition = position;
		transform.position = position;
	}

	public string GetName() => NameText.text;

	public void SetUnitSetupPosition(UnitSetupPosition unitSetupPosition)
	{
		Init(unitSetupPosition);

		if (data == null)
		{
			unitSetupPosition.ResetData();
		}
		else
		{
			// Transfer previous data from this position to the dragged one
			unitSetupPosition.Init(unitItem, data, NameText.text);
		}
	}

	public void SwapData(UnitSetupPosition newUnitSetupPosition)
	{
		if (originalUnitSetupPosition != null)
		{
			originalUnitSetupPosition.SetUnitSetupPosition(newUnitSetupPosition);
		}
		
		Deactivate();
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
		Deactivate();
	}

	public void ResetData()
	{
		data = null;
		HideImage();
	}

	public void Activate()
	{
		gameObject.SetActive(true);
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);
	}
}
