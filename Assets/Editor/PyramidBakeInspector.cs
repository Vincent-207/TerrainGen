using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mono.Cecil;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PyramidBakeSettings))]
public class PyramidBakeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Create"))
        {
            var shaderGUID = AssetDatabase.FindAssets("PyramidBuilder").FirstOrDefault();

            if(string.IsNullOrEmpty(shaderGUID)) Debug.LogError("Couldn't find compute shader: PyramidBuilder.compute");
            else 
            {
                var shader = AssetDatabase.LoadAssetAtPath<ComputeShader>(AssetDatabase.GUIDToAssetPath(shaderGUID));

                // Opens a progress bar window
                EditorUtility.DisplayProgressBar("Building mesh", "", 0);
                // Run the baker
                var settings = serializedObject.targetObject as PyramidBakeSettings;
                bool success = PyramidBaker.Run(shader, settings, out var generatedMesh);

                EditorUtility.ClearProgressBar();
                if(success)
                {
                    SaveMesh(generatedMesh);
                    Debug.Log("Mesh saved successfully!");

                }
                else
                {
                    Debug.LogError("Failed to create mesh");
                }
            }
        }

    }

    public void SaveMesh(Mesh mesh)
    {
        // opens a file save dialogue window
        string path = EditorUtility.SaveFilePanel("Save Mesh Asset", "Assets/", name, "asset");
        // path is empty if the user exits out of the windows
        if(string.IsNullOrEmpty(path))
        {
            return;
        }

        // Transforms the path to a full system path, to help minimize bugs
        path = FileUtil.GetProjectRelativePath(path);

        var oldMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        if(oldMesh != null)
        {
            oldMesh.Clear();
            EditorUtility.CopySerialized(mesh, oldMesh);

        }
        else
        {
            AssetDatabase.CreateAsset(mesh, path);
        }

        AssetDatabase.SaveAssets();
    }
}
