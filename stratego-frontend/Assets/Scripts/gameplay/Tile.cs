using Unity.VisualScripting;
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

	public void Darken()
	{
		// Debug.Log("darken tile");
		if (status == Status.HIGHLIGHTED)
		{
			Deselect();
		}
	}

	public void Highlight()
	{
		// Debug.Log("highlight tile");
		if (status != Status.SELECTED)
		{
			spriteRenderer.material = HighlightMaterial;
			status = Status.HIGHLIGHTED;
		}
	}

	public void Select()
	{
		// Debug.Log("select tile");
		spriteRenderer.material = SelectedMaterial;
		status = Status.SELECTED;
	}

	public void Deselect()
	{
		// Debug.Log("deselect tile");
		spriteRenderer.material = NormalMaterial;
		status = Status.NORMAL;
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
}
