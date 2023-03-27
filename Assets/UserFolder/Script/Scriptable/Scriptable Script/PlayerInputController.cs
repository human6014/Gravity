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

    private int m_GravityKeyInput = 1;  //중력 조절       Z,X,C             입력
    private int m_EquipmentKeyInput = 1;//아이템 선택     1,2,3,4,5         입력

    private float m_MouseScroll;        //중력 조절       마우스 스크롤     입력
    private float m_Horizontal;         //좌우            A,D               입력
    private float m_Vertical;           //상하            W,S               입력
    private float m_MouseX;             //화면 전환       마우스 좌,우      입력
    private float m_MouseY;             //화면 전환       마우스 위,아래    입력

    private bool m_Jump;                //점프            SpaceBar          입력
    private bool m_Reload;              //재장전          R                 입력
    private bool m_IsWalking;           //걷기,뛰기       LeftShift         입력
    private bool m_IsCrouch;            //앉기            LeftCtrl          입력
    private bool m_IsAiming;            //조준            마우스 우클릭     입력
    private bool m_IsFiring;            //사격            마우스 좌클릭     입력
    private bool m_Heal;                //회복            E                 입력
    private bool m_TimeSlow;            //시간 슬로우     F                 입력

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

        // FixedUpdate 에서 넘어옴
        m_Horizontal = Input.GetAxis("Horizontal");
        m_Vertical = Input.GetAxis("Vertical");

        m_IsWalking = !(Input.GetKey(KeyCode.LeftShift) && m_Vertical > 0);

        m_IsFiring = Input.GetKey(KeyCode.Mouse0);
        m_IsAiming = Input.GetKey(KeyCode.Mouse1);
        //

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
