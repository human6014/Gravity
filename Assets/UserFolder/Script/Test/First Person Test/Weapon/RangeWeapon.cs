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
        [Space(15)]
        [Header("Child")]
        [Header("Scripatble")]
        //각종 소리를 담은 스크립터블
        [SerializeField] private RangeWeaponSoundScriptable m_RangeWeaponSound;  
        //Stat 관련 스크립터블
        [SerializeField] private RangeWeaponStatScriptable m_RangeWeaponStat;

        [Header("Index")]
        //풀링용 인덱스
        [SerializeField] private int m_CasingIndex = -1;
        [SerializeField] private int m_MagazineIndex = -1;

        [Header("Aiming adjustment factor")]
        //조준시 에임 위치
        [SerializeField] private Transform m_Pivot;                 //위치 조정용 부모 오브젝트
        [SerializeField] private Vector3 m_AimingPivotPosition;     //위치 조정용 옮길 위치
        [SerializeField] private Vector3 m_AimingPivotDirection;    //위치 조정용 옮길 각도(Vector3)

        [Header("Running adjustment factor")]
        //달릴때 총(pivot) 위치
        [SerializeField] private Vector3 runningPivotPosition;      //달릴 때 pivot 위치
        [SerializeField] private Vector3 runningPivotRotation;      //달릴 때 pivot 각도

        [Header("Fire recoil")]
        //총 반동
        [SerializeField] private Transform m_UpAxisTransform;         //상하 반동 오브젝트
        [SerializeField] private Transform m_RightAxisTransform;      //좌우 반동 오브젝트

        [Header("Reload")]
        [Tooltip("쏘고 바로 장전인지")]
        [SerializeField] private bool m_IsInstantReload;

        [Header("Fire mode")]
        [CustomAttribute.MultiEnum] [SerializeField] private FireMode m_FireMode;

        private Vector3 m_OriginalPivotPosition;            //위치 조정용 부모 오브젝트 원래 위치
        private Quaternion m_OriginalPivotRotation;         //위치 조정용 부모 오브젝트 원래 각도
        private Quaternion m_AimingPivotRotation;           //위치 조정용 옮길 각도(Quaternion)

        private bool m_IsFiring;
        private bool m_IsRunning;
        private bool m_IsAiming;

        private WaitForSeconds m_BurstFireTime;
        private Coroutine m_RunningCoroutine;

        private Fireable m_Fireable;
        private Reloadable m_Reloadable;

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

        [SerializeField] private bool m_IsEmpty;
        protected override void Awake()
        {
            base.Awake();
            m_OriginalPivotPosition = m_Pivot.localPosition;
            m_OriginalPivotRotation = m_Pivot.localRotation;
            m_AimingPivotRotation = Quaternion.Euler(m_AimingPivotDirection);
            m_BurstFireTime = new WaitForSeconds(m_RangeWeaponStat.m_BurstAttackTime);

            m_Fireable = GetComponent<Fireable>();
            m_Reloadable = GetComponent<Reloadable>();

            AssignFireMode();
        }

        private void Start()
        {
            if (m_CasingIndex != -1) m_Fireable.SetupCasingPooling(m_WeaponManager.GetCasingPoolingObject(m_CasingIndex));
            if (m_MagazineIndex != -1) m_Reloadable.SetupMagazinePooling(m_WeaponManager.GetMagazinePoolingObject(m_MagazineIndex));

            m_Fireable.Setup(m_RangeWeaponStat, m_WeaponManager.GetEffectPoolingObjects(), m_SurfaceManager, m_FirstPersonController);
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

        private void AssignKeyAction()
        {
            m_PlayerInputController.Reload += TryReload;
            m_PlayerInputController.ChangeFireMode += TryChangeFireMode;
            m_PlayerInputController.AutoFire += TryAutoFire;
            m_PlayerInputController.SemiFire += TrySemiFire;
            m_PlayerInputController.Aiming += TryAiming;
        }
        #endregion

        //Arm 애니메이터 덮어씌우기
        public override void Init()
        {
            base.Init();

            m_CrossHairController.SetCrossHair((int)m_RangeWeaponStat.m_DefaultCrossHair);

            AssignKeyAction();
        }        
        
        private void Update()
        {
            m_CurrentFireTime += Time.deltaTime;
            //isRunning = !firstPersonController.m_IsWalking;
            if (!m_FirstPersonController.m_IsWalking)
            {
                if (!m_IsRunning)
                {
                    m_IsRunning = true;
                    if (m_RunningCoroutine != null) StopCoroutine(m_RunningCoroutine);
                    m_RunningCoroutine = StartCoroutine(RunningPos());
                }
            }
            else if (m_IsRunning)
            {
                m_IsRunning = false;
                if (m_RunningCoroutine != null) StopCoroutine(m_RunningCoroutine);
            }
        }

        private void TryAiming(bool isAiming)
        {
            if (m_IsRunning) return;
            m_IsAiming = isAiming;

            int currentCrossHair = isAiming ? 0 : (int)m_RangeWeaponStat.m_DefaultCrossHair;
            m_CrossHairController.SetCrossHair(currentCrossHair);
            if (isAiming) AimingPosRot(m_AimingPivotPosition, m_AimingPivotRotation);
            else AimingPosRot(m_OriginalPivotPosition, m_OriginalPivotRotation);

            m_IsRunning = false;
            SetCurrentFireIndex();
        }

        private void AimingPosRot(Vector3 EndPosition, Quaternion EndRotation)
        {
            m_Pivot.localPosition = Vector3.Lerp(m_Pivot.localPosition, EndPosition, m_RangeWeaponStat.m_AimingPosTimeRatio);
            m_Pivot.localRotation = Quaternion.Lerp(m_Pivot.localRotation, EndRotation, m_RangeWeaponStat.m_AimingPosTimeRatio);
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
                //m_FireModeIndex <<= 1;
                if ((m_FireModeIndex <<= 1) >= m_FireModeLength) m_FireModeIndex = 1;
            }
            while (!m_FireMode.HasFlag((FireMode)m_FireModeIndex));
            int afterChangeIndex = m_FireModeIndex;

            if (beforeChangeIndex == afterChangeIndex) return false;
            m_CurrentFireMode = (FireMode)m_FireModeIndex;
            return true;
        }

        private void ChangeCrossHair()
        {
            //Do
        }

        private void SetCurrentFireIndex()
        {
            float fireIndex = m_IsAiming ? 1 : 0;
            int idleIndex = m_IsAiming ? 0 : 1;

            m_EquipmentAnimator.SetFloat("Fire Index", fireIndex);
            m_ArmAnimator.SetFloat("Fire Index", fireIndex);

            m_EquipmentAnimator.SetInteger("Idle Index", idleIndex);
            m_ArmAnimator.SetInteger("Idle Index", idleIndex);
        }

        private void TryReload()
        {
            if (m_Reloadable.m_IsReloading || m_IsFiring) return;

            m_Reloadable.DoReload(m_IsEmpty); //변경해야함
        }

        private IEnumerator RunningPos()
        {
            float currentTime = 0;
            float elapsedTime;
            Vector3 startLocalPosition = m_Pivot.localPosition;
            Quaternion startLocalRotation = m_Pivot.localRotation;
            while (currentTime < m_RangeWeaponStat.m_RunningPosTime)
            {
                currentTime += Time.deltaTime;

                elapsedTime = currentTime / m_RangeWeaponStat.m_RunningPosTime;
                m_Pivot.localPosition = Vector3.Lerp(startLocalPosition, runningPivotPosition, elapsedTime);
                m_Pivot.localRotation = Quaternion.Lerp(startLocalRotation, Quaternion.Euler(runningPivotRotation),elapsedTime);

                yield return elapsedTime;
            }
        }

        #region Fire
        private bool CanFire() => m_CurrentFireTime >= m_RangeWeaponStat.m_AttackTime && !m_IsRunning && 
                m_Reloadable.CanFire() && !m_IsFiring;
        

        private void TryAutoFire()
        {
            if (m_CurrentFireMode != FireMode.Auto) return;
            if (CanFire()) DoFire();
        }

        private void TrySemiFire()
        {
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

        public override void Dispose()
        {
            base.Dispose();

            DischargeKeyAction();
        }

        private void DischargeKeyAction()
        {
            m_PlayerInputController.Reload -= TryReload;
            m_PlayerInputController.ChangeFireMode -= TryChangeFireMode;
            m_PlayerInputController.AutoFire -= TryAutoFire;
            m_PlayerInputController.SemiFire -= TrySemiFire;
            m_PlayerInputController.Aiming -= TryAiming;
        }
    }
}
