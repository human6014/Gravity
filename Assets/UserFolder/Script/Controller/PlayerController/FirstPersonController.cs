using System;
using UnityEngine;
using Manager;
using Manager.AI;
using Contoller.Player.Utility;
using System.Collections;

namespace Contoller.Player
{
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
        [SerializeField] [Range(0f, 3f)] private float m_WalkStepLenghten = 1.2f;

        [Tooltip("�޸� �� �� ������ �ӵ� (Ŭ���� �� ���� ����)")]
        [SerializeField] [Range(0f, 3f)] private float m_RunStepLenghten = 2f;


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
        private AudioSource m_AudioSource;
        private PlayerData m_PlayerData;
        private Camera m_Camera;
        private Rigidbody m_RigidBody;

        private Transform m_GrabCameraPosition;
        private Transform m_GrabBodyPosition;
        private Transform m_GrabRotation;
        private Transform m_ThrowingPosition;

        private Vector3 m_IdlePos;                                //�Ͼ ���� ���� ��ġ
        private Vector3 m_CrouchPos;                              //���� ���� ��ġ
        private Vector3 m_MoveDir;
        private Vector3 m_OriginalCameraPosition;
        private Vector3 m_DesiredMove;
        private Vector2 m_Input;

        private readonly float m_InterporationDist = 0.3f;
        private float m_MovementSpeed;
        private float m_StepCycle;
        private float m_NextStep;

        private bool m_IsGrabed;
        private bool m_IsThrowing;
        private bool m_IsJumping;             //�����ϰ� �ִ���
        private bool m_IsMoving;

        private bool m_WasGround;           //���� �����ӿ��� �����̾�����
        private bool m_IsGround;            //���� �����ӿ��� ��������

        private bool m_WasWalking;
        private bool m_IsWalking;

        private void Awake()
        {
            m_PlayerInputController = GetComponent<PlayerInputController>();
            m_Camera = m_RightAxisTransform.GetComponentInChildren<Camera>();
            m_PlayerData = GetComponent<PlayerData>();
            m_AudioSource = GetComponent<AudioSource>();
            m_RigidBody = GetComponent<Rigidbody>();

            AIManager.PlayerTransform = transform;

            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_UpAxisTransfrom, m_StepInterval);
            m_MouseLook.Setup(m_RightAxisTransform, m_UpAxisTransfrom);

            m_PlayerData.GrabAction += GrabAction;
            m_PlayerData.GrabPoint += GrabActionPoint;

            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_IsJumping = false;

            AssignKeyAction();

