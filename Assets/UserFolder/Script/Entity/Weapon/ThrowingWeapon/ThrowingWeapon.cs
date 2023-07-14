using Scriptable;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Scriptable.Equipment;

namespace Entity.Object.Weapon
{
    public class ThrowingWeapon : Weapon
    {
        [Space(15)]
        [Header("Child")]
        [SerializeField] private Explosible m_Explosive;
        [SerializeField] private Transform m_SpawnPos;

        [Header("Pooling")]
        [SerializeField] private Transform m_ActiveObjectPool;

        [Header("Appear")]
        [SerializeField] private SkinnedMeshRenderer m_RendererObject;

        [SerializeField] private bool m_HasParticle;
        [SerializeField] private GameObject m_ParticleObject;

        private ThrowingWeaponSoundScriptable m_ThrowingWeaponSound;
        private ThrowingWeaponStatScriptable m_ThrowingWeaponStat;
        private Transform m_CameraTransform;
        private Manager.ObjectPoolManager.PoolingObject m_PoolingObject;

        private bool m_IsThrowing;
        private bool m_IsRunning;
        private const int m_MaxBullet = 1;
        private const float m_ReInitTime = 0.35f;

        private Coroutine m_RunningCoroutine;
        private Quaternion m_RunningPivotRotation;

        public override int MaxBullet => m_MaxBullet;
        public override bool CanChangeWeapon => base.CanChangeWeapon && !m_IsThrowing;

        public override void PreAwake()
        {
            base.PreAwake();
            m_ThrowingWeaponStat = (ThrowingWeaponStatScriptable)base.m_WeaponStatScriptable;
        }

        protected override void Awake()
        {
            base.Awake();
            m_ThrowingWeaponSound = (ThrowingWeaponSoundScriptable)base.m_WeaponSoundScriptable;

            m_RunningPivotRotation = Quaternion.Euler(m_ThrowingWeaponStat.m_RunningPivotDirection);

            m_CameraTransform = MainCamera.transform;
        }

        private void Start() => AssignPooling();

        public override void Init()
        {
            base.Init();
        }

        protected override void DoAppearObject()
        {
            bool isActive = WeaponInfo.m_CurrentRemainBullet > 0;

            m_RendererObject.enabled = isActive;
            m_ArmController.AppearArms(isActive);
            if(m_HasParticle) m_ParticleObject.SetActive(isActive);
        }

        public void ReInit()
        {
            Debug.Log("ReInit");
            m_IsThrowing = false;
            m_RendererObject.enabled = true;
            m_ArmController.AppearArms(true);
            m_ArmAnimator.SetTrigger("Equip");
            EquipmentAnimator.SetTrigger("Equip");
        }

        private void AssignPooling()
        {
            m_PoolingObject = Manager.ObjectPoolManager.Register(m_Explosive, m_ActiveObjectPool);
            m_PoolingObject.GenerateObj(2);
        }

        protected override void AssignKeyAction()
        {
            base.AssignKeyAction();
            PlayerInputController.SemiFire += TryLongThrow;
            PlayerInputController.HeavyFire += TryShortThrow;
        }

        private void Update()
        {
            if (PlayerData.PlayerState.PlayerBehaviorState == PlayerBehaviorState.Running)
            {
                if (!m_IsRunning)
                {
                    m_IsRunning = true;
                    if (m_RunningCoroutine != null) StopCoroutine(m_RunningCoroutine);
                    m_RunningCoroutine = StartCoroutine(PosChange(m_ThrowingWeaponStat.m_RunningPivotPosition, m_RunningPivotRotation));
                }
            }
            else
            {
                if (m_IsRunning)
                {
                    m_IsRunning = false;
                    if (m_RunningCoroutine != null) StopCoroutine(m_RunningCoroutine);
                    m_RunningCoroutine = StartCoroutine(PosChange(WeaponManager.OriginalPivotPosition, WeaponManager.OriginalPivotRotation));
                }
                MainCamera.fieldOfView = Mathf.Lerp(MainCamera.fieldOfView, WeaponManager.OriginalFOV, m_ThrowingWeaponStat.m_FOVMultiplier * Time.deltaTime);
            }
        }

