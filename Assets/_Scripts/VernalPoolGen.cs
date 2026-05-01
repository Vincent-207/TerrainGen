using Unity.Collections;
using UnityEngine;

public class VernalPoolGen : MonoBehaviour
{
    public bool regen;
    public float amplitude, frequency;
    public AnimationCurve heightCurve;
    public float heightThreshold;
    public int seed;
    Mesh mesh;
    void Start()
    {
        mesh = new Mesh();

        Mesh baseMesh = GetComponent<MeshFilter>().sharedMesh;
        mesh.vertices = baseMesh.vertices;
        mesh.uv = baseMesh.uv;
        mesh.triangles = baseMesh.triangles;
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    void Update()
    {
        if(regen)
        {
            Apply();
            // CreateColors();
            regen = false;
        }
    }

    public void Apply()
    {
        Vector3[] verticies = mesh.vertices;
        // Run in 2d. 
        int sideLength = (int) Mathf.Sqrt(verticies.Length);
        for(int x = 0, vertIndex = 0; x < sideLength; x++)
        {
            for(int z = 0; z < sideLength; z++)
            {
                float height = GetHeight(x + seed, z + seed);
                verticies[vertIndex].y = height;
                // verticies[vertIndex].z = 0;
                // verticies[vertIndex] = new Vector3(x, vertIndex, z);
                vertIndex++;

            }
        }

        mesh.vertices = verticies;
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
    }

    float GetHeight(int x, int z)
    {
        float height = amplitude * Mathf.PerlinNoise(x / frequency, z /frequency);
        float proportion = height / amplitude;
        float output = heightCurve.Evaluate(proportion) * amplitude;
        return output;
    }

    void CreateColors()
    {
        Color[] colors = mesh.colors;
        Debug.Log("Colors length: " + colors.Length);
        // Color[] colors = new Color[mesh.vertices.Length];

        for(int i = 0; i < mesh.vertices.Length; i++)
        {
            if(mesh.vertices[i].y <= heightThreshold * amplitude)
            {
                colors[i] = Color.blue;
                Debug.Log("blue!");
            }
            else colors[i] = Color.white;
        }
        mesh.colors = colors;
    }
    
}
