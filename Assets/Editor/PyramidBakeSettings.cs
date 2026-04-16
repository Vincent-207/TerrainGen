using UnityEditor.EditorTools;
using UnityEngine;

[CreateAssetMenu(fileName = "PyramidBakeSettings", menuName = "mesh gen/PyramidBakeSettings")]
public class PyramidBakeSettings : ScriptableObject
{
    [Tooltip("The sourcce mesh to build off of")]
    public Mesh sourceMesh;
    [Tooltip("The submesh index of the source mesh to use")]
    public int sourceSubMeshIndex;
    [Tooltip("A scale to apply to the source mesh before generating the pyramids.")]
    public Vector3 scale;
    [Tooltip("A rotaion to apply to the source mesh before generating the pyramids")]
    public Vector3 rotaion;
    [Tooltip("The hieght of each extruded pyramid")]
    public float pyramidHeight;

}
