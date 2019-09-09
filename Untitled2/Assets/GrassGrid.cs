using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GrassGrid : MonoBehaviour
{
    public int numCellsX = 10, numCellsY = 10;
    private Vector3[] vertices;
    private Mesh mesh;

    /*
    void OnDrawGizmos()
    {
        if (vertices == null) return;
        for (int row = 0; row < numCellsY + 1; row++)
        {
            for (int col = 0; col < numCellsX + 1; col++)
            {
                Gizmos.DrawSphere(vertices[row * (numCellsX + 1) + col], 0.1f);
            }
        }
    }
    */

    IEnumerator Start()
    {
        vertices = new Vector3[(numCellsX + 1) * (numCellsY + 1)];
        for (int row = 0; row < numCellsY + 1; row++)
        {
            for (int col = 0; col < numCellsX + 1; col++)
            {
                vertices[row * (numCellsX + 1) + col] = new Vector3(row, 0, col);
            }
        }
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.name = "Grass Grid";
        mesh.vertices = vertices;

        int[] triangle = new int[3 * 2 * numCellsX * numCellsY];
        int i = 0;

        for (int row = 0; row < numCellsY; row++)
        {
            for (int col = 0; col < numCellsX; col++)
            {
                triangle[i] = row * (numCellsX + 1) + col;
                triangle[i + 1] = row * (numCellsX + 1) + col + 1;
                triangle[i + 2] = (row + 1) * (numCellsX + 1) + col;

                triangle[i + 3] = (row + 1) * (numCellsX + 1) + col;
                triangle[i + 4] = row * (numCellsX + 1) + col + 1;
                triangle[i + 5] = (row + 1) * (numCellsX + 1) + col + 1;

                i += 6;
                mesh.triangles = triangle;
                mesh.RecalculateNormals();
                mesh.RecalculateTangents();
                yield return new WaitForSeconds(0.03f);
            }
        }
        mesh.triangles = triangle;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
