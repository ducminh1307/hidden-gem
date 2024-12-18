using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AddPickaxePanel : MonoBehaviour
{
    [SerializeField] private RectTransform panel;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        panel.anchoredPosition = new Vector2(0, 1500);
        gameObject.SetActive(true);
        panel.DOLocalMoveY(0, .65f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        panel.DOLocalMoveY(1500, .65f).SetEase(Ease.InBack).OnComplete(()=> { gameObject.SetActive(false); });
    }
}
