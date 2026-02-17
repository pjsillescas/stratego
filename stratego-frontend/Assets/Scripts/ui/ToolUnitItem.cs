using System;
using TMPro;
using UnityEngine;

public class ToolUnitItem : MonoBehaviour
{
	public static event EventHandler OnNumItemsChanged;

	[SerializeField]
	private TextMeshProUGUI NumUnitsText;
	[SerializeField]
	private TextMeshProUGUI NameText;

	[SerializeField]
	private Transform PicTransform;

	[SerializeField]
	private GameObject UnitImagePrefab;

	[SerializeField]
	private PieceData Data;

	private int numUnits;
	private UnitImage unitImage;

	public int GetNumUnits() => numUnits;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		numUnits = Data.numUnits;
		NumUnitsText.text = numUnits.ToString();
		NameText.text = Data.rankName;
		var obj = Instantiate(UnitImagePrefab, PicTransform);
		unitImage = obj.GetComponent<UnitImage>();
		unitImage.Init(Data, this);
	}

	public UnitImage GetUnitImage() => unitImage;

	public bool DecrementNumUnits()
	{
		if (numUnits <= 0)
		{
			return false;
		}

		numUnits--;
		NumUnitsText.text = numUnits.ToString();
		OnNumItemsChanged?.Invoke(this, EventArgs.Empty);
		return true;
	}

	public bool IncrementNumUnits()
	{
		numUnits++;
		NumUnitsText.text = numUnits.ToString();
		return true;
	}
}
