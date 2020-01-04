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

    // Controls speed of drop and player movement
    float m_dropInterval = 0.5f;
    float m_dropIntervalModded;
    float m_timeToDrop;
    float m_timeToNextKeyLeftRight;
    [Range(0.02f, 1f)]
    public float m_keyRepeatRateLeftRight = 0.3f;
    float m_timeToNextKeyDown;
    [Range(0.01f, 1f)]
    public float m_keyRepeatRateDown = 0.02f;

    // Keep track of states
    public bool m_movingRight = false;
    public bool m_movingLeft = false;

    // Scoring aspects related to player movement
    const int m_softDropFactor = 1;
    const int m_hardDropFactor = 2;

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
    const float m_introLength = 2.5f;
    public GameObject m_pausePanel;

    // Singleton pattern
    static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

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

        if (m_gameOverPanel)
        {
            m_gameOverPanel.SetActive(false);
        }

        if (m_pausePanel)
        {
            m_pausePanel.SetActive(false);
        }

        StartCoroutine("IntroDelayRoutine");
    }

    // Update is called once per frame
    void Update()
    {
        // Pre-check
        if (!m_gameBoard ||
            !m_spawner ||
            !m_activeShape ||
            m_gameOver)
        {
            return;
        }

        PlayerInput();
    }

    void LateUpdate()
    {
        if (m_ghost && m_activeShape)
        {
            m_ghost.DrawGhost(m_activeShape, m_gameBoard);
        }
    }

    // Used to control behaviour of game while intro is playing
    IEnumerator IntroDelayRoutine()
    {
        // Disable pausing for length of intro
        m_canPause = false;
        yield return new WaitForSeconds(m_introLength);

        // Spawn the first shape
        m_spawner.transform.position = Vector3Int.RoundToInt(m_spawner.transform.position);
        if (m_activeShape == null)
        {
            m_activeShape = m_spawner.SpawnShape();
        }

        // Re-enable pausing
        m_canPause = true;
    }

    // Used to handle all player input during each frame
    void PlayerInput()
    {
        // Player stops going right
        bool case1 = Input.GetButtonUp("MoveRight");
        // Player stops going left
        bool case2 = Input.GetButtonUp("MoveLeft");
        // Player is holding right and switches to left
        bool case3 = (Input.GetButton("MoveRight") && Input.GetButtonDown("MoveLeft"));
        // Player is holding left and switches to right
        bool case4 = (Input.GetButton("MoveLeft") && Input.GetButtonDown("MoveRight"));
        // Reset to base interval on left/right release or direction change
        if (case1 || case2 || case3 || case4)
        {
            m_keyRepeatRateLeftRight = 0.3f;

            if (case1)
            {
                m_movingRight = false;
            }

            if (case2)
            {
                m_movingLeft = false;
            }

            if (case3)
            {
                m_movingRight = false;
                m_movingLeft = true;
            }
            else if (case4)
            {
                m_movingLeft = false;
                m_movingRight = true;
            }
        }

        if ((Input.GetButton("MoveRight") && (Time.time > m_timeToNextKeyLeftRight)) || Input.GetButtonDown("MoveRight"))
        {
            if (!m_movingLeft)
            {
                m_keyRepeatRateLeftRight = Mathf.Clamp(m_keyRepeatRateLeftRight / 2, 0.05f, 0.15f);
                m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
                m_activeShape.MoveRight();
                if (!m_gameBoard.IsValidPosition(m_activeShape))
                {
                    m_activeShape.MoveLeft();
                    PlaySound(SoundManager.Instance.m_errorSound, 0.5f);
                }
                else
                {
                    PlaySound(SoundManager.Instance.m_moveSound, 0.5f);
                }
            }
        }

        if ((Input.GetButton("MoveLeft") && (Time.time > m_timeToNextKeyLeftRight)) || Input.GetButtonDown("MoveLeft"))
        {
            if (!m_movingRight)
            {
                m_keyRepeatRateLeftRight = Mathf.Clamp(m_keyRepeatRateLeftRight / 2, 0.05f, 0.15f);
                m_timeToNextKeyLeftRight = Time.time + m_keyRepeatRateLeftRight;
                m_activeShape.MoveLeft();
                if (!m_gameBoard.IsValidPosition(m_activeShape))
                {
                    m_activeShape.MoveRight();
                    PlaySound(SoundManager.Instance.m_errorSound, 0.5f);
                }
                else
                {
                    PlaySound(SoundManager.Instance.m_moveSound, 0.5f);
                }
            }
        }

        if (Input.GetButtonDown("RotateRight") && m_activeShape.m_canRotate)
        {
            m_activeShape.RotateClockwise(m_clockwise);
            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.RotateClockwise(!m_clockwise);
                PlaySound(SoundManager.Instance.m_errorSound, 0.5f);
            }
            else
            {
                PlaySound(SoundManager.Instance.m_rotateSound, 0.5f);
            }
        }
        else if (Input.GetButtonDown("RotateLeft") && m_activeShape.m_canRotate)
        {
            m_activeShape.RotateClockwise(!m_clockwise);
            if (!m_gameBoard.IsValidPosition(m_activeShape))
            {
                m_activeShape.RotateClockwise(m_clockwise);
                PlaySound(SoundManager.Instance.m_errorSound, 0.5f);
            }
            else
            {
                PlaySound(SoundManager.Instance.m_rotateSound, 0.5f);
            }
        }
        else if (Input.GetButtonDown("HardDrop"))
        {
            bool hitBottom = false;
            int numRowsDropped = 0;
            while (!hitBottom)
            {
                m_activeShape.MoveDown();
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
                    hitBottom = true;
                }
                else
                {
                    numRowsDropped++;
                }
            }

            // Hard drop bonus
            ScoreManager.Instance.AddScore(m_hardDropFactor * numRowsDropped);
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

            // Soft drop score bonus
            if (Input.GetButton("MoveDown"))
            {
                ScoreManager.Instance.AddScore(m_softDropFactor);
            }
        }
        else if (Input.GetButtonDown("ToggleRotation"))
        {
            ToggleRotationDir();
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

    // Handles the logic and visual effects from landing a shape
    // on the bottom of the grid.
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
            PlaySound(SoundManager.Instance.m_dropSound, 0.75f);

            // Clear rows (if any)
            m_gameBoard.StartCoroutine("ClearAllRowsRoutine");

            if (m_gameBoard.m_completedRows > 0)
            {
                // Update score
                ScoreManager.Instance.ScoreLines(m_gameBoard.m_completedRows);
                if (ScoreManager.Instance.m_didLevelUp)
                {
                    PlaySound(SoundManager.Instance.m_levelUpSound, 1f);
                    // Gradually increase difficulty with each level, up to a max of 0.05f fall speed
                    m_dropIntervalModded = Mathf.Clamp(m_dropInterval - (((float) ScoreManager.Instance.m_level - 1) * 0.1f), 0.05f, 1f);
                }
                else
                {
                    PlaySound(SoundManager.Instance.m_clearRowSound, 0.5f);
                }
                if (m_gameBoard.m_completedRows == 2)
                {
                    AudioClip randomVocal = SoundManager.Instance.GetRandomClip(SoundManager.Instance.m_doubleVocalClips);
                    PlaySound(randomVocal, 5f);
                }
                else if (m_gameBoard.m_completedRows == 3)
                {
                    AudioClip randomVocal = SoundManager.Instance.GetRandomClip(SoundManager.Instance.m_tripleVocalClips);
                    PlaySound(randomVocal, 5f);
                }
                else if (m_gameBoard.m_completedRows >= 4)
                {
                    PlaySound(SoundManager.Instance.m_tetrisVocalClip, 5f);
                }
            }
        }
    }

    // Invokes game over sequence
    void GameOver()
    {
        m_activeShape.MoveUp();
        m_gameOver = true;

        StartCoroutine("GameOverRoutine");

        PlaySound(SoundManager.Instance.m_gameOverSound, 5f);
        SoundManager.Instance.StopMusic();
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

    // Reload the current level
    public void Restart()
    {
        // Reload the active scene at normal speed
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Play a one-shot audio of the given clip at the given volume
    void PlaySound(AudioClip clip, float volMultiplier)
    {
        if (clip && SoundManager.Instance.m_fxEnabled)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, SoundManager.Instance.m_fxVolume * volMultiplier);
        }
    }

    // Reverses rotation direction
    public void ToggleRotationDir()
    {
        m_clockwise = !m_clockwise;
        if (m_rotIconToggle)
        {
            m_rotIconToggle.ToggleIcon(m_clockwise);
        }
    }

    // Brings up pause menu and pauses game if not paused,
    // and vice versa.
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

            // Play music quieter if paused
            SoundManager.Instance.m_musicSource.volume = (m_isPaused) ? SoundManager.Instance.m_musicVolume = 0.25f : SoundManager.Instance.m_musicVolume = 1f;

            // Stop time if paused
            Time.timeScale = (m_isPaused) ? 0f : 1f;
        }
    }

    // Places the current active shape in the holder,
    // or swaps the current active shape with the currently held shape.
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
            PlaySound(SoundManager.Instance.m_holdSound, 1f);
            m_activeShape = m_spawner.SpawnShape();
        }
        else if (m_holder.m_canRelease)
        {
            // Swap the shape with the currently held shape
            Shape temp = m_activeShape;
            m_activeShape = m_holder.Release();
            m_activeShape.transform.position = m_spawner.transform.position;
            m_holder.Catch(temp);
            PlaySound(SoundManager.Instance.m_holdSound, 1f);
        }
        else
        {
            Debug.LogWarning("WARN: Wait for cooldown!");
            PlaySound(SoundManager.Instance.m_errorSound, 1f);
        }

        // New shape is spawning, so reset the ghost shape
        if (m_ghost)
        {
            m_ghost.Reset();
        }
    }
}
