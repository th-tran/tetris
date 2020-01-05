using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    // Shape properties
    public bool m_canRotate = true;

    // Shape queue data
    public Vector3 m_queueOffset;

    // Used for particle effects on landing a shape
    GameObject[] m_glowSquareFx;
    public string glowSquareTag = "LandShapeFx";
    Color m_shapeColor;

    // Start is called before the first frame update
    void Start()
    {
        // Attach the shape with the all the glowing square objects.
        // There is expected to be at least 4 within the game hierarchy.
        if (glowSquareTag != "")
        {
            m_glowSquareFx = GameObject.FindGameObjectsWithTag(glowSquareTag);
        }

        m_shapeColor = GetComponentInChildren<SpriteRenderer>().color;
    }

    // Plays the particle effects from landing a shape
    public void LandShapeFX()
    {
        // Play the particle effect of landing a square
        // for each respective square of the shape.
        int i = 0;

        foreach (Transform child in gameObject.transform)
        {
            if (m_glowSquareFx[i])
            {
                m_glowSquareFx[i].transform.position = new Vector3(child.position.x, child.position.y, -2f);
                ParticlePlayer particlePlayer = m_glowSquareFx[i].GetComponent<ParticlePlayer>();
                ParticleColor particleColor = particlePlayer.GetComponentInChildren<ParticleColor>();
                particleColor.ChangeColor(m_shapeColor);

                if (particlePlayer)
                {
                    particlePlayer.Play();
                }

                i++;
            }
        }
    }

    void Move(Vector3 moveDirection)
    {
        transform.position += moveDirection;
    }

    public void MoveLeft()
    {
        Move(new Vector3(-1, 0, 0));
    }

    public void MoveRight()
    {
        Move(new Vector3(1, 0, 0));
    }

    public void MoveDown()
    {
        Move(new Vector3(0, -1, 0));
    }

    public void MoveUp()
    {
        Move(new Vector3(0, 1, 0));
    }

    void RotateRight()
    {
        if (m_canRotate)
        {
            transform.Rotate(0, 0, -90);
        }
    }

    void RotateLeft()
    {
        if (m_canRotate)
        {
            transform.Rotate(0, 0, 90);
        }
    }

    public void RotateClockwise(bool clockwise)
    {
        if (clockwise)
        {
            RotateRight();
        }
        else
        {
            RotateLeft();
        }
    }
}
