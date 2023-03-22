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
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask bulletLayer;
        [SerializeField] private float bulletMaxRange = 100;

        //발사 시 탄피 나가는거
        [SerializeField] private Transform casingSpawnPos;
        [SerializeField] private GameObject casingObj;
        [SerializeField] private float spinValue = 17;

        //총 반동
        [SerializeField] private FirstPersonController firstPersonController;
        [SerializeField] private Transform upAxisTransform;
        [SerializeField] private Transform rightAxisTransform;
        [SerializeField] private float upAxisRecoil;
        [SerializeField] private float rightAxisRecoil;

        private bool isAiming;

        private float currentFireRatio;
        private float fireRatio = 0.15f;
        private bool isAimingIn;
        private bool isAimingOut;
        private void Awake()
        {
            mainCamera = Camera.main;
            equipmentAnimator = GetComponent<Animator>();
            originalPivotPosition = pivot.localPosition;
            originalPivotRotation = pivot.localEulerAngles;
        }

        private void Start()
        {
            armAnimator.runtimeAnimatorController = armOverrideController;

            equipmentAnimator.SetBool("Equip", true);
            armAnimator.SetBool("Equip", true);
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

            if(Input.GetKey(KeyCode.Mouse1))
            {
                isAiming = true;
                equipmentAnimator.SetInteger("Idle Index", 0);
                armAnimator.SetInteger("Idle Index", 0);

                pivot.localPosition = Vector3.Lerp(pivot.localPosition, aimingPivotPosition, 0.07f);
                pivot.localEulerAngles = Vector3.Lerp(pivot.localEulerAngles, aimingPivotRotation, 0.07f);
            }
            else
            {
                isAiming = false;
                equipmentAnimator.SetInteger("Idle Index", 1);
                armAnimator.SetInteger("Idle Index", 1);

                pivot.localPosition = Vector3.Lerp(pivot.localPosition,originalPivotPosition, 0.07f);
                pivot.localEulerAngles = Vector3.Lerp(pivot.localEulerAngles,originalPivotRotation, 0.07f);
            }

            currentFireRatio += Time.deltaTime;
            if (Input.GetKey(KeyCode.Mouse0) && currentFireRatio > fireRatio)
            {
                Fire();
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
            if (Physics.Raycast(muzzle.position, mainCamera.transform.forward, out RaycastHit hit, bulletMaxRange, bulletLayer, QueryTriggerInteraction.Ignore))
            {
                Debug.Log(hit.transform.name);
            }
        }

        private void FireRecoil()
        {
            float upRandom = Random.Range(-0.2f, 0.4f);
            float rightRandom = Random.Range(-0.15f, 0.2f);

            upRandom += upAxisRecoil;
            rightRandom += rightAxisRecoil;
            firstPersonController.M_MouseLook.AddRecoil(upRandom * 0.2f, rightRandom * 0.2f);
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
