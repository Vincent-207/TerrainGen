using System;
using UnityEditor;
using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEngine.ProBuilder;

public class SaveProMesh : MonoBehaviour
{
    [SerializeField] String path;
    ProBuilderMesh meshFilter;
    void Awake()
    {
        meshFilter = GetComponent<ProBuilderMesh>();
    }
    public void Save()
    {
        
       /*  meshFilter = GetComponent<ProBuilderMesh>();
        // Copy mesh
        Mesh meshToSave = new();
        meshToSave.SetVertices(meshFilter.Vertices);
        meshToSave.triangles = meshFilter.sharedMesh.triangles;
        meshToSave.SetUVs(1,meshFilter.sharedMesh.uv);
        meshToSave.SetNormals(meshFilter.sharedMesh.normals);
        AssetDatabase.CreateAsset(meshToSave, "Assets/" + path + ".asset");
        AssetDatabase.SaveAssets(); */

    }
}
