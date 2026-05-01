using Unity.VisualScripting;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager instance;
    public GameObject orderPrefab;
    public float spawnRange = 20f;
    public LayerMask whatIsGround;
    // public FractionalBrownianMotion noiseGen;
    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    void Start()
    {
        SpawnOrder();
    }

    public void SpawnOrder()
    {
        JourneyTracker.instance.EnableJourney();
        Vector2 random = Random.insideUnitCircle * spawnRange;
        Vector2 flatPos2 = new(transform.position.x, transform.position.z);
        Vector3 worldDir = new(random.x, 0,random.y);
        Vector3 flatChunkGenPos = chunkGen.instance.transform.position;
        flatChunkGenPos.y =0;
        Vector3 spawnPosFlat = worldDir + flatChunkGenPos;
        if(chunkGen.instance == null) Debug.Log("NULLL1");
        Debug.Log("flatpos: "  + spawnPosFlat);
        chunkGen.instance.CreateSquare(spawnPosFlat);


        RaycastHit hitInfo;
        Vector3 spawnPos = new();
        Debug.DrawRay(spawnPosFlat + Vector3.up * 100f, Vector3.down);
        if(Physics.Raycast(spawnPosFlat + Vector3.up * 1000f, Vector3.down, out hitInfo, 10000f, whatIsGround))
        {
            spawnPos = hitInfo.point;
        }
        else
        {
            Debug.LogWarning("COuldn't find ground to spawn on. ");
            
            return;
        }
        

        
        GameObject order = Instantiate(orderPrefab, spawnPos, Quaternion.identity);
        
        Compass.instance.UpdateObjectiveTransform(order.transform);
        
        float orderWeight = 20f;
        JourneyTracker.instance.SetWeight(orderWeight);
        JourneyTracker.instance.isInProgress = true;

    }
    public void CompleteOrder()
    {
        // Pause game
        Time.timeScale = 0;
        JourneyTracker.instance.DisableJourney();
        // Get delivery quality info
        float timeTraveled = JourneyTracker.instance.GetTime();
        float weightCarried = JourneyTracker.instance.GetWeight();
        float collisions = JourneyTracker.instance.GetCollisions();
        Delivery delivery = new (timeTraveled, weightCarried, collisions);

        // Display order menu.
        OrderMenu.instance.Display(delivery);
        // SpawnOrder();
    }
}

