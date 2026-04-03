using System;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class SaveMesh : MonoBehaviour
{
    [SerializeField] String path;
    MeshFilter meshFilter;
    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }
    public void Save()
    {
        // Copy mesh
        Mesh meshToSave = new();
        meshToSave.SetVertices(meshFilter.sharedMesh.vertices);
        meshToSave.triangles = meshFilter.sharedMesh.triangles;
        meshToSave.SetUVs(1,meshFilter.sharedMesh.uv);
        meshToSave.SetNormals(meshFilter.sharedMesh.normals);
        AssetDatabase.CreateAsset(meshToSave, "Assets/" + path + ".asset");
        AssetDatabase.SaveAssets();

    }
}
