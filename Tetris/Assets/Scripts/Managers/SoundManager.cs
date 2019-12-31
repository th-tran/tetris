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
    public AudioClip m_dropSound;
    public AudioClip m_gameOverSound;
    public AudioSource m_musicSource;
    // List of background music tracks
    public AudioClip[] m_musicClips;
    AudioClip m_backgroundMusic;

    // Start is called before the first frame update
    void Start()
    {
        m_backgroundMusic = GetRandomClip(m_musicClips);
        PlayBackgroundMusic(m_backgroundMusic);
    }

    // Update is called once per frame
    void Update()
    {

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
        m_musicSource.Stop();

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
                m_musicSource.Stop();
            }
        }
    }

    public void ToggleMusic()
    {
        m_musicEnabled = !m_musicEnabled;
        UpdateMusic();
    }
}
