public class Tile
{
    public enum TileType
    {
        Empty,
        Stone,
        Gem,
        Dynamite
    }

    public int GemId;
    public TileType Type;
    
    public Tile(TileType type)
    {
        GemId = -1;
        Type = type;
    }
}
