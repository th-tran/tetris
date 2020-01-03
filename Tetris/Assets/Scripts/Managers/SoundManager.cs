using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public bool m_musicEnabled = true;
    public bool m_fxEnabled = true;
    [Range(0,1)]
    public float m_musicVolume = 1.0f;
    [Range(0,1)]
    public float m_fxVolume = 1.0f;
    public AudioClip m_clearRowSound;
    public AudioClip m_moveSound;
    public AudioClip m_rotateSound;
    public AudioClip m_dropSound;
    public AudioClip m_holdSound;
    public AudioClip m_levelUpSound;
    public AudioClip m_gameOverSound;
    public AudioClip m_errorSound;
    public AudioSource m_musicSource;
    // List of background music tracks
    public AudioClip[] m_musicClips;
    AudioClip m_backgroundMusic;
    // List of vocals
    public AudioClip[] m_doubleVocalClips;
    public AudioClip[] m_tripleVocalClips;
    public AudioClip m_tetrisVocalClip;
    public IconToggle m_musicIconToggle;
    public IconToggle m_fxIconToggle;

    // Singleton pattern
    static SoundManager _instance;
    public static SoundManager Instance { get { return _instance; } }

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
        m_backgroundMusic = GetRandomClip(m_musicClips);
        PlayBackgroundMusic(m_backgroundMusic);
    }

    public AudioClip GetRandomClip(AudioClip[] clips)
    {
        AudioClip randomClip = clips[Random.Range(0, clips.Length)];
        return randomClip;
    }

    public void PlayBackgroundMusic(AudioClip musicClip)
    {
        // Pre-checks
        if (!m_musicEnabled || !musicClip || !m_musicSource)
        {
            return;
        }

        // Stop music if it is playing already
        StopMusic();

        m_musicSource.clip = musicClip;

        // Set the music volume
        m_musicSource.volume = m_musicVolume;

        // Set music to repeat
        m_musicSource.loop = true;

        m_musicSource.Play();
    }

    void UpdateMusic()
    {
        // Check for toggle in music enabled and act accordingly
        if (m_musicSource.isPlaying != m_musicEnabled)
        {
            if (m_musicEnabled)
            {
                m_backgroundMusic = GetRandomClip(m_musicClips);
                PlayBackgroundMusic(m_backgroundMusic);
            }
            else
            {
                StopMusic();
            }
        }
    }

    public void ToggleMusic()
    {
        m_musicEnabled = !m_musicEnabled;
        UpdateMusic();

        if (m_musicIconToggle)
        {
            m_musicIconToggle.ToggleIcon(m_musicEnabled);
        }
    }

    public void ToggleFX()
    {
        m_fxEnabled = !m_fxEnabled;

        if (m_fxIconToggle)
        {
            m_fxIconToggle.ToggleIcon(m_fxEnabled);
        }
    }

    public void StopMusic()
    {
        m_musicSource.Stop();
    }
}
