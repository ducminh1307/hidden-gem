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
    public ChestData ChestData { get; private set; }

    private void Awake()
    {
        chestButton.onClick.AddListener(()=> OnCheckChest.Invoke(ChestData.StageReward));
    }

    public void InitChest(ChestData data, int stageNumber)
    {
        ChestData = data;
        chestIcon.sprite = data.ChestSprite;
        buttonText.text = $"Stage {data.StageReward}";
        chestParticles.gameObject.SetActive(false);
        openButton.interactable = false;
    }

    public void ActivateChest()
    {
        chestParticles.gameObject.SetActive(true);
        openButton.interactable = true;
        buttonText.text = "OPEN";
    }

    public void OpenChest()
    {
        chestParticles.gameObject.SetActive(false);
        openButton.interactable = false;
        buttonText.text = "OPENED";
        GameManager.instance.AddPickaxe(ChestData.ChestPickaxe);
    }
    
    
}
