using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Contoller.Player;
using Scriptable;

namespace Test
{
    public class AnimationGun_Test : MonoBehaviour
    {
        [SerializeField] private PlayerInputController m_PlayerInputController;

        //애니메이션
        private Animator equipmentAnimator; //현재 자신의 무기 에니메이터
        [SerializeField] private Animator armAnimator; //팔 애니메이터
        [SerializeField] private AnimatorOverrideController equipmentOverrideController = null; // 덮어씌울 무기 애니메이션들
        [SerializeField] private AnimatorOverrideController armOverrideController = null;   // 덮어씌울 팔 애니메이션들

        //조준시 에임 위치
        [SerializeField] private Transform pivot; //위치 조정용 부모 오브젝트
        [SerializeField] private Vector3 aimingPivotPosition;   //위치 조정용 옮길 위치
        [SerializeField] private Vector3 aimingPivotRotation;   //위치 조정용 옮길 각도
        private Vector3 originalPivotPosition;  //위치 조정용 부모 오브젝트 원래 위치
        private Vector3 originalPivotRotation;  //위치 조정용 부모 오브젝트 원래 각도


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
        [SerializeField] private Scriptable.GunInfoScriptable gunInfo;  //각종 소리를 담은 스크립터블


        //달릴때 총(pivot) 위치
        [SerializeField] private Vector3 runningPivotPosition;      //달릴 때 pivot 위치
        [SerializeField] private Vector3 runningPivotRotation;      //달릴 때 pivot 각도
        [SerializeField] private float runPosTime = 1;              //달리는 자세로 전환 시간


        //재장전시 탄알집 떨어뜨리기
        [SerializeField] private GameObject magazine;           //탄알집 오브젝트
        [SerializeField] private Transform magazineSpawnPos;    //탄알집 생성 위치

        //앉기랑 다시 일어나기
        [SerializeField] Vector3 crouchInterporatePos;          //앉기, 일어나기 증감값
        [SerializeField] float crouchTime = 0.3f;               //앉기, 일어나기 자세 전환 시간
        private Vector3 idlePos;                                //일어난 상태 원래 위치
        private Vector3 crouchPos;                              //앉은 상태 위치


        private bool isEquip;
        private bool isRunning;
        private bool isAiming;
        private bool canFirePosture;
        private bool isCrouch;

        private float currentFireRatio;

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
            mainCamera = Camera.main;
            equipmentAnimator = GetComponent<Animator>();
            surfaceManager = FindObjectOfType<Manager.SurfaceManager>();
            originalPivotPosition = pivot.localPosition;
            originalPivotRotation = pivot.localEulerAngles;

            idlePos = rightAxisTransform.localPosition;
            crouchPos = rightAxisTransform.localPosition - crouchInterporatePos;

            m_PlayerInputController.Reload += TryReload;
            m_PlayerInputController.Crouch += TryCrouch;
            m_PlayerInputController.ChangeFireMode += TryChangeFireMode;
            m_PlayerInputController.AutoFire += TryAutoFire;
            m_PlayerInputController.SemiFire += TrySemiFire;
            m_PlayerInputController.Aiming += TryAiming;
        }

        private void Start()
        {
            armAnimator.runtimeAnimatorController = armOverrideController;

            equipmentAnimator.SetBool("Equip", true);
            armAnimator.SetBool("Equip", true);

            crossHairController.SetCrossHair(1);
        }

        //Arm 애니메이터 덮어씌우기
        public void Init()
        {
            armAnimator.runtimeAnimatorController = armOverrideController;
        }
        
        private void Update()
        {
            currentFireRatio += Time.deltaTime;
            if (!firstPersonController.m_IsWalking)
            {
                canFirePosture = false;
                if (!isRunning)
                {
                    isRunning = true;
                    StopAllCoroutines();
                    StartCoroutine(RunningPos());
                }
            }
            else
            {
                if (isRunning) StopAllCoroutines();
            }
        }

        private void TryAiming(bool isAiming)
        {
            this.isAiming = isAiming;

            if (isAiming) AimingPosRot(aimingPivotPosition, Quaternion.Euler(aimingPivotRotation));
            else AimingPosRot(originalPivotPosition, Quaternion.Euler(originalPivotRotation));
            isRunning = false;
            canFirePosture = true;
            SetCurrentFireIndex();
        }

