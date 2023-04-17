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

    public class WeaponStatScriptable : ScriptableObject
    {
        [Header("Parent")]
        [Header("Attack info")]
        [Tooltip("�Ϲ� ���� �ӵ�")]
        public float m_AttackTime = 0.15f;
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


        [Header("RunningAnimPos")]
        [Tooltip("�޸� �� �Ǻ� ��ġ")]
        public Vector3 m_RunningPivotPosition;

        [Tooltip("�޸� �� �Ǻ� ����")]
        public Vector3 m_RunningPivotDirection;

        [Header("")]
        public float m_FOVMultiplier = 4;
    }
}
