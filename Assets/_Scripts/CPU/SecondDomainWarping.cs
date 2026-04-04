using UnityEngine;

public class SecondDomainWarping : FirstDomainWarping
{
    public override void ApplyNoise()
    {
        for(int vert = 0; vert < mesh.vertices.Length; vert++)
        {
            Vector3 vertice = new(mesh.vertices[vert].x , 0,  mesh.vertices[vert].z);
            Vector2 pos = new(vertice.x, vertice.z);

            // float noise = GetNoiseAtPos(new(vertice.x + offset.x, vertice.z + offset.y));
            Vector2 q = new(GetNoiseAtPos(pos + new Vector2(0.0f, 0.0f)), GetNoiseAtPos(pos + new Vector2(5.2f, 1.3f)));
            Vector2 r = new(
                GetNoiseAtPos(pos + q*4f + new Vector2(1.7f, 9.2f)),
                GetNoiseAtPos(pos + q *4f + new Vector2(8.3f, 2.8f))
            );


            float output = GetNoiseAtPos(pos + 4f * r);

            
            // Debug.Log("NOISE: " + noise);
            vertice.y = output;
            mesh.vertices[vert] = vertice;
            verticies[vert] = vertice;
        }
    }
}