        private void AimingPosRot(Vector3 EndPosition, Quaternion EndRotation)
        {
            pivot.localPosition = Vector3.Lerp(pivot.localPosition, EndPosition, 0.07f);
            pivot.localRotation = Quaternion.Lerp(pivot.localRotation, EndRotation, 0.07f);
        }

        private void TryChangeFireMode()
        {
            equipmentAnimator.SetTrigger("ChangeFireMode");
            armAnimator.SetTrigger("ChangeFireMode");

            audioSource.PlayOneShot(gunInfo.changeModeSound[0]);

            fireModeIndex = (fireModeIndex + 1) % 3;
            fireMode = (FireMode)fireModeIndex;
        }

        private bool CanFire() => currentFireRatio > fireRatio && canFirePosture;

        private void TryAutoFire()
        {
            if (fireMode != FireMode.Auto) return;
            if(CanFire()) Fire();
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

        private void TryCrouch(bool isCrouch)
        {
            this.isCrouch = isCrouch;
            StopAllCoroutines();
            if (isCrouch)
                StartCoroutine(CrouchPos(crouchTime, rightAxisTransform, crouchPos));
            else
                StartCoroutine(CrouchPos(crouchTime, rightAxisTransform, idlePos));
        }

        private void SetCurrentFireIndex()
        {
            if(isAiming)
            {
                equipmentAnimator.SetFloat("Fire Index", 1);
                armAnimator.SetFloat("Fire Index", 1); 
                
                equipmentAnimator.SetInteger("Idle Index", 0);
                armAnimator.SetInteger("Idle Index", 0);
            }
            else
            {
                equipmentAnimator.SetFloat("Fire Index", 0);
                armAnimator.SetFloat("Fire Index", 0);

                equipmentAnimator.SetInteger("Idle Index", 1);
                armAnimator.SetInteger("Idle Index", 1);
            }
        }

        private void TryReload()
        {
            canFirePosture = false;
            StartCoroutine(Reload(true, gunInfo.emptyReloadSoundClips));
            //StartCoroutine(Reload(false, gunInfo.emptyReloadSoundClips));
        }

        private IEnumerator Reload(bool isEmpty, GunInfoScriptable.DelaySoundClip[] ReloadSoundClip)
        {
            string animParamName = isEmpty == true ? "Empty Reload" : "Reload";
            int magazineSpawnTiming = ReloadSoundClip.Length / 2;
            float elapsedTime = 0;
            float delayTime;

            equipmentAnimator.SetTrigger(animParamName);
            armAnimator.SetTrigger(animParamName);

            for(int i = 0; i < ReloadSoundClip.Length; i++)
            {
                delayTime = ReloadSoundClip[i].delayTime;
                yield return new WaitForSeconds(delayTime);
                elapsedTime += delayTime;
                audioSource.PlayOneShot(ReloadSoundClip[i].audioClip);
                if (i == magazineSpawnTiming) InstanceMagazine();
            }
            canFirePosture = true;
        }

        private IEnumerator CrouchPos(float runTotalTime, Transform target, Vector3 endLocalPosition)
        {
            float currentTime = 0;
            float elapsedTime;
            Vector3 startLocalPosition = target.localPosition;
            while (currentTime < runTotalTime)
            {
                currentTime += Time.deltaTime;

                elapsedTime = currentTime / runTotalTime;
                target.localPosition = Vector3.Lerp(startLocalPosition, endLocalPosition, elapsedTime);
                yield return elapsedTime;
            }
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

            equipmentAnimator.SetBool("Fire", true);
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
                yield return new WaitForSeconds(0.1f);
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
            AudioClip audioClip = gunInfo.fireSound[Random.Range(0, gunInfo.fireSound.Length)];

            audioSource.PlayOneShot(audioClip);
            if (Physics.Raycast(muzzle.position, mainCamera.transform.forward, out RaycastHit hit, bulletMaxRange, bulletLayer, QueryTriggerInteraction.Ignore))
            {
                GameObject effectObject;
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
    }
}
