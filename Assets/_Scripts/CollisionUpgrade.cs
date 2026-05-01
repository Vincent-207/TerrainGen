using TMPro;
using UnityEngine;

public class CollisionUpgrade : MonoBehaviour
{
    int cost = 250;
    [SerializeField]
    TMP_Text costDisplay;
    void Start()
    {
        UpdateCostDisplay();
    }
    public void TryUpgrade()
    {
        if(OrderMenu.instance.hasCoins(cost))
        {
            DoUpgrade();    
        }
    }

    void DoUpgrade()
    {
        cost += 250;
        JourneyTracker.instance.collisionThreshold += 500;
        OrderMenu.instance.RemoveCoins(cost);
        UpdateCostDisplay();
    }

    void UpdateCostDisplay()
    {
        costDisplay.text = "" + cost;
    }
}
