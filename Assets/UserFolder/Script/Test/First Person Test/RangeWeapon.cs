using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Contoller.Player;
using Scriptable;

namespace Test
{
    public class RangeWeapon : MonoBehaviour
    {
        [SerializeField] private PlayerInputController m_PlayerInputController;

        //애니메이션
        private Animator m_EquipmentAnimator; //현재 자신의 무기 에니메이터
        [SerializeField] private Animator armAnimator; //팔 애니메이터
        [SerializeField] private AnimatorOverrideController equipmentOverrideController = null; // 덮어씌울 무기 애니메이션들
        [SerializeField] private AnimatorOverrideController armOverrideController = null;   // 덮어씌울 팔 애니메이션들

        //조준시 에임 위치
        [SerializeField] private Transform pivot; //위치 조정용 부모 오브젝트
        [SerializeField] private Vector3 aimingPivotPosition;   //위치 조정용 옮길 위치
        [SerializeField] private Vector3 aimingPivotRotation;   //위치 조정용 옮길 각도
        private Vector3 m_OriginalPivotPosition;  //위치 조정용 부모 오브젝트 원래 위치
        private Vector3 m_OriginalPivotRotation;  //위치 조정용 부모 오브젝트 원래 각도


        //발사 시 총구 빛
        [SerializeField] private TestFireLight fireLight;       //총구 화염 제어 스크립트
        [SerializeField] private Transform muzzle;      //총구 위치

        
        //발사 시 총 나가는거
        [SerializeField] private Camera mainCamera;         //총 발사 위치용 메인카메라
        [SerializeField] private LayerMask bulletLayer;     //총 피격 레이어
        [SerializeField] private float bulletMaxRange = 100;    //총 사거리
        [SerializeField] private float fireRatio = 0.15f;       //총 발사 속도

        //발사 시 탄피 나가는거
        [SerializeField] private Transform casingSpawnPos;  //탄피 생성 위치
        [SerializeField] private GameObject casingObj;      //탄피 오브젝트
        [SerializeField] private float spinValue = 17;      //탄피 회전값


        //총 반동
        [SerializeField] private FirstPersonController firstPersonController;   //반동시 카메라 각도 변경용 MouseLook 참조값 얻기
        [SerializeField] private Transform upAxisTransform;         //상하 반동 오브젝트
        [SerializeField] private Transform rightAxisTransform;      //좌우 반동 오브젝트
        [SerializeField] private float upAxisRecoil;            //상하 반동 값
        [SerializeField] private float rightAxisRecoil;         //좌우 반동 값


        //피격시 자국
        private Manager.SurfaceManager surfaceManager;

        //크로스 헤어 총기별로 설정
        [SerializeField] private CrossHairController crossHairController;   //총기별 크로스 헤어 설정을 위한 UI관리 스크립트

        //발사 + 피격 소리
        [SerializeField] private AudioSource audioSource;               //소리 내기 위한 AudioSource 
        [SerializeField] private Scriptable.RangeWeaponSoundScriptable m_WeaponSound;  //각종 소리를 담은 스크립터블


        //달릴때 총(pivot) 위치
        [SerializeField] private Vector3 runningPivotPosition;      //달릴 때 pivot 위치
        [SerializeField] private Vector3 runningPivotRotation;      //달릴 때 pivot 각도
        [SerializeField] private float runPosTime = 1;              //달리는 자세로 전환 시간


        //재장전시 탄알집 떨어뜨리기
        [SerializeField] private GameObject magazine;           //탄알집 오브젝트
        [SerializeField] private Transform magazineSpawnPos;    //탄알집 생성 위치

        private bool isEquip;
        private bool isRunning;
        private bool isReloading;
        private bool isAiming;
        private bool canFirePosture = true;

        private float currentFireRatio;

        private readonly WaitForSeconds m_BurstRatioTime = new WaitForSeconds(0.1f);
        private Coroutine runningCoroutine;

        private enum FireMode
        {
            Auto = 0,
            Semi,
            Burst
        }
        FireMode fireMode = FireMode.Auto;
        private int fireModeIndex = 0;

        private void Awake()
        {
            m_EquipmentAnimator = GetComponent<Animator>();
            surfaceManager = FindObjectOfType<Manager.SurfaceManager>();
            m_OriginalPivotPosition = pivot.localPosition;
            m_OriginalPivotRotation = pivot.localEulerAngles;

            AssignKeyAction();
            //Awake자리 아님
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
            armAnimator.runtimeAnimatorController = armOverrideController;

            m_EquipmentAnimator.SetTrigger("Equip");
            armAnimator.SetTrigger("Equip");

            crossHairController.SetCrossHair(1);
        }

