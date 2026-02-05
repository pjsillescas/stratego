using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Piece : MonoBehaviour
{
	[SerializeField]
	private TextMeshPro TextName;

	[SerializeField]
	private SpriteRenderer RankImage;

	private bool isHost;
	private Rank rank;
	private Tile tile;
	private string pieceName;
	private bool isHidden;

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

	public bool GetIsHidden() => isHidden;

	public void Darken()
	{
		if (status == Status.HIGHLIGHTED)
		{
			Deselect();
		}
	}

	public void Highlight()
	{
		if (status != Status.SELECTED && HighlightMaterial != null)
		{
			meshRenderer.material = HighlightMaterial;
			status = Status.HIGHLIGHTED;
		}
	}

	public void Select()
	{
		if (SelectedMaterial == null)
		{
			return;			
		}
		
		meshRenderer.material = SelectedMaterial;
		status = Status.SELECTED;
	}

	public void Deselect()
	{
		if(NormalMaterial == null)
		{
			return;
		}

		meshRenderer.material = NormalMaterial;
		status = Status.NORMAL;
	}

	public bool IsHost() => isHost;

	public void Initialize(PieceData data, bool isHost, bool isHidden)
	{
		if (data != null)
		{
			rank = data.rank;
			RankImage.sprite = data.sprite;
			pieceName = data.rankName;
		}
		else
		{
			rank = Rank.DISABLED;
			pieceName = "";
		}

		this.isHost = isHost;
		this.isHidden = isHidden;

		if (meshRenderer == null)
		{
			meshRenderer = GetComponent<MeshRenderer>();
		}

		//GetComponent<MeshRenderer>().material.color = (isHost) ? HOSTCOLOR : GUESTCOLOR;
		if (TextName != null)
		{
			TextName.text = pieceName;
		}

		NormalMaterial = isHost ? NormalMaterialHost : NormalMaterialGuest;
		HighlightMaterial = isHost ? HighlightMaterialHost : HighlightMaterialGuest;
		SelectedMaterial = isHost ? SelectedMaterialHost : SelectedMaterialGuest;

		Deselect();

		if (isHidden)
		{
			HideData();
		}
	}

	public void HideData()
	{
		if (TextName != null)
		{
			TextName.gameObject.SetActive(false);
			RankImage.gameObject.SetActive(false);
		}
	}

	public void ShowData()
	{
		if (TextName != null)
		{
			TextName.gameObject.SetActive(true);
			RankImage.gameObject.SetActive(true);
		}
	}

	public virtual void SetTile(Tile tile)
	{
		this.tile = tile;
	}

	public Tile GetTile() => tile;

	public Rank GetRank() => rank;

	public bool GetIsHost() => isHost;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{

	}
}
