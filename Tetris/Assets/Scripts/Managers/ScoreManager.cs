using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    // Data controlled by ScoreManager
    int m_score = 0;
    int m_lines;
    public int m_level = 1;

    // Number of lines to clear before next level
    public int m_linesPerLevel = 3;

    // References to labels in UI
    public Text m_linesText;
    public Text m_levelText;
    public Text m_scoreText;

    public bool m_didLevelUp = false;

    // Min/max number of lines that can be cleared in a single shape landing
    const int m_minLines = 1;
    const int m_maxLines = 4;

    // Determines max possible score and factors used in scoring
    const int m_maxScoreDigits = 5;

    const int m_singleFactor = 40;
    const int m_doubleFactor = 100;
    const int m_tripleFactor = 300;
    const int m_tetrisFactor = 1200;

    // Used for particle effect on level up
    public ParticlePlayer m_levelUpFx;

    // Singleton pattern
    static ScoreManager _instance;
    public static ScoreManager Instance { get { return _instance; } }

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
        Reset();
    }

    // Used to update score based on given number of lines cleared
    public void ScoreLines(int n)
    {
        m_didLevelUp = false;

        n = Mathf.Clamp(n, m_minLines, m_maxLines);

        switch (n)
        {
            case 1:
                m_score += m_singleFactor * m_level;
                break;
            case 2:
                m_score +=  m_doubleFactor * m_level;
                break;
            case 3:
                m_score += m_tripleFactor * m_level;
                break;
            case 4:
                m_score += m_tetrisFactor * m_level;
                break;
            default:
                break;
        }

        // Subtract from current lines until level up.
        // If the number of lines was met, level up.
        m_lines -= n;
        if (m_lines <= 0)
        {
            LevelUp();
        }

        UpdateUIText();
    }

    // Add the given amount to score and update on screen
    public void AddScore(int n)
    {
        m_score += n;
        UpdateUIText();
    }

    // Resets everything related to levels, lines and score to default
    public void Reset()
    {
        m_level = 1;
        m_lines = m_linesPerLevel * m_level;
        UpdateUIText();
    }

    void UpdateUIText()
    {
        if (m_linesText)
        {
            m_linesText.text = m_lines.ToString();
        }

        if (m_levelText)
        {
            m_levelText.text = m_level.ToString();
        }

        if (m_scoreText)
        {
            m_scoreText.text = PadZero(m_score, m_maxScoreDigits);
        }
    }

    // Helper function that returns the given number as a string
    // padded with zeroes up to the given number of digits.
    string PadZero(int n, int padDigits)
    {
        string nStr = n.ToString();

        while (nStr.Length < padDigits)
        {
            nStr = "0" + nStr;
        }

        return nStr;
    }

    // Handles the events on leveling up
    public void LevelUp()
    {
        m_level++;
        m_lines = m_linesPerLevel * m_level;
        m_didLevelUp = true;

        if (m_levelUpFx)
        {
            m_levelUpFx.Play();
        }
    }
}