        //Arm 애니메이터 덮어씌우기
        public void Init()
        {
            armAnimator.runtimeAnimatorController = armOverrideController;

            m_EquipmentAnimator.SetTrigger("Equip");
            armAnimator.SetTrigger("Equip");

            crossHairController.SetCrossHair(1);
        }        
        
        private void Update()
        {
            currentFireRatio += Time.deltaTime;

            //isRunning = !firstPersonController.m_IsWalking;
            if (!firstPersonController.m_IsWalking)
            {
                if (!isRunning)
                {
                    isRunning = true;
                    if (runningCoroutine != null) StopCoroutine(runningCoroutine);
                    runningCoroutine = StartCoroutine(RunningPos());
                }
            }
            else if (isRunning)
            {
                isRunning = false;
                if (runningCoroutine != null) StopCoroutine(runningCoroutine);
            }
        }


        private void TryAiming(bool isAiming)
        {
            if (isRunning) return;
            this.isAiming = isAiming;

            if (isAiming) AimingPosRot(aimingPivotPosition, Quaternion.Euler(aimingPivotRotation));
            else AimingPosRot(m_OriginalPivotPosition, Quaternion.Euler(m_OriginalPivotRotation));
            isRunning = false;
            SetCurrentFireIndex();
        }

        private void AimingPosRot(Vector3 EndPosition, Quaternion EndRotation)
        {
            pivot.localPosition = Vector3.Lerp(pivot.localPosition, EndPosition, 0.07f);
            pivot.localRotation = Quaternion.Lerp(pivot.localRotation, EndRotation, 0.07f);
        }

        private void TryChangeFireMode()
        {
            m_EquipmentAnimator.SetTrigger("ChangeFireMode");
            armAnimator.SetTrigger("ChangeFireMode");

            audioSource.PlayOneShot(m_WeaponSound.changeModeSound[0]);

            fireModeIndex = (fireModeIndex + 1) % 3;
            fireMode = (FireMode)fireModeIndex;
        }

        private bool CanFire() => currentFireRatio > fireRatio && !isRunning && canFirePosture;

        private void TryAutoFire()
        {
            if (fireMode != FireMode.Auto) return;
            if (CanFire()) Fire();
        }

        private void TrySemiFire()
        {
            if (fireMode == FireMode.Auto) return;
            if (CanFire())
            {
                if (fireMode == FireMode.Semi) Fire();
                else if (fireMode == FireMode.Burst) StartCoroutine(BurstFire());
            }
        }

        private void SetCurrentFireIndex()
        {
            if(isAiming)
            {
                m_EquipmentAnimator.SetFloat("Fire Index", 1);
                armAnimator.SetFloat("Fire Index", 1); 
                
                m_EquipmentAnimator.SetInteger("Idle Index", 0);
                armAnimator.SetInteger("Idle Index", 0);
            }
            else
            {
                m_EquipmentAnimator.SetFloat("Fire Index", 0);
                armAnimator.SetFloat("Fire Index", 0);

                m_EquipmentAnimator.SetInteger("Idle Index", 1);
                armAnimator.SetInteger("Idle Index", 1);
            }
        }

        private void TryReload()
        {
            canFirePosture = false;
            StartCoroutine(Reload(true, m_WeaponSound.emptyReloadSoundClips));
            //StartCoroutine(Reload(false, gunInfo.emptyReloadSoundClips));
        }

        private IEnumerator Reload(bool isEmpty, RangeWeaponSoundScriptable.DelaySoundClip[] ReloadSoundClip)
        {
            string animParamName = isEmpty == true ? "Empty Reload" : "Reload";
            int magazineSpawnTiming = ReloadSoundClip.Length / 2;
            float delayTime = 0;

            m_EquipmentAnimator.SetTrigger(animParamName);
            armAnimator.SetTrigger(animParamName);

            for(int i = 0; i < ReloadSoundClip.Length; i++)
            {
                delayTime = ReloadSoundClip[i].delayTime;
                yield return new WaitForSeconds(delayTime);
                audioSource.PlayOneShot(ReloadSoundClip[i].audioClip);
                if (i == magazineSpawnTiming) InstanceMagazine();
            }
            yield return new WaitForSeconds(delayTime);
            canFirePosture = true;
        }

