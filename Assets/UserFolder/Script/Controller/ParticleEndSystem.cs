using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEndSystem : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] m_ParticleSystems;
    [SerializeField] private float m_Duration;
    public bool IsEndParticle { get; private set; }


    public void TurnOffParticles()
    {
        foreach(ParticleSystem ps in m_ParticleSystems)
        {
            ps.Stop(true);
        }
    }

    public IEnumerator EndParicle()
    {
        float elapsedTime = 0;
        while(elapsedTime < m_Duration)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }
}
