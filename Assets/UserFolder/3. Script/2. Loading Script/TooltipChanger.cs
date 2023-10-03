using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class TooltipChanger : MonoBehaviour
{
    [SerializeField] private GameObject[] m_Tooltips;
    [SerializeField] private float m_ChangeTimer = 4;

    private int m_CurrentIndex;
    private float m_TooltipTimer;

    private void Start()
    {
        m_CurrentIndex = Random.Range(0,m_Tooltips.Length);
        m_Tooltips[m_CurrentIndex].SetActive(true);
    }

    private void Update()
    {
        m_TooltipTimer += Time.deltaTime;
        if (m_TooltipTimer >= m_ChangeTimer)
        {
            m_TooltipTimer = 0;
            ChangeText();
        }
    }

    private void ChangeText()
    {
        IEnumerable<int> range = Enumerable.Range(0, m_Tooltips.Length).Where(i => i!= m_CurrentIndex);

        m_Tooltips[m_CurrentIndex].SetActive(false);
        m_CurrentIndex = range.ElementAt(Random.Range(0, m_Tooltips.Length - 1));
        m_Tooltips[m_CurrentIndex].SetActive(true);
    }
}
