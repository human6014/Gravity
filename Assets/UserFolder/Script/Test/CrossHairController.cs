using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scriptable;

public class CrossHairController : MonoBehaviour
{
    [SerializeField] private CrossHairScripatble[] crossHairInfo;

    private Image [] crossHairImage = new Image[5];

    public CrossHairScripatble GetCrossHairInfo(int index) => crossHairInfo[index];

    private void Awake()
    {
        crossHairImage = GetComponentsInChildren<Image>();
    }


    /// <summary>
    /// �ѱ� ������ ���� ũ�ν� �ؾ� ����
    /// </summary>
    /// <param name="index">0 : ���� ����, 1: �Ϲ����� ��, 2: ���Ƿ� ��, 3 : ����</param>
    public void SetCrossHair(int index)
    {
        for(int i = 0; i < crossHairImage.Length; i++)
        {
            crossHairImage[i].sprite = crossHairInfo[index].crossHairSprite[i];
        }
    }
}
