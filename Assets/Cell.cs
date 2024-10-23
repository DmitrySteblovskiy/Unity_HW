using UnityEngine;

public class Cell : MonoBehaviour
{
    // Текущее состояние клетки: живая или мёртвая
    public bool isAlive = false;

    // Состояние клетки в следующем поколении
    private bool nextState = false;

    // Ссылка на компонент SpriteRenderer для изменения цвета клетки
    private SpriteRenderer spriteRenderer;

    // Размер клетки (для позиционирования)
    public Vector2Int gridPosition;

    // Ссылка на GameManager для доступа к сетке
    private GameManager gameManager;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = FindObjectOfType<GameManager>();
        UpdateColor();
    }

    // Метод для обновления цвета клетки в зависимости от её состояния
    public void UpdateColor()
    {
        if (isAlive)
        {
            spriteRenderer.color = Color.black; // Живая клетка - черная
        }
        else
        {
            spriteRenderer.color = Color.white; // Мёртвая клетка - белая
        }
    }

    // Метод для установки состояния клетки
    public void SetState(bool state)
    {
        isAlive = state;
        UpdateColor();
    }

    // Метод, вызываемый при клике мыши на клетку
    private void OnMouseDown()
    {
        // Позволяем изменять состояние только если симуляция на паузе
        if (!gameManager.isSimulating)
        {
            isAlive = !isAlive;
            UpdateColor();
        }
    }

    // Метод для расчета следующего состояния клетки
    public void CalculateNextState(int aliveNeighbors)
    {
        if (isAlive)
        {
            // Правила для живой клетки
            if (aliveNeighbors < 2 || aliveNeighbors > 3)
            {
                nextState = false; // Умирает
            }
            else
            {
                nextState = true; // Остаётся живой
            }
        }
        else
        {
            // Правила для мёртвой клетки
            if (aliveNeighbors == 3)
            {
                nextState = true; // Рождается жизнь
            }
            else
            {
                nextState = false; // Остаётся мёртвой
            }
        }
    }

    // Метод для применения следующего состояния клетки
    public void ApplyNextState()
    {
        if (isAlive != nextState)
        {
            isAlive = nextState;
            UpdateColor();
        }
    }
}
