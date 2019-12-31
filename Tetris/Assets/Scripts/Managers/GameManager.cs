using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    Board m_gameBoard;
    Spawner m_spawner;
    Shape m_activeShape;
    float m_dropInterval = 0.25f;
    float m_timeToDrop;

    // Start is called before the first frame update
    void Start()
    {
        m_gameBoard = GameObject.FindWithTag("Board").GetComponent<Board>();
        m_spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();

        if (!m_gameBoard)
        {
            Debug.LogWarning("WARN: There is no game board defined!");
        }

        if (!m_spawner)
        {
            Debug.LogWarning("WARN: There is no spawner defined!");
        }
        else
        {
            m_spawner.transform.position = Vector3Int.RoundToInt(m_spawner.transform.position);
            if (m_activeShape == null)
            {
                m_activeShape = m_spawner.SpawnShape();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_gameBoard || !m_spawner)
        {
            return;
        }
        // Drop in specified increments
        if (Time.time > m_timeToDrop)
        {
            m_timeToDrop = Time.time + m_dropInterval;
            if (m_activeShape)
            {
                m_activeShape.MoveDown();

                // Validate the shape's position within board
                if (!m_gameBoard.IsValidPosition(m_activeShape))
                {
                    // Shape lands and registers in grid
                    m_activeShape.MoveUp();
                    m_gameBoard.StoreShapeInGrid(m_activeShape);

                    // Spawn a new shape
                    if (m_spawner)
                    {
                        m_activeShape = m_spawner.SpawnShape();
                    }
                 }
            }
        }
    }
}
