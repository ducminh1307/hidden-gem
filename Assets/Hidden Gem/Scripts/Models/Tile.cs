public class Tile
{
    public enum TileType
    {
        Empty,
        Stone,
        Gem,
        Dynamite
    }

    public TileType Type;
    public bool IsRevealed;

    public Tile(TileType type)
    {
        this.Type = type;
        this.IsRevealed = false;
    }
}
