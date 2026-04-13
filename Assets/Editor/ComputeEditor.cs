using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DerivativeComputeController))]
public class ComputeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DerivativeComputeController controller = (DerivativeComputeController)target;
        base.OnInspectorGUI();
        if(GUILayout.Button("Gen"))
        {
            controller.DoRender();
        }
    }
}
