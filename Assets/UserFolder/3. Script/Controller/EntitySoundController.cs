using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySoundController : MonoBehaviour
{
    [SerializeField] private AudioClip[] m_AudioClips;

    private AudioSource m_AudioSource;

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(int index)
    {
        m_AudioSource.clip = m_AudioClips[index];
        m_AudioSource.Play();
    }

    public void PlayOneShot(int index)
    {
        m_AudioSource.PlayOneShot(m_AudioClips[index]);
    }
}
