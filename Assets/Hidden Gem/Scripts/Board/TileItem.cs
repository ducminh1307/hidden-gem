using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TileItem : MonoBehaviour, IPointerClickHandler
{
    private BoardManager _boardManager;
    private int _positionX;
    private int _positionY;

    [SerializeField] Image icon;

    [SerializeField] private ParticleSystem destroyedEffect;
    public UnityEvent OnTileClicked;
    public UnityEvent OnDestroyedTile;

    private void OnEnable()
    {
        OnTileClicked.AddListener(OnClick);
        OnDestroyedTile.AddListener(DestroyedTile);
    }

    public void Initialize(BoardManager boardManager, int positionX, int positionY)
    {
        _boardManager = boardManager;
        _positionX = positionX;
        _positionY = positionY;
    }

    private void OnClick()
    {
        GameManager.instance.UsePickaxe();
        OnTileClicked.RemoveListener(OnClick);
    }

    private void DestroyedTile()
    {
        _boardManager.Dig(_positionX, _positionY);
        icon.gameObject.SetActive(false);
        destroyedEffect.Play();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.instance.Pickaxe <= 0)
            GameManager.instance.ShowAddPickaxePanel();
        else
        {
            OnTileClicked?.Invoke();
            OnDestroyedTile?.Invoke();
        }
    }

    private void OnDestroy()
    {
        OnTileClicked.RemoveListener(OnClick);
    }
}