using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
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

	private enum Status { NORMAL, HIGHLIGHTED, SELECTED }

	[SerializeField]
	private Material NormalMaterialHost;
	[SerializeField]
	private Material HighlightMaterialHost;
	[SerializeField]
	private Material SelectedMaterialHost;

	[SerializeField]
	private Material NormalMaterialGuest;
	[SerializeField]
	private Material HighlightMaterialGuest;
	[SerializeField]
	private Material SelectedMaterialGuest;

	private Material NormalMaterial;
	private Material HighlightMaterial;
	private Material SelectedMaterial;

	private Status status;
	private MeshRenderer meshRenderer;

	public void Darken()
	{
		if (status == Status.HIGHLIGHTED)
		{
			Deselect();
		}
	}

	public void Highlight()
	{
		if (status != Status.SELECTED)
		{
			meshRenderer.material = HighlightMaterial;
			status = Status.HIGHLIGHTED;
		}
	}

	public void Select()
	{
		meshRenderer.material = SelectedMaterial;
		status = Status.SELECTED;
	}

	public void Deselect()
	{
		meshRenderer.material = NormalMaterial;
		status = Status.NORMAL;
	}

	public bool IsHost() => isHost;

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

		if (meshRenderer == null)
		{
			meshRenderer = GetComponent<MeshRenderer>();
		}

		//GetComponent<MeshRenderer>().material.color = (isHost) ? HOSTCOLOR : GUESTCOLOR;
		TextName.text = names[rank];

		NormalMaterial = isHost ? NormalMaterialHost : NormalMaterialGuest;
		HighlightMaterial = isHost ? HighlightMaterialHost : HighlightMaterialGuest;
		SelectedMaterial = isHost ? SelectedMaterialHost : SelectedMaterialGuest;

		Deselect();
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
