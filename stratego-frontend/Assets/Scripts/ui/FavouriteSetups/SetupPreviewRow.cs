using System.Collections.Generic;
using UnityEngine;

public class SetupPreviewRow : MonoBehaviour
{
	[SerializeField]
	private List<UnitSetupPreviewPosition> positions;

	public List<UnitSetupPreviewPosition> GetPositions() => positions;
}
