using System.Collections.Generic;
using UnityEngine;

public class chunkGen : MonoBehaviour
{
    /*
        Generate chunk at 0,0

    */
    [SerializeField] DerivativeComputeController derivativeComputeController;
    [SerializeField]
    GameObject chunkPrefab;

    Dictionary<Vector2Int, bool> isChunkInstantied;
    Dictionary<Vector2Int, GameObject> instantiedChunks;
    public static chunkGen instance;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;
        if(isChunkInstantied == null) isChunkInstantied = new();
        if(instantiedChunks == null) instantiedChunks = new();
    }
    void Start()
    {
        // CreateChunk(Vector2Int.up);
        // instantiatedChunks[Vector2Int.one] = true;
        
    }

    void Update()
    {
        CreateRadius(worldToGridPos(transform.position), 5);
        // CreateSquare(worldToGridPos(transform.position));
    }

    Vector2Int worldToGridPos(Vector3 worldPos)
    {
        Vector2Int pos = new();
        pos.x = Mathf.RoundToInt(worldPos.x / 20f);
        pos.y = Mathf.RoundToInt(worldPos.z / 20f);
        return pos;
    }

    void CreateRadius(Vector2Int pos, int sideLength)
    {
        for(int x = 0; x < sideLength; x++)
        {
            for(int y = 0; y < sideLength; y++)
            {
                CreateChunk(new Vector2Int(x, y) + pos);
            }
        }

        return;
    }

    void CreateSquare(Vector2Int pos)
    {
        CreateChunk(Vector2Int.zero + pos);
        CreateChunk(Vector2Int.up + pos);
        CreateChunk(Vector2Int.down + pos);
        CreateChunk(Vector2Int.right + pos);
        CreateChunk(Vector2Int.right + Vector2Int.up + pos);
        CreateChunk(Vector2Int.right + Vector2Int.down + pos);
        CreateChunk(Vector2Int.left + pos);
        CreateChunk(Vector2Int.left + Vector2Int.up + pos);
        CreateChunk(Vector2Int.left + Vector2Int.down + pos);
    }

    public void CreateSquare(Vector3 worldPos)
    {
        CreateSquare(worldToGridPos(worldPos));
    }
    GameObject CreateChunk(Vector2Int pos)
    {
        

        if(isChunkInstantied.ContainsKey(pos) )
        {
            // Debug.Log("already exxists:");
            
        }
        if(isChunkInstantied.ContainsKey(pos) && isChunkInstantied[pos])
        {
            // Debug.Log("already exxists:");
            // Already exists
            return null;
        }

        // Generate maps
        derivativeComputeController.SetPos(pos);
        derivativeComputeController.DoRender();

        // Spawn prefab at correct pos.
        float gridUnitLength = 20;
        GameObject chunk = Instantiate(chunkPrefab);
        chunk.transform.position = new Vector3(gridUnitLength * pos.x, 0f, gridUnitLength * pos.y);
        // Apply height map to prefab.
        chunk.GetComponent<MapsToMesh>().Generate();
        instantiedChunks[pos] = chunk;
        isChunkInstantied[pos] = true;

        return chunk;

    }

    public GameObject CreateChunk(Vector3 worldPos)
    {
        return CreateChunk(worldToGridPos(worldPos));

    }

    public GameObject getChunk(Vector3 worldPos)
    {
        Vector2Int gridPos = worldToGridPos(worldPos);
        return getChunk(gridPos);
    }

    public GameObject getChunk(Vector2Int gridPos)
    {
        GameObject chunk = instantiedChunks[gridPos];
        return chunk;
        
    }
}
