using TMPro;
using UnityEngine;

public class PickaxeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI quantityText;

    public void UpdatePickaxeText(int pickaxeAmount)
    {
        quantityText.text = $"x{pickaxeAmount}";
    }
}
