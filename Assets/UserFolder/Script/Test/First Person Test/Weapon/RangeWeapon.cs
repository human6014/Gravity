using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scriptable;
using Manager;
using Entity.Object;

namespace Test
{
    public class RangeWeapon : Weapon
    {
        #region SerializeField
        [Space(15)]
        [Header("Child")]

        [Header("Fire recoil")]
        //총 반동
        [SerializeField] private Transform m_UpAxisTransform;         //상하 반동 오브젝트
        [SerializeField] private Transform m_RightAxisTransform;      //좌우 반동 오브젝트

        [Header("Reload")]
        [Tooltip("쏘고 바로 장전인지")]
        [SerializeField] private bool m_IsInstantReload;

        [Header("Fire mode")]
        [CustomAttribute.MultiEnum] [SerializeField] private FireMode m_FireMode;

        [SerializeField] private bool m_IsEmpty; //Reload Test;
        #endregion

        private Quaternion m_AimingPivotRotation;           //위치 조정용 옮길 각도(Quaternion)
        private Quaternion m_RunningPivotRotation;          //위치 조정용 옮길 각도

        private bool m_IsFiring;
        private bool m_IsRunning;
        private float m_AimingFOV;

        private WaitForSeconds m_BurstFireTime;
        private Coroutine m_RunningCoroutine;

        private Fireable m_Fireable;
        private Reloadable m_Reloadable;

        private RangeWeaponStatScriptable m_RangeWeaponStat;
        private RangeWeaponSoundScriptable m_RangeWeaponSound;

        public override bool CanChangeWeapon => base.CanChangeWeapon && !m_IsFiring && !IsReloading;
        [System.Flags]
        private enum FireMode
        {
            Auto = 1,
            Burst = 2,
            Semi= 4,
        }
        
        private FireMode m_CurrentFireMode = FireMode.Auto;
        private int m_FireModeLength;
        private int m_FireModeIndex = 1;

        private float m_CurrentFireTime;
        private float m_CurrentPosTime;

        private bool IsReloading => m_Reloadable.m_IsReloading; 

        protected override void Awake()
        {
            base.Awake();

            m_RangeWeaponStat = (RangeWeaponStatScriptable)base.m_WeaponStatScriptable;
            m_RangeWeaponSound = (RangeWeaponSoundScriptable)base.m_WeaponSoundScriptable;
            m_BurstFireTime = new WaitForSeconds(m_RangeWeaponStat.m_BurstAttackTime);

            m_Fireable = GetComponent<Fireable>();
            m_Reloadable = GetComponent<Reloadable>();

            m_AimingPivotRotation = Quaternion.Euler(m_RangeWeaponStat.m_AimingPivotDirection);
            m_RunningPivotRotation = Quaternion.Euler(m_RangeWeaponStat.m_RunningPivotDirection);
            m_AimingFOV = m_WeaponManager.m_OriginalFOV - m_RangeWeaponStat.m_AimingFOV;

            AssignFireMode();
        }

        private void Start()
        {
            m_Fireable.Setup(m_RangeWeaponStat, m_WeaponManager.m_EffectPoolingObjectArray, m_PlayerState, m_MainCamera);
            m_Reloadable.Setup(m_RangeWeaponSound, m_ArmAnimator);
        }

        #region Assign
        private void AssignFireMode()
        {
            if (m_FireMode == 0)
            {
                Debug.LogError("FireMode must not be Nothing");
                return;
            }
            
            int length = System.Enum.GetValues(typeof(FireMode)).Length;
            m_FireModeLength = (int)Mathf.Pow(2, length);
            if (!m_FireMode.HasFlag((FireMode)m_FireModeIndex)) ChangeFlag();
        }

        protected override void AssignKeyAction()
        {
            base.AssignKeyAction();
            m_PlayerInputController.Reload += TryReload;
            m_PlayerInputController.ChangeFireMode += TryChangeFireMode;
            m_PlayerInputController.AutoFire += TryAutoFire;
            m_PlayerInputController.SemiFire += TrySemiFire;
            m_PlayerInputController.Aiming += TryAiming;
        }
        #endregion

        private void Update()
        {
            m_CurrentFireTime += Time.deltaTime;
            if (m_PlayerState.PlayerBehaviorState == PlayerBehaviorState.Running)
            {
                if (!m_IsRunning)
                {
                    m_IsRunning = true;
                    if (m_RunningCoroutine != null) StopCoroutine(m_RunningCoroutine);
                    m_RunningCoroutine = StartCoroutine(PosChange(m_RangeWeaponStat.m_RunningPivotPosition, m_RunningPivotRotation));
                }
            }
            else
            {
                m_IsRunning = false;
                if (m_RunningCoroutine != null) StopCoroutine(m_RunningCoroutine);
            }
        }
        
        private IEnumerator PosChange(Vector3 EndPosition, Quaternion EndRotation)
        {
            float currentTime = 0;
            float elapsedTime;
            Vector3 startLocalPosition = PivotTransform.localPosition;
            Quaternion startLocalRotation = PivotTransform.localRotation;
            while (currentTime < m_RangeWeaponStat.m_RunningPosTime)
            {
                currentTime += Time.deltaTime;

                elapsedTime = currentTime / m_RangeWeaponStat.m_RunningPosTime;
                PivotTransform.localPosition = Vector3.Lerp(startLocalPosition, EndPosition, elapsedTime);
                PivotTransform.localRotation = Quaternion.Lerp(startLocalRotation, EndRotation, elapsedTime);

                yield return elapsedTime;
            }
        }

        
        private void TryAiming(bool isAiming)
        {
            if (m_PlayerState.PlayerBehaviorState == PlayerBehaviorState.Running) return;

            if (isAiming)
            {
                m_PlayerState.SetWeaponAiming();
                AimingPosRot(m_RangeWeaponStat.m_AimingPivotPosition, m_AimingPivotRotation);
                m_MainCamera.fieldOfView = Mathf.Lerp(m_MainCamera.fieldOfView, m_AimingFOV, m_RangeWeaponStat.m_FOVMultiplier * Time.deltaTime);
            }
            else
            {
                m_PlayerState.SetWeaponIdle();
                AimingPosRot(m_WeaponManager.m_OriginalPivotPosition, m_WeaponManager.m_OriginalPivotRotation);
                m_MainCamera.fieldOfView = Mathf.Lerp(m_MainCamera.fieldOfView, m_WeaponManager.m_OriginalFOV, m_RangeWeaponStat.m_FOVMultiplier * Time.deltaTime);
            }
            SetCurrentFireIndex(isAiming);
        }

