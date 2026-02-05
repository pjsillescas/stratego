using UnityEngine;

public class Tile : MonoBehaviour
{
	private enum Status { NORMAL, HIGHLIGHTED, SELECTED }

	[SerializeField]
	private Material NormalMaterial;
	[SerializeField]
	private Material HighlightMaterial;
	[SerializeField]
	private Material SelectedMaterial;

	private Status status;
	private SpriteRenderer spriteRenderer;
	private int row;
	private int col;


	public void DisableTile()
	{
		spriteRenderer.enabled = false;
	}

	public void EnableTile()
	{
		spriteRenderer.enabled = true;
		Deselect();
	}

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
			spriteRenderer.material = HighlightMaterial;
			status = Status.HIGHLIGHTED;
		}
	}

	public void Select()
	{
		spriteRenderer.material = SelectedMaterial;
		status = Status.SELECTED;
	}

	public void Deselect()
	{
		spriteRenderer.material = NormalMaterial;
		status = Status.NORMAL;
	}

	public void Initialize(int row, int col)
	{
		this.row = row;
		this.col = col;
	}

	public int GetRow() => row;
	public int GetCol() => col;

	public bool HasCoordinates(int row, int col)
	{
		return row == this.row && col == this.col;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		Deselect();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public override string ToString()
	{
		return $"({row},{col})";
	}
}
