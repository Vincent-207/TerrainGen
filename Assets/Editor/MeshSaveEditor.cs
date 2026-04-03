using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SaveMesh))]
public class MeshSaveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SaveMesh saveMesh = (SaveMesh)target;
        base.OnInspectorGUI();
        if(GUILayout.Button("Save"))
        {
            saveMesh.Save();
        }
    }
}
