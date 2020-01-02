using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IconToggle : MonoBehaviour
{
    // References to "on/off" icons of the attached object, respectively
    public Sprite m_iconTrue;
    public Sprite m_iconFalse;

    // Defaults to "on" icon
    public bool m_defaultIconState = true;

    // Used to store the image data of the attached object
    Image m_image;

    // Start is called before the first frame update
    void Start()
    {
        m_image = GetComponent<Image>();
        m_image.sprite = (m_defaultIconState) ? m_iconTrue : m_iconFalse;
    }

    // Switches the image to use the "off" icon if the current icon is "on",
    // and vice versa.
    public void ToggleIcon(bool state)
    {
        if (!m_image || !m_iconTrue || !m_iconFalse)
        {
            Debug.LogWarning("WARN: ICONTOGGLE missing iconTrue or iconFalse!");
            return;
        }
        m_image.sprite = (state) ? m_iconTrue : m_iconFalse;
    }
}
