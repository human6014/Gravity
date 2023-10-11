using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BackGroundImageChanger : MonoBehaviour
{
    [SerializeField] private Image m_FirstImage;
    [SerializeField] private Image m_SecondImage;

    [SerializeField] private Sprite[] m_BGSprites;
    [SerializeField] private float m_ImageDuration = 3;

    private readonly Color m_DisableColor = new Color(255,255,255,0);
    private readonly float m_FadeInOutTime = 1.5f;

    private bool m_IsActiveFirst;
    private float m_CurrentDuration;
    private int m_CurrentIndex;

    private void Start()
    {
        m_FirstImage.color = Color.white;
        m_SecondImage.color = m_DisableColor;

        m_CurrentIndex = Random.Range(0, m_BGSprites.Length);
        m_FirstImage.sprite = m_BGSprites[m_CurrentIndex];
        
        m_IsActiveFirst = true;
    }

    private void Update()
    {
        m_CurrentDuration += Time.deltaTime;
        if(m_CurrentDuration >= m_ImageDuration + m_FadeInOutTime)
        {
            m_CurrentDuration = 0;
            ChangeImage();
        }
    }

    private int GetNonDuplicateNumber()
    {
        IEnumerable<int> range = Enumerable.Range(0, m_BGSprites.Length).Where(i => i != m_CurrentIndex);
        int tempIndex = Random.Range(0, m_BGSprites.Length - 1);
        int index = range.ElementAt(tempIndex);
        return index;
    }

    private void ChangeImage()
    {
        m_CurrentIndex = GetNonDuplicateNumber();

        if (m_IsActiveFirst) m_SecondImage.sprite = m_BGSprites[m_CurrentIndex];
        else m_FirstImage.sprite = m_BGSprites[m_CurrentIndex];

        StartCoroutine(ChangeFade());
    }

    private IEnumerator ChangeFade()
    {
        float elapsedTime = 0;
        while (elapsedTime < m_FadeInOutTime)
        {
            elapsedTime += Time.deltaTime;
            if (m_IsActiveFirst) m_SecondImage.color = Color.Lerp(m_DisableColor, Color.white, elapsedTime / m_FadeInOutTime);
            else m_SecondImage.color = Color.Lerp(Color.white, m_DisableColor, elapsedTime / m_FadeInOutTime);

            yield return null;
        }

        m_SecondImage.color = m_IsActiveFirst ? Color.white : m_DisableColor;
        m_IsActiveFirst = !m_IsActiveFirst;
    }
}
