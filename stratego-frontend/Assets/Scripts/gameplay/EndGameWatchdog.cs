using UnityEngine;

public class EndGameWatchdog : MonoBehaviour
{
	[SerializeField]
	private EndGameWidget EndGameWidget;

	private void OnEnable()
	{
		GameManager.OnPieceCaptured += OnPieceCaptured;
	}

	private void OnDisable()
	{
		GameManager.OnPieceCaptured -= OnPieceCaptured;
	}

	private void OnPieceCaptured(object sender, Piece piece)
	{
		if (piece.GetRank() == Rank.FLAG)
		{
			EndGameWidget.Activate(piece.GetIsHost());
		}
	}
}
