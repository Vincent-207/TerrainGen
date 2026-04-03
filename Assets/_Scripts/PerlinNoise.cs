using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class PerlinNoise : MonoBehaviour
{
    public float maxHeight;
    public void ApplyNoise()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        int vertCount = mesh.vertexCount;
        for(int vertIndex = 0, x =0; x < vertCount/2; x++)
        {
            for(int z = 0; z < vertCount/2; z++)
            {
                Vector3 vertex = mesh.vertices[vertIndex];
                vertex.y = (x + z)/vertCount;
                mesh.vertices[vertIndex] = vertex;
                
                
                vertIndex++;
                if(vertIndex >= vertCount)
                {
                    vertIndex = 0;
                }
                
            }
        } 
    }
}
