using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private int _stage = 1;
    private int _pickaxe = 10;

    [SerializeField] private int boardSize;
    
    public int BoardSize => boardSize;
    
    public int Stage => _stage;
    
    public int Pickaxe => _pickaxe;
}
