using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitImage : MonoBehaviour, IUnitTool, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
	private Transform parent;
	private Vector3 defaultPosition;
	private RectTransform rectTransform;
	private Canvas canvas;
	private CanvasGroup canvasGroup;
	private ToolUnitItem toolUnitItem;
	private PieceData data;

	public string GetName() => data.rankName;

	public PieceData GetData() => data;

	public void Init(PieceData data, ToolUnitItem toolUnitItem)
	{
		this.data = data;
		GetComponent<RawImage>().texture = data.texture;
		this.toolUnitItem = toolUnitItem;
	}

	public ToolUnitItem GetToolUnitItem() => toolUnitItem;

	public bool IncrementNumUnits()
	{
		return toolUnitItem.IncrementNumUnits();
	}
	public bool DecrementNumUnits()
	{
		return toolUnitItem.DecrementNumUnits();
	}

	private void Awake()
	{
		parent = transform.parent;
		defaultPosition = transform.localPosition;
		rectTransform = GetComponent<RectTransform>();
		canvas = FindFirstObjectByType<Canvas>();
		canvasGroup = GetComponent<CanvasGroup>();
	}

	public void Show()
	{
		//TextName.gameObject.SetActive(true);
	}
	public void Hide()
	{
		//TextName.gameObject.SetActive(true);
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (toolUnitItem.GetNumUnits() <= 0)
		{
			return;
		}
		
		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (toolUnitItem.GetNumUnits() <= 0)
		{
			return;
		}

		transform.SetParent(canvas.transform, true);
		canvasGroup.alpha = 0.6f;
		canvasGroup.blocksRaycasts = false;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		transform.SetParent(parent, true);
		canvasGroup.alpha = 1.0f;
		canvasGroup.blocksRaycasts = true;
		transform.localPosition = defaultPosition;
	}

	public void OnDrop(PointerEventData eventData)
	{
		Debug.Log("ondrop");
	}
}
