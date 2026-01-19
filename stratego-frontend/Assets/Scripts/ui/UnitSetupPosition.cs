using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
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
		//var toolUnitItem = unitSetupPosition.GetPreviousToolUnitItem();
		//data = unitSetupPosition.GetData();
		this.data = data;
		previousToolUnitItem = toolUnitItem;
		//Debug.Log($"ondrop setupposition {rank}");
		ShowImage();
		NameText.text = nameText;
		RankImage.texture = data.texture;

	}

	public string GetName() => NameText.text;

	public void OnDrop(PointerEventData eventData)
	{
		if (eventData.pointerDrag.TryGetComponent(out UnitImage unitImage))
		{
			var toolUnitItem = unitImage.GetToolUnitItem();
			if (toolUnitItem.DecrementNumUnits())
			{
				if (previousToolUnitItem != null)
				{
					previousToolUnitItem.IncrementNumUnits();
				}
				/*
				previousToolUnitItem = toolUnitItem;
				data = unitImage.GetData();
				//Debug.Log($"ondrop setupposition {rank}");
				ShowImage();
				NameText.text = unitImage.GetName();
				RankImage.texture = data.texture;
				*/

				Init(toolUnitItem, unitImage.GetData(), unitImage.GetName());
			}
		}

		if (eventData.pointerDrag.TryGetComponent(out UnitSetupPosition unitSetupPosition))
		{
			if (previousToolUnitItem == null)
			{
				/*
				var toolUnitItem = unitSetupPosition.GetPreviousToolUnitItem();
				var data = unitSetupPosition.GetData();
				previousToolUnitItem = toolUnitItem;
				//Debug.Log($"ondrop setupposition {rank}");
				ShowImage();
				NameText.text = unitImage.GetName();
				RankImage.texture = data.texture;
				*/
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
				;
			}
				/*
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
				*/
				Debug.Log("unitsetupposition");
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
		defaultPosition = transform.localPosition;
		Debug.Log($"position {name} in position {defaultPosition}");

		yield return null;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void OnDrag(PointerEventData eventData)
	{
		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		canvasGroup.alpha = 0.6f;
		canvasGroup.blocksRaycasts = false;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		canvasGroup.alpha = 1.0f;
		canvasGroup.blocksRaycasts = true;
		transform.localPosition = defaultPosition;
	}
}
