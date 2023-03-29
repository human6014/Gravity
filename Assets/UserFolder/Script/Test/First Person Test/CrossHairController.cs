using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scriptable;

public class CrossHairController : MonoBehaviour
{
    [SerializeField] private CrossHairScripatble[] crossHairInfo;

    private Image [] crossHairImage = new Image[5];
    private Color color;
    public CrossHairScripatble GetCrossHairInfo(int index) => crossHairInfo[index];

    private void Awake()
    {
        crossHairImage = GetComponentsInChildren<Image>();
    }


    /// <summary>
    /// 총기 종류에 따라 크로스 해어 설정
    /// </summary>
    /// <param name="index">0 : 근접 무기, 1: 일반적인 총, 2: 샷건류 총, 3 : 석궁</param>
    public void SetCrossHair(int index)
    {
        for(int i = 0; i < crossHairImage.Length; i++)
        {
            if (crossHairInfo[index].crossHairSprite[i] == null)
            {
                crossHairImage[i].enabled = false;
                continue;
            }
            crossHairImage[i].sprite = crossHairInfo[index].crossHairSprite[i];
            crossHairImage[i].enabled = true;
        }
    }
}
