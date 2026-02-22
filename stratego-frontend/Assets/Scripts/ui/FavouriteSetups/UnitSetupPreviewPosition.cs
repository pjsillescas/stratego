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

	private string GetName(Rank rank)
	{
		switch(rank)
		{
			case Rank.BOMB:
				return "BB";
			case Rank.FLAG:
				return "FG";
			case Rank.MARSHAL:
				return "MS";
			case Rank.GENERAL:
				return "GN";
			case Rank.COLONEL:
				return "CL";
			case Rank.MAJOR:
				return "MJ";
			case Rank.CAPTAIN:
				return "CP";
			case Rank.LIEUTENANT:
				return "LT";
			case Rank.SERGEANT:
				return "SG";
			case Rank.MINER:
				return "MN";
			case Rank.SCOUT:
				return "SC";
			case Rank.SPY:
				return "SY";
			default:
				return "";
		};
	}

	public void Init(ToolUnitItem toolUnitItem, PieceData data)
	{
		this.data = data;
		previousToolUnitItem = toolUnitItem;
		ShowImage();
		NameText.text = GetName(data.rank);
		RankImage.texture = data.texture;

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
