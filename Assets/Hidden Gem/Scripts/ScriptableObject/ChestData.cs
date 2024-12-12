using UnityEngine;

[CreateAssetMenu(fileName = "New Chest Data", menuName = "Hidden Gem/Chest Data")]
public class ChestData : ScriptableObject
{
    [SerializeField] private Sprite chestSprite;
    [SerializeField] private int chestPickaxe;
    
    public Sprite ChestSprite => chestSprite;
    
    public int ChestPickaxe => chestPickaxe;
}
