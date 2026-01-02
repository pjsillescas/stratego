using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Piece : MonoBehaviour
{
	private readonly Color HOSTCOLOR = Color.blue;
	private readonly Color GUESTCOLOR = Color.red;

	[SerializeField]
	private TextMeshPro TextName;

	private bool isHost;
	private Rank rank;

	private int row;
	private int col;

	private Dictionary<Rank, string> names;

	private void Awake()
	{
		names = new() {
			{ Rank.MARSHAL, "Marshal" },
			{ Rank.GENERAL, "General" },
			{ Rank.COLONEL, "Colonel" },
			{ Rank.MAJOR, "Major" },
			{ Rank.CAPTAIN, "Captain" },
			{ Rank.LIEUTENANT, "Lieutenant" },
			{ Rank.SERGEANT, "Sergeant" },
			{ Rank.MINER, "Miner" },
			{ Rank.SCOUT, "Scout" },
			{ Rank.SPY, "Spy" },
			{ Rank.FLAG, "Flag" },
			{ Rank.BOMB, "Bomb" },
		};

	}
	public void Initialize(Rank rank, bool isHost)
	{
		this.rank = rank;
		this.isHost = isHost;
		
		GetComponent<MeshRenderer>().material.color = (isHost) ? HOSTCOLOR : GUESTCOLOR;
		TextName.text = names[rank];
	}

	public void SetCoordinates(int row, int col)
	{
		this.row = row;
		this.col = col;
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
