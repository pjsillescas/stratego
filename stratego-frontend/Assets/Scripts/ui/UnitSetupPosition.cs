using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Image))]
public class UnitSetupPosition : MonoBehaviour, IDropHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
	[SerializeField]
	private RawImage UnitImage;
	[SerializeField]
	private TextMeshProUGUI NameText;
	[SerializeField]
	private RawImage RankImage;

	private ToolUnitItem previousToolUnitItem;
	private PieceData data;

	private RectTransform rectTransform;
	private Canvas canvas;
	private Vector3 defaultPosition;
	private CanvasGroup canvasGroup;
	private UnitSetupDragPosition unitSetupDragPosition;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		canvas = FindFirstObjectByType<Canvas>();
		canvasGroup = GetComponent<CanvasGroup>();
	}


	public Rank GetRank() => data.rank;

	public PieceData GetData() => data;

	public ToolUnitItem GetPreviousToolUnitItem() => previousToolUnitItem;

	public void Init(ToolUnitItem toolUnitItem, PieceData data, string nameText)
	{
		this.data = data;
		previousToolUnitItem = toolUnitItem;
		ShowImage();
		NameText.text = nameText;
		RankImage.texture = data.texture;
	}

	public string GetName() => NameText.text;

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
			if (unitSetupPosition.GetData() == null)
			{
				ResetData();
			}
			else
			{
				Init(unitSetupPosition.GetPreviousToolUnitItem(), unitSetupPosition.GetData(), unitSetupPosition.GetName());
				unitSetupPosition.ResetData();
			}
		}
		else
		{
			var thisToolUnitItem = previousToolUnitItem;
			var thisData = data;
			var thisName = NameText.text;

			if (unitSetupPosition.GetData() == null)
			{
				ResetData();
			}
			else
			{
				// copy dragged data to this
				Init(unitSetupPosition.GetPreviousToolUnitItem(), unitSetupPosition.GetData(), unitSetupPosition.GetName());
			}

			// Transfer previous data from this position to the dragged one
			unitSetupPosition.Init(thisToolUnitItem, thisData, thisName);
		}
	}

	public void OnDrop(PointerEventData eventData)
	{
		Debug.Log("ondrop");
		if (eventData.pointerDrag.TryGetComponent(out UnitImage unitImage))
		{
			SetUnitImage(unitImage);
		}
		/*
		if (eventData.pointerDrag.TryGetComponent(out UnitSetupPosition unitSetupPosition))
		{
			SetUnitSetupPosition(unitSetupPosition);
		}
		*/
		else
		/*
		if (eventData.pointerDrag.TryGetComponent(out UnitSetupDragPosition unitSetupDragPosition))
		{
			Debug.Log("swap if");
			//unitSetupDragPosition.SetNewSetupPosition(this);
			//SetUnitSetupPosition(unitSetupDragPosition.GetUnitSetupPosition());
			//unitSetupDragPosition.OverrideOriginalSetupPosition();
			unitSetupDragPosition.SwapData(this);
		}
		else
		*/
		{
			Debug.Log("swap else");
			this.unitSetupDragPosition.SwapData(this);
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
		StartCoroutine(GetPositionCoroutine());
		ResetData();

		unitSetupDragPosition = FindFirstObjectByType<UnitSetupDragPosition>();
		if (unitSetupDragPosition == null)
		{
			throw new System.Exception("There is no UnitSetupDragPosition element in the scene");
		}
	}

	public void ResetData()
	{
		previousToolUnitItem = null;
		data = null;
		HideImage();
	}

	private IEnumerator GetPositionCoroutine()
	{
		yield return new WaitForSeconds(1.0f);
		//defaultPosition = transform.localPosition;
		defaultPosition = transform.position;
		yield return null;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void OnDrag(PointerEventData eventData)
	{
		if (data == null)
		{
			return;
		}
		//rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
		unitSetupDragPosition.AddAnchoredPosition(eventData.delta / canvas.scaleFactor);

	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		//canvasGroup.alpha = 0.6f;
		//canvasGroup.blocksRaycasts = false;
		if (data == null)
		{
			return;
		}

		unitSetupDragPosition.SetPosition(defaultPosition);
		unitSetupDragPosition.Init(this);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (data == null)
		{
			return;
		}
		unitSetupDragPosition.gameObject.SetActive(false);
		//canvasGroup.alpha = 1.0f;
		//canvasGroup.blocksRaycasts = true;
		//transform.localPosition = defaultPosition;
		//unitSetupDragPosition.SetPosition(defaultPosition);
	}
}
