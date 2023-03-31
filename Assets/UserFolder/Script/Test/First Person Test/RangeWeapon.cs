using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scriptable;

namespace Test
{
    public class RangeWeapon : Weapon
    {
        [Space(15)]
        [Header("Child")]
        [Header("Aiming adjustment factor")]
        //조준시 에임 위치
        [SerializeField] private Transform m_Pivot; //위치 조정용 부모 오브젝트
        [SerializeField] private Vector3 m_AimingPivotPosition;   //위치 조정용 옮길 위치
        [SerializeField] private Vector3 m_AimingPivotDirection;   //위치 조정용 옮길 각도(Vector3)

        [Header("Running adjustment factor")]
        //달릴때 총(pivot) 위치
        [SerializeField] private Vector3 runningPivotPosition;      //달릴 때 pivot 위치
        [SerializeField] private Vector3 runningPivotRotation;      //달릴 때 pivot 각도
        [SerializeField] private float m_RunPosTime = 0.5f;              //달리는 자세로 전환 시간

        [Header("Fire light")]
        //발사 시 총구 빛
        [SerializeField] private TestFireLight m_FireLight;       //총구 화염 제어 스크립트
        [SerializeField] private Transform m_MuzzlePos;      //총구 위치

        [Header("Fire ray")]
        //발사 시 총 나가는거
        [SerializeField] private Camera mainCamera;         //총 발사 위치용 메인카메라
        [SerializeField] private float bulletMaxRange = 100f;    //총 사거리
        [SerializeField] private float fireRatio = 0.15f;       //총 발사 속도

        [Header("Fire recoil")]
        //총 반동
        [SerializeField] private Transform m_UpAxisTransform;         //상하 반동 오브젝트
        [SerializeField] private Transform m_RightAxisTransform;      //좌우 반동 오브젝트
        [SerializeField] private float m_UpAxisRecoil = 1.7f;            //상하 반동 값
        [SerializeField] private float m_RightAxisRecoil = 0.9f;         //좌우 반동 값

        [Header("Fire casing")]
        //발사 시 탄피 나가는거
        [SerializeField] private GameObject m_CasingObj;      //탄피 오브젝트
        [SerializeField] private Transform m_CasingSpawnPos;  //탄피 생성 위치
        [SerializeField] private float m_SpinValue = 17;      //탄피 회전값

        [Header("Reload magazine")]
        //재장전시 탄알집 떨어뜨리기
        [SerializeField] private GameObject m_MagazineObj;           //탄알집 오브젝트
        [SerializeField] private Transform m_MagazineSpawnPos;    //탄알집 생성 위치

        [Header("Fire mode")]
        [MultiEnum] [SerializeField] private FireMode m_FireMode;

        private Vector3 m_OriginalPivotPosition;  //위치 조정용 부모 오브젝트 원래 위치
        private Quaternion m_OriginalPivotRotation;  //위치 조정용 부모 오브젝트 원래 각도
        private Quaternion m_AimingPivotRotation;   //위치 조정용 옮길 각도(Quaternion)

        private bool m_IsRunning;
        private bool m_IsReloading;
        private bool m_IsAiming;
        private bool m_CanFirePosture = true;

        private float m_CurrentFireRatio;

        private readonly WaitForSeconds m_BurstRatioTime = new WaitForSeconds(0.1f);
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

        protected override void Awake()
        {
            base.Awake();
            m_OriginalPivotPosition = m_Pivot.localPosition;
            m_OriginalPivotRotation = m_Pivot.localRotation;
            m_AimingPivotRotation = Quaternion.Euler(m_AimingPivotDirection);

            AssignFireMode();
            AssignKeyAction();
            //Awake자리 아님
        }

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

        private void Start()
        {
            m_ArmAnimator.runtimeAnimatorController = m_ArmOverrideController;

            m_EquipmentAnimator.SetTrigger("Equip");
            m_ArmAnimator.SetTrigger("Equip");

            m_CrossHairController.SetCrossHair(1);
        }

        //Arm 애니메이터 덮어씌우기
        public override void Init()
        {
            base.Init();

            m_CrossHairController.SetCrossHair(1);

            AssignKeyAction();
        }        
        
        private void Update()
        {
            m_CurrentFireRatio += Time.deltaTime;

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

            if (isAiming) AimingPosRot(m_AimingPivotPosition, m_AimingPivotRotation);
            else AimingPosRot(m_OriginalPivotPosition, m_OriginalPivotRotation);
            m_IsRunning = false;
            SetCurrentFireIndex();
        }

        private void AimingPosRot(Vector3 EndPosition, Quaternion EndRotation)
        {
            m_Pivot.localPosition = Vector3.Lerp(m_Pivot.localPosition, EndPosition, 0.07f);
            m_Pivot.localRotation = Quaternion.Lerp(m_Pivot.localRotation, EndRotation, 0.07f);
        }

