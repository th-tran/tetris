using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePlayer : MonoBehaviour
{
    // List of all particle systems of the attached object
    public ParticleSystem[] allParticles;

    // Start is called before the first frame update
    void Start()
    {
        allParticles = GetComponentsInChildren<ParticleSystem>();
    }

    public void Play()
    {
        foreach (ParticleSystem ps in allParticles)
        {
            ps.Stop();
            ps.Play();
        }
    }
}
