using UnityEngine;

[CreateAssetMenu(fileName = "New Chest Data", menuName = "Hidden Gem/Chest Data")]
public class ChestData : ScriptableObject
{
    [SerializeField] private int stageReward;
    [SerializeField] private Sprite chestSprite;
    [SerializeField] private int chestPickaxe;

    public int StageReward => stageReward;

    public Sprite ChestSprite => chestSprite;
    
    public int ChestPickaxe => chestPickaxe;
}
