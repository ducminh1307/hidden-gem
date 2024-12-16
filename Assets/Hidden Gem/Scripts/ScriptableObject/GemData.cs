using UnityEngine;

[CreateAssetMenu(fileName = "New Gem", menuName = "Hidden Gem/Gem")]
public class GemData : ScriptableObject
{
    [SerializeField] private Sprite gemSprite;
    [SerializeField] private int width;
    [SerializeField] private int height;
    
    public Sprite GemSprite => gemSprite;
    
    public int Width => width;
    
    public int Height => height;
}
