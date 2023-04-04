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
    /// <param name="index">0 : 없음, 1: 점, 2: 십자선, 3 : 원형</param>
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
