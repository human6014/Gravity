using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable
{
    public enum CrossHair
    {
        None = 0,           //����
        Normal = 1,         //�߾���
        Reticle = 2,        //���ڼ�
        Circle = 3,         //������
    }

    [CreateAssetMenu(fileName = "WeaponStatSetting", menuName = "Scriptable Object/WeaponStatSettings", order = int.MaxValue - 11)]
    public class WeaponStatScriptable : ScriptableObject
    {
        [Header("Parent")]
        [Header("Attack info")]
        [Tooltip("���� ������")]
        public int m_Damage;

        [Tooltip("���ݿ� �ɸ� ���̾�")]
        public LayerMask m_AttackableLayer;

        [Header("Pos")]
        [Tooltip("�޸��� ��ȯ �ð�")]
        public float m_RunningPosTime = 0.5f;
        
        [Header("UI")]
        [Tooltip("ũ�ν� ��� UI ����")]
        public CrossHair m_DefaultCrossHair = 0;
    }
}
