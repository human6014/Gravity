using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
using Manager.AI;
using Contoller.Player.Utility;

namespace Contoller.Player
{
    public class FirstPersonControllerTest : MonoBehaviour
    {
        public MouseLook MouseLook { get => m_MouseLook; }

        #region SerializeField
        [Tooltip("걷기 속도")]
        [SerializeField] private float m_WalkSpeed;

        [Tooltip("달리기 속도")]
        [SerializeField] private float m_RunSpeed;

        [Tooltip("점프 속도")]
        [SerializeField] private float m_JumpSpeed;

        [Tooltip("걸을 때 한 걸음의 속도 (클수록 더 많이 걸음)")]
        [SerializeField] [Range(0f, 2f)] private float m_WalkStepLenghten;

        [Tooltip("달릴 때 한 걸음의 속도 (클수록 더 많이 걸음)")]
        [SerializeField] [Range(0f, 2f)] private float m_RunStepLenghten;


        [Tooltip("땅에 달라붙는 힘")]
        [SerializeField] private float m_StickToGroundForce;

        [Tooltip("중력 값")]
        [SerializeField] private float m_GravityMultiplier;

        [Space(15)]
        [Tooltip("마우스 입력 감지 클래스")]
        [SerializeField] private MouseLook m_MouseLook;


        [Space(15)]
        [Tooltip("달릴 때 시야(FOV) 변경")]
        [SerializeField] private bool m_UseFovKick;

        [Tooltip("시야각 변경 클래스")]
        [SerializeField] private FOVKick m_FovKick;// = new FOVKick();


        [Space(15)]
        [Tooltip("움직일 때 화면 흔들림")]
        [SerializeField] private bool m_UseHeadBob;

        [Tooltip("이동 시 흔들림 클래스")]
        [SerializeField] private CurveControlledBob m_HeadBob;// = new CurveControlledBob();

        [Space(15)]
        [Tooltip("점프 시 흔들림 클래스")]
        [SerializeField] private LerpControlledBob m_JumpBob;// = new LerpControlledBob();

        [Tooltip("걸을 때 발 간격 보정치")]
        [SerializeField] private float m_StepInterval;


        [Space(15)]
        [Tooltip("무작위 사운드 리스트")]
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.

        [Tooltip("점프 사운드")]
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.

        [Tooltip("착지 사운드")]
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

        [Space(15)]
        [Tooltip("좌우 담당 Transform(몸체)")]
        [SerializeField] private Transform m_RightAxisTransform;

        [Tooltip("위아래 담당 Transform(카메라)")]
        [SerializeField] private Transform m_UpAxisTransfrom;

        [Tooltip("플레이어 콜라이더")]
        [SerializeField] private CapsuleCollider m_CapsuleCollider;

        [Tooltip("플레이어 위쪽 위치")]
        [SerializeField] private LayerMask reversePosLayer;

        [Tooltip("플레이어 지면 감지 레이어")]
        [SerializeField] private LayerMask m_GroundLayer;

        [Tooltip("앉기, 일어나기")]
        [SerializeField] Vector3 m_CrouchInterporatePos;          //앉기, 일어나기 증감값
        [SerializeField] float m_CrouchTime = 0.3f;               //앉기, 일어나기 자세 전환 시간

        [SerializeField] private float m_GroundDrag = 5f;
        [SerializeField] private float m_AirMultiplier = 0.5f;
        #endregion
        private PlayerInputController m_PlayerInputController;
        private CharacterController m_CharacterController;
        private AudioSource m_AudioSource;
        private PlayerData m_PlayerData;
        private Camera m_Camera;
        private Transform m_GrabTransform;
        private Rigidbody m_RigidBody;

        private Vector3 m_IdlePos;                                //일어난 상태 원래 위치
        private Vector3 m_CrouchPos;                              //앉은 상태 위치
        private Vector3 m_MoveDir = Vector3.zero;
        private Vector3 m_OriginalCameraPosition;
        private Vector3 m_DesiredMove;
        private Vector2 m_Input;

        private readonly float m_InterporationDist = -0.1f;
        private float m_MovementSpeed;
        private float m_StepCycle;
        private float m_NextStep;

        private bool m_CanJump;
        private bool m_PreviouslyGrounded;  //이전 프레임에서 지상이었는지
        private bool m_Jumping;             //점프하고 있는지
        private bool m_Jump;                //점프키 입력 감지
        private bool m_IsGround;            //현재 프레임에서 지상인지
        private bool m_WasGround;
        private bool m_WasWalking;
        private bool m_IsGrabed;

        public bool m_IsIdle { get; private set; }
        public bool m_IsCrouch { get; private set; }
        public bool m_IsWalking { get; private set; }