            m_IdlePos = m_RightAxisTransform.localPosition;
            m_CrouchPos = m_RightAxisTransform.localPosition - m_CrouchInterporatePos;
        }


        private void GrabActionPoint(Transform grabCameraPosition,Transform grabBodyPosition, Transform grabRotation, Transform throwingPosition)
        {
            m_GrabCameraPosition = grabCameraPosition;
            m_GrabBodyPosition = grabBodyPosition;
            m_GrabRotation = grabRotation;
            m_ThrowingPosition = throwingPosition;
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
            if (m_IsGrabed)
            {
                transform.position = m_GrabCameraPosition.position;
                m_MouseLook.LookRotation(m_GrabRotation, m_Camera.transform);
                return;
            }
            if (m_IsGround && !m_WasGround)
            {
                if (m_IsJumping)
                {
                    ApplyToGravity(false, 0);
                    m_IsJumping = false;
                    m_PlayerData.m_PlayerState.SetBehaviorJumping(false);
                }
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
            }

            if (!m_IsGround && !m_IsJumping && m_WasGround) ApplyToGravity(false, 0);
            
            m_WasGround = m_IsGround;

            PositionRay();
        }

        private void GrabAction(bool isActive)
        {
            m_IsGrabed = isActive;
            if (!isActive) PlayerThrowing();
        }

        private void PlayerThrowing()
        {
            transform.position = m_GrabBodyPosition.position;
            m_Camera.transform.localRotation = Quaternion.identity;
            m_IsThrowing = true;
            Vector3 throwingVector = (m_GrabCameraPosition.position - m_ThrowingPosition.position).normalized;
            m_RigidBody.AddForce(throwingVector * 50, ForceMode.Impulse);
            m_RigidBody.useGravity = true;
        }

        private void FixedUpdate()
        {
            if (m_IsGrabed) return;
            m_IsGround = GroundCheck(out RaycastHit hitInfo);
            AIManager.PlayerIsGround = m_IsGround;

            if (m_IsThrowing)
            {
                if (m_IsGround)
                {
                    m_RigidBody.useGravity = false;
                    m_IsThrowing = false;
                }
                return;
            }

            if (m_IsGround)
            {
                if (!m_IsJumping)
                    ApplyToGravity(false, m_StickToGroundForce * GravityManager.GravityDirectionValue);
            }
            else m_MoveDir += Physics.gravity * Time.fixedDeltaTime;
            

            m_DesiredMove = m_RightAxisTransform.forward * m_Input.y + m_RightAxisTransform.right * m_Input.x;
            m_DesiredMove = Vector3.ProjectOnPlane(m_DesiredMove, hitInfo.normal).normalized;

            ApplyToGravity(true, m_MovementSpeed);

            m_RigidBody.velocity = m_MoveDir;

            ProgressStepCycle(m_MovementSpeed);
            UpdateCameraPosition(m_MovementSpeed);
        }

        private bool GroundCheck(out RaycastHit hitInfo)
           => Physics.Raycast(transform.position, GravityManager.GravityVector, out hitInfo, m_CapsuleCollider.height * 0.5f + m_InterporationDist, m_GroundLayer);

        private void ApplyToGravity(bool isDuple, float value)
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
            if (!m_IsGround || m_IsJumping || !m_PlayerData.CanJumping()) return;

            ApplyToGravity(false, m_JumpSpeed * -GravityManager.GravityDirectionValue);
            m_PlayerData.m_PlayerState.SetBehaviorJumping(true);
            PlayJumpSound();

            m_IsJumping = true;
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
            if (m_IsMoving)
                m_StepCycle += (m_RigidBody.velocity.magnitude + (speed * (m_IsWalking ? m_WalkStepLenghten : m_RunStepLenghten))) * Time.fixedDeltaTime;
            
            if (m_StepCycle <= m_NextStep) return;
            
            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }

        private void PlayFootStepAudio()
        {
            if (!m_IsGround) return;

            int n = UnityEngine.Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);

            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }

        private void UpdateCameraPosition(float speed)
        {
            if (!m_UseHeadBob) return;
            Vector3 newCameraPosition = m_UpAxisTransfrom.localPosition;

            if (m_IsMoving && m_IsGround && 
                m_PlayerData.m_PlayerState.PlayerWeaponState != PlayerWeaponState.Aiming)
            {
                m_UpAxisTransfrom.localPosition = m_HeadBob.DoHeadBob(m_RigidBody.velocity.magnitude +
                                      (speed * (m_IsWalking ? m_WalkStepLenghten : m_RunStepLenghten)));
                newCameraPosition.y = m_UpAxisTransfrom.localPosition.y - m_JumpBob.Offset();
            }
            else newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            
            m_UpAxisTransfrom.localPosition = newCameraPosition;
        }
        
        private void TryMovement(float horizontal, float vertical)
        {
            m_MovementSpeed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            if (m_Input == Vector2.zero) m_PlayerData.m_PlayerState.SetBehaviorIdle();
            else m_PlayerData.m_PlayerState.SetBehaviorWalking();
            m_IsMoving = m_Input != Vector2.zero;

            if (m_Input.sqrMagnitude > 1) m_Input.Normalize();

            if (m_IsWalking != m_WasWalking && m_UseFovKick && m_IsMoving)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }

        private void TryRun(bool isRunning)
        {
            m_WasWalking = m_IsWalking;

            bool canRunning = true;
            if (m_IsWalking) canRunning = m_PlayerData.CanStartRunning();

            if (canRunning && isRunning && m_PlayerData.CanRunning()) m_IsWalking = false;
            else m_IsWalking = true;

            m_PlayerData.m_PlayerState.SetBehaviorRunning(!m_IsWalking);
        }

        private void PositionRay()
        {
            bool isHitReverseGround = Physics.Raycast(transform.position, transform.up, out RaycastHit hit, 10, reversePosLayer);
            AIManager.PlayerRerversePosition = isHitReverseGround ? hit.point : Vector3.zero;
        }

        #region Crouch
        private void TryCrouch(bool isCrouch)
        {
            m_PlayerData.m_PlayerState.SetBehaviorCrouching(isCrouch);

            Vector3 posture = isCrouch ? m_CrouchPos : m_IdlePos;
            StopAllCoroutines();
            StartCoroutine(CrouchPos(m_CrouchTime, m_RightAxisTransform, posture));
        }

        private IEnumerator CrouchPos(float runTotalTime, Transform target, Vector3 endLocalPosition)
        {
            float currentTime = 0;
            Vector3 startLocalPosition = target.localPosition;
            while (currentTime < runTotalTime)
            {
                currentTime += Time.deltaTime;

                target.localPosition = Vector3.Lerp(startLocalPosition, endLocalPosition, currentTime / runTotalTime);
                yield return null;
            }
        }
        #endregion
    }
}
