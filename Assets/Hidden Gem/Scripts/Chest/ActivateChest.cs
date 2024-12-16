using System;
using DG.Tweening;
using UnityEngine;

public class ActivateChest : MonoBehaviour
{
    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void ActiveChest(float anchorPosX)
    {
        _rect.anchoredPosition = new Vector2(anchorPosX, _rect.anchoredPosition.y);
    }
    
    public void MoveActiveToNextChest(float anchorPosX)
    {
        _rect.DOAnchorPosX(anchorPosX, .5f).SetEase(Ease.InOutQuad);
    }
}
