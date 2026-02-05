public class DisablePiece : Piece
{
	private void Awake()
	{
		Initialize(null, false, true);
	}

	public override void SetTile(Tile tile)
	{
		base.SetTile(tile);
		tile.DisableTile();
	}
}
