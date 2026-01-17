using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PieceSet", menuName = "Scriptable Objects/Piece Set")]
public class PiecesSet : ScriptableObject
{
	public List<PieceData> pieces;
}
