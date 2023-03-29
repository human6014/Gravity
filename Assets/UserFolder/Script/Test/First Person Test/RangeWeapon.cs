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

        //�ִϸ��̼�
        private Animator m_EquipmentAnimator; //���� �ڽ��� ���� ���ϸ�����
        [SerializeField] private Animator armAnimator; //�� �ִϸ�����
        [SerializeField] private AnimatorOverrideController equipmentOverrideController = null; // ����� ���� �ִϸ��̼ǵ�
        [SerializeField] private AnimatorOverrideController armOverrideController = null;   // ����� �� �ִϸ��̼ǵ�

        //���ؽ� ���� ��ġ
        [SerializeField] private Transform pivot; //��ġ ������ �θ� ������Ʈ
        [SerializeField] private Vector3 aimingPivotPosition;   //��ġ ������ �ű� ��ġ
        [SerializeField] private Vector3 aimingPivotRotation;   //��ġ ������ �ű� ����
        private Vector3 m_OriginalPivotPosition;  //��ġ ������ �θ� ������Ʈ ���� ��ġ
        private Vector3 m_OriginalPivotRotation;  //��ġ ������ �θ� ������Ʈ ���� ����


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
        [SerializeField] private Scriptable.RangeWeaponSoundScriptable m_WeaponSound;  //���� �Ҹ��� ���� ��ũ���ͺ�


        //�޸��� ��(pivot) ��ġ
        [SerializeField] private Vector3 runningPivotPosition;      //�޸� �� pivot ��ġ
        [SerializeField] private Vector3 runningPivotRotation;      //�޸� �� pivot ����
        [SerializeField] private float runPosTime = 1;              //�޸��� �ڼ��� ��ȯ �ð�


        //�������� ź���� ����߸���
        [SerializeField] private GameObject magazine;           //ź���� ������Ʈ
        [SerializeField] private Transform magazineSpawnPos;    //ź���� ���� ��ġ

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
            //Awake�ڸ� �ƴ�
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

        //Arm �ִϸ����� ������
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
                GameObject effectObject; //Ǯ�� ����
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
