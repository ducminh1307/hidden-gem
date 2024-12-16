using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    [SerializeField] private Image chestIcon;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Button openButton;
    [SerializeField] private Button chestButton;
    [SerializeField] private ParticleSystem chestParticles;
    public UnityAction<int> OnCheckChest;
    private ChestData _chestData;

    private void Awake()
    {
        chestButton.onClick.AddListener(()=> OnCheckChest.Invoke(_chestData.StageReward));
    }

    public void InitChest(ChestData data, int stageNumber)
    {
        _chestData = data;
        chestIcon.sprite = data.ChestSprite;
        buttonText.text = $"Stage {data.StageReward}";
        chestParticles.gameObject.SetActive(false);
        openButton.interactable = false;
    }

    public void ActivateChest()
    {
        chestParticles.gameObject.SetActive(true);
        openButton.interactable = true;
        buttonText.text = "Open";
    }

    public void OpenChest()
    {
        chestParticles.gameObject.SetActive(false);
        openButton.interactable = false;
        buttonText.text = "Opened";
        GameManager.Instance.Pickaxe += _chestData.ChestPickaxe;
    }
    
    
}
