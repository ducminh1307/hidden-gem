using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class BoardStage
{
    public int boardSize;
    public int amountOfDynamites;
    public List<GemData> gems;
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool isDebugMode = true;

    [field: SerializeField] public int _stage { get; private set; } = 1;
    private int _pickaxe = 10;
    [SerializeField] private AddPickaxePanel addPickaxePanel;

    [SerializeField] private int chanceToDigGem = 30;
    [Header("Pickaxe")] [SerializeField] private PickaxeUI pickaxeUI;

    [Header("Board")] [SerializeField] private BoardManager boardManager;

    [SerializeField] private List<BoardStage> boardStages;

    [Header("Chest")] [SerializeField] private ChestManager chestManager;
    [SerializeField] private List<ChestData> dataChests;


    public static GameManager instance;

    public List<BoardStage> BoardStages => boardStages;

    public int Stage => _stage;

    public int Pickaxe => _pickaxe;

    public int ChanceToDigGem => chanceToDigGem;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        Debug.unityLogger.logEnabled = isDebugMode;
    }

    private void Start()
    {
        StartCoroutine(InitGame());
    }

    private IEnumerator InitGame()
    {
        chestManager.InitializeChests(dataChests);
        boardManager.GenerateBoard(boardStages[_stage - 1]);

        yield return null;

        chestManager.ActivateChest(_stage);
        pickaxeUI.UpdatePickaxeText(_pickaxe);
    }

    public void NextStage()
    {
        _stage++;

        DOVirtual.DelayedCall(0.5f, () =>
        {
            chestManager.UnlockChest(_stage);

            if (_stage <= boardStages.Count)
            {
                boardManager.GenerateBoard(boardStages[_stage - 1]);
                chestManager.MoveActivateToNextChest(_stage);
            }
        });
    }

    public void UsePickaxe()
    {
        _pickaxe--;
        pickaxeUI.UpdatePickaxeText(_pickaxe);
    }

    public void AddPickaxe(int amount)
    {
        _pickaxe += amount;
        pickaxeUI.UpdatePickaxeText(_pickaxe);
    }
    
    public void ShowAddPickaxePanel() => addPickaxePanel.Show();
}