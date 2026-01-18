using UnityEngine;

[CreateAssetMenu(fileName = "PieceData", menuName = "Scriptable Objects/Piece Data")]
public class PieceData : ScriptableObject
{
	public Rank rank;

	public Sprite sprite;
	public Texture texture;
	public string rankName;
	public int numUnits;
}
