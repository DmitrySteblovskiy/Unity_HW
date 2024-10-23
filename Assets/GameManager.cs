using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Публичные параметры для настройки сетки
    public int gridWidth = 50;
    public int gridHeight = 50;
    public float cellSize = 0.1f;

    // Префаб клетки
    public GameObject cellPrefab;

    // 2D массив для хранения ссылок на клетки
    private Cell[,] grid;

    // Флаг, указывающий, идет ли симуляция
    public bool isSimulating = false;

    // Интервал обновления поколений
    public float updateInterval = 0.1f;

    // Ссылка на UI элементы
    public Button startPauseButton;
    public Button randomizeButton;
    public Button clearButton;
    public Slider speedSlider;
    public TextMeshProUGUI speedText;

    // Корутина для симуляции
    private Coroutine simulationCoroutine;

    // Для проверки повторяющихся состояний (можно расширить для периодических конфигураций)
    private HashSet<string> previousStates = new HashSet<string>();

    private void Start()
    {
        InitializeGrid();
        SetupUI();
    }

    // Метод для инициализации сетки клеток
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

        // Центрируем сетку
        float offsetX = (gridWidth * cellSize) / 2 - cellSize / 2;
        float offsetY = (gridHeight * cellSize) / 2 - cellSize / 2;
        this.transform.position = new Vector3(-offsetX, -offsetY, 0);
    }

    // Метод для настройки пользовательского интерфейса
    private void SetupUI()
    {
        // Настройка кнопки Start/Pause
        startPauseButton.onClick.AddListener(ToggleSimulation);
        UpdateStartPauseButtonText();

        // Настройка кнопки Randomize
        randomizeButton.onClick.AddListener(RandomizeGrid);

        // Настройка кнопки Clear
        clearButton.onClick.AddListener(ClearGrid);

        // Настройка слайдера скорости
        speedSlider.minValue = 0.01f;
        speedSlider.maxValue = 1f;
        speedSlider.value = updateInterval;
        speedSlider.onValueChanged.AddListener(OnSpeedChanged);
        UpdateSpeedText();
    }

    // Метод для запуска или приостановки симуляции
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

    // Метод для запуска симуляции
    private void StartSimulation()
    {
        isSimulating = true;
        UpdateStartPauseButtonText();
        simulationCoroutine = StartCoroutine(SimulationLoop());
    }

    // Метод для остановки симуляции
    private void StopSimulation()
    {
        isSimulating = false;
        UpdateStartPauseButtonText();
        if (simulationCoroutine != null)
        {
            StopCoroutine(simulationCoroutine);
        }
    }

    // Корутина основного цикла симуляции
    private IEnumerator SimulationLoop()
    {
        while (isSimulating)
        {
            StepSimulation();

            // Проверка условий остановки
            if (CheckTerminationConditions())
            {
                StopSimulation();
                yield break;
            }

            yield return new WaitForSeconds(updateInterval);
        }
    }

    // Метод для выполнения одного шага симуляции
    private void StepSimulation()
    {
        // Сначала рассчитываем следующее состояние для каждой клетки
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                int aliveNeighbors = CountAliveNeighbors(x, y);
                grid[x, y].CalculateNextState(aliveNeighbors);
            }
        }

        // Затем применяем все рассчитанные состояния
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                grid[x, y].ApplyNextState();
            }
        }

        // Проверка на повторяющееся состояние (можно расширить для периодических конфигураций)
        string currentState = GetGridStateHash();
        if (previousStates.Contains(currentState))
        {
            Debug.Log("Повторяющаяся конфигурация обнаружена. Симуляция остановлена.");
            isSimulating = false;
            UpdateStartPauseButtonText();
            StopCoroutine(simulationCoroutine);
        }
        else
        {
            previousStates.Add(currentState);
        }
    }

    // Метод для подсчета живых соседей клетки
    private int CountAliveNeighbors(int x, int y)
    {
        int count = 0;

        // Перебираем всех 8 соседей
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                    continue; // Пропускаем саму клетку

                int neighborX = x + dx;
                int neighborY = y + dy;

                // Проверка выхода за границы
                if (neighborX >= 0 && neighborX < gridWidth && neighborY >= 0 && neighborY < gridHeight)
                {
                    if (grid[neighborX, neighborY].isAlive)
                        count++;
                }
            }
        }

        return count;
    }

    // Метод для получения хэша текущего состояния сетки
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

    // Метод для проверки условий остановки симуляции
    private bool CheckTerminationConditions()
    {
        // Проверка отсутствия живых клеток
        foreach (var cell in grid)
        {
            if (cell.isAlive)
                return false;
        }

        Debug.Log("Все клетки мёртвые. Симуляция остановлена.");
        return true;
    }

    // Метод для рандомного заполнения сетки
    public void RandomizeGrid()
    {
        foreach (var cell in grid)
        {
            bool state = Random.value > 0.5f;
            cell.SetState(state);
        }
        previousStates.Clear();
    }

    // Метод для очистки сетки
    public void ClearGrid()
    {
        foreach (var cell in grid)
        {
            cell.SetState(false);
        }
        previousStates.Clear();
    }

    // Метод для обработки изменения скорости через слайдер
    private void OnSpeedChanged(float value)
    {
        updateInterval = value;
        UpdateSpeedText();
    }

    // Метод для обновления текста скорости
    private void UpdateSpeedText()
    {
        speedText.text = $"Скорость: {updateInterval:F2} сек";
    }

    // Метод для обновления текста кнопки Start/Pause
    private void UpdateStartPauseButtonText()
    {
        if (startPauseButton != null)
        {
            TextMeshProUGUI buttonText = startPauseButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = isSimulating ? "Пауза" : "Старт";
            }
            else
            {
                Debug.LogError("Компонент TextMeshProUGUI не найден в дочерних объектах StartPauseButton.");
            }
        }
        else
        {
            Debug.LogError("startPauseButton не назначен в инспекторе.");
        }
    }
}
