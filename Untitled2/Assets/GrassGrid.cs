using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GrassGrid : MonoBehaviour
{
    private int numCellsX = 10, numCellsY = 10;
    private float spawnTime = 0.05f;
    float surfaceLengthX = 10, surfaceLengthY = 10;
    int startCellX = 0, startCellY = 0;
    Vector3 collisionPoint; // local

    int[] triangles;
    Vector3[] vertices;
    Mesh mesh;

    public void Init(float surfaceLengthX, float surfaceLengthY, Vector3 collisionPoint)
    {
        this.surfaceLengthX = surfaceLengthX;
        this.surfaceLengthY = surfaceLengthY;
        this.collisionPoint = collisionPoint;

        numCellsX = (int)Mathf.Ceil(surfaceLengthX);
        numCellsY = (int)Mathf.Ceil(surfaceLengthY);

        startCellX = Mathf.FloorToInt((collisionPoint.z + 0.5f) * (float)numCellsY);
        startCellY = Mathf.FloorToInt((collisionPoint.x + 0.5f) * (float)numCellsX);
    }

    /*
    void OnDrawGizmos()
    {
        if (vertices == null) return;
        for (int row = 0; row < numCellsY + 1; row++)
        {
            for (int col = 0; col < numCellsX + 1; col++)
            {
                Gizmos.DrawSphere(transform.position + vertices[row * (numCellsX + 1) + col], 0.1f);
            }
        }
    }
    */
    
    void SetTriangles()
    {
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }

    void DrawCell(ref int i, int row, int col)
    {
        triangles[i] = row * (numCellsX + 1) + col;
        triangles[i + 1] = (row + 1) * (numCellsX + 1) + col;
        triangles[i + 2] = row * (numCellsX + 1) + col + 1;

        triangles[i + 3] = (row + 1) * (numCellsX + 1) + col;
        triangles[i + 4] = (row + 1) * (numCellsX + 1) + col + 1;
        triangles[i + 5] = row * (numCellsX + 1) + col + 1;

        i += 6;
    }

    void DrawRow(ref int i, Vector2Int startCell, Vector2Int endCell)
    {
        if(endCell.y < startCell.y) (startCell, endCell) = (endCell, startCell);

        int row = startCell.x;
        for(int col = startCell.y; col <= endCell.y; col++)
        {
            DrawCell(ref i, row, col);
        }
    }

    void DrawCol(ref int i, Vector2Int startCell, Vector2Int endCell)
    {
        if (endCell.x < startCell.x) (startCell, endCell) = (endCell, startCell);

        int col = startCell.y;
        for(int row = startCell.x; row <= endCell.x; row++)
        {
            DrawCell(ref i, row, col);
        }
    }

    IEnumerator DrawGrid(int startRow, int startCol)
    {
        int i = 0;
        DrawCell(ref i, startRow, startCol);
        SetTriangles();
        yield return new WaitForSeconds(spawnTime);

        Vector2Int topLeft = new Vector2Int(startRow + 1, startCol - 1);
        Vector2Int botLeft = new Vector2Int(startRow - 1, startCol - 1);
        Vector2Int topRight = new Vector2Int(startRow + 1, startCol + 1);
        Vector2Int botRight = new Vector2Int(startRow - 1, startCol + 1);
        Vector2Int oneCol = new Vector2Int(0, 1);
        Vector2Int oneRow = new Vector2Int(1, 0);

        bool topHit = false, botHit = false, leftHit = false, rightHit = false;

        // .x = row, .y = col, numCellsY = numRows, numCellsX = numCols
        for(;;)
        {
            if (topLeft.x > numCellsY - 1)
            {
                topLeft.x = numCellsY - 1;
                topRight.x = numCellsY - 1;
                topHit = true;
            }
            if(topLeft.y < 0)
            {
                topLeft.y = 0;
                botLeft.y = 0;
                leftHit = true;
            }
            if(botLeft.x < 0)
            {
                botLeft.x = 0;
                botRight.x = 0;
                botHit = true;
            }
            if(botRight.y > numCellsX - 1)
            {
                botRight.y = numCellsX - 1;
                topRight.y = numCellsX - 1;
                rightHit = true;
            }

            if (!leftHit) DrawCol(ref i, botLeft, topLeft);
            if (!rightHit) DrawCol(ref i, botRight, topRight);
            if (!topHit)
            {
                Vector2Int t_topLeft = topLeft;
                Vector2Int t_topRight = topRight;
                if (!leftHit) t_topLeft += oneCol;
                if (!rightHit) t_topRight -= oneCol;
                DrawRow(ref i, t_topLeft, t_topRight);
            }
            if (!botHit)
            {
                Vector2Int t_botLeft = botLeft;
                Vector2Int t_botRight = botRight;
                if (!leftHit) t_botLeft += oneCol;
                if (!rightHit) t_botRight -= oneCol;
                DrawRow(ref i, t_botLeft, t_botRight);
            }
            if (topHit && botHit && rightHit && leftHit) break;

            topLeft += oneRow - oneCol;
            topRight += oneRow + oneCol;
            botLeft -= oneRow + oneCol;
            botRight += oneCol - oneRow;

            SetTriangles();
            yield return new WaitForSeconds(spawnTime);
        }
    }

    IEnumerator Start()
    {
        float stepX = surfaceLengthX / numCellsX;
        float stepY = surfaceLengthY / numCellsY;

        vertices = new Vector3[(numCellsX + 1) * (numCellsY + 1)];
        triangles = new int[3 * 2 * numCellsX * numCellsY];

        for (int row = 0; row < numCellsY + 1; row++)
        {
            for (int col = 0; col < numCellsX + 1; col++)
            {
                vertices[row * (numCellsX + 1) + col] = new Vector3(col * stepX, 0, row * stepY);
               
            }
        }
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.name = "Grass Grid";
        mesh.vertices = vertices;

        return DrawGrid(startCellX, startCellY);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
