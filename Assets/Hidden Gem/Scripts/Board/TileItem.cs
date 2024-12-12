using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TileItem : MonoBehaviour
{
    private BoardManager _boardManager;
    private int _positionX;
    private int _positionY;
    
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    public void Initialize(BoardManager boardManager, int positionX, int positionY)
    {
        _boardManager = boardManager;
        _positionX = positionX;
        _positionY = positionY;
    }

    private void OnClick()
    {
        _boardManager.Dig(_positionX, _positionY);
        Destroy(gameObject);
    }
}
