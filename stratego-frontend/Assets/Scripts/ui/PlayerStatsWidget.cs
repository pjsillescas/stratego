using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerStatsWidget : MonoBehaviour
{
	[Serializable]
	public struct ListNode
	{
		public Rank rank;
		public RankStatsWidget widget;
	}

	[SerializeField]
	private bool IsHost;
	[SerializeField]
	private TextMeshProUGUI PlayerText;
	[SerializeField]
	private List<ListNode> Stats;

	private void Awake()
	{
		PlayerText.text = (IsHost) ? "Blue" : "Red";
	}

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
		if (IsHost && !piece.IsHost() || !IsHost && piece.IsHost())
		{
			var statNode = Stats.Where(node => node.rank == piece.GetRank()).First();

			statNode.widget.IncrementUnits(1);
		}
	}

	public void ResetStats()
	{
		Stats.ForEach(node => node.widget.ResetUnits());
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
