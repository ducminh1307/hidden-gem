using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ChestManager : MonoBehaviour
{
    [Header("Elements")] [SerializeField] private Slider progressSlider;
    [SerializeField] private RectTransform rectTransformChestParent;
    [SerializeField] private GameObject chestPrefab;
    [SerializeField] private ActivateChest chestActive;
    [SerializeField] private InfoChest chestInfo;

    private List<ChestData> _dataChests = new List<ChestData>();
    private List<RectTransform> rectChests = new List<RectTransform>();
    private List<Chest> chests = new List<Chest>();

    public void InitializeChests(List<ChestData> dataChests)
    {
        //Sap xep lai list data chest theo stageReward
        dataChests.Sort((a, b) => a.StageReward.CompareTo(b.StageReward));

        _dataChests = dataChests;
        progressSlider.maxValue = dataChests.Count - 1;
        chestInfo.gameObject.SetActive(false);

        for (int i = 0; i < dataChests.Count; i++)
        {
            var chest = Instantiate(chestPrefab, rectTransformChestParent);
            var chestScript = chest.GetComponent<Chest>();
            chestScript.InitChest(dataChests[i], i + 1);
            chests.Add(chestScript);
            rectChests.Add(chest.GetComponent<RectTransform>());
        }

        foreach (var chest in chests)
        {
            chest.OnCheckChest += ShowInfoChest;
        }
    }

    public void ActivateChest(int stage) => chestActive.ActiveChest(rectChests[stage - 1].anchoredPosition.x);

    public void MoveActivateToNextChest(int stage)
    {
        int index = stage - 1;

        chestActive.MoveActiveToNextChest(rectChests[index].anchoredPosition.x);
        progressSlider.DOValue(index, .5f).SetEase(Ease.OutQuad);
    }

    private void UnlockChest(int index)
    {
        if (index > 0) chests[index].ActivateChest();
    }

    private void ShowInfoChest(int stage)
    {
        ChestData data = _dataChests.Find(data => data.StageReward == stage);
        float anchorPosX = rectChests[stage - 1].anchoredPosition.x;

        chestInfo.gameObject.SetActive(false);
        chestInfo.Show(data.ChestPickaxe, anchorPosX);
    }
}