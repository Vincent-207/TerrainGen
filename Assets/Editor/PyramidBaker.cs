using UnityEngine;

public static class PyramidBaker
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct SourceVertex
    {
        public Vector3 position;
        public Vector2 uv;
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct GeneratedVertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 uv;
    }

    private const int SOURCE_VERT_STRIDE = sizeof(float) * (3 + 2);
    private const int SOURCE_INDEX_STRIDE = sizeof(int);
    private const int GENERATED_VERT_STRIDE = sizeof(float) * (3 + 3 + 2);
    private const int GENERATED_INDEX_STRIDE = sizeof(int);

    // takes in a mesh and decomposes it into vertex and index arrays
    private static void DecomposeMesh(Mesh mesh, int subMeshIndex, out SourceVertex[] verts, out int[] indices)
    {
        var subMesh = mesh.GetSubMesh(subMeshIndex);

        Vector3[] allVertices = mesh.vertices;
        Vector2[] allUVs = mesh.uv;
        int[] allIndices = mesh.triangles;

        verts = new SourceVertex[subMesh.vertexCount];
        indices = new int[subMesh.indexCount];
        for(int i = 0; i < subMesh.vertexCount; i++)
        {
            // find the index in the entire buffer
            int wholeMeshIndex = i + subMesh.firstVertex;
            verts[i] = new SourceVertex()
            {
                position = allVertices[wholeMeshIndex],
                uv = allUVs[wholeMeshIndex],
            };
        }

        for(int i = 0; i < subMesh.indexCount; i++)
        {
            // we need to offset the indices in the mesh index buffer to match
            // the indices in our new vertex buffer, subtract by submesh.firstvertex
            // basevertex is an offset unity may define which is a global
            // offset for all indices in this submesh
            indices[i] = allIndices[i + subMesh.indexStart] + subMesh.baseVertex - subMesh.firstVertex;

        }
    }

    // takes in a vertex and index list and converts it into a Mesh object

    private static Mesh ComposeMesh(GeneratedVertex[] verts, int[] indices)
    {
        Mesh mesh = new();
        Vector3[] verticies = new Vector3[verts.Length];
        Vector3[] normals = new Vector3[verts.Length];
        Vector2[] uvs = new Vector2[verts.Length];
        for(int i = 0; i < verts.Length; i++)
        {
            var v = verts[i];
            verticies[i] = v.position;
            normals[i] = v.normal;
            uvs[i] = v.uv;
        }

        mesh.SetVertices(verticies);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0, true);
        mesh.Optimize();
        return mesh;
    }

    public static bool Run(ComputeShader shader, PyramidBakeSettings settings, out Mesh generatedMesh)
    {
        //  Decompose mesh into buffers
        DecomposeMesh(settings.sourceMesh, settings.sourceSubMeshIndex, out var sourceVertices, out var sourceIndices);

        // the mesh toplogy is triangles, so there are threee indices per triangle;
        int numSourceTriangles = sourceIndices.Length/3;

        // we generate 3 triangles per source triangle, and there are three verticies per trianglew
        GeneratedVertex[] generatedVertices = new GeneratedVertex[numSourceTriangles * 3 * 3];
        int[] generatedIndices = new int[generatedVertices.Length];
        
        // A graphics buffer is a better version of a compute buffer
        GraphicsBuffer sourceVertBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, sourceVertices.Length, SOURCE_VERT_STRIDE);
        GraphicsBuffer sourceIndexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, sourceIndices.Length, SOURCE_INDEX_STRIDE);
        GraphicsBuffer genVertBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, generatedVertices.Length, GENERATED_VERT_STRIDE);
        GraphicsBuffer genIndexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, generatedIndices.Length, GENERATED_INDEX_STRIDE);

        // cache the kernel id
        int idGrassKernel = shader.FindKernel("CSMain");

        // Set buffers and variables
        shader.SetBuffer(idGrassKernel, "_SourceVertices", sourceVertBuffer);
        shader.SetBuffer(idGrassKernel, "_SourceIndicies", sourceIndexBuffer);
        shader.SetBuffer(idGrassKernel, "_GeneratedIndices", genIndexBuffer);
        // Convert the scale and rotation settings into transformation matrix
        shader.SetMatrix("_Transform", Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(settings.rotaion), settings.scale));
        shader.SetFloat("_PyramidHeight", settings.pyramidHeight);
        shader.SetInt("_NumSourceTriangles", numSourceTriangles);

        // set data in the buffers
        sourceVertBuffer.SetData(sourceVertices);
        sourceIndexBuffer.SetData(sourceIndices);

        // Find the needed dispatch size, so that each triangle will be run over
        shader.GetKernelThreadGroupSizes(idGrassKernel, out uint threadGroupSize, out _, out _);
        int dispatchSize = Mathf.CeilToInt((float) numSourceTriangles / threadGroupSize);
        // Dispatch the shader
        shader.Dispatch(idGrassKernel, dispatchSize, 1, 1);

        // Get the data from the compute shader
        // Unity will wait here until the compute shdaer is completed
        // Don't do this as runtime, Look into AsyncGPUReadback
        genVertBuffer.GetData(generatedVertices);
        genIndexBuffer.GetData(generatedIndices);

        // Compose the vertex/index buffers into a mesh
        generatedMesh = ComposeMesh(generatedVertices, generatedIndices);
        
        sourceVertBuffer.Release();
        sourceIndexBuffer.Release();
        genVertBuffer.Release();
        genIndexBuffer.Release();
        
        // no errors encountered.
        return true; 

    }
}
