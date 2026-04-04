using UnityEngine;

public class Billow : PerlinGen
{
    
    internal override void ApplyNoise()
    {
        Debug.Log("Applying!");
        for(int vert = 0; vert < mesh.vertices.Length; vert++)
        {
            Vector3 vertice = verticies[vert];
            offset = Vector2.down;
            float noise =  Mathf.PerlinNoise((vertice.x / frequency), vertice.z /frequency);
            // float offset = -0.5f;
            float elevation = Mathf.Abs(noise - 0.5f) * amplitude;
            vertice.y = elevation;
            Debug.Log("Eleveation: " + elevation);
            verticies[vert] = vertice;
        }
    }
}
