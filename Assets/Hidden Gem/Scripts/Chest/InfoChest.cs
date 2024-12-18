using DG.Tweening;
using TMPro;
using UnityEngine;

public class InfoChest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI quantityText;
    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void Show(int quantity, float anchorPosX)
    {
        gameObject.SetActive(true);
        quantityText.text = $"x {quantity}";
        
        _rect.anchoredPosition = new Vector2(anchorPosX, _rect.anchoredPosition.y);
        
        // DOVirtual.DelayedCall(2f, () => gameObject.SetActive(false));
    }
}