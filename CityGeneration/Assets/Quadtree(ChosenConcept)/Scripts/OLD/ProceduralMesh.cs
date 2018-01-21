using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralMesh : MonoBehaviour
{
    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles;

    public float scale = 1.0f;

    private float adj_scale;


    public void Initialise(float _scale)
    {
        scale = _scale;

        mesh = GetComponent<MeshFilter>().mesh;
        adj_scale = scale * 0.5f;

        ConstructMesh();
        UpdateMesh();


    }


    void ConstructMesh()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();

        for (int i = 0; i < 6; i++)
        {
            ConstructFace(i);
        }
    }


    void ConstructFace(int _dir)
    {
        vertices.AddRange(CubeMeshData.FaceVertices(_dir, adj_scale));

        int v_count = vertices.Count;

        triangles.Add(v_count - 4);
        triangles.Add(v_count - 4 + 1);
        triangles.Add(v_count - 4 + 2);
        triangles.Add(v_count - 4);
        triangles.Add(v_count - 4 + 2);
        triangles.Add(v_count - 4 + 3);
    }


    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}
