using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // References to other game components
    Board m_gameBoard;
    Spawner m_spawner;
    Shape m_activeShape;
    Ghost m_ghost;
    Holder m_holder;
    SoundManager m_soundManager;
    ScoreManager m_scoreManager;

    // Controls speed of drop and player movement
    float m_dropInterval = 0.5f;
    float m_dropIntervalModded;
    float m_timeToDrop;
    float m_timeToNextKeyLeftRight;
    [Range(0.02f, 1f)]
    public float m_keyRepeatRateLeftRight = 0.25f;
    float m_timeToNextKeyDown;
    [Range(0.01f, 1f)]
    public float m_keyRepeatRateDown = 0.02f;

    //Controls game over state and effect
    bool m_gameOver = false;
    public GameObject m_gameOverPanel;
    public ParticlePlayer m_gameOverFx;

    // Controls rotation toggle on UI
    public IconToggle m_rotIconToggle;
    bool m_clockwise = true;

    // Controls pausing
    public bool m_isPaused = false;
    bool m_canPause = true;
    float m_introLength = 2.5f;
    public GameObject m_pausePanel;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize time-related variables
        m_timeToDrop = Time.time;
        m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
        m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;
        m_dropIntervalModded = m_dropInterval;

        // Grab reference to game components
        m_gameBoard = GameObject.FindWithTag("Board").GetComponent<Board>();
        m_spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();
        m_soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        m_scoreManager = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>();
        m_ghost = GameObject.FindWithTag("Ghost").GetComponent<Ghost>();
        m_holder = GameObject.FindWithTag("Holder").GetComponent<Holder>();

        // Pre-checks
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
            // Spawn the first shape
            m_spawner.transform.position = Vector3Int.RoundToInt(m_spawner.transform.position);
            if (m_activeShape == null)
            {
                m_activeShape = m_spawner.SpawnShape();
            }
        }

        if (!m_soundManager)
        {
            Debug.LogWarning("WARN: There is no sound manager defined!");
        }

        if (!m_scoreManager)
        {
            Debug.LogWarning("WARN: There is no score manager defined!");
        }

        if (m_gameOverPanel)
        {
            m_gameOverPanel.SetActive(false);
        }

        if (m_pausePanel)
        {
            m_pausePanel.SetActive(false);
        }

        StartCoroutine("PauseDelayRoutine");
    }

    // Update is called once per frame
    void Update()
    {
        // Pre-check
        if (!m_gameBoard ||
            !m_spawner ||
            !m_soundManager ||
            !m_scoreManager ||
            !m_activeShape ||
            m_gameOver)
        {
            return;
        }

        PlayerInput();
    }

    void LateUpdate()
    {
        if (m_ghost)
        {
            m_ghost.DrawGhost(m_activeShape, m_gameBoard);
        }
    }

    IEnumerator PauseDelayRoutine()
    {
        m_canPause = false;
        yield return new WaitForSeconds(m_introLength);
        m_canPause = true;
    }

    void PlayerInput()
    {
        if ((Input.GetButton("MoveRight") && (Time.time > m_timeToNextKeyLeftRight)) || Input.GetButtonDown("MoveRight"))
        {
            m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
            m_activeShape.MoveRight();
            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.MoveLeft();
                PlaySound(m_soundManager.m_errorSound, 0.5f);
            }
            else
            {
                PlaySound(m_soundManager.m_moveSound, 0.5f);
            }
        }
        else if ((Input.GetButton("MoveLeft") && (Time.time > m_timeToNextKeyLeftRight)) || Input.GetButtonDown("MoveLeft"))
        {
            m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
            m_activeShape.MoveLeft();
            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.MoveRight();
                PlaySound(m_soundManager.m_errorSound, 0.5f);
            }
            else
            {
                PlaySound(m_soundManager.m_moveSound, 0.5f);
            }
        }
        else if (Input.GetButtonDown("Rotate") && m_activeShape.m_canRotate)
        {
            m_activeShape.RotateClockwise(m_clockwise);
            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.RotateClockwise(!m_clockwise);
                PlaySound(m_soundManager.m_errorSound, 0.5f);
            }
            else
            {
                PlaySound(m_soundManager.m_rotateSound, 0.5f);
            }
        }
        else if ((Input.GetButton("MoveDown") && (Time.time > m_timeToNextKeyDown)) || (Time.time > m_timeToDrop))
        {
            m_timeToNextKeyDown = Time.time + m_keyRepeatRateDown;
            m_timeToDrop = Time.time + m_dropIntervalModded;
            m_activeShape.MoveDown();

            // Validate the shape's position within board
            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                if (m_gameBoard.IsOverLimit(m_activeShape))
                {
                    GameOver();
                }
                else
                {
                    LandShape();
                }
            }
        }
        else if (Input.GetButtonDown("ToggleRot"))
        {
            ToggleRotDirection();
        }
        else if (Input.GetButtonDown("Pause") && m_canPause)
        {
            TogglePause();
        }
        else if (Input.GetButtonDown("Hold"))
        {
            Hold();
        }
    }

    void LandShape()
    {
        if (m_activeShape)
        {
            // No input delay after shape lands
            m_timeToNextKeyLeftRight = Time.time;
            m_timeToNextKeyDown = Time.time;

            // Land the shape
            m_activeShape.MoveUp();
            m_gameBoard.StoreShapeInGrid(m_activeShape);

            m_activeShape.LandShapeFX();

            // Reset the ghost shape before spawning a new shape
            if (m_ghost)
            {
                m_ghost.Reset();
            }

            // Reactivate the shape holder
            if (m_holder)
            {
                m_holder.m_canRelease = true;
            }

            m_activeShape = m_spawner.SpawnShape();

            // Play sound FX for shape landing
            PlaySound(m_soundManager.m_dropSound, 0.75f);

            // Clear rows (if any)
            m_gameBoard.StartCoroutine("ClearAllRowsRoutine");

            if (m_gameBoard.m_completedRows > 0)
            {
                // Update score
                m_scoreManager.ScoreLines(m_gameBoard.m_completedRows);
                if (m_scoreManager.m_didLevelUp)
                {
                    PlaySound(m_soundManager.m_levelUpSound, 1f);
                    // Gradually increase difficulty with each level, up to a max of 0.05f fall speed
                    m_dropIntervalModded = Mathf.Clamp(m_dropInterval - (((float) m_scoreManager.m_level - 1) * 0.1f), 0.05f, 1f);
                }
                else
                {
                    PlaySound(m_soundManager.m_clearRowSound, 0.5f);
                }
                if (m_gameBoard.m_completedRows == 2)
                {
                    AudioClip randomVocal = m_soundManager.GetRandomClip(m_soundManager.m_doubleVocalClips);
                    PlaySound(randomVocal, 5f);
                }
                else if (m_gameBoard.m_completedRows == 3)
                {
                    AudioClip randomVocal = m_soundManager.GetRandomClip(m_soundManager.m_tripleVocalClips);
                    PlaySound(randomVocal, 5f);
                }
                else if (m_gameBoard.m_completedRows >= 4)
                {
                    PlaySound(m_soundManager.m_tetrisVocalClip, 5f);
                }
            }
        }
    }

    void GameOver()
    {
        m_activeShape.MoveUp();
        m_gameOver = true;

        StartCoroutine("GameOverRoutine");

        PlaySound(m_soundManager.m_gameOverSound, 5f);
        m_soundManager.StopMusic();
    }

    IEnumerator GameOverRoutine()
    {
        if (m_gameOverFx)
        {
            m_gameOverFx.Play();
        }

        // Delay before bringing up game over screen
        yield return new WaitForSeconds(0.3f);

        if (m_gameOverPanel)
        {
            m_gameOverPanel.SetActive(true);
        }
    }

    public void Restart()
    {
        // Reload the active scene at normal speed
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void PlaySound(AudioClip clip, float volMultiplier)
    {
        if (clip && m_soundManager.m_fxEnabled)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, m_soundManager.m_fxVolume * volMultiplier);
        }
    }

    public void ToggleRotDirection()
    {
        m_clockwise = !m_clockwise;
        if (m_rotIconToggle)
        {
            m_rotIconToggle.ToggleIcon(m_clockwise);
        }
    }

    public void TogglePause()
    {
        if (m_gameOver)
        {
            return;
        }

        m_isPaused = !m_isPaused;

        if (m_pausePanel)
        {
            m_pausePanel.SetActive(m_isPaused);

            if (m_soundManager)
            {
                // Play music quieter if paused
                m_soundManager.m_musicSource.volume = (m_isPaused) ? m_soundManager.m_musicVolume = 0.25f : m_soundManager.m_musicVolume = 1f;
            }

            // Stop time if paused
            Time.timeScale = (m_isPaused) ? 0f : 1f;
        }
    }

    public void Hold()
    {
        if (!m_holder)
        {
            return;
        }

        if (!m_holder.m_heldShape)
        {
            // Hold the shape
            m_holder.Catch(m_activeShape);
            PlaySound(m_soundManager.m_holdSound, 1f);
            m_activeShape = m_spawner.SpawnShape();
        }
        else if (m_holder.m_canRelease)
        {
            // Swap the shape with the currently held shape
            Shape temp = m_activeShape;
            m_activeShape = m_holder.Release();
            m_activeShape.transform.position = m_spawner.transform.position;
            m_holder.Catch(temp);
            PlaySound(m_soundManager.m_holdSound, 1f);
        }
        else
        {
            Debug.LogWarning("WARN: Wait for cooldown!");
            PlaySound(m_soundManager.m_errorSound, 1f);
        }

        // New shape is spawning, so reset the ghost shape
        if (m_ghost)
        {
            m_ghost.Reset();
        }
    }
}
