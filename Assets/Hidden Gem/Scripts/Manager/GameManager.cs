using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class BoardStage
{
    public int boardSize;
    public int amountOfDynamites;
    public List<GemData> gems;
}

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private bool isDebugMode = true;
    
    [SerializeField] private int _stage = 1;
    private int _pickaxe = 10;

    [SerializeField] private int chanceToDigGem = 30;
    
    [Header("Board")]
    [SerializeField] private BoardManager boardManager;

    [SerializeField] private List<BoardStage> boardStages;
    
    [Header("Chest")]
    [SerializeField] private ChestManager chestManager;
    [SerializeField] private List<ChestData> dataChests;
    
    
    public List<BoardStage> BoardStages => boardStages;
    
    public int Stage => _stage;

    public int Pickaxe
    {
        get => _pickaxe;
        set => _pickaxe = value;
    }
    
    public int ChanceToDigGem => chanceToDigGem;

    protected override void Awake()
    {
        Debug.unityLogger.logEnabled = isDebugMode;
    }

    private void Start()
    {
        StartCoroutine(InitGame());
    }

    private IEnumerator InitGame()
    {
        chestManager.InitializeChests(dataChests);
        boardManager.GenerateBoard(boardStages[_stage-1]);
        
        yield return null;
        
        chestManager.ActivateChest(_stage);
    }
}
