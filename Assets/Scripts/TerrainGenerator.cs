using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    //public variables
    public List<Vector3> newVertices = new List<Vector3>();
    public List<int> newTriangles = new List<int>();
    public List<Vector2> newUV = new List<Vector2>();

    //private variables
    private Mesh mesh;
    private float tUnitX = 1/24f;
    private float tUnitY = 1/34f;
    private Vector2 tGrass = new Vector2(3, 33);
    private int squareCount;

    void GenerateSquare(int x, int y, Vector2 texture)
    {
        newVertices.Add(new Vector3(x, y, 0));
        newVertices.Add(new Vector3(x+1, y, 0));
        newVertices.Add(new Vector3(x+1, y-1, 0));
        newVertices.Add(new Vector3(x, y-1, 0));

        newTriangles.Add(0);
        newTriangles.Add(1);
        newTriangles.Add(2);
        newTriangles.Add(0);
        newTriangles.Add(2);
        newTriangles.Add(3);

        newUV.Add(new Vector2(tUnitX * texture.x, tUnitY * texture.y + tUnitY));
        newUV.Add(new Vector2(tUnitX * texture.x + tUnitX, tUnitY * texture.y + tUnitY));
        newUV.Add(new Vector2(tUnitX * texture.x + tUnitX, tUnitY * texture.y));
        newUV.Add(new Vector2(tUnitX * texture.x, tUnitY * texture.y));
    }

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        mesh.Clear();
        GenerateSquare(0, 0, tGrass);
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.uv = newUV.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
