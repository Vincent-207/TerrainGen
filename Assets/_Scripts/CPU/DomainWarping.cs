using UnityEngine;

public class FirstDomainWarping : MeshGen
{
    [SerializeField]
    public int octaves;
    public float baseFrequency;
    public float baseAmplitude;
    public bool regen;
    public int seed;
    public float scale;
    public float lacunarity = 2f, gain = 0.5f; 
    public Vector2 offset;
    public void Init(Vector2 gridPos, int seed)
    {
        offset = gridPos;
        this.seed = seed;
    }
    public void Generate()
    {
        
        CreateMesh(); 
        meshFilter = GetComponent<MeshFilter>();
        
        verticies = mesh.vertices;
        ApplyNoise();
        mesh.vertices = verticies;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        meshFilter.sharedMesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
    void OnDestroy()
    {
        Destroy(mesh);
    }
    public virtual void ApplyNoise()
    {
        for(int vert = 0; vert < mesh.vertices.Length; vert++)
        {
            Vector3 vertice = new(mesh.vertices[vert].x , 0,  mesh.vertices[vert].z);
            Vector2 pos = new(vertice.x, vertice.z);

            // float noise = GetNoiseAtPos(new(vertice.x + offset.x, vertice.z + offset.y));
            Vector2 q = new(GetNoiseAtPos(pos + new Vector2(0.0f, 0.0f)), GetNoiseAtPos(pos + new Vector2(5.2f, 1.3f)));
            float output = GetNoiseAtPos(pos + 4f * q);

            
            // Debug.Log("NOISE: " + noise);
            vertice.y = output;
            mesh.vertices[vert] = vertice;
            verticies[vert] = vertice;
        }
    }
    public float GetNoiseAtPos(Vector2 pos)
    {
        return GetNoiseAtPos(pos.x, pos.y);
    }
    float GetNoiseAtPos(float x, float z)
    {
        float amplitude = baseAmplitude;
        float frequency = baseFrequency;

        float sum  = 0;
        Vector2 pos = new(x, z);
        for(int i = 0; i < octaves; i++)
        {
            Vector2 scaledPos = (pos / frequency) * scale;
            sum += Mathf.PerlinNoise(scaledPos.x, scaledPos.y) * amplitude;
            // Debug.Log("Elevation: " + elevation);
            frequency /= lacunarity;
            amplitude *= gain;
        }
        // elevation = Mathf.Clamp(elevation, minHeight, maxHeight);
        return sum;
    }

    

    void Update()
    {
        if(regen)
        {
            // CreateMesh();
            // ApplyNoise();
            // UpdateMesh();
            Generate();
            regen = false;
        }
    }
}
