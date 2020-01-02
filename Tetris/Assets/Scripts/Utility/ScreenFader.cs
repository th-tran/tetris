using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MaskableGraphic))]
public class ScreenFader : MonoBehaviour
{
    // ScreenFader properties
    public float m_startAlpha = 1f;
    public float m_targetAlpha = 0f;
    public float m_delay = 0f;
    public float m_timeToFade = 1f;

    // Used to calculate the incremenet in the fade formula,
    // based on properties of the attached object.
    float m_inc;

    // Used to store data of the attached object
    float m_currentAlpha;
    MaskableGraphic m_graphic;
    Color m_originalColor;

    // Start is called before the first frame update
    void Start()
    {
        m_graphic = GetComponent<MaskableGraphic>();

        m_originalColor = m_graphic.color;

        m_currentAlpha = m_startAlpha;

        Color tempColor = new Color(m_originalColor.r, m_originalColor.g, m_originalColor.b, m_currentAlpha);

        m_graphic.color = tempColor;

        m_inc = ((m_targetAlpha - m_startAlpha) / m_timeToFade) * Time.deltaTime;

        StartCoroutine("FadeRoutine");
    }

    // Fades the attached object to the target alpha
    IEnumerator FadeRoutine()
    {
        // Starting delay
        yield return new WaitForSeconds(m_delay);

        // Begin fade in/out
        while (Mathf.Abs(m_targetAlpha - m_currentAlpha) > 0.01f)
        {
            yield return new WaitForEndOfFrame();

            m_currentAlpha += m_inc;

            Color tempColor = new Color(m_originalColor.r, m_originalColor.g, m_originalColor.b, m_currentAlpha);

            m_graphic.color = tempColor;
        }
    }
}
