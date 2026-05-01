using System;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class MapsToMesh : MonoBehaviour
{
    public RenderTexture heightMap, normalMap;
    public Texture2D heightTexture;
    public float amplitude;
    // public Mesh baseMesh;
    public Mesh sourceMesh;
    Mesh genMesh;
    public bool gen;

    public int highlightPos;
    public float radius;
    public Vector3 debugVec;
    void Update()
    {
        
        if(gen)
        {
            float start = Time.realtimeSinceStartup;
            Debug.Log("Start: " + start);
            Generate();
            gen = false;
            float end = Time.realtimeSinceStartup;
            Debug.Log("Elapsed: " + (end - start));
        }
    }

    public void Generate()
    {

        // sourceMesh.isReadable = true;
        float start = Time.realtimeSinceStartup;
        // Debug.Log("Start time: " + start);
        genMesh = new Mesh();
        Vector3[] vertices = genMesh.vertices = sourceMesh.vertices;
        // Vector3[] normals = new Vector3[vertices.Length];
        genMesh.triangles = sourceMesh.triangles;
        genMesh.uv = sourceMesh.uv;
        float copyTime = Time.realtimeSinceStartup - start;   
        start = Time.realtimeSinceStartup;     
        int sideLength = (int) Mathf.Sqrt(vertices.Length);
        
        Texture2D heightText = new(sideLength, sideLength, TextureFormat.RGBA32, false);
        RenderTexture.active = heightMap;
        heightText.ReadPixels(new Rect(0, 0, sideLength, sideLength), 0, 0);
        heightText.Apply();
        float heightTextTime = Time.realtimeSinceStartup - start;
        start = Time.realtimeSinceStartup;     

        // Setup normal texture so it c an be read by cpu
        Vector3[] normals = new Vector3[vertices.Length];
        // Debug.Log("Format: " + normalMap.format);
        Texture2D normalTexture = new Texture2D(normalMap.width, normalMap.height, TextureFormat.RGBA32, false);
        RenderTexture.active = normalMap;
        normalTexture.ReadPixels(new Rect(0, 0, normalMap.width, normalMap.height), 0, 0);
        normalTexture.Apply();
        float normalTextTime = Time.realtimeSinceStartup - start;
        start = Time.realtimeSinceStartup;     

        heightText.anisoLevel = 0;
        heightText.wrapMode = TextureWrapMode.Clamp;
        heightText.filterMode = FilterMode.Point;
        heightTexture = heightText;
        for(int i = 0; i < vertices.Length ; i++)
        {
            Vector2 uv = genMesh.uv[i];
            // Vector2Int texturePosInt = Vector2Int.FloorToInt(uv * sideLength);
           
            float height = heightText.GetPixelBilinear(uv.x, uv.y).r / amplitude; 
            vertices[i].z = height;
            // Debug.Log("Texturepos: " + (uv *  ((float) sideLength)) + " - "  + texturePosInt);


            // if((uv.x == 1f || uv.x == -1f) || (uv.y == 1f || uv.y == -1f))
            // {
            //     height = heightText.GetPixel(texturePosInt.x, texturePosInt.y).r / amplitude;
            // }
            // Debug.Log("height: " + height);
            // Color normal = normalTexture.GetPixelBilinear(uv.x, uv.y);
            // normals[i] = new Vector3(normal.b, -normal.r, normal.g);
            // Debug.Log("Normal: " + normals[i]);
        }

        float toArrTime = Time.realtimeSinceStartup - start;
        start = Time.realtimeSinceStartup;

        genMesh.vertices = vertices;
        genMesh.normals = normals;
        genMesh.RecalculateTangents();
        genMesh.RecalculateNormals();
        genMesh.RecalculateBounds();
        GetComponent<MeshFilter>().mesh = genMesh;
        GetComponent<MeshCollider>().sharedMesh =  genMesh;
        float toGenCopy = Time.realtimeSinceStartup - start;
        start = Time.realtimeSinceStartup;
       
       float end = Time.timeSinceLevelLoad;
    //    Debug.Log("Elapsed: " + (end - start));
    //    Debug.Log(Time.deltaTime);

    /* 
        Debug.Log("Copy time: " + copyTime);
        Debug.Log("Height text time: " + heightTextTime);
        Debug.Log("normal text time: " + normalTextTime);
        Debug.Log("to array time: " + toArrTime);
        Debug.Log("to gen copy: " + toGenCopy);
    */     
    }

    void OnDrawGizmos()
    {
       /*  Mesh currentMesh = GetComponent<MeshFilter>().sharedMesh;
        Gizmos.color = Color.red;

        Vector3 vertex = currentMesh.vertices[highlightPos];
        Vector2 uv = currentMesh.uv[highlightPos];
        Vector3 uv3 = new(uv.x, uv.y, 0);
        Debug.DrawRay(transform.position, vertex);
        Vector3 pos =  vertex;
        pos.x *= transform.localScale.x;
        pos.y *= transform.localScale.y;
        pos.z *= transform.localScale.z;
        pos = transform.rotation * pos;
        uv3.x *= transform.localScale.x;
        uv3.y *= transform.localScale.y; 
        uv3.z *= transform.localScale.z;
        uv3 = transform.rotation * uv3; 
        Vector3 normal = currentMesh.normals[highlightPos];
        normal = transform.rotation * normal;   
        Gizmos.DrawSphere(pos + transform.position, radius);
        Gizmos.color = Color.blue;
        // Gizmos.DrawSphere(uv3 + transform.position, radius);
        
        Gizmos.DrawSphere(pos + transform.position + normal, radius);
            debugVec = uv;
        Gizmos.color = Color.purple;
        Gizmos.DrawLine(pos + transform.position, pos + transform.position + normal); */
    }
}
