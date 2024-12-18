using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class GemCollectManager : MonoBehaviour
{
    [SerializeField] private GameObject gemCollectPrefab;
    [SerializeField] private RectTransform gemCollectContainer;

    private int _gemCount;

    public UnityEvent OnCollectAllGem;
    public UnityEvent OnCollectCompleted;

    public List<GemCollectItem> GemCollectItems { get; } = new List<GemCollectItem>();

    public void InitializeGemCollect(List<Gem> gems)
    {
        gameObject.SetActive(true);
        _gemCount = gems.Count;
        foreach (var gem in gems)
        {
            var gemCollect = Instantiate(gemCollectPrefab, gemCollectContainer);
            var gemCollectSript = gemCollect.GetComponent<GemCollectItem>();

            gemCollectSript.OnGemCollect += OnGemCollected;
            gemCollectSript.InitGemCollectItem(gem);
            GemCollectItems.Add(gemCollectSript);
        }
    }

    private void OnGemCollected()
    {
        _gemCount--;
        if (_gemCount == 0)
        {
            OnCollectAllGem?.Invoke();
            DOVirtual.DelayedCall(1f, () =>
            {
                GetComponent<RectTransform>().DOAnchorPosY(550f, 1).SetEase(Ease.OutQuad).OnComplete(() =>
                {
                    ResetGemCollect();
                    gameObject.SetActive(false);
                    OnCollectCompleted?.Invoke();
                });
            });
        }
    }

    private void ResetGemCollect()
    {
        GemCollectItems.Clear();
        for (int i = 0; i < gemCollectContainer.childCount; i++)
        {
            Destroy(gemCollectContainer.GetChild(i).gameObject);
        }
    }
}