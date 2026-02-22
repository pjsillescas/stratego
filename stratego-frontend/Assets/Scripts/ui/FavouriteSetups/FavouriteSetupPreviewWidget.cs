using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FavouriteSetupPreviewWidget : MonoBehaviour
{
	[SerializeField]
	private List<SetupPreviewRow> SetupItems;
	[SerializeField]
	private List<PieceData> Data;


	private PieceData GetData(Rank rank)
	{
		return Data.FirstOrDefault(pieceData => pieceData.rank == rank);
	}

	public void LoadSetup(FavouriteSetupDTO setup)
	{
		for (int iRow = 0; iRow < SetupItems.Count; iRow++)
		{
			var row = SetupItems[iRow].GetPositions();
			var setupRow = setup.armySetupDTO.army[iRow];
			for (int iCol = 0; iCol < row.Count; iCol++)
			{
				var position = row[iCol];
				var rank = setupRow[iCol];
				var pieceData = GetData(rank);
				position.Init(null, pieceData);
			}
		}
	}

	public void ResetWidget()
	{
		SetupItems.ForEach(row => row.GetPositions().ForEach(position => position.ResetData()));
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
