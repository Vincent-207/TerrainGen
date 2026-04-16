using UnityEngine;

public class RenderTextureToHeight : MonoBehaviour
{   
    [SerializeField]
    Texture2D renderTexture;
    Mesh mesh;
    void Start()
    {
        mesh = new();
        Mesh filterMesh = GetComponent<MeshFilter>().sharedMesh;
        mesh.vertices = filterMesh.vertices;
        mesh.triangles = filterMesh.triangles;
        mesh.uv = filterMesh.uv;
        mesh.normals = filterMesh.normals;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Apply()
    {
        Vector3[] vertices = mesh.vertices;
        int verticeSideLength = (int) Mathf.Sqrt(mesh.vertices.Length);
        ScalableBufferManager.ResizeBuffers(verticeSideLength, verticeSideLength);
        byte[] data = renderTexture.GetRawTextureData();
        for(int x = 0, i = 0; x < verticeSideLength; x++)
        {
            for(int z = 0; z < verticeSideLength; z++)
            {
                i++;
                float height = data[i];
                
            }
        }
    }
}
