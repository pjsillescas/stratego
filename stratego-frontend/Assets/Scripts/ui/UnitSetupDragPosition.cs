using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class UnitSetupDragPosition : MonoBehaviour, IDragHandler, IEndDragHandler
{
	[SerializeField]
	private RawImage UnitImage;
	[SerializeField]
	private TextMeshProUGUI NameText;
	[SerializeField]
	private RawImage RankImage;

	private ToolUnitItem previousToolUnitItem;

	private RectTransform rectTransform;
	private Canvas canvas;
	private CanvasGroup canvasGroup;
	private UnitSetupPosition originalUnitSetupPosition;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		canvas = FindFirstObjectByType<Canvas>();
		canvasGroup = GetComponent<CanvasGroup>();

		canvasGroup.alpha = 0.6f;
		canvasGroup.blocksRaycasts = false;
	}

	public UnitSetupPosition GetUnitSetupPosition() => originalUnitSetupPosition;

	public void Init(UnitSetupPosition unitSetupPosition)
	{
		originalUnitSetupPosition = unitSetupPosition;
		ShowImage();
		NameText.text = unitSetupPosition.GetName();
		RankImage.texture = unitSetupPosition.GetData().texture;
	}

	public string GetName() => NameText.text;

	public void SetUnitSetupPosition(UnitSetupPosition unitSetupPosition)
	{
		if (previousToolUnitItem == null)
		{
			Init(unitSetupPosition);
			unitSetupPosition.ResetData();
		}
		else
		{
			// copy dragged data to this
			Init(unitSetupPosition);
			// Transfer previous data from this position to the dragged one
			unitSetupPosition.Init(unitSetupPosition.GetPreviousToolUnitItem(), unitSetupPosition.GetData(), NameText.text);
		}
	}

	private UnitSetupPosition newSetupPosition;
	public void SetNewSetupPosition(UnitSetupPosition unitSetupPosition)
	{
		newSetupPosition = unitSetupPosition;
	}

	public void OverrideOriginalSetupPosition()
	{
		originalUnitSetupPosition.SetUnitSetupPosition(newSetupPosition);
	}

	public void SetOriginalSetupPosition(UnitSetupPosition newUnitSetupPosition)
	{
		originalUnitSetupPosition.SetUnitSetupPosition(newUnitSetupPosition);
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
		gameObject.SetActive(false);
	}

	public void ResetData()
	{
		previousToolUnitItem = null;
		HideImage();
	}

	public void OnDrag(PointerEventData eventData)
	{
		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		canvasGroup.alpha = 1.0f;
		canvasGroup.blocksRaycasts = true;
		transform.localPosition = Vector3.zero;
		ResetData();

		gameObject.SetActive(false);
	}
}
