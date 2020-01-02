using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holder : MonoBehaviour
{
    // Holder data and properties
    public Transform m_holderXform;
    public Shape m_heldShape = null;
    float m_scale = 0.5f;
    public bool m_canRelease = false;

    // Puts the given shape into the holder,
    // or swaps it with the currently held shape.
    // Does nothing if the holder is on cooldown.
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
            // Put the shape into the holder
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

    // Returns the currently held shape,
    // or null if the holder is empty.
    public Shape Release()
    {
        if (m_heldShape)
        {
            // Release the shape from holder
            m_heldShape.transform.localScale = Vector3.one;
            Shape shape = m_heldShape;
            m_heldShape = null;

            // Deactivate shape holder
            m_canRelease = false;

            return shape;
        }
        else
        {
            return null;
        }
    }
}
