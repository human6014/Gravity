using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Manager;
using Manager.AI;
using Contoller.Player.Utility;

namespace Contoller.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        public MouseLook M_MouseLook { get => m_MouseLook; private set => m_MouseLook = value; }

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
        [Tooltip("")]
        [SerializeField] private Transform m_MouseLookTransform;

        [Tooltip("")]
        [SerializeField] private Transform m_UpAxisTransfrom;

        [Tooltip("")]
        [SerializeField] private CapsuleCollider m_CapsuleCollider;

        [Tooltip("")]
        [SerializeField] private LayerMask reversePosLayer;
        #endregion

        private Camera m_Camera;
        private CharacterController m_CharacterController;
        private AudioSource m_AudioSource;
        private GravityRotation m_GravityRotation;
        private Vector3 m_MoveDir = Vector3.zero;
        private Vector3 m_OriginalCameraPosition;
        private Vector2 m_Input;

        private float m_StepCycle;
        private float m_NextStep;
        private readonly float m_InterporationDist = -0.1f;

        private bool m_IsWalking;           //걷고 있는지
        private bool m_PreviouslyGrounded;  //이전 프레임에서 지상이었는지
        private bool m_Jumping;             //점프하고 있는지
        private bool m_Jump;                //점프키 입력 감지
        private bool m_IsGround;            //현재 프레임에서 지상인지

        private int m_GravityKeyInput = 1;

        private readonly KeyCode[] m_GravityChangeInput =
        {
            KeyCode.Z,
            KeyCode.X,
            KeyCode.C
        };
        private void Awake()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = m_MouseLookTransform.GetComponentInChildren<Camera>();
            m_AudioSource = GetComponent<AudioSource>();
            m_GravityRotation = GetComponent<GravityRotation>();

            AIManager.PlayerTransfrom = transform;

            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_UpAxisTransfrom, m_StepInterval);
            m_MouseLook.Setup(m_MouseLookTransform, m_UpAxisTransfrom);

            M_MouseLook = m_MouseLook;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_Jumping = false;
        }


        private void Update()
        {
            RotateView();
            // the jump state needs to read here to make sure it is not missed
            float mouseScroll = Input.GetAxis("Mouse ScrollWheel");

            for (int i = 0; i < m_GravityChangeInput.Length; i++)
            {
                if (Input.GetKeyDown(m_GravityChangeInput[i]))
                {
                    m_GravityKeyInput = i;
                    break;
                }
            }
            if (mouseScroll != 0 && !GravityManager.IsGravityChanging) m_GravityRotation.GravityChange(m_GravityKeyInput, mouseScroll);

            if (!m_Jump) m_Jump = Input.GetButtonDown("Jump");
            if (!m_PreviouslyGrounded && m_IsGround)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                CustomGravityChange(false,0);
                //m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_IsGround && !m_Jumping && m_PreviouslyGrounded)
            {
                CustomGravityChange(false,0);
                //m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_IsGround;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            float maxDistansce = m_CapsuleCollider.height * 0.5f - m_CapsuleCollider.radius + 0.1f;
            Gizmos.DrawSphere(transform.position - transform.up * (m_CapsuleCollider.height * 0.5f - m_CapsuleCollider.radius) , m_CapsuleCollider.radius + m_InterporationDist);
        }

        private Vector3 desiredMove;
        private void FixedUpdate()
        {
            GetInput(out float speed);
            PositionRay();
            // always move along the camera forward as it is the direction that it being aimed at
            desiredMove = m_MouseLookTransform.forward * m_Input.y + m_MouseLookTransform.right * m_Input.x;

            float maxDistansce = m_CapsuleCollider.height * 0.5f - m_CapsuleCollider.radius - 0.1f;
            float radius = m_CapsuleCollider.radius + m_InterporationDist + 0.1f;

            if (Physics.SphereCast(transform.position, radius, -transform.up, out RaycastHit hitInfo, maxDistansce, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                m_IsGround = true;
            else m_IsGround = false;

            /*
            float maxDistansce = m_CapsuleCollider.height * 0.5f - m_CapsuleCollider.radius;
            if (Physics.SphereCast(transform.position, m_CapsuleCollider.radius + m_InterporationDist, -transform.up, out RaycastHit hitInfo, maxDistansce, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                m_isGround = true;
            else m_isGround = false;
            */

            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            CustomGravityChange(true, speed);
            //m_MoveDir.x = desiredMove.x * speed;
            //m_MoveDir.z = desiredMove.z * speed;

            if (m_IsGround)
            {
                if (m_Jump)
                {
                    CustomGravityChange(false, m_JumpSpeed * -GravityManager.GravityDirectionValue);
                    PlayJumpSound();

                    m_Jump = false;
                    m_Jumping = true;
                }
                else CustomGravityChange(false, m_StickToGroundForce * GravityManager.GravityDirectionValue);
            }
            else m_MoveDir += Time.fixedDeltaTime * Physics.gravity;
            
            
            m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();
        }

        private void CustomGravityChange(bool isDuple, float value)
        {
            switch (GravityManager.currentGravityType)
            {
                case EnumType.GravityType.xUp:
                case EnumType.GravityType.xDown:
                    if(!isDuple) m_MoveDir.x = value;
                    else
                    {
                        m_MoveDir.y = desiredMove.y * value;
                        m_MoveDir.z = desiredMove.z * value;
                    }
                    break;

                case EnumType.GravityType.yUp:
                case EnumType.GravityType.yDown:
                    if (!isDuple) m_MoveDir.y = value;
                    else
                    {
                        m_MoveDir.x = desiredMove.x * value;
                        m_MoveDir.z = desiredMove.z * value;
                    }
                    break;

                case EnumType.GravityType.zUp:
                case EnumType.GravityType.zDown:
                    if (!isDuple) m_MoveDir.z = value;
                    else
                    {
                        m_MoveDir.x = desiredMove.x * value;
                        m_MoveDir.y = desiredMove.y * value;
                    }
                    break;
            }
        }

        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + 0.5f;
        }

        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }

        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? m_WalkStepLenghten : m_RunStepLenghten))) * Time.fixedDeltaTime;
            
            if (m_StepCycle <= m_NextStep) return;
            
            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }

        private void PlayFootStepAudio()
        {
            if (!m_IsGround) return;
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0

            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);

            //// move picked sound to index 0 so it's not picked next time

            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }

        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob) return;
            if (m_CharacterController.velocity.magnitude > 0 && m_IsGround)
            {
                m_UpAxisTransfrom.localPosition = m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed * (m_IsWalking ? m_WalkStepLenghten : m_RunStepLenghten)));
                newCameraPosition = m_UpAxisTransfrom.localPosition;
                newCameraPosition.y = m_UpAxisTransfrom.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_UpAxisTransfrom.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_UpAxisTransfrom.localPosition = newCameraPosition;
        }

        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            bool wasWalking = m_IsWalking;

            m_IsWalking = !(Input.GetKey(KeyCode.LeftShift) && vertical > 0);

            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)  m_Input.Normalize();
            
            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != wasWalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }

        private void RotateView()
        {
            m_MouseLook.LookRotation(m_MouseLookTransform, m_UpAxisTransfrom);
        }

        private void PositionRay()
        {
            if (Physics.Raycast(transform.position, transform.up, out RaycastHit hit, 10, reversePosLayer))
                AIManager.PlayerRerversePosition = hit.point;
            else
                AIManager.PlayerRerversePosition = Vector3.zero;
        }
    }
}
