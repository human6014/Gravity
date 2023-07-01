using Manager;
using Scriptable.Equipment;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Entity.Object.Weapon
{
    public class MeleeWeapon : Weapon
    {
        [Space(15)]
        [Header("Child")]
        [SerializeField] private float m_SwingRadius;
        [SerializeField] private float m_MaxDistance;
        [SerializeField] private bool m_CanComboAttack;

        private MeleeWeaponSoundScripatble m_MeleeWeaponSound;
        private MeleeWeaponStatScriptable m_MeleeWeaponStat;

        private ObjectPoolManager.PoolingObject[] m_EffectPoolingObject;
        private SurfaceManager m_SurfaceManager;
        private Transform m_CameraTransform;
        private Coroutine m_RunningCoroutine;
        private Attackable m_Attackable;

        private Quaternion m_RunningPivotRotation;
        private float m_CurrentFireTime;
        private int m_SwingIndex;
        private bool m_IsLightAttacking;
        private bool m_IsHeavyAttacking;
        private bool m_IsAttacking;
        private bool m_IsRunning;

        public override bool CanChangeWeapon => base.CanChangeWeapon && !m_IsAttacking;
        private bool CanComboAttacking() => m_CanComboAttack && (m_IsLightAttacking && !m_IsHeavyAttacking);


        public override void PreAwake()
        {
            base.PreAwake();
            m_MeleeWeaponStat = (MeleeWeaponStatScriptable)base.m_WeaponStatScriptable;
        }

        protected override void Awake()
        {
            base.Awake();

            m_MeleeWeaponSound = (MeleeWeaponSoundScripatble)base.m_WeaponSoundScriptable;

            m_Attackable = GetComponent<Attackable>();
            m_SurfaceManager = FindObjectOfType<SurfaceManager>();
            m_CameraTransform = MainCamera.transform;

            m_RunningPivotRotation = Quaternion.Euler(m_MeleeWeaponStat.m_RunningPivotDirection);
        }

        private void Start() 
            => m_Attackable.Setup(m_MeleeWeaponStat, WeaponManager.EffectPoolingObjectArray, MainCamera.transform);
        

        private void AssignPoolingObject()
            => m_EffectPoolingObject = WeaponManager.EffectPoolingObjectArray;


        protected override void AssignKeyAction()
        {
            base.AssignKeyAction();
            PlayerInputController.SemiFire += TryLightAttack;
            PlayerInputController.HeavyFire += TryHeavyAttack;
        }

        private void Update()
        {
            m_CurrentFireTime += Time.deltaTime;
            if (PlayerData.m_PlayerState.PlayerBehaviorState == PlayerBehaviorState.Running)
            {
                if (!m_IsRunning)
                {
                    m_IsRunning = true;
                    if (m_RunningCoroutine != null) StopCoroutine(m_RunningCoroutine);
                    m_RunningCoroutine = StartCoroutine(PosChange(m_MeleeWeaponStat.m_RunningPivotPosition, m_RunningPivotRotation));
                }
            }
            else
            {
                if (m_IsRunning)
                {
                    m_IsRunning = false;
                    if (m_RunningCoroutine != null) StopCoroutine(m_RunningCoroutine);
                    m_RunningCoroutine = StartCoroutine(PosChange(WeaponManager.OriginalPivotPosition, WeaponManager.OriginalPivotRotation));
                }
                MainCamera.fieldOfView = Mathf.Lerp(MainCamera.fieldOfView, WeaponManager.OriginalFOV, m_MeleeWeaponStat.m_FOVMultiplier * Time.deltaTime);
            }
        }

        private IEnumerator PosChange(Vector3 EndPosition, Quaternion EndRotation)
        {
            float currentTime = 0;
            float elapsedTime;
            Vector3 startLocalPosition = m_Pivot.localPosition;
            Quaternion startLocalRotation = m_Pivot.localRotation;
            while (currentTime < m_MeleeWeaponStat.m_RunningPosTime)
            {
                currentTime += Time.deltaTime;

                elapsedTime = currentTime / m_MeleeWeaponStat.m_RunningPosTime;
                m_Pivot.localPosition = Vector3.Lerp(startLocalPosition, EndPosition, elapsedTime);
                m_Pivot.localRotation = Quaternion.Lerp(startLocalRotation, EndRotation, elapsedTime);

                yield return elapsedTime;
            }
        }

        private void TryLightAttack()
        {
            if (m_CurrentFireTime > m_MeleeWeaponStat.m_LightFireTime)
            {
                m_CurrentFireTime = 0;
                m_IsLightAttacking = true;
                Attack(0);
            }
        }

        private void TryHeavyAttack()
        {
            if ((m_CurrentFireTime > m_MeleeWeaponStat.m_HeavyFireTime) || CanComboAttacking())
            {
                m_CurrentFireTime = 0;
                m_IsHeavyAttacking = true;
                Attack(1);
            }
        }

        private void Attack(int swingIndex)
        {
            m_SwingIndex = swingIndex;
            m_ArmAnimator.SetFloat("Swing Index", swingIndex);
            EquipmentAnimator.SetFloat("Swing Index", swingIndex);

            m_ArmAnimator.SetTrigger("Swing");
            EquipmentAnimator.SetTrigger("Swing");
        }

        #region Animation Event
        private void StartAnimation()
        {
            m_IsAttacking = true;

            AudioClip[] playingAudio = m_SwingIndex == 1 ? m_MeleeWeaponSound.m_HeavyAttackSound : m_MeleeWeaponSound.m_LightAttackSound;

            AudioSource.PlayOneShot(playingAudio[Random.Range(0, playingAudio.Length)]);
            if (m_Attackable.SwingCast()) PlayerData.HitEnemy();
        }

        private void EndAnimation()
        {
            m_IsLightAttacking = false;
            m_IsHeavyAttacking = false;
            m_IsAttacking = false;
        }
        #endregion

        protected override void DischargeKeyAction()
        {
            base.DischargeKeyAction();
            PlayerInputController.SemiFire -= TryLightAttack;
            PlayerInputController.HeavyFire -= TryHeavyAttack;
        }
    }
}
