using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitImage : MonoBehaviour, IUnitTool, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler /* , IPointerDownHandler */
{
	[SerializeField]
	private TextMeshProUGUI TextName;

	[SerializeField]
	private Rank Rank;

	[SerializeField]
	private string DefaultName;

	private Transform parent;
	private Vector3 defaultPosition;
	private RectTransform rectTransform;
	private Canvas canvas;
	private CanvasGroup canvasGroup;
	private ToolUnitItem toolUnitItem;

	public string GetName() => TextName.text;

	public void Init(string name, Rank rank)
	{
		TextName.text = name;
		Rank = rank;
	}

	public void SetToolUnitItem(ToolUnitItem toolUnitItem)
	{
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
		TextName.text = DefaultName;
		parent = transform.parent;
		defaultPosition = transform.localPosition;
		rectTransform = GetComponent<RectTransform>();
		canvas = FindFirstObjectByType<Canvas>();
		canvasGroup = GetComponent<CanvasGroup>();
	}

	public void Show()
	{
		TextName.gameObject.SetActive(true);
	}
	public void Hide()
	{
		TextName.gameObject.SetActive(true);
	}

	public Rank GetRank() => Rank;

	public void OnDrag(PointerEventData eventData)
	{
		if (toolUnitItem.GetNumUnits() <= 0)
		{
			return;
		}
		
		//transform.position = eventData.position;
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
		//transform.parent = null;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		/*
		if (toolUnitItem.GetNumUnits() <= 0)
		{
			return;
		}
		*/

		transform.SetParent(parent, true);
		canvasGroup.alpha = 1.0f;
		canvasGroup.blocksRaycasts = true;
		//transform.position = defaultPosition;
		transform.localPosition = defaultPosition;
	}

	public void OnDrop(PointerEventData eventData)
	{
		Debug.Log("ondrop");
	}
	/*
	public void OnPointerDown(PointerEventData eventData)
	{
		Debug.Log("onpointerdown");
	}
	*/
}
