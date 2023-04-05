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
        //���� �Ҹ��� ���� ��ũ���ͺ�
        [SerializeField] private RangeWeaponSoundScriptable m_RangeWeaponSound;  
        //Stat ���� ��ũ���ͺ�
        [SerializeField] private RangeWeaponStatScriptable m_RangeWeaponStat;

        [Header("Index")]
        //Ǯ���� �ε���
        [SerializeField] private int m_CasingIndex = -1;
        [SerializeField] private int m_MagazineIndex = -1;

        [Header("Aiming adjustment factor")]
        //���ؽ� ���� ��ġ
        [SerializeField] private Transform m_Pivot;                 //��ġ ������ �θ� ������Ʈ
        [SerializeField] private Vector3 m_AimingPivotPosition;     //��ġ ������ �ű� ��ġ
        [SerializeField] private Vector3 m_AimingPivotDirection;    //��ġ ������ �ű� ����(Vector3)

        [Header("Running adjustment factor")]
        //�޸��� ��(pivot) ��ġ
        [SerializeField] private Vector3 runningPivotPosition;      //�޸� �� pivot ��ġ
        [SerializeField] private Vector3 runningPivotRotation;      //�޸� �� pivot ����

        [Header("Fire ray")]
        //�߻� �� �� �����°�
        [SerializeField] private Camera mainCamera;                 //�� �߻� ��ġ�� ����ī�޶�

        [Header("Fire recoil")]
        //�� �ݵ�
        [SerializeField] private Transform m_UpAxisTransform;         //���� �ݵ� ������Ʈ
        [SerializeField] private Transform m_RightAxisTransform;      //�¿� �ݵ� ������Ʈ

        [Header("Fire mode")]
        [CustomAttribute.MultiEnum] [SerializeField] private FireMode m_FireMode;

        private Vector3 m_OriginalPivotPosition;            //��ġ ������ �θ� ������Ʈ ���� ��ġ
        private Quaternion m_OriginalPivotRotation;         //��ġ ������ �θ� ������Ʈ ���� ����
        private Quaternion m_AimingPivotRotation;           //��ġ ������ �ű� ����(Quaternion)

        private bool m_IsRunning;
        private bool m_IsAiming;

        private WaitForSeconds m_BurstFireTime;
        private Coroutine m_RunningCoroutine;

        [System.Flags]
        private enum FireMode
        {
            Auto = 1,
            Semi = 2,
            Burst= 4,
        }
        
        private FireMode m_CurrentFireMode = FireMode.Auto;
        private int m_FireModeLength;
        private int m_FireModeIndex = 1;

        [SerializeField] private bool m_IsInstanceReload;
        [SerializeField] private bool m_IsShotgun;
        private IFireable m_Fireable;
        private IReloadable m_Reloadable;

        private float m_CurrentFireTime;
        protected override void Awake()
        {
            base.Awake();
            m_OriginalPivotPosition = m_Pivot.localPosition;
            m_OriginalPivotRotation = m_Pivot.localRotation;
            m_AimingPivotRotation = Quaternion.Euler(m_AimingPivotDirection);
            m_BurstFireTime = new WaitForSeconds(m_RangeWeaponStat.m_BurstAttackTime);

            m_Fireable = GetComponent<IFireable>();
            m_Reloadable = GetComponent<IReloadable>();

            AssignFireMode();
        }

        private void Start()
        {
            if (m_CasingIndex != -1) m_Fireable.SetupCasingPooling(m_WeaponManager.GetCasingPoolingObject(m_CasingIndex));
            if (m_MagazineIndex != -1) m_Reloadable.SetupMagazinePooling(m_WeaponManager.GetMagazinePoolingObject(m_MagazineIndex));

            m_Fireable.Setup(m_RangeWeaponStat, m_WeaponManager.GetEffectPoolingObjects(), m_SurfaceManager, m_FirstPersonController);
            m_Reloadable.Setup(m_RangeWeaponSound);
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

        //Arm �ִϸ����� ������
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
            if(m_IsAiming)
            {
                m_EquipmentAnimator.SetFloat("Fire Index", 1);
                m_ArmAnimator.SetFloat("Fire Index", 1); 
                
                m_EquipmentAnimator.SetInteger("Idle Index", 0);
                m_ArmAnimator.SetInteger("Idle Index", 0);
            }
            else
            {
                m_EquipmentAnimator.SetFloat("Fire Index", 0);
                m_ArmAnimator.SetFloat("Fire Index", 0);

                m_EquipmentAnimator.SetInteger("Idle Index", 1);
                m_ArmAnimator.SetInteger("Idle Index", 1);
            }
        }

        private void TryReload()
        {
            bool isEmpty = false;
            m_Reloadable.DoReload(isEmpty); //�����ؾ���

            string animParamName = isEmpty == true ? "Empty Reload" : "Reload";
            m_EquipmentAnimator.SetTrigger(animParamName);
            m_ArmAnimator.SetTrigger(animParamName);
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
        private bool CanFire() => m_CurrentFireTime >= m_RangeWeaponStat.m_AttackTime && !m_IsRunning && !m_Reloadable.GetIsReloading();

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
            m_CurrentFireTime = 0;
            AudioClip audioClip = m_RangeWeaponSound.fireSound[Random.Range(0, m_RangeWeaponSound.fireSound.Length)];
            m_AudioSource.PlayOneShot(audioClip);

            m_EquipmentAnimator.SetBool("Fire", true);
            m_ArmAnimator.SetBool("Fire", true);

            m_Fireable.DoFire();

            audioClip = m_RangeWeaponSound.fireTailSound[Random.Range(0, m_RangeWeaponSound.fireTailSound.Length)];
            m_AudioSource.PlayOneShot(audioClip);
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
