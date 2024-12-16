using System.Collections.Generic;
using UnityEngine;

public class Gem
{
    public int GemId { get; }
    public bool CanMove;
    public GemData GemData { get; }
    public Vector2Int StartPoint;
    public List<Vector2Int> OccupiedPositions = new List<Vector2Int>();

    public Gem(int gemId, GemData gemData)
    {
        GemId = gemId;
        GemData = gemData;
        CanMove = true;
    }
}