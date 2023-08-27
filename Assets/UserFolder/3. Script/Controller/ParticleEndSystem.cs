using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEndSystem : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] m_ParticleSystems;

    public bool IsEndParticle { get; private set; }


    public void TurnOffParticles()
    {
        foreach(ParticleSystem ps in m_ParticleSystems)
            ps.Stop(true);
    }
}