        private IEnumerator PosChange(Vector3 EndPosition, Quaternion EndRotation)
        {
            float currentTime = 0;
            float elapsedTime;
            Vector3 startLocalPosition = m_Pivot.localPosition;
            Quaternion startLocalRotation = m_Pivot.localRotation;
            while (currentTime < m_ThrowingWeaponStat.m_RunningPosTime)
            {
                currentTime += Time.deltaTime;

                elapsedTime = currentTime / m_ThrowingWeaponStat.m_RunningPosTime;
                m_Pivot.localPosition = Vector3.Lerp(startLocalPosition, EndPosition, elapsedTime);
                m_Pivot.localRotation = Quaternion.Lerp(startLocalRotation, EndRotation, elapsedTime);

                yield return elapsedTime;
            }
        }

        private bool CanThrowing() => !m_IsThrowing && PlayerData.PlayerState.PlayerBehaviorState != PlayerBehaviorState.Running && WeaponInfo.m_CurrentRemainBullet > 0;
        private void TryLongThrow()
        {
            if (!CanThrowing()) return;
            m_ArmAnimator.SetTrigger("Long Throw");
            EquipmentAnimator.SetTrigger("Long Throw");
            StartCoroutine(PlayThrowSound(true));
        }

        private void TryShortThrow()
        {
            if (!CanThrowing()) return;
            m_ArmAnimator.SetTrigger("Short Throw");
            EquipmentAnimator.SetTrigger("Short Throw");
            StartCoroutine(PlayThrowSound(false));
        }

        private IEnumerator PlayThrowSound(bool isLong)
        {
            PlayerData.RangeWeaponFire();
            m_IsThrowing = true;
            WeaponSoundScriptable.DelaySoundClip[] playingSound = m_ThrowingWeaponSound.throwSound;
            for (int i = 0; i < playingSound.Length; i++)
            {
                yield return new WaitForSeconds(playingSound[i].delayTime);
                AudioSource.PlayOneShot(playingSound[i].audioClip);
            }
            m_RendererObject.enabled = false;
            Throw(isLong);
            EndThrow();
        }

        private void Throw(bool isLong)
        {
            if (Physics.Raycast(m_CameraTransform.position, m_CameraTransform.forward, out RaycastHit hit, 300f, m_ThrowingWeaponStat.m_AttackableLayer))
            {
                SetThrowVector(isLong, hit.point, out Vector3 forceToAdd, out Vector3 TorquToAdd);

                Explosible poolable = (Explosible)m_PoolingObject.GetObject(false);

                poolable.gameObject.SetActive(true);
                poolable.Init(m_PoolingObject, m_SpawnPos.position, m_CameraTransform.rotation, BulletType);
                poolable.TryGetComponent(out Rigidbody throwingRigid);

                throwingRigid.AddForce(forceToAdd, ForceMode.Impulse);
                throwingRigid.AddTorque(TorquToAdd);
            }
        }

        public void EndThrow()
        {
            Debug.Log("EndThrow");
            if (WeaponInfo.m_MagazineRemainBullet <= 0) m_IsThrowing = false;
            else
            {
                PlayerData.RangeWeaponCountingReload();
                Invoke(nameof(ReInit), m_ReInitTime);
            }
        }

        private void SetThrowVector(bool isLong, Vector3 hitPoint, out Vector3 forceToAdd, out Vector3 TorquToAdd)
        {
            float forwardForce;
            float upwardForce;
            if (isLong)
            {
                forwardForce = m_ThrowingWeaponStat.longThrowForwardForce;
                upwardForce = m_ThrowingWeaponStat.longThrowUpwardForce;
            }
            else
            {
                forwardForce = m_ThrowingWeaponStat.shortThrowForwardForce;
                upwardForce = m_ThrowingWeaponStat.shortThrowUpwardForce;
            }
            forceToAdd = (hitPoint - m_SpawnPos.position).normalized * forwardForce + m_CameraTransform.up * upwardForce;
            TorquToAdd = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
        }

        protected override void DischargeKeyAction()
        {
            base.DischargeKeyAction();
            PlayerInputController.SemiFire -= TryLongThrow;
            PlayerInputController.HeavyFire -= TryShortThrow;
        }
    }
}