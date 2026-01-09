using TMPro;
using UnityEngine;

public class RankStatsWidget : MonoBehaviour
{
	[SerializeField]
	private Rank Rank;

	[SerializeField]
	private int NumUnits;

	[SerializeField]
	private TextMeshProUGUI RankText;
	[SerializeField]
	private TextMeshProUGUI UnitText;

	private void Awake()
	{
		RankText.text = Rank.ToString();
		ResetUnits();
	}

	public void IncrementUnits(int numUnits)
	{
		NumUnits += numUnits;
		UnitText.text = NumUnits.ToString();
	}

	public void ResetUnits()
	{
		NumUnits = 0;
		UnitText.text = NumUnits.ToString();
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}
