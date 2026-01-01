using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SetupRow : MonoBehaviour
{
	[SerializeField]
	private List<UnitSetupPosition> positions;

	public List<UnitSetupPosition> GetPositions() => positions;
}
