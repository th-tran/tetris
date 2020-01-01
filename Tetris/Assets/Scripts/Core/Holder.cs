using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holder : MonoBehaviour
{
    public Transform m_holderXform;
    public Shape m_heldShape = null;
    float m_scale = 0.5f;
    public bool m_canRelease = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Catch(Shape shape)
    {
        if (m_heldShape)
        {
            Debug.LogWarning("WARN: Release a shape before trying to hold!");
        }

        if (!shape)
        {
            Debug.LogWarning("WARN: Invalid shape!");
            return;
        }

        if (m_holderXform)
        {
            shape.transform.position = m_holderXform.position + shape.m_queueOffset;
            shape.transform.rotation = Quaternion.identity;
            shape.transform.localScale = new Vector3(m_scale, m_scale, m_scale);
            m_heldShape = shape;
        }
        else
        {
            Debug.LogWarning("WARN: Holder has no transform assigned!");
        }
    }

    public Shape Release()
    {
        // Release the shape from holder
        m_heldShape.transform.localScale = Vector3.one;
        Shape shape = m_heldShape;
        m_heldShape = null;

        // Deactivate shape holder
        m_canRelease = false;

        return shape;
    }
}
