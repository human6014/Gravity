using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Contoller.Player;

namespace Test
{
    public class AnimationGun_Test : MonoBehaviour
    {
        //�ִϸ��̼�
        private Animator equipmentAnimator; //���� �ڽ��� ���� ���ϸ�����
        [SerializeField] private Animator armAnimator; //�� �ִϸ�����
        [SerializeField] private AnimatorOverrideController equipmentOverrideController = null; // ����� ���� �ִϸ��̼ǵ�
        [SerializeField] private AnimatorOverrideController armOverrideController = null;   // ����� �� �ִϸ��̼ǵ�

        //���ؽ� ���� ��ġ
        [SerializeField] private Transform pivot; //��ġ ������ �θ� ������Ʈ
        [SerializeField] private Vector3 aimingPivotPosition;   //��ġ ������ �ű� ��ġ
        [SerializeField] private Vector3 aimingPivotRotation;   //��ġ ������ �ű� ����
        private Vector3 originalPivotPosition;  //��ġ ������ �θ� ������Ʈ ���� ��ġ
        private Vector3 originalPivotRotation;  //��ġ ������ �θ� ������Ʈ ���� ����


        //�߻� �� �ѱ� ��
        [SerializeField] private TestFireLight fireLight;       //�ѱ� ȭ�� ���� ��ũ��Ʈ
        [SerializeField] private Transform muzzle;      //�ѱ� ��ġ

        
        //�߻� �� �� �����°�
        [SerializeField] private Camera mainCamera;         //�� �߻� ��ġ�� ����ī�޶�
        [SerializeField] private LayerMask bulletLayer;     //�� �ǰ� ���̾�
        [SerializeField] private float bulletMaxRange = 100;    //�� ��Ÿ�


        //�߻� �� ź�� �����°�
        [SerializeField] private Transform casingSpawnPos;  //ź�� ���� ��ġ
        [SerializeField] private GameObject casingObj;      //ź�� ������Ʈ
        [SerializeField] private float spinValue = 17;      //ź�� ȸ����


        //�� �ݵ�
        [SerializeField] private FirstPersonController firstPersonController;   //�ݵ��� ī�޶� ���� ����� MouseLook ������ ���
        [SerializeField] private Transform upAxisTransform;         //���� �ݵ� ������Ʈ
        [SerializeField] private Transform rightAxisTransform;      //�¿� �ݵ� ������Ʈ
        [SerializeField] private float upAxisRecoil;            //���� �ݵ� ��
        [SerializeField] private float rightAxisRecoil;         //�¿� �ݵ� ��


        //�ǰݽ� �ڱ�
        private Manager.SurfaceManager surfaceManager;

        //ũ�ν� ��� �ѱ⺰�� ����
        [SerializeField] private CrossHairController crossHairController;   //�ѱ⺰ ũ�ν� ��� ������ ���� UI���� ��ũ��Ʈ

        //�߻� + �ǰ� �Ҹ�
        [SerializeField] private AudioSource audioSource;           //�Ҹ� ���� ���� AudioSource 
        [SerializeField] private Scriptable.GunInfoScriptable gunInfo;  //���� �Ҹ��� ���� ��ũ���ͺ�


        //�޸��� ��(pivot) ��ġ
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

        //Arm �ִϸ����� ������
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
