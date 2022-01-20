using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpriteToMesh : MonoBehaviour
{
    public Sprite sprite;
    public int depth;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshFilter>().mesh = spriteToMesh(sprite);
    }

    private Mesh spriteToMesh(Sprite sprite)
    {
        Mesh mesh = new Mesh();
        List<Vector3> inVertices = Array.ConvertAll(sprite.vertices, i => (Vector3)i).ToList();
        List<Vector2> uvs = sprite.uv.ToList();
        List<int> triangles = Array.ConvertAll(sprite.triangles, i => (int)i).ToList();
        if (depth <= 0)
        {
            mesh.SetVertices(inVertices);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(triangles, 0);
        }
        else
        {
            List<Vector3> depthVerticles = new List<Vector3>(inVertices);
            List<Vector2> depthUvs = new List<Vector2>(uvs);
            List<int> depthTriangles = new List<int>(triangles);
            foreach (Vector3 vector in inVertices)
            {
                depthVerticles.Add(new Vector3(vector.x, vector.y, depth));
            }
            foreach (Vector2 vector in uvs)
            {
                depthUvs.Add(new Vector2(vector.x, vector.y));
            }
            //depthTriangles.Add(triangles[0] + inVertices.Count);
            for (int i = triangles.Count - 1; i >= 0; i--)
            {
                depthTriangles.Add((triangles[i] + inVertices.Count));
            }
            List<int> depthTrianglesWithJoinFaces = new List<int>(depthTriangles);
            for (int i = 0; i + 1 < triangles.Count; i++)
            {
                depthTrianglesWithJoinFaces.Add(triangles[i+1]);
                depthTrianglesWithJoinFaces.Add(triangles[i]);
                depthTrianglesWithJoinFaces.Add(triangles[i+1] + inVertices.Count);
                depthTrianglesWithJoinFaces.Add(triangles[i+1] + inVertices.Count);
                depthTrianglesWithJoinFaces.Add(triangles[i]);
                depthTrianglesWithJoinFaces.Add(triangles[i] + inVertices.Count);
            }
            for (int i = 0; i + 2 + inVertices.Count < depthVerticles.Count; i ++)
            {
                depthTrianglesWithJoinFaces.Add(i);
                depthTrianglesWithJoinFaces.Add(i + 2);
                depthTrianglesWithJoinFaces.Add(i + 2 + inVertices.Count);
                depthTrianglesWithJoinFaces.Add(i);
                depthTrianglesWithJoinFaces.Add(i + 2 + inVertices.Count);
                depthTrianglesWithJoinFaces.Add(i + inVertices.Count);
            }
            for (int i = 0; i + 1 + inVertices.Count < depthVerticles.Count; i++)
            {
                depthTrianglesWithJoinFaces.Add(i);
                depthTrianglesWithJoinFaces.Add(i + 1);
                depthTrianglesWithJoinFaces.Add(i + 1 + inVertices.Count);
                depthTrianglesWithJoinFaces.Add(i);
                depthTrianglesWithJoinFaces.Add(i + 1 + inVertices.Count);
                depthTrianglesWithJoinFaces.Add(i + inVertices.Count);
            }
            mesh.SetVertices(depthVerticles.ToList());
            mesh.SetUVs(0, depthUvs);
            mesh.SetTriangles(depthTrianglesWithJoinFaces, 0);
            string values = "";
            int j = 0;
            foreach(int i in depthTrianglesWithJoinFaces)
            {
                if(j > 3)
                {
                    j = 0;
                    values += "\n";
                }
                values += ";" + i;
            }
        }
        return mesh;
    }
}
