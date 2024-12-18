using UnityEngine;
using UnityEngine.UI;

public class GemItem : MonoBehaviour
{
    [field:SerializeField] public Image gemIcon;
    private RectTransform _rect;

    public Gem Gem { get; private set; }

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void InitGem(Gem gem, float tileSize, Vector2 anchoredPos)
    {
        Gem = gem;
        gemIcon.sprite = gem.GemData.GemSprite;
        _rect.sizeDelta = new Vector2(tileSize * gem.GemData.Width, tileSize * gem.GemData.Height);
        
        _rect.anchoredPosition = new Vector2(anchoredPos.x + (gem.GemData.Width - 1) * (tileSize / 2), anchoredPos.y - (gem.GemData.Height - 1) * (tileSize / 2));
    }
}
