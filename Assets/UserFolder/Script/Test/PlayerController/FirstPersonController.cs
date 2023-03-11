using System;
using UnityEngine;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
using Manager;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
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
        [Tooltip("")]
        [SerializeField] private GravityRotation m_GravityRotation;
        #endregion

        private Camera m_Camera;
        private CharacterController m_CharacterController;
        private AudioSource m_AudioSource;
        private CollisionFlags m_CollisionFlags;
        private Vector3 m_MoveDir = Vector3.zero;
        private Vector3 m_OriginalCameraPosition;
        private Vector2 m_Input;

        private float m_YRotation;
        private float m_StepCycle;
        private float m_NextStep;

        private bool m_IsWalking;           //�Ȱ� �ִ���
        private bool m_PreviouslyGrounded;  //
        private bool m_Jumping;             //�����ϰ� �ִ���
        private bool m_Jump;                //����Ű �Է� ����

        private void Awake()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = GetComponentInChildren<Camera>();
            m_AudioSource = GetComponent<AudioSource>();

            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_MouseLook.Setup(transform, m_Camera.transform);

            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_Jumping = false;
        }


        private void Update()
        {
            RotateView();
            // the jump state needs to read here to make sure it is not missed

            if (!m_Jump) m_Jump = Input.GetButtonDown("Jump");
            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
                m_MoveDir.y = 0f;

            m_PreviouslyGrounded = m_CharacterController.isGrounded;

        }


        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + 0.5f;
        }


        private void FixedUpdate()
        {
            GetInput(out float speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

            // get a normal for the surface that is being touched to move along it
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out RaycastHit hitInfo,
                               m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x * speed;
            m_MoveDir.z = desiredMove.z * speed;

            if (m_CharacterController.isGrounded)
            {
                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
                else m_MoveDir.y = -m_StickToGroundForce;
            }
            else m_MoveDir += m_GravityMultiplier * Time.fixedDeltaTime  * Physics.gravity;
            
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();
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
            
            if (!(m_StepCycle > m_NextStep)) return;
            
            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded) return;
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
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition = m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed * (m_IsWalking ? m_WalkStepLenghten : m_RunStepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
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
            m_MouseLook.LookRotation(transform, m_Camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below) return;
            
            if (body == null || body.isKinematic) return;
            
            body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
