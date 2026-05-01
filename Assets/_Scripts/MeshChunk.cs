using System.Runtime.CompilerServices;
using System.Security;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class MeshChunk : MonoBehaviour
{
    public RenderTexture heightMap, normalMap;
    Texture2D heightText;
    public Mesh sourceMesh;

    public int size;
    public Vector2Int offset;
    public bool doGen;
    public GameObject parentMesh;
    public float amplitude = 85f;
    public Vector3 dbug;
    public int vert;
    public float rad;
    void Update()
    {
        if(doGen)
        {
            GenerateMesh();
            doGen = false;
        }
    }
    void RenderToTexture2D()
    {
        int sideLength = (int) Mathf.Sqrt(sourceMesh.vertices.Length);

        heightText = new(sideLength, sideLength, TextureFormat.RGBA32, false);
        RenderTexture.active = heightMap;
        heightText.ReadPixels(new Rect(0, 0, sideLength, sideLength), 0, 0);
        heightText.Apply();
    }
    void GenerateMesh()
    {
        
        RenderToTexture2D();
        // if(heightText == null) RenderToTexture2D();
        Mesh genMesh = new();
        genMesh.vertices = sourceMesh.vertices;
        genMesh.uv = sourceMesh.uv;
        genMesh.normals = sourceMesh.normals;
        genMesh.tangents= sourceMesh.tangents;
        genMesh.triangles = sourceMesh.triangles;
        genMesh.RecalculateBounds();
        genMesh.RecalculateUVDistributionMetric(0);
        int sideLength = (int) Mathf.Sqrt(genMesh.vertices.Length);
        Vector3[] vertices = genMesh.vertices;
        for(int vertIndex = 0, x = 0; x < sideLength; x++)
        {
            for(int y = 0; y < sideLength; y++)
            {
                // Vector2 pos = new(offset.x + x, offset.y + y);
                // pos/= sideLength;
                // float value = heightText.GetPixelBilinear(pos.x, pos.y).r;
                // Debug.Log("Value: " + value );
                // Debug.Log("{" + pos + "}");
                // vertices[vertIndex].y = value / amplitude;
                // vertIndex++;
                Vector2 pos = genMesh.uv[vertIndex];
                float value = heightText.GetPixelBilinear(pos.x, pos.y).g;
                vertices[vert].y = value / amplitude;
                vertIndex++;

            }
        }
        genMesh.vertices = vertices;
        genMesh.RecalculateBounds();
        genMesh.RecalculateNormals();
        genMesh.RecalculateTangents();

        GetComponent<MeshFilter>().mesh = genMesh;
        GetComponent<MeshCollider>().sharedMesh = genMesh;
        Vector3 basePos = new(-10f + 0.5f + (offset.x * 0.1f), 0, -10f + 0.5f + (offset.y * 0.1f));
        transform.position = basePos + parentMesh.transform.position;

    }

    void OnDrawGizmos()
    {
        Mesh currentMesh = GetComponent<MeshFilter>().sharedMesh;
        Gizmos.color = Color.red;
        
    }
}
