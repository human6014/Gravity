using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Object;
using Scriptable.Equipment;

namespace Entity.Object.Weapon
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
        #endregion

        private Quaternion m_AimingPivotRotation;           //위치 조정용 옮길 각도(Quaternion)
        private Quaternion m_RunningPivotRotation;          //위치 조정용 옮길 각도

        private WaitForSeconds m_BurstFireTime;
        private Coroutine m_RunningCoroutine;

        private Fireable m_Fireable;
        private Reloadable m_Reloadable;

        private RangeWeaponStatScriptable m_RangeWeaponStat;
        private RangeWeaponSoundScriptable m_RangeWeaponSound;

        private int m_FireModeLength;
        private int m_FireModeIndex = 1;

        private int m_CurrentMaxBullet;

        private float m_CurrentFireTime;
        private float m_CurrentPosTime;
        private float m_AimingFOV;

        private bool m_IsFiring;
        private bool m_IsRunning;

        public override int MaxBullet { get => m_CurrentMaxBullet; set => m_CurrentMaxBullet = value; }
        public override bool CanChangeWeapon => base.CanChangeWeapon && !m_IsFiring && !IsReloading;
        private bool IsReloading => m_Reloadable.IsReloading;

        public override void PreAwake()
        {
            base.PreAwake();
            m_RangeWeaponStat = (RangeWeaponStatScriptable)base.m_WeaponStatScriptable;
            m_CurrentMaxBullet = m_RangeWeaponStat.m_MaxBullets;
        }

        protected override void Awake()
        {
            base.Awake();

            m_RangeWeaponSound = (RangeWeaponSoundScriptable)base.m_WeaponSoundScriptable;
            m_BurstFireTime = new WaitForSeconds(m_RangeWeaponStat.m_BurstAttackTime);

            m_Fireable = GetComponent<Fireable>();
            m_Reloadable = GetComponent<Reloadable>();

            m_AimingPivotRotation = Quaternion.Euler(m_RangeWeaponStat.m_AimingPivotDirection);
            m_RunningPivotRotation = Quaternion.Euler(m_RangeWeaponStat.m_RunningPivotDirection);

            m_AimingFOV = WeaponManager.OriginalFOV - m_RangeWeaponStat.m_AimingFOV;
            
            AssignFireMode();
        }

        private void Start()
        {
            m_Fireable.Setup(m_RangeWeaponStat, WeaponManager.EffectPoolingObjectArray, PlayerData.PlayerState);
            m_Reloadable.Setup(m_RangeWeaponSound, m_ArmAnimator);
        }

        #region Assign
        private void AssignFireMode()
        {
            #if UNITY_EDITOR
            if (m_FireMode == 0)
            {
                Debug.LogError("FireMode must not be Nothing");
                return;
            }
            #endif

            int length = System.Enum.GetValues(typeof(FireMode)).Length;
            m_FireModeLength = (int)Mathf.Pow(2, length);
            if (!m_FireMode.HasFlag((FireMode)m_FireModeIndex)) ChangeFlag();
        }

        protected override void AssignKeyAction()
        {
            base.AssignKeyAction();
            PlayerInputController.Reload += TryReload;
            PlayerInputController.ChangeFireMode += TryChangeFireMode;
            PlayerInputController.AutoFire += TryAutoFire;
            PlayerInputController.SemiFire += TrySemiFire;
            PlayerInputController.Aiming += TryAiming;
            PlayerInputController.ToggleAiming += TryToggleAiming;
        }
        #endregion

        #region Running & Walking
        private void Update()
        {
            m_CurrentFireTime += Time.deltaTime * WeaponManager.AttackSpeedUpPercentage;
            if (PlayerData.PlayerState.PlayerBehaviorState == PlayerBehaviorState.Running)
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

            if (m_ToggleMode)
            {
                if (PlayerData.PlayerState.PlayerBehaviorState == PlayerBehaviorState.Running)
                {
                    m_IsAiming = false;
                    PlayerData.PlayerState.SetWeaponIdle();
                    return;
                }
                if (m_IsAiming)
                {
                    PlayerData.PlayerState.SetWeaponAiming();
                    AimingPosRot(m_RangeWeaponStat.m_AimingPivotPosition, m_AimingPivotRotation, m_AimingFOV);
                }
                else
                {
                    PlayerData.PlayerState.SetWeaponIdle();
                    AimingPosRot(WeaponManager.OriginalPivotPosition, WeaponManager.OriginalPivotRotation, WeaponManager.OriginalFOV);
                }
                SetCurrentFireIndex(m_IsAiming);
            }
        }
        
        private IEnumerator PosChange(Vector3 EndPosition, Quaternion EndRotation)
        {
            float elapsedTime = 0;
            float t;
            Vector3 startLocalPosition = m_Pivot.localPosition;
            Quaternion startLocalRotation = m_Pivot.localRotation;
            while (elapsedTime < m_RangeWeaponStat.m_RunningPosTime)
            {
                elapsedTime += Time.deltaTime;

                t = elapsedTime / m_RangeWeaponStat.m_RunningPosTime;
                m_Pivot.localPosition = Vector3.Lerp(startLocalPosition, EndPosition, t);
                m_Pivot.localRotation = Quaternion.Lerp(startLocalRotation, EndRotation, t);

                yield return null;
            }
        }
        #endregion

        #region Aiming
        private bool m_ToggleMode = false;
        private bool m_IsAiming = false;

        private void TryToggleAiming()
        {
            m_ToggleMode = true;
            m_IsAiming = !m_IsAiming;
        }

        private void TryAiming(bool isAiming)
        {
            m_ToggleMode = false;
            if (PlayerData.PlayerState.PlayerBehaviorState == PlayerBehaviorState.Running)
            {
                PlayerData.PlayerState.SetWeaponIdle();
                return;
            }

            if (isAiming)
            {
                PlayerData.PlayerState.SetWeaponAiming();
                AimingPosRot(m_RangeWeaponStat.m_AimingPivotPosition, m_AimingPivotRotation, m_AimingFOV);
            }
            else
            {
                PlayerData.PlayerState.SetWeaponIdle();
                AimingPosRot(WeaponManager.OriginalPivotPosition, WeaponManager.OriginalPivotRotation, WeaponManager.OriginalFOV);
            }
            SetCurrentFireIndex(isAiming);
        }

        private void AimingPosRot(Vector3 endPosition, Quaternion endRotation, float endFOV)
        {
            m_Pivot.localPosition = Vector3.Lerp(m_Pivot.localPosition, endPosition, m_RangeWeaponStat.m_AimingPosTimeRatio * Time.deltaTime);
            m_Pivot.localRotation = Quaternion.Lerp(m_Pivot.localRotation, endRotation, m_RangeWeaponStat.m_AimingPosTimeRatio * Time.deltaTime);
            MainCamera.fieldOfView = Mathf.Lerp(MainCamera.fieldOfView, endFOV, m_RangeWeaponStat.m_FOVMultiplier * Time.deltaTime);
        }
        #endregion

        #region FireMode
        private void TryChangeFireMode()
        {
            if (!ChangeFlag()) return;

            EquipmentAnimator.SetTrigger("ChangeFireMode");
            m_ArmAnimator.SetTrigger("ChangeFireMode");

            AudioSource.PlayOneShot(m_RangeWeaponSound.changeModeSound[0]);
            PlayerData.ChangeFireMode(CurrentFireMode);
        }

        private bool ChangeFlag()
        {
            int beforeChangeIndex = m_FireModeIndex;
            do
            {
                if ((m_FireModeIndex <<= 1) >= m_FireModeLength) m_FireModeIndex = 2;
            }
            while (!m_FireMode.HasFlag((FireMode)m_FireModeIndex));
            int afterChangeIndex = m_FireModeIndex;

            if (beforeChangeIndex == afterChangeIndex) return false;
            CurrentFireMode = (FireMode)m_FireModeIndex;
            return true;
        }

        private void SetCurrentFireIndex(bool isAiming)
        {
            float fireIndex = isAiming ? 1 : 0;
            int idleIndex = isAiming ? 0 : 1;

            EquipmentAnimator.SetFloat("Fire Index", fireIndex);
            m_ArmAnimator.SetFloat("Fire Index", fireIndex);

            EquipmentAnimator.SetInteger("Idle Index", idleIndex);
            m_ArmAnimator.SetInteger("Idle Index", idleIndex);
        }
        #endregion

        #region Reload
        private void TryReload()
        {
            if (base.IsEquiping || base.IsUnequiping) return;
            if (m_Reloadable.IsReloading || m_IsFiring) return;
            if (!WeaponInfo.CanReload()) return;

            bool isEmpty = WeaponInfo.m_CurrentRemainBullet <= 0;

            //WeaponManager.PlayerShakeController.ShakeAllTransform(ShakeType.Reloading);
            //재장전은 Shake좀 복잡함
            //일부 총기는 한발씩 장전,
            //총기마다 장전 시간 다 다름,
            //빈 탄알집 장전 시간 다름,
            //Skill up으로 장전 시간 변동
            //등등 고려해야할게 많으니 대기
            WeaponInfo.GetDifferenceValue(out int difference);
            m_Reloadable.SetReloadSpeedPercentage(WeaponManager.ReloadSpeedUpPercentage);
            m_Reloadable.DoReload(isEmpty, difference);
        }
        #endregion

        #region Fire
        private bool CanFire() => m_CurrentFireTime >= m_RangeWeaponStat.m_AttackTime  &&
            PlayerData.PlayerState.PlayerBehaviorState != PlayerBehaviorState.Running && m_Reloadable.CanFire() && !m_IsFiring;
        
        private bool m_OnFireSound;
        private void TryAutoFire()
        {
            if (base.IsEquiping || base.IsUnequiping) return;
            if (CurrentFireMode != FireMode.Auto) return;
            if (!CanFire()) return;

            if (WeaponInfo.m_CurrentRemainBullet <= 0 && !m_OnFireSound) PlayEmptySound();
            else DoFire();
        }

        private void TrySemiFire()
        {
            if (base.IsEquiping || base.IsUnequiping) return;
            if (CurrentFireMode == FireMode.Auto) return;
            if (!CanFire()) return;

            if (WeaponInfo.m_CurrentRemainBullet <= 0) PlayEmptySound();
            else
            {
                if (CurrentFireMode == FireMode.Semi) DoFire();
                else if (CurrentFireMode == FireMode.Burst) StartCoroutine(BurstFire());
            }
        }

        private IEnumerator BurstFire()
        {
            for (int i = 0; i < 3; i++)
            {
                DoFire();
                if (WeaponInfo.m_CurrentRemainBullet <= 0) yield break;
                yield return m_BurstFireTime;
            }
        }

        private void PlayEmptySound()
        {
            m_CurrentFireTime = 0;
            AudioSource.PlayOneShot(m_RangeWeaponSound.emptySound[0]);
        }

        private void DoFire()
        {
            m_IsFiring = true;
            m_CurrentFireTime = 0;

            m_Reloadable.StopReload();
            PlayerData.RangeWeaponFire();
            PlayerData.PlayerState.SetWeaponFiring();

            AudioClip audioClip = m_RangeWeaponSound.fireSound[Random.Range(0, m_RangeWeaponSound.fireSound.Length)];
            AudioSource.PlayOneShot(audioClip);

            EquipmentAnimator.SetBool("Fire", true);
            m_ArmAnimator.SetBool("Fire", true);

            m_Fireable.SetDamageUpPercentage(WeaponManager.DamageUpPercentage);
            if (m_Fireable.DoFire()) PlayerData.HitEnemy();

            audioClip = m_RangeWeaponSound.fireTailSound[Random.Range(0, m_RangeWeaponSound.fireTailSound.Length)];
            AudioSource.PlayOneShot(audioClip);

            if (m_IsInstantReload) StartCoroutine(InstantReload());
            else m_IsFiring = false;
        }

        private IEnumerator InstantReload()
        {
            WeaponSoundScriptable.DelaySoundClip[] sounds = m_RangeWeaponSound.instantReloadSoundClips;
            for (int i=0; i< sounds.Length; i++)
            {
                yield return new WaitForSeconds(sounds[i].delayTime);
                AudioSource.PlayOneShot(sounds[i].audioClip);
            }
            m_IsFiring = false;
        }

        #endregion

        protected override void DischargeKeyAction()
        {
            base.DischargeKeyAction();
            m_IsAiming = false;
            PlayerInputController.Reload -= TryReload;
            PlayerInputController.ChangeFireMode -= TryChangeFireMode;
            PlayerInputController.AutoFire -= TryAutoFire;
            PlayerInputController.SemiFire -= TrySemiFire;
            PlayerInputController.Aiming -= TryAiming;
            PlayerInputController.ToggleAiming -= TryToggleAiming;
        }
    }
}
