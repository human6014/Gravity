using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    private readonly KeyCode[] m_GravityChangeInput =
    {
            KeyCode.Z,
            KeyCode.X,
            KeyCode.C
    };

    private readonly KeyCode[] m_EquipmentChangeInput =
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5
    };

    private int m_GravityKeyInput = 1;  //�߷� ����       Z,X,C             �Է�
    private int m_EquipmentKeyInput = 1;//������ ����     1,2,3,4,5         �Է�

    private float m_MouseScroll;        //�߷� ����       ���콺 ��ũ��     �Է�
    private float m_Horizontal;         //�¿�            A,D               �Է�
    private float m_Vertical;           //����            W,S               �Է�
    private float m_MouseX;             //ȭ�� ��ȯ       ���콺 ��,��      �Է�
    private float m_MouseY;             //ȭ�� ��ȯ       ���콺 ��,�Ʒ�    �Է�

    private bool m_Jump;                //����            SpaceBar          �Է�
    private bool m_Reload;              //������          R                 �Է�
    private bool m_IsWalking;           //�ȱ�,�ٱ�       LeftShift         �Է�
    private bool m_IsCrouch;            //�ɱ�            LeftCtrl          �Է�
    private bool m_IsAiming;            //����            ���콺 ��Ŭ��     �Է�
    private bool m_IsFiring;            //���            ���콺 ��Ŭ��     �Է�
    private bool m_Heal;                //ȸ��            E                 �Է�
    private bool m_TimeSlow;            //�ð� ���ο�     F                 �Է�

    private void Update()
    {
        m_MouseX = Input.GetAxis("Mouse X");
        m_MouseY = Input.GetAxis("Mouse Y");

        m_MouseScroll = Input.GetAxis("Mouse ScrollWheel");

        for (int i = 0; i < m_GravityChangeInput.Length; i++)
        {
            if (Input.GetKeyDown(m_GravityChangeInput[i]))
            {
                m_GravityKeyInput = i;
                break;
            }
        }
        for(int i = 0; i < m_EquipmentChangeInput.Length; i++)
        {
            if (Input.GetKeyDown(m_EquipmentChangeInput[i]))
            {
                m_EquipmentKeyInput = i;
                break;
            }
        }

        if(!m_Jump) m_Jump = Input.GetButtonDown("Jump");
        m_Reload = Input.GetKeyDown(KeyCode.R);
        m_IsCrouch = Input.GetKeyDown(KeyCode.LeftControl);
        m_Heal = Input.GetKeyDown(KeyCode.E);
        m_TimeSlow = Input.GetKeyDown(KeyCode.F);

        // FixedUpdate ���� �Ѿ��
        m_Horizontal = Input.GetAxis("Horizontal");
        m_Vertical = Input.GetAxis("Vertical");

        m_IsWalking = !(Input.GetKey(KeyCode.LeftShift) && m_Vertical > 0);

        m_IsFiring = Input.GetKey(KeyCode.Mouse0);
        m_IsAiming = Input.GetKey(KeyCode.Mouse1);
        //

    }

    /*  //������ �ؿ��� �޴� �ڵ���
     *  //but ���� ����
    private void FixedUpdate()
    {
        m_Horizontal = Input.GetAxis("Horizontal");
        m_Vertical = Input.GetAxis("Vertical");

        m_IsWalking = !(Input.GetKey(KeyCode.LeftShift) && m_Vertical > 0);

        m_IsFiring = Input.GetKey(KeyCode.Mouse0);
        m_IsAiming = Input.GetKey(KeyCode.Mouse1);
    }
    */
}