        private void Awake()
        {
            m_PlayerInputController = GetComponent<PlayerInputController>();
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = m_RightAxisTransform.GetComponentInChildren<Camera>();
            m_PlayerData = GetComponent<PlayerData>();
            m_AudioSource = GetComponent<AudioSource>();
            m_RigidBody = GetComponent<Rigidbody>();
            m_CapsuleCollider = GetComponent<CapsuleCollider>();

            AIManager.PlayerTransform = transform;

            //m_FovKick.Setup(m_Camera);
            //m_HeadBob.Setup(m_UpAxisTransfrom, m_StepInterval);
            m_MouseLook.Setup(m_RightAxisTransform, m_UpAxisTransfrom);
            //m_PlayerData.GrabAction += GrabAction;

            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_Jumping = false;

            AssignKeyAction();

            m_IdlePos = m_RightAxisTransform.localPosition;
            m_CrouchPos = m_RightAxisTransform.localPosition - m_CrouchInterporatePos;
        }

        private void AssignKeyAction()
        {
            m_PlayerInputController.PlayerMovement += TryMovement;
            m_PlayerInputController.Run += TryRun;
            m_PlayerInputController.Jump += TryJump;

            m_PlayerInputController.MouseMovement += (float mouseHorizontal, float mouseVertical)
                    => m_MouseLook.LookRotation(mouseHorizontal, mouseVertical);
        }

        private void Update()
        {
            m_WasGround = m_IsGround;
            m_IsGround = Physics.Raycast(transform.position, GravityManager.GravityVector, m_CapsuleCollider.height * 0.5f + 0.2f, m_GroundLayer);
            //SpeedControl();
            if (m_IsGround && !m_WasGround) ResetJump();

            //m_RigidBody.drag = m_IsGround ? m_GroundDrag : 0;
        }

        private void SpeedControl()
        {
            //Vector3 flatVelocity = new Vector3(m_RigidBody.velocity.x, 0, m_RigidBody.velocity.z);  //y
            Vector3 flatVelocity = new Vector3(0, m_RigidBody.velocity.y, m_RigidBody.velocity.z); //x
            //Vector3 flatVelocity = new Vector3(m_RigidBody.velocity.x, m_RigidBody.velocity.y, 0); //z
            //

            if (flatVelocity.magnitude > m_MovementSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * m_MovementSpeed;
                //m_RigidBody.velocity = new Vector3(limitedVelocity.x, m_RigidBody.velocity.y, limitedVelocity.z);
                m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, limitedVelocity.y, limitedVelocity.z);
                //m_RigidBody.velocity = new Vector3(limitedVelocity.x, limitedVelocity.y, m_RigidBody.velocity.z);
                //
            }
        }

        private Vector3 m_MoveDirection;
        private void TryMovement(float horizontal, float vertical)
        {
            m_MoveDirection = (m_RightAxisTransform.forward * vertical + m_RightAxisTransform.right * horizontal).normalized;
            float multiplier = m_IsGround ? 1 : m_AirMultiplier;
            m_RigidBody.MovePosition(transform.position + m_MovementSpeed * Time.deltaTime * m_MoveDirection);
            //m_RigidBody.AddForce(10 * multiplier * m_MovementSpeed * m_MoveDirection.normalized, ForceMode.Force);
        }

        private void TryRun(bool isRunning)
        {
            m_IsWalking = !isRunning;
            if (isRunning)
            {
                m_MovementSpeed = m_RunSpeed;
            }
            else
            {
                m_MovementSpeed = m_WalkSpeed;
            }
            
        }

        private void TryJump()
        {
            if (!m_CanJump || !m_IsGround) return;
            m_CanJump = false;
            //m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0, m_RigidBody.velocity.z);
            m_RigidBody.velocity = new Vector3(0, m_RigidBody.velocity.y, m_RigidBody.velocity.z);
            //m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, m_RigidBody.velocity.y, 0);
            //

            m_RigidBody.AddForce(transform.up * m_JumpSpeed, ForceMode.Impulse);
        }

        private void ResetJump()
        {
            m_CanJump = true;
        }

        private Vector3 GravityVelocity(float x, float y, float z)
        {
            Vector3 value = Vector3.zero;
            switch (GravityManager.m_CurrentGravityType)
            {
                case EnumType.GravityType.xDown:
                case EnumType.GravityType.xUp:
                    value = new Vector3(y, x, z);
                    break;

                case EnumType.GravityType.yDown:
                case EnumType.GravityType.yUp:
                    value = new Vector3(x, y, z);
                    break;

                case EnumType.GravityType.zDown:
                case EnumType.GravityType.zUp:
                    value = new Vector3(x, z, y);
                    break;
            }
            return value;
        }

        private void TryCrouch(bool isCrouch)
        {
            m_IsCrouch = isCrouch;
            m_PlayerData.PlayerState.SetBehaviorCrouching(isCrouch);

            Vector3 posture = isCrouch ? m_CrouchPos : m_IdlePos;
            StopAllCoroutines();
            StartCoroutine(CrouchPos(m_CrouchTime, m_RightAxisTransform, posture));
        }

        private IEnumerator CrouchPos(float runTotalTime, Transform target, Vector3 endLocalPosition)
        {
            float currentTime = 0;
            float elapsedTime;
            Vector3 startLocalPosition = target.localPosition;
            while (currentTime < runTotalTime)
            {
                currentTime += Time.deltaTime;

                elapsedTime = currentTime / runTotalTime;
                target.localPosition = Vector3.Lerp(startLocalPosition, endLocalPosition, elapsedTime);
                yield return elapsedTime;
            }
        }
    }
}
