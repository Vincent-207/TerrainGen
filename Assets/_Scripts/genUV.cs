using UnityEditor;
using UnityEngine;
[ExecuteInEditMode]
public class genUV : MonoBehaviour
{
    public bool save;
    public Mesh source;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(save)
        {
            gen();
            save = false;
        }
    }

    void gen()
    {
        Mesh mesh = new();
        mesh.vertices = source.vertices;
        mesh.uv = source.uv;
        mesh.normals = source.normals;
        mesh.triangles = source.triangles;
        mesh.RecalculateUVDistributionMetric(0);
        mesh.bounds = source.bounds;

        AssetDatabase.CreateAsset(mesh, "Assets/"  + "BaseFixedUv" + ".asset");
        AssetDatabase.SaveAssets();
    }
}