        private void AimingPosRot(Vector3 EndPosition, Quaternion EndRotation)
        {
            PivotTransform.localPosition = Vector3.Lerp(PivotTransform.localPosition, EndPosition, m_RangeWeaponStat.m_AimingPosTimeRatio);
            PivotTransform.localRotation = Quaternion.Lerp(PivotTransform.localRotation, EndRotation, m_RangeWeaponStat.m_AimingPosTimeRatio);
        }

        private void TryChangeFireMode()
        {
            if (!ChangeFlag()) return;

            m_EquipmentAnimator.SetTrigger("ChangeFireMode");
            m_ArmAnimator.SetTrigger("ChangeFireMode");

            m_AudioSource.PlayOneShot(m_RangeWeaponSound.changeModeSound[0]);
        }

        private bool ChangeFlag()
        {
            int beforeChangeIndex = m_FireModeIndex;
            do
            {
                if ((m_FireModeIndex <<= 1) >= m_FireModeLength) m_FireModeIndex = 1;
            }
            while (!m_FireMode.HasFlag((FireMode)m_FireModeIndex));
            int afterChangeIndex = m_FireModeIndex;

            if (beforeChangeIndex == afterChangeIndex) return false;
            m_CurrentFireMode = (FireMode)m_FireModeIndex;
            return true;
        }

        private void SetCurrentFireIndex(bool isAiming)
        {
            float fireIndex = isAiming ? 1 : 0;
            int idleIndex = isAiming ? 0 : 1;

            m_EquipmentAnimator.SetFloat("Fire Index", fireIndex);
            m_ArmAnimator.SetFloat("Fire Index", fireIndex);

            m_EquipmentAnimator.SetInteger("Idle Index", idleIndex);
            m_ArmAnimator.SetInteger("Idle Index", idleIndex);
        }

        private void TryReload()
        {
            if (base.IsEquiping || base.IsUnequiping) return;
            if (m_Reloadable.m_IsReloading || m_IsFiring) return;

            m_Reloadable.DoReload(m_IsEmpty); //변경해야함
        }


        #region Fire
        private bool CanFire() => m_CurrentFireTime >= m_RangeWeaponStat.m_AttackTime && 
            m_PlayerState.PlayerBehaviorState != PlayerBehaviorState.Running && m_Reloadable.CanFire() && !m_IsFiring;
        

        private void TryAutoFire()
        {
            if (base.IsEquiping || base.IsUnequiping) return;
            if (m_CurrentFireMode != FireMode.Auto) return;
            if (CanFire()) DoFire();
        }

        private void TrySemiFire()
        {
            if (base.IsEquiping || base.IsUnequiping) return;
            if (m_CurrentFireMode == FireMode.Auto) return;
            if (CanFire())
            {
                if (m_CurrentFireMode == FireMode.Semi) DoFire();
                else if (m_CurrentFireMode == FireMode.Burst) StartCoroutine(BurstFire());
            }
        }

        private IEnumerator BurstFire()
        {
            for (int i = 0; i < 3; i++)
            {
                DoFire();
                yield return m_BurstFireTime;
            }
        }

        private void DoFire()
        {
            m_IsFiring = true;
            m_CurrentFireTime = 0;

            m_Reloadable.StopReload();
            m_PlayerState.SetWeaponFiring();

            AudioClip audioClip = m_RangeWeaponSound.fireSound[Random.Range(0, m_RangeWeaponSound.fireSound.Length)];
            m_AudioSource.PlayOneShot(audioClip);

            m_EquipmentAnimator.SetBool("Fire", true);
            m_ArmAnimator.SetBool("Fire", true);

            m_Fireable.DoFire();

            audioClip = m_RangeWeaponSound.fireTailSound[Random.Range(0, m_RangeWeaponSound.fireTailSound.Length)];
            m_AudioSource.PlayOneShot(audioClip);

            if (m_IsInstantReload) StartCoroutine(InstantReload());
            else m_IsFiring = false;
        }

        private IEnumerator InstantReload()
        {
            WeaponSoundScriptable.DelaySoundClip[] sounds = m_RangeWeaponSound.instantReloadSoundClips;
            for (int i=0; i< sounds.Length; i++)
            {
                yield return new WaitForSeconds(sounds[i].delayTime);
                m_AudioSource.PlayOneShot(sounds[i].audioClip);
            }
            m_IsFiring = false;
        }

        #endregion

        protected override void DischargeKeyAction()
        {
            base.DischargeKeyAction();
            m_PlayerInputController.Reload -= TryReload;
            m_PlayerInputController.ChangeFireMode -= TryChangeFireMode;
            m_PlayerInputController.AutoFire -= TryAutoFire;
            m_PlayerInputController.SemiFire -= TrySemiFire;
            m_PlayerInputController.Aiming -= TryAiming;
        }
    }
}
