using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    private Image screenImage;
    [SerializeField] private float fadeInTime = 3;
    private void Awake() => screenImage = GetComponent<Image>();
    
    private void Start()
    {
        Color color = screenImage.color;
        color.a = 1;
        screenImage.color = color;
        
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float currentTime =0 ;
        float t;
        Color cureentColor = screenImage.color;
        Color newColor = screenImage.color;
        newColor.a = 0;
        while (currentTime < fadeInTime)
        {
            currentTime += Time.deltaTime;

            t = currentTime / fadeInTime;
            screenImage.color = Color.Lerp(cureentColor, newColor,t);

            yield return t;
        }
    }
}
