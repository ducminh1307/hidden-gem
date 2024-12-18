using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GemCollectItem : MonoBehaviour
{
    // [SerializeField] private DOTweenAnimation animation;
    [SerializeField] private Image shadow;
    [SerializeField] private Image icon;
    public Gem Gem;
    
    public UnityAction OnGemCollect;
    
    public void InitGemCollectItem(Gem gem)
    {
        Gem = gem;
        shadow.sprite = gem.GemData.GemSprite;
        icon.sprite = gem.GemData.GemSprite;
        
        icon.gameObject.SetActive(false);
    }
    
    public void Activate()
    {
        OnGemCollect.Invoke();
        icon.gameObject.SetActive(true);
    }
}
