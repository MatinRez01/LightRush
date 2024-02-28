using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSetup : MonoBehaviour
{
    public Color color = Color.white;
    ParticleSystem particle;
    bool setupComplete = false;
    private void OnEnable()
    {
        if (setupComplete) return;
        particle = GetComponent<ParticleSystem>();
        particle.collision.AddPlane(GameObject.FindWithTag("Grid").transform);
        setupComplete = true;
      //  particle.startColor = color;
       
    }
}
