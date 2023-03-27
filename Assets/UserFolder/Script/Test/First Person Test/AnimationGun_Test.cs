using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Contoller.Player;
using Scriptable;

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
        [SerializeField] private float fireRatio = 0.15f;       //�� �߻� �ӵ�

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
        [SerializeField] private AudioSource audioSource;               //�Ҹ� ���� ���� AudioSource 
        [SerializeField] private Scriptable.GunInfoScriptable gunInfo;  //���� �Ҹ��� ���� ��ũ���ͺ�


        //�޸��� ��(pivot) ��ġ
        [SerializeField] private Vector3 runningPivotPosition;      //�޸� �� pivot ��ġ
        [SerializeField] private Vector3 runningPivotRotation;      //�޸� �� pivot ����
        [SerializeField] private float runPosTime = 1;              //�޸��� �ڼ��� ��ȯ �ð�


        //�������� ź���� ����߸���
        [SerializeField] private GameObject magazine;           //ź���� ������Ʈ
        [SerializeField] private Transform magazineSpawnPos;    //ź���� ���� ��ġ

        //�ɱ�� �ٽ� �Ͼ��
        [SerializeField] Vector3 crouchInterporatePos;          //�ɱ�, �Ͼ�� ������
        [SerializeField] float crouchTime = 0.3f;               //�ɱ�, �Ͼ�� �ڼ� ��ȯ �ð�
        private Vector3 idlePos;                                //�Ͼ ���� ���� ��ġ
        private Vector3 crouchPos;                              //���� ���� ��ġ


        private bool isEquip;
        private bool isRunning;
        private bool isAiming;
        private bool canFire;
        private bool isCrouch;

        private float currentFireRatio;

        private enum FireMode
        {
            fullAuto = 0,
            semiAuto,
            burst
        }
        FireMode fireMode = FireMode.fullAuto;
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
        }

        private void Start()
        {
            armAnimator.runtimeAnimatorController = armOverrideController;

            equipmentAnimator.SetBool("Equip", true);
            armAnimator.SetBool("Equip", true);

            crossHairController.SetCrossHair(1);
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
                canFire = false;
                
                if (false) //�Ѿ��� ���� ��
                    StartCoroutine(Reload(false, gunInfo.reloadSoundClips));
                else //�Ѿ��� ���� ��
                    StartCoroutine(Reload(true, gunInfo.emptyReloadSoundClips));
            }

            if (!firstPersonController.m_IsWalking)
            {
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
                if (Input.GetKeyDown(KeyCode.LeftControl))
                {
                    isCrouch = !isCrouch;
                    StopAllCoroutines();
                    if (isCrouch)
                        StartCoroutine(CrouchPos(crouchTime, rightAxisTransform, crouchPos));
                    else
                        StartCoroutine(CrouchPos(crouchTime, rightAxisTransform, idlePos));
                }

                if (Input.GetKey(KeyCode.Mouse1))
                {
                    isAiming = true;

                    pivot.localPosition = Vector3.Lerp(pivot.localPosition, aimingPivotPosition, 0.07f);
                    pivot.localRotation = Quaternion.Lerp(pivot.localRotation, Quaternion.Euler(aimingPivotRotation), 0.07f);
                }
                else
                {
                    isAiming = false;

                    pivot.localPosition = Vector3.Lerp(pivot.localPosition, originalPivotPosition, 0.07f);
                    pivot.localRotation = Quaternion.Lerp(pivot.localRotation, Quaternion.Euler(originalPivotRotation), 0.07f);
                }
            }

            SetCurrentFireIndex();
            if (Input.GetKeyDown(KeyCode.N))
            {
                equipmentAnimator.SetTrigger("ChangeFireMode");
                armAnimator.SetTrigger("ChangeFireMode");

                audioSource.PlayOneShot(gunInfo.changeModeSound[0]);
                
                fireModeIndex = (fireModeIndex + 1) % 3;
                fireMode = (FireMode)fireModeIndex;
            }

            currentFireRatio += Time.deltaTime;
            if(currentFireRatio > fireRatio && canFire)
            {
                switch (fireMode)
                {
                    case FireMode.fullAuto:
                        if (Input.GetKey(KeyCode.Mouse0))
                            Fire();
                        break;
                    case FireMode.semiAuto:
                        if (Input.GetKeyDown(KeyCode.Mouse0))
                            Fire();
                        break;
                    case FireMode.burst:
                        if (Input.GetKeyDown(KeyCode.Mouse0))
                            StartCoroutine(BurstFire());
                        break;
                }
            }
            //if (Input.GetKey(KeyCode.Mouse0) && currentFireRatio > fireRatio && canFire)
            //{
            //    Fire();
            //}
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
            canFire = true;
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
