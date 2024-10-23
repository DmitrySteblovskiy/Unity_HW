using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // ��������� ��������� ��� ��������� �����
    public int gridWidth = 50;
    public int gridHeight = 50;
    public float cellSize = 0.1f;

    // ������ ������
    public GameObject cellPrefab;

    // 2D ������ ��� �������� ������ �� ������
    private Cell[,] grid;

    // ����, �����������, ���� �� ���������
    public bool isSimulating = false;

    // �������� ���������� ���������
    public float updateInterval = 0.1f;

    // ������ �� UI ��������
    public Button startPauseButton;
    public Button randomizeButton;
    public Button clearButton;
    public Slider speedSlider;
    public TextMeshProUGUI speedText;

    // �������� ��� ���������
    private Coroutine simulationCoroutine;

    // ��� �������� ������������� ��������� (����� ��������� ��� ������������� ������������)
    private HashSet<string> previousStates = new HashSet<string>();

    private void Start()
    {
        InitializeGrid();
        SetupUI();
    }

    // ����� ��� ������������� ����� ������
    private void InitializeGrid()
    {
        grid = new Cell[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 position = new Vector3(x * cellSize, y * cellSize, 0);
                GameObject cellObject = Instantiate(cellPrefab, position, Quaternion.identity);
                cellObject.transform.parent = this.transform;
                cellObject.name = $"Cell_{x}_{y}";

                Cell cell = cellObject.GetComponent<Cell>();
                cell.gridPosition = new Vector2Int(x, y);
                grid[x, y] = cell;
            }
        }

        // ���������� �����
        float offsetX = (gridWidth * cellSize) / 2 - cellSize / 2;
        float offsetY = (gridHeight * cellSize) / 2 - cellSize / 2;
        this.transform.position = new Vector3(-offsetX, -offsetY, 0);
    }

    // ����� ��� ��������� ����������������� ����������
    private void SetupUI()
    {
        // ��������� ������ Start/Pause
        startPauseButton.onClick.AddListener(ToggleSimulation);
        UpdateStartPauseButtonText();

        // ��������� ������ Randomize
        randomizeButton.onClick.AddListener(RandomizeGrid);

        // ��������� ������ Clear
        clearButton.onClick.AddListener(ClearGrid);

        // ��������� �������� ��������
        speedSlider.minValue = 0.01f;
        speedSlider.maxValue = 1f;
        speedSlider.value = updateInterval;
        speedSlider.onValueChanged.AddListener(OnSpeedChanged);
        UpdateSpeedText();
    }

    // ����� ��� ������� ��� ������������ ���������
    public void ToggleSimulation()
    {
        if (isSimulating)
        {
            StopSimulation();
        }
        else
        {
            StartSimulation();
        }
    }

    // ����� ��� ������� ���������
    private void StartSimulation()
    {
        isSimulating = true;
        UpdateStartPauseButtonText();
        simulationCoroutine = StartCoroutine(SimulationLoop());
    }

    // ����� ��� ��������� ���������
    private void StopSimulation()
    {
        isSimulating = false;
        UpdateStartPauseButtonText();
        if (simulationCoroutine != null)
        {
            StopCoroutine(simulationCoroutine);
        }
    }

    // �������� ��������� ����� ���������
    private IEnumerator SimulationLoop()
    {
        while (isSimulating)
        {
            StepSimulation();

            // �������� ������� ���������
            if (CheckTerminationConditions())
            {
                StopSimulation();
                yield break;
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }

    // ����� ��� ���������� ������ ���� ���������
    private void StepSimulation()
    {
        // ������� ������������ ��������� ��������� ��� ������ ������
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                int aliveNeighbors = CountAliveNeighbors(x, y);
                grid[x, y].CalculateNextState(aliveNeighbors);
            }
        }

        // ����� ��������� ��� ������������ ���������
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                grid[x, y].ApplyNextState();
            }
        }

        // �������� �� ������������� ��������� (����� ��������� ��� ������������� ������������)
        string currentState = GetGridStateHash();
        if (previousStates.Contains(currentState))
        {
            Debug.Log("������������� ������������ ����������. ��������� �����������.");
            isSimulating = false;
            UpdateStartPauseButtonText();
            StopCoroutine(simulationCoroutine);
        }
        else
        {
            previousStates.Add(currentState);
        }
    }

    // ����� ��� �������� ����� ������� ������
    private int CountAliveNeighbors(int x, int y)
    {
        int count = 0;

        // ���������� ���� 8 �������
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                    continue; // ���������� ���� ������

                int neighborX = x + dx;
                int neighborY = y + dy;

                // �������� ������ �� �������
                if (neighborX >= 0 && neighborX < gridWidth && neighborY >= 0 && neighborY < gridHeight)
                {
                    if (grid[neighborX, neighborY].isAlive)
                        count++;
                }
            }
        }

        return count;
    }

    // ����� ��� ��������� ���� �������� ��������� �����
    private string GetGridStateHash()
    {
        string hash = "";
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                hash += grid[x, y].isAlive ? "1" : "0";
            }
        }
        return hash;
    }

    // ����� ��� �������� ������� ��������� ���������
    private bool CheckTerminationConditions()
    {
        // �������� ���������� ����� ������
        foreach (var cell in grid)
        {
            if (cell.isAlive)
                return false;
        }

        Debug.Log("��� ������ ������. ��������� �����������.");
        return true;
    }

    // ����� ��� ���������� ���������� �����
    public void RandomizeGrid()
    {
        foreach (var cell in grid)
        {
            bool state = Random.value > 0.5f;
            cell.SetState(state);
        }
        previousStates.Clear();
    }

    // ����� ��� ������� �����
    public void ClearGrid()
    {
        foreach (var cell in grid)
        {
            cell.SetState(false);
        }
        previousStates.Clear();
    }

    // ����� ��� ��������� ��������� �������� ����� �������
    private void OnSpeedChanged(float value)
    {
        updateInterval = value;
        UpdateSpeedText();
    }

    // ����� ��� ���������� ������ ��������
    private void UpdateSpeedText()
    {
        speedText.text = $"��������: {updateInterval:F2} ���";
    }

    // ����� ��� ���������� ������ ������ Start/Pause
    private void UpdateStartPauseButtonText()
    {
        if (startPauseButton != null)
        {
            TextMeshProUGUI buttonText = startPauseButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = isSimulating ? "�����" : "�����";
            }
            else
            {
                Debug.LogError("��������� TextMeshProUGUI �� ������ � �������� �������� StartPauseButton.");
            }
        }
        else
        {
            Debug.LogError("startPauseButton �� �������� � ����������.");
        }
    }
}
