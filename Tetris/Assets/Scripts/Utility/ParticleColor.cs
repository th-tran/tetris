using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleColor : MonoBehaviour
{
    ParticleSystem.MainModule main;

    // Start is called before the first frame update
    void Start()
    {
        main = GetComponent<ParticleSystem>().main;
    }

    public void ChangeColor(Color color)
    {
        main.startColor = color;
    }
}
