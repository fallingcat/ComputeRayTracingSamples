using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quad : MonoBehaviour
{
    public Material m_Material;
        
    public void Start()
    {
        MeshRenderer MeshRenderer = gameObject.AddComponent<MeshRenderer>();
        MeshRenderer.sharedMaterial = m_Material;

        MeshFilter MeshFilter = gameObject.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();

        Vector3[] Vertices = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(1.0f, 0, 0),
            new Vector3(0, 1.0f, 0),
            new Vector3(1.0f, 1.0f, 0)
        };
        mesh.vertices = Vertices;

        int[] Tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = Tris;

        Vector2[] UV = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = UV;

        MeshFilter.mesh = mesh;
    }
}
