using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetStartupResolution : MonoBehaviour
{
    public int m_width = 405;
    public int m_height = 720;
    public bool m_isFullScreen = false;
    public int m_refreshRate = 60;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("FirstStartup") != 1)
        {
            PlayerPrefs.SetInt("FirstStartup", 1);
            Screen.SetResolution(m_width, m_height, m_isFullScreen, m_refreshRate);
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("FirstStartup", 0);
    }
}
