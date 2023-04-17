using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private UI.Manager.SettingUIManager m_SettingUIManager;

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

    private int m_GravityKeyInput = 1;  //중력 조절       Z,X,C             누르기
    private int m_EquipmentKeyInput = 1;//아이템 선택     1,2,3,4,5         누르기

    private float m_MouseScroll;        //중력 조절       마우스 스크롤     누르기
    private float m_Horizontal;         //좌우            A,D               꾹 누르기
    private float m_Vertical;           //상하            W,S               꾹 누르기
    private float m_MouseX;             //화면 전환       마우스 좌,우      꾹 누르기
    private float m_MouseY;             //화면 전환       마우스 위,아래    꾹 누르기

    private bool m_Jump;                //점프            SpaceBar          누르기
    private bool m_Reload;              //재장전          R                 누르기          원거리 무기 전용
    private bool m_IsCrouch;            //앉기            LeftCtrl          누르기
    private bool m_IsRunning;           //뛰기            LeftShift         꾹 누르기
    private bool m_IsAiming;            //조준            마우스 우클릭     꾹 누르기       원거리 무기 전용
    private bool m_IsAutoFiring;        //공격            마우스 좌클릭     꾹 누르기       원거리 무기 전용
    private bool m_IsSemiFiring;        //사격(단발, 점사)마우스 좌클릭     누르기
    private bool m_Heal;                //회복            E                 누르기
    private bool m_TimeSlow;            //시간 슬로우     F                 누르기
    private bool m_ChangeFireMode;      //사격 방식 변경  N                 누르기
    private bool m_IsHeavyFiring;       //강공격          마우스 우클릭     누르기          근접 무기 전용


    private bool m_WasCrouch;
    private bool m_WasTimeSlow;

    //keyDown Movement
    public Action<float, float> MouseMovement { get; set; }

    public Action<float, float> PlayerMovement { get; set; }


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
    public Action HeavyFire { get; set; }
    public Action<bool> Run { get; set; }             //Down

    public Action ChangeFireMode { get; set; }


    private void Update()
    {
        if (m_SettingUIManager.IsActiveSettingUI) return;

        m_MouseX = Input.GetAxis("Mouse X");
        m_MouseY = Input.GetAxis("Mouse Y");

        MouseMovement?.Invoke(m_MouseX, m_MouseY);

        GravityChangInput();
        EquipmentChangeInput();

        m_MouseScroll = Input.GetAxis("Mouse ScrollWheel");
        if (m_MouseScroll != 0) DoGravityChange?.Invoke(m_GravityKeyInput, m_MouseScroll);

        m_Jump = Input.GetKeyDown(KeyCode.Space);
        if (m_Jump) Jump?.Invoke();
        
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


        // FixedUpdate 에서 넘어옴

        m_IsRunning = Input.GetKey(KeyCode.LeftShift) && m_Vertical > 0;
        Run?.Invoke(m_IsRunning);

        m_Horizontal = Input.GetAxis("Horizontal");
        m_Vertical = Input.GetAxis("Vertical");
        PlayerMovement?.Invoke(m_Horizontal, m_Vertical);


        m_IsAutoFiring = Input.GetKey(KeyCode.Mouse0);
        if (m_IsAutoFiring) AutoFire?.Invoke();

        m_IsSemiFiring = Input.GetKeyDown(KeyCode.Mouse0);
        if (m_IsSemiFiring) SemiFire?.Invoke();

        m_IsAiming = Input.GetKey(KeyCode.Mouse1);
        Aiming?.Invoke(m_IsAiming);

        m_IsHeavyFiring = Input.GetKeyDown(KeyCode.Mouse1);
        if(m_IsHeavyFiring) HeavyFire?.Invoke();
        //
    }

    private void GravityChangInput()
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

    private void EquipmentChangeInput()
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

    /*  //원본은 밑에서 받는 코드임
     *  //but 위로 통합
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
