using TreeEditor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PerlinNoise))]
public class PerlinNoiseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PerlinNoise perlinNoise = (PerlinNoise)target;
        base.OnInspectorGUI();

        if(GUILayout.Button("Apply noise"))
        {
            perlinNoise.ApplyNoise();
        }
    }
}
