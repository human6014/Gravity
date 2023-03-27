using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private SettingUIManager m_SettingUIManager;

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
    private bool m_IsRunning;           //�ٱ�            LeftShift         �Է�
    private bool m_IsCrouch;            //�ɱ�            LeftCtrl          �Է�
    private bool m_IsAiming;            //����            ���콺 ��Ŭ��     �Է�
    private bool m_IsAutoFiring;            //���            ���콺 ��Ŭ��     �Է�
    private bool m_IsSemiFiring;        //���(�ܹ�, ����)���콺 ��Ŭ��     �Է�
    private bool m_Heal;                //ȸ��            E                 �Է�
    private bool m_TimeSlow;            //�ð� ���ο�     F                 �Է�
    private bool m_ChangeFireMode;  //��� ��� ����  N                �Է�

    private bool m_WasCrouch;
    private bool m_WasTimeSlow;
    //keyDown Movement
    public Action<float,float> MouseInput { get; set; }


    public Action<float> PlayerHorizontal { get; set; }
    public Action<float> PlayerVertical { get; set; }


    //Scroll
    public Action<int, float> DoGravityChange { get; set; } //

    public Action<int> ChangeEquipment { get; set; }
    

    public Action<bool> Crouch { get; set; }          //Toggle
    public Action<bool> TimeSlow { get; set; }        //Toggle


    //keyDown
    public Action Reload { get; set; }          //Trigger
    public Action Jump { get; set; }            //Trigger
    public Action Heal { get; set; }            //Trigger


    //key
    public Action<bool> Aiming { get; set; }          //Down
    public Action AutoFire { get; set; }        //Down, + @
    public Action SemiFire { get; set; }
    public Action Run { get; set; }             //Down

    public Action ChangeFireMode { get; set; }


    private void Update()
    {
        //if (!Input.anyKey) return;
        if (m_SettingUIManager.m_IsActiveSettingUI) return;

        m_MouseX = Input.GetAxis("Mouse X");
        m_MouseY = Input.GetAxis("Mouse Y");

        MouseInput?.Invoke(m_MouseX, m_MouseY);

        GravityChangInput();
        EquipmentChangeInput();

        m_MouseScroll = Input.GetAxis("Mouse ScrollWheel");
        if (m_MouseScroll != 0) DoGravityChange?.Invoke(m_GravityKeyInput, m_MouseScroll);

        if (!m_Jump)
        {
            m_Jump = Input.GetKeyDown(KeyCode.Space);
            if (m_Jump) Jump?.Invoke();
        }

        m_Reload = Input.GetKeyDown(KeyCode.R);
        if (m_Reload) Reload?.Invoke();

        m_Heal = Input.GetKeyDown(KeyCode.E);
        if (m_Heal) Heal?.Invoke();

        m_ChangeFireMode = Input.GetKeyDown(KeyCode.N);
        if (m_ChangeFireMode) ChangeFireMode?.Invoke();

        m_IsCrouch = Input.GetKeyDown(KeyCode.LeftControl);
        if (m_IsCrouch)
        {
            m_WasCrouch = !m_WasCrouch;
            Crouch?.Invoke(m_WasCrouch);
        }

        m_TimeSlow = Input.GetKeyDown(KeyCode.F);
        if (m_TimeSlow)
        {
            m_WasTimeSlow = !m_WasTimeSlow;
            TimeSlow?.Invoke(m_WasTimeSlow);
        }

        // FixedUpdate ���� �Ѿ��
        m_Horizontal = Input.GetAxis("Horizontal");
        if(m_Horizontal != 0) PlayerHorizontal?.Invoke(m_Horizontal);
        
        m_Vertical = Input.GetAxis("Vertical");
        if(m_Vertical != 0) PlayerVertical?.Invoke(m_Vertical);
        
        m_IsRunning = Input.GetKey(KeyCode.LeftShift) && m_Vertical > 0;
        if (m_IsRunning) Run?.Invoke();
        
        m_IsAutoFiring = Input.GetKey(KeyCode.Mouse0);
        if (m_IsAutoFiring) AutoFire?.Invoke();

        m_IsSemiFiring = Input.GetKeyDown(KeyCode.Mouse0);
        if (m_IsSemiFiring) SemiFire?.Invoke();

        m_IsAiming = Input.GetKey(KeyCode.Mouse1);
        Aiming?.Invoke(m_IsAiming);
        //
    }

    public void GravityChangInput()
    {
        for (int i = 0; i < m_GravityChangeInput.Length; i++)
        {
            if (Input.GetKeyDown(m_GravityChangeInput[i]))
            {
                m_GravityKeyInput = i;
                return;
            }
        }
    }

    public void EquipmentChangeInput()
    {
        for (int i = 0; i < m_EquipmentChangeInput.Length; i++)
        {
            if (Input.GetKeyDown(m_EquipmentChangeInput[i]))
            {
                m_EquipmentKeyInput = i;
                ChangeEquipment?.Invoke(m_EquipmentKeyInput);
                return;
            }
        }
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
