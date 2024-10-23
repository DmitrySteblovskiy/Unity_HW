using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    public int gridWidth = 50;
    public int gridHeight = 50;
    public float cellSize = 0.1f;
    public Color gridColor = Color.black;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        DrawGrid();
    }

    void DrawGrid()
    {
        int totalLines = (gridWidth + gridHeight) * 2;
        lineRenderer.positionCount = totalLines * 2;
        int index = 0;

        // Вертикальные линии
        for (int x = 0; x <= gridWidth; x++)
        {
            lineRenderer.SetPosition(index++, new Vector3(x * cellSize, 0, 0));
            lineRenderer.SetPosition(index++, new Vector3(x * cellSize, gridHeight * cellSize, 0));
        }

        // Горизонтальные линии
        for (int y = 0; y <= gridHeight; y++)
        {
            lineRenderer.SetPosition(index++, new Vector3(0, y * cellSize, 0));
            lineRenderer.SetPosition(index++, new Vector3(gridWidth * cellSize, y * cellSize, 0));
        }
    }
}
