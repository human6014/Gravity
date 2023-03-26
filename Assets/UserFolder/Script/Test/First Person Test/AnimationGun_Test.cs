using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Contoller.Player;

namespace Test
{
    public class AnimationGun_Test : MonoBehaviour
    {
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
        [SerializeField] private AudioSource audioSource;           //소리 내기 위한 AudioSource 
        [SerializeField] private Scriptable.GunInfoScriptable gunInfo;  //각종 소리를 담은 스크립터블


        //달릴때 총(pivot) 위치
        [SerializeField] private Vector3 runningPivotPosition;  
        [SerializeField] private Vector3 runningPivotRotation;
        private Quaternion lookRotation;

        private bool isRunning;
        private bool isAiming;
        private bool canFire;

        private float currentFireRatio;
        private float fireRatio = 0.15f;
        private bool isAimingIn;
        private bool isAimingOut;
        private void Awake()
        {
            mainCamera = Camera.main;
            equipmentAnimator = GetComponent<Animator>();
            surfaceManager = FindObjectOfType<Manager.SurfaceManager>();
            originalPivotPosition = pivot.localPosition;
            originalPivotRotation = pivot.localEulerAngles;

            //crossHairController = FindObjectOfType<CrossHairController>();
        }

        private void Start()
        {
            armAnimator.runtimeAnimatorController = armOverrideController;

            equipmentAnimator.SetBool("Equip", true);
            armAnimator.SetBool("Equip", true);

            crossHairController.SetCrossHair(1);

            lookRotation = Quaternion.Euler(runningPivotRotation);
        }

        //Arm 애니메이터 덮어씌우기
        public void Init()
        {
            armAnimator.runtimeAnimatorController = armOverrideController;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                equipmentAnimator.SetTrigger("Reload");
                armAnimator.SetTrigger("Reload");
            }


            if (!firstPersonController.m_IsWalking)
            {
                Debug.Log("Running");

                //pivot.localPosition = Vector3.Lerp(pivot.localPosition, runningPivotPosition, 0.08f);
                //pivot.localRotation = Quaternion.Lerp(pivot.rotation, Quaternion.Euler(runningPivotRotation), 0.08f);

                canFire = false;
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
                isRunning = false;
                canFire = true;
                if (Input.GetKey(KeyCode.Mouse1))
                {
                    isAiming = true;
                    equipmentAnimator.SetInteger("Idle Index", 0);
                    armAnimator.SetInteger("Idle Index", 0);

                    pivot.localPosition = Vector3.Lerp(pivot.localPosition, aimingPivotPosition, 0.07f);
                    pivot.localRotation = Quaternion.Lerp(pivot.localRotation, Quaternion.Euler(aimingPivotRotation), 0.07f);
                }
                else
                {
                    isAiming = false;
                    equipmentAnimator.SetInteger("Idle Index", 1);
                    armAnimator.SetInteger("Idle Index", 1);

                    pivot.localPosition = Vector3.Lerp(pivot.localPosition, originalPivotPosition, 0.07f);
                    pivot.localRotation = Quaternion.Lerp(pivot.localRotation, Quaternion.Euler(originalPivotRotation), 0.07f);
                }
            }

            currentFireRatio += Time.deltaTime;
            if (Input.GetKey(KeyCode.Mouse0) && currentFireRatio > fireRatio && canFire)
            {
                Fire();
            }
        }

        private void Reload()
        {

        }

        private IEnumerator RunningPos()
        {
            float currentTime = 0;
            float t;
            Vector3 currentLocalPosition = pivot.localPosition;
            Quaternion currentLocalRotation = pivot.localRotation;
            while (currentTime < 1)
            {
                currentTime += Time.deltaTime;

                t = currentTime / 1;
                pivot.localPosition = Vector3.Lerp(currentLocalPosition, runningPivotPosition, t);
                pivot.localRotation = Quaternion.Lerp(currentLocalRotation, Quaternion.Euler(runningPivotRotation),t);

                yield return t;
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(muzzle.position, muzzle.position + mainCamera.transform.forward * bulletMaxRange);
        }

        private void EndFire()
        {
            fireLight.Stop(true);
        }

    }
}
