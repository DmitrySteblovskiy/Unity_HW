using UnityEngine;

public class Cell : MonoBehaviour
{
    // ������� ��������� ������: ����� ��� ������
    public bool isAlive = false;

    // ��������� ������ � ��������� ���������
    private bool nextState = false;

    // ������ �� ��������� SpriteRenderer ��� ��������� ����� ������
    private SpriteRenderer spriteRenderer;

    // ������ ������ (��� ����������������)
    public Vector2Int gridPosition;

    // ������ �� GameManager ��� ������� � �����
    private GameManager gameManager;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = FindObjectOfType<GameManager>();
        UpdateColor();
    }

    // ����� ��� ���������� ����� ������ � ����������� �� � ���������
    public void UpdateColor()
    {
        if (isAlive)
        {
            spriteRenderer.color = Color.black; // ����� ������ - ������
        }
        else
        {
            spriteRenderer.color = Color.white; // ̸����� ������ - �����
        }
    }

    // ����� ��� ��������� ��������� ������
    public void SetState(bool state)
    {
        isAlive = state;
        UpdateColor();
    }

    // �����, ���������� ��� ����� ���� �� ������
    private void OnMouseDown()
    {
        // ��������� �������� ��������� ������ ���� ��������� �� �����
        if (!gameManager.isSimulating)
        {
            isAlive = !isAlive;
            UpdateColor();
        }
    }

    // ����� ��� ������� ���������� ��������� ������
    public void CalculateNextState(int aliveNeighbors)
    {
        if (isAlive)
        {
            // ������� ��� ����� ������
            if (aliveNeighbors < 2 || aliveNeighbors > 3)
            {
                nextState = false; // �������
            }
            else
            {
                nextState = true; // ������� �����
            }
        }
        else
        {
            // ������� ��� ������ ������
            if (aliveNeighbors == 3)
            {
                nextState = true; // ��������� �����
            }
            else
            {
                nextState = false; // ������� ������
            }
        }
    }

    // ����� ��� ���������� ���������� ��������� ������
    public void ApplyNextState()
    {
        if (isAlive != nextState)
        {
            isAlive = nextState;
            UpdateColor();
        }
    }
}
