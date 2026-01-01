using System;
using TMPro;
using UnityEngine;

public class ToolUnitItem : MonoBehaviour
{
	public static event EventHandler OnNumItemsChanged;
	
	[SerializeField]
	private TextMeshProUGUI NumUnitsText;

	[SerializeField]
	private Transform PicTransform;

	[SerializeField]
	private GameObject PicTransformPrefab;

	[SerializeField]
	private Transform ParentTransform;

	[SerializeField]
	private int NumUnits;

	public int GetNumUnits() => NumUnits;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		NumUnitsText.text = NumUnits.ToString();
		var obj = Instantiate(PicTransformPrefab, PicTransform);
		obj.GetComponent<UnitImage>().SetToolUnitItem(this);
	}

	public bool DecrementNumUnits()
	{
		if (NumUnits <= 0)
		{
			return false;
		}

		NumUnits--;
		NumUnitsText.text = NumUnits.ToString();
		OnNumItemsChanged?.Invoke(this, EventArgs.Empty);
		return true;
	}

	public bool IncrementNumUnits()
	{
		NumUnits++;
		NumUnitsText.text = NumUnits.ToString();
		return true;
	}
}
