using System;
using UnityEditor.Callbacks;
using UnityEngine;
[ExecuteInEditMode]
public class DerivativeComputeController : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture renderTexture, normalMap;
    [SerializeField] [Range(0.0f, 64.0f)] float baseFrequency = 4f;
    [SerializeField] [Range(0.0f, 16.0f)] float baseAmplitude = 1f;
    [SerializeField] [Range(0.0f, 2.0f)] float scale = 0.5f;
    [SerializeField] [Range(0,16)] int octaves = 8;
    [SerializeField] [Range(0.0f, 4.0f)] float lacunarity = 2f;
    [SerializeField] [Range(0.0f, 4.0f)] float gain = 0.5f;
    [SerializeField] Vector2 offset;
    public int vertSideLength = 256;
    public Vector2 seed;

    public bool reRender;

    void Start()
    {
        DoRender();
    }

    void Update()
    {
        // if(reRender)
        // {
        //     DoRender();
        //     reRender = false;
        // 
        DoRender();
    }


    public void DoRender()
    {
        int kernal = computeShader.FindKernel("CSMain");
        computeShader.SetTexture(kernal, "Result", renderTexture);
        computeShader.SetTexture(kernal, "NormalMap", normalMap);
        computeShader.SetFloat("_BaseFrequency", baseFrequency);
        computeShader.SetFloat("_BaseAmplitude", baseAmplitude);
        computeShader.SetFloat("_Scale", scale);
        computeShader.SetFloat("_Gain", gain);
        computeShader.SetFloat("_Lacunarity", lacunarity);
        computeShader.SetInt("_Octaves", octaves);
        computeShader.SetVector("_Offset", offset + seed);
        computeShader.Dispatch(kernal, 64, 64, 1);
    }

    public void SetPos(Vector2 flatPos)
    {
        flatPos *= -1;
        offset = (flatPos * 32);
    }
}
