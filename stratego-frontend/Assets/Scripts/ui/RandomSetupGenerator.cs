using System.Collections.Generic;
using UnityEngine;

public class RandomSetupGenerator : MonoBehaviour
{
	private UnitSetupPosition GetNextFreePosition(List<SetupRow> setup, int rowInit, int colInit)
	{
		UnitSetupPosition position;
		SetupRow setupRow;

		int nRows = setup.Count;
		int nCols = setup[0].GetPositions().Count;

		var row = rowInit;
		var col = colInit;
		do
		{
			setupRow = setup[row];
			position = setupRow.GetPositions()[col];
			if (position.GetData() != null)
			{
				position = null;
			}

			col++;
			if (col >= nCols)
			{
				col = 0;
				row++;
				if (row >= nRows)
				{
					row = 0;
				}
			}

		} while (position == null && !(row == rowInit && col == colInit));

		return position;
	}

	public void UseRandomSetup(List<SetupRow> setup, List<ToolUnitItem> toolUnitItems)
	{
		Debug.Log("setting random setup");

		var toolUnitItemsList = new List<ToolUnitItem>(toolUnitItems);

		int nRows = setup.Count;
		int nCols = setup[0].GetPositions().Count;

		while (toolUnitItemsList.Count > 0)
		{
			int i = Random.Range(0, toolUnitItemsList.Count);

			var toolUnitItem = toolUnitItemsList[i];
			Debug.Log($"Selected toolItem {toolUnitItem.GetUnitImage().GetData().rank}");

			var row = Random.Range(0, nRows);
			var col = Random.Range(0, nCols);

			var position = GetNextFreePosition(setup, row, col);
			if (position != null)
			{
				position.SetUnitImage(toolUnitItem.GetUnitImage());

				if(toolUnitItem.GetNumUnits() == 0)
				{
					toolUnitItemsList.Remove(toolUnitItem);
					Debug.Log($"Removing toolItem {toolUnitItem.GetUnitImage().GetData().rank}");
				}
			}
		}

		Debug.Log("Random setup finished");
	}
}
