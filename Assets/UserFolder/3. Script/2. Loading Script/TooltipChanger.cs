using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TooltipChanger : MonoBehaviour
{
    [SerializeField] private string[] m_Tooltips;

    private Text m_TooltipText;
    private int m_CurrentIndex;

    private readonly float m_ChangeTimer = 4;
    private float m_TooltipTimer;

    private void Awake() => m_TooltipText = GetComponent<Text>();

    private void Start() => ChangeText();

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
        m_CurrentIndex = range.ElementAt(Random.Range(0, m_Tooltips.Length - 1));
        m_TooltipText.text = m_Tooltips[m_CurrentIndex];
    }
}
