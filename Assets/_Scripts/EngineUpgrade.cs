using TMPro;
using UnityEngine;

public class EngineUpgrade : MonoBehaviour
{
    int cost = 200;
    [SerializeField]
    TMP_Text costDisplay;
    void Start()
    {
        UpdateCostDisplay();
    }
    void UpdateCostDisplay()
    {
        costDisplay.text = "" + cost;
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
        CarController car = FindAnyObjectByType<CarController>();
        car.maxAccel += 6000;
        OrderMenu.instance.RemoveCoins(cost);
        cost += 200;
        UpdateCostDisplay();
    }

}
