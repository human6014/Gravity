using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestFireLight : MonoBehaviour
{
    public bool IsPlaying => m_IsPlaying;

    [SerializeField]
    private float m_Intensity = 1f;

    [SerializeField]
    [Range(0f, 10f)]
    private float m_Range = 1f;

    [SerializeField]
    private Color m_Color = Color.yellow;

    [Space]

    [SerializeField]
    [Range(0f, 2f)]
    private float m_FadeInTime = 0.5f;

    [SerializeField]
    [Range(0f, 2f)]
    private float m_FadeOutTime = 0.5f;


    private float m_Weight;

    private bool m_IsPlaying;
    private bool m_FadingIn;
    private bool m_FadingOut;

    private Light m_Lights;


    public void Play(bool fadeIn)
    {
        if (m_IsPlaying)
            return;

        m_IsPlaying = true;
        m_FadingIn = fadeIn;
        m_Weight = m_FadingIn ? 0f : 1f;
    }

    public void Stop(bool fadeOut)
    {
        m_IsPlaying = false;
        m_FadingOut = fadeOut;

        if (!m_FadingOut)
            m_Weight = 0f;
    }

    private void Awake()
    {
        m_Lights = GetComponent<Light>();
        m_Lights.enabled = false;
    }

    private void Update()
    {
        float intensity = m_Intensity;
        float range = m_Range;
        Color color = m_Color;

        // Fade in & out
        if (m_FadingIn)
        {
            m_Weight = Mathf.MoveTowards(m_Weight, 1f, Time.deltaTime * (1f / m_FadeInTime));

            if (m_Weight == 1f) m_FadingIn = false;
        }
        else if (m_FadingOut)
        {
            m_Weight = Mathf.MoveTowards(m_Weight, 0f, Time.deltaTime * (1f / m_FadeOutTime));

            if (m_Weight == 0f)
            {
                m_Lights.enabled = false;
                m_Lights.intensity = 0;
                m_FadingOut = false;
            }
        }

        // Enable the light if effects started playing
        if (!m_Lights.enabled && m_IsPlaying) m_Lights.enabled = true;

        // Apply effects to the light
        if (m_Lights.enabled)
            SetLightsParameters(intensity * m_Weight, range, color);
    }

    private void SetLightsParameters(float intensity, float range, Color color)
    {
        m_Lights.intensity = intensity;
        m_Lights.range = range;
        m_Lights.color = color;
    }


}
