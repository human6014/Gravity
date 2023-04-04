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

        [Header("Fire light")]
        //�߻� �� �ѱ� ��
        [SerializeField] private TestFireLight m_FireLight;         //�ѱ� ȭ�� ���� ��ũ��Ʈ
        //[SerializeField] 
        private Transform m_MuzzlePos;             //�ѱ� ��ġ

        [Header("Fire ray")]
        //�߻� �� �� �����°�
        [SerializeField] private Camera mainCamera;                 //�� �߻� ��ġ�� ����ī�޶�

        [Header("Fire recoil")]
        //�� �ݵ�
        [SerializeField] private Transform m_UpAxisTransform;         //���� �ݵ� ������Ʈ
        [SerializeField] private Transform m_RightAxisTransform;      //�¿� �ݵ� ������Ʈ

        [Header("Fire casing")]
        //�߻� �� ź�� �����°�
        [SerializeField] private Transform m_CasingSpawnPos;        //ź�� ���� ��ġ

        [Header("Reload magazine")]
        //�������� ź���� ����߸���
        [SerializeField] private Transform m_MagazineSpawnPos;      //ź���� ���� ��ġ

        [Header("Fire mode")]
        [CustomAttribute.MultiEnum] [SerializeField] private FireMode m_FireMode;

        private Vector3 m_OriginalPivotPosition;            //��ġ ������ �θ� ������Ʈ ���� ��ġ
        private Quaternion m_OriginalPivotRotation;         //��ġ ������ �θ� ������Ʈ ���� ����
        private Quaternion m_AimingPivotRotation;           //��ġ ������ �ű� ����(Quaternion)

        private bool m_IsRunning;
        private bool m_IsReloading;
        private bool m_IsAiming;
        private bool m_CanFirePosture = true;

        private float m_CurrentFireTime;

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

        private ObjectPoolManager.PoolingObject[] m_BulletEffectPoolingObjects; //0 : Concrete, 1 : Metal, 2 : Wood
        private ObjectPoolManager.PoolingObject m_CasingPoolingObject;
        private ObjectPoolManager.PoolingObject m_MagazinePoolingObject;
        protected override void Awake()
        {
            base.Awake();
            m_OriginalPivotPosition = m_Pivot.localPosition;
            m_OriginalPivotRotation = m_Pivot.localRotation;
            m_AimingPivotRotation = Quaternion.Euler(m_AimingPivotDirection);
            m_BurstFireTime = new WaitForSeconds(m_RangeWeaponStat.m_BurstAttackTime);

            m_MuzzlePos = m_FireLight.GetComponent<Transform>();

            AssignFireMode();
            //Awake�ڸ� �ƴ�
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

        private void AssignPoolingObject()
        {
            m_BulletEffectPoolingObjects = m_WeaponManager.GetEffectPoolingObjects();
            if(m_CasingIndex != -1) m_CasingPoolingObject = m_WeaponManager.GetCasingPoolingObject(m_CasingIndex);
            if(m_MagazineIndex != -1) m_MagazinePoolingObject = m_WeaponManager.GetMagazinePoolingObject(m_MagazineIndex);
        }
        #endregion

        //Arm �ִϸ����� ������
        public override void Init()
        {
            base.Init();

            m_CrossHairController.SetCrossHair((int)m_RangeWeaponStat.m_DefaultCrossHair);

            AssignKeyAction();
            AssignPoolingObject();
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

        }

        private bool CanFire() => m_CurrentFireTime > m_RangeWeaponStat.m_AttackTime && !m_IsRunning && m_CanFirePosture;

        private void TryAutoFire()
        {
            if (m_CurrentFireMode != FireMode.Auto) return;
            if (CanFire()) Fire();
        }

        private void TrySemiFire()
        {
            if (m_CurrentFireMode == FireMode.Auto) return;
            if (CanFire())
            {
                if (m_CurrentFireMode == FireMode.Semi) Fire();
                else if (m_CurrentFireMode == FireMode.Burst) StartCoroutine(BurstFire());
            }
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
            m_CanFirePosture = false;
            //StartCoroutine(Reload(true, m_WeaponSound.emptyReloadSoundClips)); //�� ź���� ������
            StartCoroutine(Reload(false, m_RangeWeaponSound.reloadSoundClips));
        }

        private IEnumerator Reload(bool isEmpty, RangeWeaponSoundScriptable.DelaySoundClip[] ReloadSoundClip)
        {
            string animParamName = isEmpty == true ? "Empty Reload" : "Reload";
            int magazineSpawnTiming = (int)(ReloadSoundClip.Length * 0.5f);
            float delayTime = 0;

            m_EquipmentAnimator.SetTrigger(animParamName);
            m_ArmAnimator.SetTrigger(animParamName);

            for(int i = 0; i < ReloadSoundClip.Length; i++)
            {
                delayTime = ReloadSoundClip[i].delayTime;
                yield return new WaitForSeconds(delayTime);
                m_AudioSource.PlayOneShot(ReloadSoundClip[i].audioClip);
                if (i == magazineSpawnTiming) InstanceMagazine();
            }
            yield return new WaitForSeconds(delayTime);
            m_CanFirePosture = true;
        }

        private void InstanceMagazine()
        {
            if (m_MagazineIndex == -1) return;
            Quaternion magazineSpawnRotation = Quaternion.Euler(Random.Range(-30, 30), Random.Range(-30, 30), Random.Range(-30, 30));
            //Instantiate(m_MagazineObj, m_MagazineSpawnPos.position, magazineSpawnRotation);//.GetComponent<Entity.Object.DefaultPoolingScript>().Init();
            DefaultPoolingScript magazinePoolingObject = (DefaultPoolingScript)m_MagazinePoolingObject.GetObject(false);
            magazinePoolingObject.Init(m_MagazineSpawnPos.position, magazineSpawnRotation, m_MagazinePoolingObject);
            magazinePoolingObject.gameObject.SetActive(true);
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

        private void Fire()
        {
            m_CurrentFireTime = 0;

            m_EquipmentAnimator.SetBool("Fire", true);
            m_ArmAnimator.SetBool("Fire", true);

            FireRay();
            FireRecoil();
            m_FireLight.Play(true);
            InstanceBullet();
            Invoke(nameof(EndFire), 0.1f);
        }

        private IEnumerator BurstFire()
        {
            for(int i = 0; i < 3; i++)
            {
                Fire();
                yield return m_BurstFireTime;
            }
        }

        private void InstanceBullet()
        {
            if (m_CasingIndex == -1) return;
            Quaternion cassingSpawnRotation = Quaternion.Euler(Random.Range(-30, 30), Random.Range(-30, 30), Random.Range(-30, 30));

            DefaultPoolingScript casingPoolingObject = (DefaultPoolingScript)m_CasingPoolingObject.GetObject(false);

            //GameObject cassing = Instantiate(m_CasingObj, m_CasingSpawnPos.position, cassingSpawnRotation);
            casingPoolingObject.Init(m_CasingSpawnPos.position,cassingSpawnRotation,m_CasingPoolingObject);
            casingPoolingObject.gameObject.SetActive(true);
            Rigidbody cassingRB = casingPoolingObject.GetComponent<Rigidbody>();

            float spinValue = m_RangeWeaponStat.m_SpinValue;
            Vector3 randomForce = new Vector3(Random.Range(0.75f,1.25f), Random.Range(0.75f,1.25f),Random.Range(0.75f,1.25f));
            Vector3 randomTorque = new Vector3(Random.Range(-spinValue, spinValue), Random.Range(-spinValue, spinValue), Random.Range(-spinValue, spinValue));
            
            cassingRB.velocity = m_CasingSpawnPos.right + randomForce * 0.5f;
            cassingRB.angularVelocity = randomTorque;
        }

        private void FireRay()
        {
            AudioClip audioClip = m_RangeWeaponSound.fireSound[Random.Range(0, m_RangeWeaponSound.fireSound.Length)];
            m_AudioSource.PlayOneShot(audioClip);

            ProcessingRaycast();

            audioClip = m_RangeWeaponSound.fireTailSound[Random.Range(0, m_RangeWeaponSound.fireTailSound.Length)];
            m_AudioSource.PlayOneShot(audioClip);
        }


        private void ProcessingRaycast()
        {
            if (Physics.Raycast(m_MuzzlePos.position, mainCamera.transform.forward, out RaycastHit hit, m_RangeWeaponStat.m_MaxRange, m_RangeWeaponStat.m_AttackableLayer, QueryTriggerInteraction.Ignore))
            {
                
                AudioClip audioClip;
                Scriptable.EffectPair effectPair;
                DefaultPoolingScript effectObj;
                int fireEffectNumber;
                int hitLayer = hit.transform.gameObject.layer;
                if (hitLayer == 14)
                {
                    fireEffectNumber = 0;
                    effectPair = m_SurfaceManager.GetBulletHitEffectPair(0);
                    effectObj = (DefaultPoolingScript)m_BulletEffectPoolingObjects[0].GetObject(false);
                    audioClip = effectPair.audioClips[Random.Range(0, effectPair.audioClips.Length)];
                }
                else if (hitLayer == 17)
                {
                    fireEffectNumber = 1;
                    effectPair = m_SurfaceManager.GetBulletHitEffectPair(1);

                    effectObj = (DefaultPoolingScript)m_BulletEffectPoolingObjects[1].GetObject(false);
                    audioClip = effectPair.audioClips[Random.Range(0, effectPair.audioClips.Length)];
                }
                else
                {
                    fireEffectNumber = 2;
                    if (!hit.transform.TryGetComponent(out MeshRenderer meshRenderer)) return;
                    //if (meshRenderer.sharedMaterial == null) return;

                    int surfaceIndex = m_SurfaceManager.IsInMaterial(meshRenderer.sharedMaterial);
                    if (surfaceIndex == -1) return;
                    effectPair = m_SurfaceManager.GetBulletHitEffectPair(surfaceIndex);

                    effectObj = (DefaultPoolingScript)m_BulletEffectPoolingObjects[surfaceIndex].GetObject(false);
                    audioClip = effectPair.audioClips[Random.Range(0, effectPair.audioClips.Length)];
                }
                Debug.Log("Hit");
                AudioSource.PlayClipAtPoint(audioClip, hit.point);
                effectObj.Init(hit.point, Quaternion.LookRotation(hit.normal), m_BulletEffectPoolingObjects[fireEffectNumber]);
                effectObj.gameObject.SetActive(true);
            }
        }

        private void FireRecoil()
        {
            float upRandom = Random.Range(m_RangeWeaponStat.m_UpRandomRecoil.x, m_RangeWeaponStat.m_UpRandomRecoil.y);
            float rightRandom = Random.Range(m_RangeWeaponStat.m_RightRandomRecoil.x, m_RangeWeaponStat.m_RightRandomRecoil.y);

            upRandom += m_RangeWeaponStat.m_UpAxisRecoil;
            rightRandom += m_RangeWeaponStat.m_RightAxisRecoil;
            m_FirstPersonController.MouseLook.AddRecoil(upRandom * 0.2f, rightRandom * 0.2f);
        }

        private void EndFire()
        {
            m_FireLight.Stop(true);

        }

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