        private void InstanceMagazine()
        {
            Quaternion magazineSpawnRotation = Quaternion.Euler(Random.Range(-30, 30), Random.Range(-30, 30), Random.Range(-30, 30));
            Instantiate(magazine, magazineSpawnPos.position, magazineSpawnRotation);
        }

        private IEnumerator RunningPos()
        {
            float currentTime = 0;
            float elapsedTime;
            Vector3 startLocalPosition = pivot.localPosition;
            Quaternion startLocalRotation = pivot.localRotation;
            while (currentTime < runPosTime)
            {
                currentTime += Time.deltaTime;

                elapsedTime = currentTime / runPosTime;
                pivot.localPosition = Vector3.Lerp(startLocalPosition, runningPivotPosition, elapsedTime);
                pivot.localRotation = Quaternion.Lerp(startLocalRotation, Quaternion.Euler(runningPivotRotation),elapsedTime);

                yield return elapsedTime;
            }
        }

        private void Fire()
        {
            currentFireRatio = 0;

            m_EquipmentAnimator.SetBool("Fire", true);
            armAnimator.SetBool("Fire", true);

            FireRay();
            FireRecoil();
            fireLight.Play(true);
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

            GameObject cassing = Instantiate(casingObj, casingSpawnPos.position, cassingSpawnRotation);

            Rigidbody cassingRB = cassing.GetComponent<Rigidbody>();

            Vector3 randomForce = new Vector3(Random.Range(0.75f,1.25f), Random.Range(0.75f,1.25f),Random.Range(0.75f,1.25f));
            Vector3 randomTorque = new Vector3(Random.Range(-spinValue, spinValue), Random.Range(-spinValue, spinValue), Random.Range(-spinValue, spinValue));
            
            cassingRB.velocity = casingSpawnPos.right + randomForce * 0.5f;
            cassingRB.angularVelocity = randomTorque;
        }

        private void FireRay()
        {
            AudioClip audioClip = m_WeaponSound.fireSound[Random.Range(0, m_WeaponSound.fireSound.Length)];

            audioSource.PlayOneShot(audioClip);
            if (Physics.Raycast(muzzle.position, mainCamera.transform.forward, out RaycastHit hit, bulletMaxRange, bulletLayer, QueryTriggerInteraction.Ignore))
            {
                GameObject effectObject; //풀링 예정
                Scriptable.EffectPair effectPair;

                int hitLayer = hit.transform.gameObject.layer;
                if (hitLayer == 14)
                {
                    effectPair = surfaceManager.GetBulletHitEffectPair(0);
                    effectObject = effectPair.effectObject;
                    audioClip = effectPair.audioClips[Random.Range(0, effectPair.audioClips.Length)];
                }
                else if (hitLayer == 17)
                {
                    effectPair = surfaceManager.GetBulletHitEffectPair(1);
                    effectObject = effectPair.effectObject;
                    audioClip = effectPair.audioClips[Random.Range(0, effectPair.audioClips.Length)];
                }
                else
                {
                    if (!hit.transform.TryGetComponent(out MeshRenderer meshRenderer)) return;
                    if (meshRenderer.sharedMaterial == null) return;
                    //Debug.Log(meshRenderer.sharedMaterial);
                    int surfaceIndex = surfaceManager.IsInMaterial(meshRenderer.sharedMaterial);
                    if (surfaceIndex == -1) return;
                    effectPair = surfaceManager.GetBulletHitEffectPair(surfaceIndex);

                    effectObject = effectPair.effectObject;
                    audioClip = effectPair.audioClips[Random.Range(0, effectPair.audioClips.Length)];
                }

                AudioSource.PlayClipAtPoint(audioClip, hit.point);

                Instantiate(effectObject, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }

        private void FireRecoil()
        {
            float upRandom = Random.Range(-0.2f, 0.4f);
            float rightRandom = Random.Range(-0.15f, 0.2f);

            upRandom += upAxisRecoil;
            rightRandom += rightAxisRecoil;
            firstPersonController.MouseLook.AddRecoil(upRandom * 0.2f, rightRandom * 0.2f);
        }

        private void EndFire()
        {
            fireLight.Stop(true);
        }

        public void Dispose()
        {
            armAnimator.SetTrigger("Unequip");
            m_EquipmentAnimator.SetTrigger("Unequip");
        }
    }
}
