using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCrossHairDisplayer : MonoBehaviour
{
    [SerializeField] private float m_Duration = 1;
    private CanvasGroup m_CanvasGroup;
    private Coroutine m_AttackCrosshairCoroutine;
    private WaitForSeconds m_DurationSeconds;

    private void Awake()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();
        m_DurationSeconds = new WaitForSeconds(m_Duration);
    }

    public void AttackCrossHairActive() 
    {
        if (m_AttackCrosshairCoroutine != null) StopCoroutine(m_AttackCrosshairCoroutine);
        m_AttackCrosshairCoroutine = StartCoroutine(AttackCrossHairCoroutine());
    }

    private IEnumerator AttackCrossHairCoroutine()
    {
        //float elapsedTime = 0;
        //while (elapsedTime < m_Duration)
        //{
        //    elapsedTime += Time.deltaTime;

        //    yield return null;
        //}
        m_CanvasGroup.alpha = 1;
        yield return m_DurationSeconds;
        m_CanvasGroup.alpha = 0;
    }
}
