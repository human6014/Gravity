using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scriptable;

public class CrossHairController : MonoBehaviour
{
    [SerializeField] private AnimatorOverrideController m_AnimatorController;
    [SerializeField] private CrossHairScripatble[] crossHairInfo;

    private Animator m_Animator;
    private Image [] crossHairImage = new Image[5];
    private Color color;
    public CrossHairScripatble GetCrossHairInfo(int index) => crossHairInfo[index];

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        crossHairImage = GetComponentsInChildren<Image>();
    }


    /// <summary>
    /// �ѱ� ������ ���� ũ�ν� �ؾ� ����
    /// </summary>
    /// <param name="index">0 : ����, 1: ��, 2: ���ڼ�, 3 : ����</param>
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

    //temp
    public void CrossHairSetTrigger(string state)
    {
        m_Animator.SetTrigger(state);   
    }

    public void CrossHairSetBool(string state, bool active)
    {
        m_Animator.SetBool(state, active);
    }
}
