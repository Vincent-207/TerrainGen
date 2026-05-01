using UnityEngine;

public class Dropoff : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player"))
        {
            CompleteOrder();    
        }
    }

    void CompleteOrder()
    {
        // Let manager handle adding data and creating new order
        OrderManager.instance.CompleteOrder();
        Destroy(gameObject);
        
    }
}
