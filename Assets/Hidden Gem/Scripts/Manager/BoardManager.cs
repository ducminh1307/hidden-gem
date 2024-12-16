using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [Header("Prefabs")] [SerializeField] private GameObject stonePrefab;
    [SerializeField] private GameObject gemPrefab;
    [SerializeField] private GameObject dynamitePrefab;

    [Header("Elements")] [SerializeField] private GridLayoutGroup gridLayoutStone;
    [SerializeField] private RectTransform rectTransformStoneParent;
    [SerializeField] private RectTransform rectTransformGemParent;

    private Tile[,] _board;
    private float _tileSize;
    private RectTransform[,] _rectTiles;
    private List<Gem> _gems = new List<Gem>();
    private List<GemItem> _gemItems = new List<GemItem>();
    private List<Vector2Int> _gemStartPositions = new List<Vector2Int>();

    private BoardStage _currentStage;

    public void GenerateBoard(BoardStage stage)
    {
        _currentStage = stage;
        _currentStage.gems.Sort((a, b) => (b.Width * b.Height).CompareTo(a.Width * a.Height));

        StartCoroutine(GenerateBoardCoroutine(stage.boardSize));
    }

    private IEnumerator GenerateBoardCoroutine(int boardSize)
    {
        gridLayoutStone.enabled = true;

        ResetData();

        _board = new Tile[boardSize, boardSize];
        _rectTiles = new RectTransform[boardSize, boardSize];

        for (int i = 0; i < _currentStage.gems.Count; i++)
        {
            _gems.Add(new Gem(i, _currentStage.gems[i]));
        }

        _tileSize = rectTransformStoneParent.rect.width / boardSize;
        gridLayoutStone.cellSize = new Vector2(_tileSize, _tileSize);

        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                _board[i, j] = new Tile(Tile.TileType.Stone);

                GameObject tileItem = Instantiate(stonePrefab, rectTransformStoneParent);
                tileItem.gameObject.name = $"Stone [{i},{j}]";
                tileItem.GetComponent<TileItem>().Initialize(this, positionX: i, positionY: j);

                _rectTiles[i, j] = tileItem.GetComponent<RectTransform>();
            }
        }

        yield return null;

        gridLayoutStone.enabled = false;

        yield return null;

        PlaceRandomGem(_gems);
    }

    private void ResetData()
    {
        if (_board != null)
        {
            _board = null;
            _rectTiles = null;
        }

        _gems?.Clear();
    }

    public void Dig(int PosX, int PosY)
    {
        switch (_board[PosX, PosY].Type)
        {
            case Tile.TileType.Stone:
                _board[PosX, PosY].Type = Tile.TileType.Empty;
                break;
            case Tile.TileType.Gem:
                _gems.Find(g => g.GemId == _board[PosX, PosY].GemId).CanMove = false;
                break;
            case Tile.TileType.Dynamite:
                break;
        }

        ResetBoardToRandom();
    }

    private void ResetBoardToRandom()
    {
        List<Gem> gemsCanRandom = _gems.Where(g => g.CanMove).ToList();

        for (int row = 0; row < _currentStage.boardSize; row++)
        {
            for (int col = 0; col < _currentStage.boardSize; col++)
            {
                if (_board[row, col].Type == Tile.TileType.Gem)
                {
                    Gem gemAtPos = _gems.Find(g => g.GemId == _board[row, col].GemId);

                    if (gemAtPos is { CanMove: true })
                    {
                        _board[row, col].Type = Tile.TileType.Stone;
                        _board[row, col].GemId = -1;
                    }
                }
            }
        }

        bool isPlacedAllGem = PlaceRandomGem(gemsCanRandom);

        Debug.Log($"Placed all gem: {isPlacedAllGem}");
        if (!isPlacedAllGem)
            RestorePreviousBoard(gemsCanRandom);
    }

    #region Random Place Gem

    private bool PlaceRandomGem(List<Gem> gems)
    {
        List<Vector2Int> allPositions = new List<Vector2Int>();

        for (int y = 0; y < _currentStage.boardSize; y++)
        {
            for (int x = 0; x < _currentStage.boardSize; x++)
            {
                if (_board[y, x].Type != Tile.TileType.Empty && _board[y, x].Type != Tile.TileType.Gem)
                {
                    allPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        ShufflePosition(allPositions);

        foreach (var gem in gems)
        {
            bool isPlaced = false;
            foreach (var position in allPositions)
            {
                int startX = position.x;
                int startY = position.y;

                // Kiểm tra kích thước gem với board
                if (startY + gem.GemData.Height > _currentStage.boardSize ||
                    startX + gem.GemData.Width > _currentStage.boardSize)
                {
                    continue;
                }

                // Kiểm tra nếu gem có thể được đặt
                if (CanPlaceGem(gem.GemData, startY, startX))
                {
                    gem.OccupiedPositions.Clear();

                    PlaceGem(gem, startY, startX);

                    // Loại bỏ các ô đã sử dụng khỏi danh sách
                    RemoveUsedPositions(allPositions, gem.GemData, startY, startX);
                    isPlaced = true;
                    break;
                }
            }

            // Nếu không đặt được gem, return false
            if (!isPlaced)
            {
                return false;
            }
        }

        UpdateVisualGem();
        return true;
    }

    private void ShufflePosition(List<Vector2Int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    private void RemoveUsedPositions(List<Vector2Int> allPositions, GemData gem, int startRow, int startCol)
    {
        for (int rowOffset = 0; rowOffset < gem.Height; rowOffset++)
        {
            for (int colOffset = 0; colOffset < gem.Width; colOffset++)
            {
                allPositions.Remove(new Vector2Int(startCol + colOffset, startRow + rowOffset));
            }
        }
    }

    private void RestorePreviousBoard(List<Gem> gems)
    {
        foreach (var gem in gems)
        {
            foreach (var position in gem.OccupiedPositions)
            {
                _board[position.x, position.y].Type = Tile.TileType.Gem;
                _board[position.x, position.y].GemId = gem.GemId;
            }
        }
    }

    private bool CanPlaceGem(GemData gem, int startRow, int startCol)
    {
        for (int rowOffset = 0; rowOffset < gem.Height; rowOffset++)
        {
            for (int colOffset = 0; colOffset < gem.Width; colOffset++)
            {
                if (_board[startRow + rowOffset, startCol + colOffset].Type != Tile.TileType.Stone)
                    return false;
            }
        }

        return true;
    }

    private void PlaceGem(Gem gem, int startRow, int startCol)
    {
        gem.StartPoint = new Vector2Int(startCol, startRow);
        for (int rowOffset = 0; rowOffset < gem.GemData.Height; rowOffset++)
        {
            for (int colOffset = 0; colOffset < gem.GemData.Width; colOffset++)
            {
                _board[startRow + rowOffset, startCol + colOffset].Type = Tile.TileType.Gem;
                _board[startRow + rowOffset, startCol + colOffset].GemId = gem.GemId;
                gem.OccupiedPositions.Add(new Vector2Int(startRow + rowOffset, startCol + colOffset));
            }
        }

        // Debug.Log($"Placed gem {gem.GemData.Width}x{gem.GemData.Height} at {startRow}x{startCol}");
    }

    private void UpdateVisualGem()
    {
        foreach (var gem in _gems)
        {
            if (_gemItems.Count != _currentStage.gems.Count)
            {
                GameObject gemItem = Instantiate(gemPrefab, rectTransformGemParent);
                var gemItemScript = gemItem.GetComponent<GemItem>();
                gemItemScript.InitGem(gem, _tileSize, _rectTiles[gem.StartPoint.y, gem.StartPoint.x].anchoredPosition);
                _gemItems.Add(gemItemScript);
            }
            else
            {
                _gemItems.Find(g => g.Gem == gem).InitGem(gem, _tileSize,
                    _rectTiles[gem.StartPoint.y, gem.StartPoint.x].anchoredPosition);
            }
        }
    }

    #endregion
}