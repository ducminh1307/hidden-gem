using System.Collections.Generic;
using UnityEngine;

public class Gem
{
    public int GemId { get; }
    public GemData GemData { get; }
    public List<Vector2Int> OccupiedPositions = new List<Vector2Int>();
    public bool IsDiscovered => OccupiedPositions.Count == 0;

    public Gem(int gemId, GemData gemData)
    {
        GemId = gemId;
        GemData = gemData;
    }
}