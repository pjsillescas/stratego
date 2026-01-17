using UnityEngine;

[CreateAssetMenu(fileName = "PieceData", menuName = "Scriptable Objects/Piece Data")]
public class PieceData : ScriptableObject
{
	public Rank rank;

	public Sprite sprite;

	public string rankName;
}
