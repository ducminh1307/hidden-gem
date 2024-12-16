using UnityEngine;
using UnityEngine.UI;

public class TileItem : MonoBehaviour
{
    private BoardManager _boardManager;
    private int _positionX;
    private int _positionY;
    
    [SerializeField] private Button tileButton;
    [SerializeField] private ParticleSystem destroyedEffect;

    private void OnEnable()
    {
        tileButton.onClick.AddListener(OnClick);
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
        tileButton.gameObject.SetActive(false);
        destroyedEffect.Play();
    }
}
