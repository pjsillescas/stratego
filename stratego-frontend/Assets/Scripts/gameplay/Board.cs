using UnityEngine;

public class Board : MonoBehaviour
{
	public const int NUM_ROWS = 10;
	public const int NUM_COLUMNS = 10;
	
	public const float TileWidth = 1f;
	public const float TileHeight = 1f;

	[SerializeField]
	private GameObject TilePrefab;

	private Tile[,] Tiles;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		Tiles = new Tile[NUM_ROWS, NUM_COLUMNS];
		for(var row = 0; row < NUM_ROWS; row++)
		{
			for (var col = 0; col < NUM_COLUMNS; col++)
			{
				var position = GetWorldPosition(row, col) + new Vector3(0, 0.001f, 0);
				var rotation = Quaternion.Euler(90, 0, 0);
				Tiles[row, col] = Instantiate(TilePrefab, position, rotation).GetComponent<Tile>();
				Tiles[row, col].Initialize(row, col);
			}
		}
	}

	public Vector3 GetWorldPosition(int row, int col)
	{
		float z = row * TileWidth - ((NUM_ROWS - 1) * TileWidth / 2);
		float x = col * TileHeight - ((NUM_COLUMNS - 1) * TileHeight / 2);
		return new Vector3(x, 0, z);

	}

	public Tile GetTile(int row, int col)
	{
		return Tiles[row, col];
	}

	// Update is called once per frame
	void Update()
	{

	}
}
