using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject stonePrefab;

    [SerializeField] private GridLayoutGroup gridLayoutStone;
    [SerializeField] private RectTransform rectTransformStoneParent;
    [SerializeField] private RectTransform rectTransformGemParent;

    private Tile[,] _board;

    private void Start()
    {
        GenerateBoard(GameManager.Instance.BoardSize);
    }

    private void GenerateBoard(int boardSize)
    {
        StartCoroutine(GenerateBoardCoroutine(boardSize));
    }

    private IEnumerator GenerateBoardCoroutine(int boardSize)
    {
        gridLayoutStone.enabled = true;
        
        if (_board != null)
            _board = null;
        _board = new Tile[boardSize, boardSize];

        float cellSize = rectTransformStoneParent.rect.width / boardSize;
        gridLayoutStone.cellSize = new Vector2(cellSize, cellSize);

        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                _board[i, j] = new Tile(Tile.TileType.Dynamite);
                
                GameObject tileItem = Instantiate(stonePrefab, rectTransformStoneParent);
                tileItem.GetComponent<TileItem>().Initialize(this, positionX: i, positionY: j);
            }
        }

        yield return null;
        
        gridLayoutStone.enabled = false;
    }

    public void Dig(int PosX, int PosY)
    {
        _board[PosX, PosY].IsRevealed = true;
        
        switch (_board[PosX, PosY].Type)
        {
            case Tile.TileType.Empty:
                break;
            case Tile.TileType.Stone:
                _board[PosX, PosY].Type = Tile.TileType.Empty;
                break;
            case Tile.TileType.Gem:
                break;
            case Tile.TileType.Dynamite:
                Debug.Log("Boom!");
                break;
        }
    }

    //Pha huy 3x3 tai vi tri PosX, PosY
    private void Explode(int PosX, int PosY)
    {
        for (int checkX = -1; checkX <= 1; checkX++)
        {
            for (int checkY = -1; checkY <= 1; checkY++)
            {
                int destroyX = PosX + checkX;
                int destroyY = PosY + checkY;

                if (destroyX >= 0 && destroyX < GameManager.Instance.BoardSize && destroyY >= 0 && destroyY < GameManager.Instance.BoardSize)
                {
                    Dig(destroyX, destroyY);
                }
            }
        }
    }
}