using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    // Board data and properties
    public Transform m_emptySprite;
    public int m_height = 30;
    public int m_width = 10;
    public int m_header = 8;

    Transform[,] m_grid;

    public int m_completedRows = 0;

    // Used for playing the particle effect on line clears
    public ParticlePlayer[] m_rowGlowFx = new ParticlePlayer[4];

    void Awake()
    {
        // Initialize the grid
        m_grid = new Transform[m_width,m_height];
    }

    // Start is called before the first frame update
    void Start()
    {
        DrawEmptyCells();
    }

    // Checks if the given (x,y) coordinates are within the board space
    bool IsWithinBoard(int x, int y)
    {
        return (x >= 0 && x < m_width && y >= 0);
    }

    // Checks if the given (x,y) coordinates are currently occupied by a shape
    bool IsOccupied(int x, int y, Shape shape)
    {
        return (m_grid[x, y] != null && m_grid[x, y].parent != shape.transform);
    }

    // Checks if the shape is in a legal position on the board
    public bool IsValidPosition(Shape shape)
    {
        // Iterate through each square of the shape
        foreach (Transform child in shape.transform)
        {
            Vector2 pos = Vector2Int.RoundToInt(child.position);
            if (!IsWithinBoard((int) pos.x, (int) pos.y))
            {
                return false;
            }
            if (IsOccupied((int) pos.x, (int) pos.y, shape))
            {
                return false;
            }
        }
        return true;
    }

    // Draws the board onto the screen
    void DrawEmptyCells()
    {
        if (m_emptySprite)
        {
            for (int y = 0; y < m_height - m_header; y++)
            {
                for (int x = 0; x < m_width; x++)
                {
                    Transform clone;
                    clone = Instantiate(m_emptySprite, new Vector3(x, y, 0), Quaternion.identity) as Transform;
                    clone.name = "Board Space { x = " + x.ToString() + " , y = " + y.ToString() + " }";
                    // Parent the clones to board
                    clone.transform.parent = transform;
                }
            }
        }
        else
        {
            Debug.LogWarning("WARN: The emptySprite object is empty!");
        }
    }

    // Registers the shape into the grid as data
    public void StoreShapeInGrid(Shape shape)
    {
        if (shape == null)
        {
            return;
        }

        foreach (Transform child in shape.transform)
        {
            Vector2 pos = Vector2Int.RoundToInt(child.position);
            m_grid[(int) pos.x, (int) pos.y] = child;
        }
    }

    // Checks if the given row has a complete line
    bool IsComplete(int y)
    {
        for (int x = 0; x < m_width; x++)
        {
            if (m_grid[x,y] == null)
            {
                return false;
            }
        }
        return true;
    }

    // Removes all data from the given row of the board
    void ClearRow(int y)
    {
        for (int x = 0; x < m_width; x++)
        {
            if (m_grid[x,y] != null)
            {
                Destroy(m_grid[x,y].gameObject);
            }
            m_grid[x,y] = null;
        }
    }

    // Moves all data of the given row of the board one row down
    void ShiftOneRowDown(int y)
    {
        for (int x = 0; x < m_width; x++)
        {
            if (m_grid[x,y] != null)
            {
                m_grid[x, y-1] = m_grid[x,y];
                m_grid[x,y] = null;
                m_grid[x, y-1].position += new Vector3(0, -1, 0);
            }
        }
    }

    // Moves all data from the given row of the board up to the top, one row down
    void ShiftRowsDown(int startY)
    {
        for (int i = startY; i < m_height; i++)
        {
            ShiftOneRowDown(i);
        }
    }

    // Handles the process of clearing lines and
    // and giving visual feedback to the player
    public IEnumerator ClearAllRowsRoutine()
    {
        m_completedRows = 0;
        // Play visual effect
        for (int y = 0; y < m_height; y++)
        {
            if (IsComplete(y))
            {
                ClearRowFX(m_completedRows, y);
                m_completedRows++;
            }
        }
        // Delay before clear
        yield return new WaitForSeconds(0.5f);
        // Clear lines and shift
        for (int y = 0; y < m_height; y++)
        {
            if (IsComplete(y))
            {
                ClearRow(y);
                ShiftRowsDown(y+1);
                // Delay between each line clear
                yield return new WaitForSeconds(0.3f);
                // Check the same row again,
                // in case the row shifted down is also a line
                // (i.e. multiple lines in one drop)
                y--;
            }
        }
    }

    // Checks if the given shape is past the board top (i.e. the line of death)
    public bool IsOverLimit(Shape shape)
    {
        foreach (Transform child in shape.transform)
        {
            if (child.transform.position.y >= (m_height - m_header - 1))
            {
                return true;
            }
        }
        return false;
    }

    // Plays the particle effects from clearing the given row on the board
    void ClearRowFX(int index, int y)
    {
        // There are 4 respective row glow objects available.
        // The index is used to access them accordingly.
        if (m_rowGlowFx[index])
        {
            m_rowGlowFx[index].transform.position = new Vector3(0, y, -2f);
            m_rowGlowFx[index].Play();
        }
    }
}
