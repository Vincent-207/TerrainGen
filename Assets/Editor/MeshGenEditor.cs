using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(MeshGen))]
public class MeshGenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MeshGen meshGen = (MeshGen)target;
        base.OnInspectorGUI();

        if(GUILayout.Button("Generate"))
        {
            meshGen.CreateMesh();
        }
    }
}
