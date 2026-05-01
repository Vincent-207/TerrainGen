using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class GenerateBoundsMesh : MonoBehaviour
{
    public Mesh sourceMesh;
    Mesh genMesh;
    public float rad;
    public bool doGen;
    void Update()
    {
        if(doGen)
        {
            Generate();
            doGen = false;
        }
    }
    public RenderTexture heightMap;
    Texture2D heightText;
    void Start()
    {
        // Generate();
    }
    void Generate()
    {
        genMesh = new();
        genMesh.vertices = sourceMesh.vertices;
        genMesh.triangles = sourceMesh.triangles;
        genMesh.uv = sourceMesh.uv;
        genMesh.normals = sourceMesh.normals;
        genMesh.tangents = sourceMesh.tangents;
        Bounds a = sourceMesh.bounds;
        Debug.Log("Center: " + a.center);
        Debug.Log("Size: " + a.size);
        
        setupTexture();

        Bounds bounds = new (sourceMesh.bounds.center, sourceMesh.bounds.size);
        Vector3 size = bounds.size;
        size.z = getMax();
        bounds.size = size;
        genMesh.bounds = bounds;

        GetComponent<MeshFilter>().sharedMesh = genMesh;

    }

    void OnDrawGizmos()
    {
        Bounds sourceBounds = sourceMesh.bounds;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + sourceBounds.center, sourceBounds.size);
        Gizmos.DrawSphere(transform.position + sourceBounds.center, rad);

        Bounds genbound = new(Vector3.zero, Vector3.zero);
        Vector3 size = sourceBounds.size;
        float max = getMax();
        size.z = max;
        genbound.size = size;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + genbound.center, genbound.size);
    }

    void setupTexture()
    {
        int sideLength = (int) Mathf.Sqrt(sourceMesh.vertices.Length);

        heightText = new(sideLength, sideLength, TextureFormat.RGBA32, false);
        RenderTexture.active = heightMap;
        heightText.ReadPixels(new Rect(0, 0, sideLength, sideLength), 0, 0);
        heightText.Apply();
    }

    float getMax()
    {
       if(heightText == null) setupTexture();
        float max = heightText.GetPixel(0,0).r;
        for(int x = 0; x < heightMap.width; x++)
        {
            for(int y = 0; y < heightMap.width; y++)
            {
                float current = heightText.GetPixel(x, y).r;
                if(current > max)
                {
                    current = max;
                }
            }
        }
        return max;
    }
}
