using System;
using UnityEngine;
using Manager;
using Manager.AI;
using Contoller.Player.Utility;
using System.Collections;

namespace Contoller.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        public MouseLook MouseLook { get => m_MouseLook;}

        #region SerializeField
        [Tooltip("�ȱ� �ӵ�")]
        [SerializeField] private float m_WalkSpeed;

        [Tooltip("�޸��� �ӵ�")]
        [SerializeField] private float m_RunSpeed;

        [Tooltip("���� �ӵ�")]
        [SerializeField] private float m_JumpSpeed;

        [Tooltip("���� �� �� ������ �ӵ� (Ŭ���� �� ���� ����)")]
        [SerializeField] [Range(0f, 2f)] private float m_WalkStepLenghten;

        [Tooltip("�޸� �� �� ������ �ӵ� (Ŭ���� �� ���� ����)")]
        [SerializeField] [Range(0f, 2f)] private float m_RunStepLenghten;


        [Tooltip("���� �޶�ٴ� ��")]
        [SerializeField] private float m_StickToGroundForce;

        [Tooltip("�߷� ��")]
        [SerializeField] private float m_GravityMultiplier;

        [Space(15)]
        [Tooltip("���콺 �Է� ���� Ŭ����")]
        [SerializeField] private MouseLook m_MouseLook;


        [Space(15)]
        [Tooltip("�޸� �� �þ�(FOV) ����")]
        [SerializeField] private bool m_UseFovKick;

        [Tooltip("�þ߰� ���� Ŭ����")]
        [SerializeField] private FOVKick m_FovKick;// = new FOVKick();


        [Space(15)]
        [Tooltip("������ �� ȭ�� ��鸲")]
        [SerializeField] private bool m_UseHeadBob;

        [Tooltip("�̵� �� ��鸲 Ŭ����")]
        [SerializeField] private CurveControlledBob m_HeadBob;// = new CurveControlledBob();

        [Space(15)]
        [Tooltip("���� �� ��鸲 Ŭ����")]
        [SerializeField] private LerpControlledBob m_JumpBob;// = new LerpControlledBob();

        [Tooltip("���� �� �� ���� ����ġ")]
        [SerializeField] private float m_StepInterval;


        [Space(15)]
        [Tooltip("������ ���� ����Ʈ")]
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.

        [Tooltip("���� ����")]
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.

        [Tooltip("���� ����")]
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

        [Space(15)]
        [Tooltip("�¿� ��� Transform(��ü)")]
        [SerializeField] private Transform m_RightAxisTransform;

        [Tooltip("���Ʒ� ��� Transform(ī�޶�)")]
        [SerializeField] private Transform m_UpAxisTransfrom;

        [Tooltip("�÷��̾� �ݶ��̴�")]
        [SerializeField] private CapsuleCollider m_CapsuleCollider;

        [Tooltip("�÷��̾� ���� ��ġ")]
        [SerializeField] private LayerMask reversePosLayer;

        [Tooltip("�÷��̾� ���� ���� ���̾�")]
        [SerializeField] private LayerMask m_GroundLayer;

        [Tooltip("�ɱ�, �Ͼ��")]
        [SerializeField] Vector3 m_CrouchInterporatePos;          //�ɱ�, �Ͼ�� ������
        [SerializeField] float m_CrouchTime = 0.3f;               //�ɱ�, �Ͼ�� �ڼ� ��ȯ �ð�

        #endregion
        private PlayerInputController m_PlayerInputController;
        private CharacterController m_CharacterController;
        private AudioSource m_AudioSource;
        private PlayerData m_PlayerData;
        private Camera m_Camera;

        private Vector3 m_IdlePos;                                //�Ͼ ���� ���� ��ġ
        private Vector3 m_CrouchPos;                              //���� ���� ��ġ
        private Vector3 m_MoveDir = Vector3.zero;
        private Vector3 m_OriginalCameraPosition;
        private Vector3 m_DesiredMove;
        private Vector2 m_Input;

        private readonly float m_InterporationDist = -0.1f;
        private float m_MovementSpeed;
        private float m_StepCycle;
        private float m_NextStep;

        private bool m_PreviouslyGrounded;  //���� �����ӿ��� �����̾�����
        private bool m_Jumping;             //�����ϰ� �ִ���
        private bool m_Jump;                //����Ű �Է� ����
        private bool m_IsGround;            //���� �����ӿ��� ��������
        private bool m_WasWalking;

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

            AIManager.PlayerTransform = transform;

            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_UpAxisTransfrom, m_StepInterval);
            m_MouseLook.Setup(m_RightAxisTransform, m_UpAxisTransfrom);

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
            m_PlayerInputController.MouseMovement += (float mouseHorizontal, float mouseVertical) 
                => m_MouseLook.LookRotation(mouseHorizontal, mouseVertical);

            m_PlayerInputController.PlayerMovement += TryMovement;
            m_PlayerInputController.Run += TryRun;
            m_PlayerInputController.Crouch += TryCrouch;
            m_PlayerInputController.Jump += TryJump;
        }

        private void Update()
        {
            if (!m_PreviouslyGrounded && m_IsGround)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                CustomGravityChange(false,0);
                m_Jumping = false;
                m_PlayerData.m_PlayerState.SetBehaviorJumping(false);
            }
            if (!m_IsGround && !m_Jumping && m_PreviouslyGrounded)
            {
                CustomGravityChange(false,0);
            }

            m_PreviouslyGrounded = m_IsGround;
        }

        private void OnDrawGizmos()
        {
            //Gizmos.color = Color.red;
            //float maxDistansce = m_CapsuleCollider.height * 0.5f - m_CapsuleCollider.radius + 0.1f;
            //Gizmos.DrawSphere(transform.position - transform.up * (m_CapsuleCollider.height * 0.5f - m_CapsuleCollider.radius) , m_CapsuleCollider.radius + m_InterporationDist);
        }

        
        private void FixedUpdate()
        {
            PositionRay();
            // always move along the camera forward as it is the direction that it being aimed at
            m_DesiredMove = m_RightAxisTransform.forward * m_Input.y + m_RightAxisTransform.right * m_Input.x;

            float maxDistansce = m_CapsuleCollider.height * 0.5f - m_CapsuleCollider.radius - 0.1f;
            float radius = m_CapsuleCollider.radius + m_InterporationDist + 0.1f;

            if (Physics.SphereCast(transform.position, radius, -transform.up, out RaycastHit hitInfo, maxDistansce, m_GroundLayer, QueryTriggerInteraction.Ignore))
                m_IsGround = true;
            else m_IsGround = false;

            m_DesiredMove = Vector3.ProjectOnPlane(m_DesiredMove, hitInfo.normal).normalized;

            CustomGravityChange(true, m_MovementSpeed);

            if (m_IsGround)
            {
                if (m_Jump)
                {
                    CustomGravityChange(false, m_JumpSpeed * -GravityManager.GravityDirectionValue);
                    m_PlayerData.m_PlayerState.SetBehaviorJumping(true);
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
                else CustomGravityChange(false, m_StickToGroundForce * GravityManager.GravityDirectionValue);
            }
            else m_MoveDir += Time.fixedDeltaTime * Physics.gravity;
            
            
            m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

            ProgressStepCycle(m_MovementSpeed);
            UpdateCameraPosition(m_MovementSpeed);
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
                        m_MoveDir.y = m_DesiredMove.y * value;
                        m_MoveDir.z = m_DesiredMove.z * value;
                    }
                    break;

                case EnumType.GravityType.yUp:
                case EnumType.GravityType.yDown:
                    if (!isDuple) m_MoveDir.y = value;
                    else
                    {
                        m_MoveDir.x = m_DesiredMove.x * value;
                        m_MoveDir.z = m_DesiredMove.z * value;
                    }
                    break;

                case EnumType.GravityType.zUp:
                case EnumType.GravityType.zDown:
                    if (!isDuple) m_MoveDir.z = value;
                    else
                    {
                        m_MoveDir.x = m_DesiredMove.x * value;
                        m_MoveDir.y = m_DesiredMove.y * value;
                    }
                    break;
            }
        }

        private void TryJump()
        {
            if (m_Jumping || !m_PlayerData.CanJumping()) return;
            m_Jump = true;
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

            int n = UnityEngine.Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);

            //// move picked sound to index 0 so it's not picked next time

            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }

        private void UpdateCameraPosition(float speed)
        {
            if (!m_UseHeadBob) return;
            Vector3 newCameraPosition = m_UpAxisTransfrom.localPosition;

            if (m_CharacterController.velocity.magnitude > 0 && m_IsGround && 
                m_PlayerData.m_PlayerState.PlayerWeaponState != PlayerWeaponState.Aiming)
            {
                m_UpAxisTransfrom.localPosition = m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed * (m_IsWalking ? m_WalkStepLenghten : m_RunStepLenghten)));
                newCameraPosition.y = m_UpAxisTransfrom.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_UpAxisTransfrom.localPosition = newCameraPosition;
        }
        
        private void TryMovement(float horizontal, float vertical)
        {
            m_MovementSpeed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            if (m_Input == Vector2.zero) m_PlayerData.m_PlayerState.SetBehaviorIdle();
            else m_PlayerData.m_PlayerState.SetBehaviorWalking();

            if (m_Input.sqrMagnitude > 1) m_Input.Normalize();

            if (m_IsWalking != m_WasWalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }

        private void TryRun(bool isRunning)
        {
            m_WasWalking = m_IsWalking;

            if (isRunning && !m_PlayerData.CanRunning())
            {
                m_IsWalking = true;
                return;
            }

            m_IsWalking = !isRunning;
            m_PlayerData.m_PlayerState.SetBehaviorRunning(isRunning);
        }

        private void PositionRay()
        {
            if (Physics.Raycast(transform.position, transform.up, out RaycastHit hit, 10, reversePosLayer))
                AIManager.PlayerRerversePosition = hit.point;
            else
                AIManager.PlayerRerversePosition = Vector3.zero;
        }

        private void TryCrouch(bool isCrouch)
        {
            m_IsCrouch = isCrouch;
            m_PlayerData.m_PlayerState.SetBehaviorCrouching(isCrouch);

            StopAllCoroutines();
            if (isCrouch)
                StartCoroutine(CrouchPos(m_CrouchTime, m_RightAxisTransform, m_CrouchPos));
            else
                StartCoroutine(CrouchPos(m_CrouchTime, m_RightAxisTransform, m_IdlePos));
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
