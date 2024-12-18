using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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

    [Space] [SerializeField] private GemCollectManager gemCollectManager;

    private Tile[,] _board;
    private TileItem[,] _tileItems;
    private float _tileSize;
    private List<Gem> _gems = new List<Gem>();
    private List<GemItem> _gemItems = new List<GemItem>();

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
        _tileItems = new TileItem[boardSize, boardSize];

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
                
                var tileItemScript = tileItem.GetComponent<TileItem>();

                tileItemScript.Initialize(this, positionX: i, positionY: j);
                _tileItems[i, j] = tileItemScript;
            }
        }

        yield return null;

        gridLayoutStone.enabled = false;

        yield return null;

        bool gemPlacementSuccess = false;
        do
        {
            gemPlacementSuccess = PlaceRandomGem(_gems);
            Debug.Log(gemPlacementSuccess);
        } while (!gemPlacementSuccess);

        gemCollectManager.InitializeGemCollect(_gems);
    }

    private void ResetData()
    {
        if (_board != null)
        {
            foreach (var gemItem in _gemItems)
            {
                Destroy(gemItem.gameObject);
            }

            foreach (var tile in _tileItems)
            {
                Destroy(tile.gameObject);
            }

            _board = null;
            _tileItems = null;
            _gemItems.Clear();
        }

        _gems?.Clear();
    }

    public void Dig(int posX, int posY)
    {
        switch (_board[posX, posY].Type)
        {
            case Tile.TileType.Stone:
                _board[posX, posY].Type = Tile.TileType.Empty;
                break;
            case Tile.TileType.Gem:
                Gem gem = _gems.Find(g => g.GemId == _board[posX, posY].GemId);
                gem.OccupiedPositions.Remove(new Vector2Int(posX, posY));

                if (gem.IsDiscovered)
                {
                    Sequence animGem = DOTween.Sequence();

                    animGem.Append(_gemItems.Find(i => i.Gem.GemId == gem.GemId).gemIcon.DOFade(0, 0.5f)
                        .SetEase(Ease.OutQuart));

                    animGem.Join(_gemItems.Find(i => i.Gem.GemId == gem.GemId).GetComponent<RectTransform>()
                        .DOScale(1.2f, 0.5f)
                        .SetEase(Ease.OutQuart));

                    gemCollectManager.GemCollectItems.Find(gc => gc.Gem.GemId == gem.GemId).Activate();
                    DOVirtual.DelayedCall(1.5f,
                        () => animGem.Play());
                }

                break;
            case Tile.TileType.Dynamite:
                _board[posX, posY].Type = Tile.TileType.Empty;
                Explode(posX, posY);
                break;
        }
    }

    private void Explode(int posX, int posY)
    {
        Debug.Log("Explode");
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int checkRow = posX + x;
                int checkCol = posY + y;
                if (checkRow >= 0 && checkRow < _currentStage.boardSize && checkCol >= 0 &&
                    checkCol < _currentStage.boardSize)
                {
                    _tileItems[checkRow, checkCol].OnDestroyedTile?.Invoke();
                    Dig(posX, posY);
                }
            }
        }
    }

    #region Random Place Gem

    private bool PlaceRandomGem(List<Gem> gems)
    {
        ResetBoardToStone();
        List<Vector2Int> allPositions = GetAvailablePosition();

        ShufflePosition(allPositions);

        foreach (var gem in gems)
        {
            bool isPlaced = false;
            foreach (var position in allPositions)
            {
                int startCol = position.x;
                int startRow = position.y;

                // Kiểm tra kích thước gem với board
                if (startRow + gem.GemData.Height > _currentStage.boardSize ||
                    startCol + gem.GemData.Width > _currentStage.boardSize)
                {
                    continue;
                }

                // Kiểm tra nếu gem có thể được đặt
                if (CanPlaceGem(gem.GemData, startRow, startCol))
                {
                    gem.OccupiedPositions.Clear();

                    PlaceGem(gem, startRow, startCol);

                    // Loại bỏ các ô đã sử dụng khỏi danh sách
                    RemoveUsedPositions(allPositions, startRow, startCol, gem.GemData);
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

        if (_currentStage.amountOfDynamites > 0)
        {
            for (int i = 0; i < _currentStage.amountOfDynamites; i++)
            {
                PlaceDynamite(allPositions[i].x, allPositions[i].y);
                RemoveUsedPositions(allPositions, allPositions[i].y, allPositions[i].x);
            }
        }

        return true;
    }

    private void PlaceDynamite(int startX, int startY)
    {
        Debug.Log($"Place Dynamite at {startX}, {startY}");
        _board[startY, startX].Type = Tile.TileType.Dynamite;
        var dynamiteObject = Instantiate(dynamitePrefab, rectTransformGemParent);

        dynamiteObject.GetComponent<RectTransform>().sizeDelta = new Vector2(_tileSize, _tileSize);

        var rectTile = _tileItems[startY, startX].GetComponent<RectTransform>();
        dynamiteObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(rectTile.anchoredPosition.x, rectTile.anchoredPosition.y);
    }

    private List<Vector2Int> GetAvailablePosition()
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        for (int y = 0; y < _currentStage.boardSize; y++)
        {
            for (int x = 0; x < _currentStage.boardSize; x++)
            {
                if (_board[y, x].Type != Tile.TileType.Empty && _board[y, x].Type != Tile.TileType.Gem)
                {
                    positions.Add(new Vector2Int(x, y));
                }
            }
        }

        return positions;
    }

    private void ShufflePosition(List<Vector2Int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    private void RemoveUsedPositions(List<Vector2Int> allPositions, int startRow, int startCol, GemData gem = null)
    {
        if (!gem)
        {
            allPositions.Remove(new Vector2Int(startCol, startRow));
        }
        else
        {
            for (int rowOffset = 0; rowOffset < gem.Height; rowOffset++)
            {
                for (int colOffset = 0; colOffset < gem.Width; colOffset++)
                {
                    allPositions.Remove(new Vector2Int(startCol + colOffset, startRow + rowOffset));
                }
            }
        }
    }

    private void ResetBoardToStone()
    {
        for (int y = 0; y < _currentStage.boardSize; y++)
        {
            for (int x = 0; x < _currentStage.boardSize; x++)
            {
                _board[y, x].Type = Tile.TileType.Stone;
            }
        }

        for (var i = 0; i < rectTransformGemParent.childCount; i++)
        {
            Destroy(rectTransformGemParent.GetChild(i).gameObject);
        }

        _gemItems.Clear();
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
        for (int rowOffset = 0; rowOffset < gem.GemData.Height; rowOffset++)
        {
            for (int colOffset = 0; colOffset < gem.GemData.Width; colOffset++)
            {
                _board[startRow + rowOffset, startCol + colOffset].Type = Tile.TileType.Gem;
                _board[startRow + rowOffset, startCol + colOffset].GemId = gem.GemId;
                gem.OccupiedPositions.Add(new Vector2Int(startRow + rowOffset, startCol + colOffset));
            }
        }

        UpdateVisualGem(gem, startRow, startCol);

        // Debug.Log($"<color=#f0f>Placed</color> gem {gem.GemData.Width}x{gem.GemData.Height} at {startRow}x{startCol}");
    }

    private void UpdateVisualGem(Gem gem, int startX, int startY)
    {
        GameObject gemItem = Instantiate(gemPrefab, rectTransformGemParent);
        var gemItemScript = gemItem.GetComponent<GemItem>();
        gemItemScript.InitGem(gem, _tileSize, _tileItems[startX, startY].GetComponent<RectTransform>().anchoredPosition);
        _gemItems.Add(gemItemScript);
    }

    #endregion

    public void CompleteBoard()
    {
        foreach (var tile in _tileItems)
        {
            tile.OnDestroyedTile?.Invoke();
        }
    }
}