using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Rendering;

public class TerrainGenerator : MonoBehaviour
{
    //public variables
    public List<Vector3> newVertices = new List<Vector3>();
    public List<int> newTriangles = new List<int>();
    public List<Vector2> newUV = new List<Vector2>();
    public byte[,] blocks;

    public List<Vector3> colVertices = new List<Vector3>();
    public List<int> colTriangles = new List<int>();

    //private variables
    private Mesh mesh;
    private MeshCollider meshCollider;
    private float tUnitX = 1/24f;
    private float tUnitY = 1/34f;
    private Vector2 tGrass = new Vector2(3, 33);
    private Vector2 tStone = new Vector2(1, 33);
    private int squareCount;
    private int colCount;

    void ColliderTriangles()
    {
        colTriangles.Add(colCount*4);
        colTriangles.Add(colCount*4 + 1);
        colTriangles.Add(colCount*4 + 3);
        colTriangles.Add(colCount*4 + 1);
        colTriangles.Add(colCount*4 + 2);
        colTriangles.Add(colCount*4 + 3);
    }

    byte Block(int x, int y)
    {
        if(x == -1 || x == blocks.GetLength(0) || y == -1 || y == blocks.GetLength(1))
        {
            return (byte)0;
        }

        return blocks[x, y];
    }

    void GenerateCollider(int x, int y)
    {
        //top collider
        if(Block(x, y+1) == 0)
        {
            colVertices.Add(new Vector3(x, y, 1));
            colVertices.Add(new Vector3(x + 1, y, 1));
            colVertices.Add(new Vector3(x + 1, y, 0));
            colVertices.Add(new Vector3(x, y, 0));

            ColliderTriangles();
            colCount++;
        }

        //bottom collider
        if(Block(x, y-1) == 0)
        {
            colVertices.Add(new Vector3(x, y-1, 1));
            colVertices.Add(new Vector3(x+1, y-1, 1));
            colVertices.Add(new Vector3(x+1, y-1, 0));
            colVertices.Add(new Vector3(x, y-1, 0));

            ColliderTriangles();
            colCount++;
        }

        //left collider
        if(Block(x-1, y) == 0)
        {
            colVertices.Add(new Vector3(x, y, 0));
            colVertices.Add(new Vector3(x, y, 1));
            colVertices.Add(new Vector3(x, y-1, 1));
            colVertices.Add(new Vector3(x, y-1, 0));

            ColliderTriangles();
            colCount++;
        }

        //right collider
        if(Block(x+1, y) == 0)
        {
            colVertices.Add(new Vector3(x+1, y, 0));
            colVertices.Add(new Vector3(x+1, y, 1));
            colVertices.Add(new Vector3(x+1, y-1, 1));
            colVertices.Add(new Vector3(x+1, y-1, 0));

            ColliderTriangles();
            colCount++;
        }

    }

    void GenerateTerrain()
    {
        //this function creates block information
        //1 - grass , 2 - rock
        blocks = new byte[10,10];

        for (int px = 0; px < blocks.GetLength(0); px++)
        {
            for (int py = 0; py < blocks.GetLength(1); py++)
            {
                if(py < 5)
                {
                    blocks[px, py] = 2;
                }
                else if(py == 5)
                {
                    blocks[px, py] = 1;
                }
            }
        }
    }

    void GenerateSquare(int x, int y, Vector2 texture)
    {
        newVertices.Add(new Vector3(x, y, 0));
        newVertices.Add(new Vector3(x+1, y, 0));
        newVertices.Add(new Vector3(x+1, y-1, 0));
        newVertices.Add(new Vector3(x, y-1, 0));

        newTriangles.Add(squareCount*4);
        newTriangles.Add(squareCount*4 + 1);
        newTriangles.Add(squareCount*4 + 2);
        newTriangles.Add(squareCount*4);
        newTriangles.Add(squareCount*4 + 2);
        newTriangles.Add(squareCount*4 + 3);

        newUV.Add(new Vector2(tUnitX * texture.x, tUnitY * texture.y + tUnitY));
        newUV.Add(new Vector2(tUnitX * texture.x + tUnitX, tUnitY * texture.y + tUnitY));
        newUV.Add(new Vector2(tUnitX * texture.x + tUnitX, tUnitY * texture.y));
        newUV.Add(new Vector2(tUnitX * texture.x, tUnitY * texture.y));

        squareCount++;
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.uv = newUV.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();

        Mesh tmpMesh = new Mesh();
        tmpMesh.vertices = colVertices.ToArray();
        tmpMesh.triangles = colTriangles.ToArray();
        meshCollider.sharedMesh = tmpMesh;

        newVertices.Clear();
        newTriangles.Clear();
        newUV.Clear();
        squareCount = 0;

        colVertices.Clear();
        colTriangles.Clear();
        colCount = 0;
    }

    void BuildMesh()
    {
        for(int px = 0; px < blocks.GetLength(0); px++)
        {
            for(int py = 0; py < blocks.GetLength(1); py++)
            {
                if(blocks[px, py] != 0)
                {
                    GenerateCollider(px, py);

                    if(blocks[px, py] == 2)
                    {
                        GenerateSquare(px, py, tStone);
                    }
                    else if(blocks[px, py] == 1)
                    {
                        GenerateSquare(px, py, tGrass);
                    }
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();

        GenerateTerrain();
        BuildMesh();
        UpdateMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