        private void TryChangeFireMode()
        {
            if (!ChangeFlag()) return;

            m_EquipmentAnimator.SetTrigger("ChangeFireMode");
            m_ArmAnimator.SetTrigger("ChangeFireMode");

            m_AudioSource.PlayOneShot(m_WeaponSound.changeModeSound[0]);
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

        private bool CanFire() => m_CurrentFireRatio > fireRatio && !m_IsRunning && m_CanFirePosture;

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
            //StartCoroutine(Reload(true, m_WeaponSound.emptyReloadSoundClips));
            StartCoroutine(Reload(false, m_WeaponSound.reloadSoundClips));
        }

        private IEnumerator Reload(bool isEmpty, RangeWeaponSoundScriptable.DelaySoundClip[] ReloadSoundClip)
        {
            string animParamName = isEmpty == true ? "Empty Reload" : "Reload";
            int magazineSpawnTiming = ReloadSoundClip.Length / 2;
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
            Quaternion magazineSpawnRotation = Quaternion.Euler(Random.Range(-30, 30), Random.Range(-30, 30), Random.Range(-30, 30));
            Instantiate(m_MagazineObj, m_MagazineSpawnPos.position, magazineSpawnRotation);
        }

        private IEnumerator RunningPos()
        {
            float currentTime = 0;
            float elapsedTime;
            Vector3 startLocalPosition = m_Pivot.localPosition;
            Quaternion startLocalRotation = m_Pivot.localRotation;
            while (currentTime < m_RunPosTime)
            {
                currentTime += Time.deltaTime;

                elapsedTime = currentTime / m_RunPosTime;
                m_Pivot.localPosition = Vector3.Lerp(startLocalPosition, runningPivotPosition, elapsedTime);
                m_Pivot.localRotation = Quaternion.Lerp(startLocalRotation, Quaternion.Euler(runningPivotRotation),elapsedTime);

                yield return elapsedTime;
            }
        }

        private void Fire()
        {
            m_CurrentFireRatio = 0;

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
                yield return m_BurstRatioTime;
            }
        }

        private void InstanceBullet()
        {
            Quaternion cassingSpawnRotation = Quaternion.Euler(Random.Range(-30, 30), Random.Range(-30, 30), Random.Range(-30, 30));

            GameObject cassing = Instantiate(m_CasingObj, m_CasingSpawnPos.position, cassingSpawnRotation);

            Rigidbody cassingRB = cassing.GetComponent<Rigidbody>();

            Vector3 randomForce = new Vector3(Random.Range(0.75f,1.25f), Random.Range(0.75f,1.25f),Random.Range(0.75f,1.25f));
            Vector3 randomTorque = new Vector3(Random.Range(-m_SpinValue, m_SpinValue), Random.Range(-m_SpinValue, m_SpinValue), Random.Range(-m_SpinValue, m_SpinValue));
            
            cassingRB.velocity = m_CasingSpawnPos.right + randomForce * 0.5f;
            cassingRB.angularVelocity = randomTorque;
        }

        private void FireRay()
        {
            AudioClip audioClip = m_WeaponSound.fireSound[Random.Range(0, m_WeaponSound.fireSound.Length)];

            m_AudioSource.PlayOneShot(audioClip);
            if (Physics.Raycast(m_MuzzlePos.position, mainCamera.transform.forward, out RaycastHit hit, bulletMaxRange, m_AttackableLayer, QueryTriggerInteraction.Ignore))
            {
                GameObject effectObject; //풀링 예정
                Scriptable.EffectPair effectPair;

                int hitLayer = hit.transform.gameObject.layer;
                if (hitLayer == 14)
                {
                    effectPair = m_SurfaceManager.GetBulletHitEffectPair(0);
                    effectObject = effectPair.effectObject;
                    audioClip = effectPair.audioClips[Random.Range(0, effectPair.audioClips.Length)];
                }
                else if (hitLayer == 17)
                {
                    effectPair = m_SurfaceManager.GetBulletHitEffectPair(1);
                    effectObject = effectPair.effectObject;
                    audioClip = effectPair.audioClips[Random.Range(0, effectPair.audioClips.Length)];
                }
                else
                {
                    if (!hit.transform.TryGetComponent(out MeshRenderer meshRenderer)) return;
                    if (meshRenderer.sharedMaterial == null) return;
                    //Debug.Log(meshRenderer.sharedMaterial);
                    int surfaceIndex = m_SurfaceManager.IsInMaterial(meshRenderer.sharedMaterial);
                    if (surfaceIndex == -1) return;
                    effectPair = m_SurfaceManager.GetBulletHitEffectPair(surfaceIndex);

                    effectObject = effectPair.effectObject;
                    audioClip = effectPair.audioClips[Random.Range(0, effectPair.audioClips.Length)];
                }

                AudioSource.PlayClipAtPoint(audioClip, hit.point);

                Instantiate(effectObject, hit.point, Quaternion.LookRotation(hit.normal));
            }
            audioClip = m_WeaponSound.fireTailSound[Random.Range(0, m_WeaponSound.fireTailSound.Length)];
            m_AudioSource.PlayOneShot(audioClip);
        }

        private void FireRecoil()
        {
            float upRandom = Random.Range(-0.2f, 0.4f);
            float rightRandom = Random.Range(-0.15f, 0.2f);

            upRandom += m_UpAxisRecoil;
            rightRandom += m_RightAxisRecoil;
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
