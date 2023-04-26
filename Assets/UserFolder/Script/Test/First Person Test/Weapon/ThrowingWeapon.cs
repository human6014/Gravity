using Scriptable;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Test
{
    public class ThrowingWeapon : Weapon
    {
        [Space(15)]
        [Header("Child")]
        [SerializeField] private Explosible m_Explosive;

        [SerializeField] private Transform m_SpawnPos;

        [Header("Pooling")]
        [SerializeField] private Transform m_ActiveObjectPool;

        [SerializeField] private GameObject m_RendererObject;
        private ThrowingWeaponSoundScriptable m_ThrowingWeaponSound;
        private ThrowingWeaponStatScriptable m_ThrowingWeaponStat;
        private Transform m_CameraTransform;
        private Manager.ObjectPoolManager.PoolingObject m_PoolingObject;

        public override bool CanChangeWeapon => base.CanChangeWeapon && !m_IsThrowing;

        private bool m_IsThrowing;
        private bool m_IsRunning;
        private const int m_MaxBullet = 1;
        private int m_TempHasCount = 1;

        private Coroutine m_RunningCoroutine;
        private Quaternion m_RunningPivotRotation;
        protected override void Awake()
        {
            base.Awake();
            m_ThrowingWeaponSound = (ThrowingWeaponSoundScriptable)base.m_WeaponSoundScriptable;
            m_ThrowingWeaponStat = (ThrowingWeaponStatScriptable)base.m_WeaponStatScriptable;

            m_RunningPivotRotation = Quaternion.Euler(m_ThrowingWeaponStat.m_RunningPivotDirection);

            m_CameraTransform = m_MainCamera.transform;
        }

        private void Start() => AssignPooling();

        private void ReInit()
        {
            m_RendererObject.SetActive(true);
            m_ArmAnimator.SetTrigger("Equip");
            m_EquipmentAnimator.SetTrigger("Equip");
        }

        private void AssignPooling()
        {
            m_PoolingObject = Manager.ObjectPoolManager.Register(m_Explosive, m_ActiveObjectPool);
            m_PoolingObject.GenerateObj(1);
        }

        protected override void AssignKeyAction()
        {
            base.AssignKeyAction();
            m_PlayerInputController.SemiFire += LongThrow;
            m_PlayerInputController.HeavyFire += ShortThrow;
        }

        private void Update()
        {
            if (m_PlayerState.PlayerBehaviorState == PlayerBehaviorState.Running)
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
                    m_RunningCoroutine = StartCoroutine(PosChange(m_WeaponManager.m_OriginalPivotPosition, m_WeaponManager.m_OriginalPivotRotation));
                }
                m_MainCamera.fieldOfView = Mathf.Lerp(m_MainCamera.fieldOfView, m_WeaponManager.m_OriginalFOV, m_ThrowingWeaponStat.m_FOVMultiplier * Time.deltaTime);
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

        private void LongThrow()
        {
            if (m_IsThrowing || m_PlayerState.PlayerBehaviorState == PlayerBehaviorState.Running || m_WeaponInfo.m_CurrentRemainBullet <= 0) return;
            m_ArmAnimator.SetTrigger("Long Throw");
            m_EquipmentAnimator.SetTrigger("Long Throw");
            StartCoroutine(PlayThrowSound(true));
        }

        private void ShortThrow()
        {
            if (m_IsThrowing || m_PlayerState.PlayerBehaviorState == PlayerBehaviorState.Running || m_WeaponInfo.m_CurrentRemainBullet <= 0) return;
            m_ArmAnimator.SetTrigger("Short Throw");
            m_EquipmentAnimator.SetTrigger("Short Throw");
            StartCoroutine(PlayThrowSound(false));
        }

        private IEnumerator PlayThrowSound(bool isLong)
        {
            m_PlayerData.RangeWeaponFire(m_MaxBullet);
            m_IsThrowing = true;
            WeaponSoundScriptable.DelaySoundClip[] playingSound = m_ThrowingWeaponSound.throwSound;
            for (int i = 0; i < playingSound.Length; i++)
            {
                yield return new WaitForSeconds(playingSound[i].delayTime);
                m_AudioSource.PlayOneShot(playingSound[i].audioClip);
            }
            m_RendererObject.SetActive(false);
            Throw(isLong);
            m_PlayerData.RangeWeaponReload(1);
        }

        private void Throw(bool isLong)
        {
            if(Physics.Raycast(m_CameraTransform.position, m_CameraTransform.forward, out RaycastHit hit, 300f, m_ThrowingWeaponStat.m_AttackableLayer))
            {
                SetThrowVector(isLong, hit.point, out Vector3 forceToAdd, out Vector3 TorquToAdd);

                Explosible poolable = (Explosible)m_PoolingObject.GetObject(false);

                poolable.gameObject.SetActive(true);
                poolable.Init(m_PoolingObject, m_SpawnPos.position, m_CameraTransform.rotation);
                poolable.TryGetComponent(out Rigidbody throwingRigid);

                throwingRigid.AddForce(forceToAdd, ForceMode.Impulse);
                throwingRigid.AddTorque(TorquToAdd);
            }
            m_IsThrowing = false;
        }

        private void EndThrow()
        {
            if (m_TempHasCount > 0)
            {

            }
            else
            {

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

        public override Task UnEquip()
        {
            m_RendererObject.SetActive(true);
            return base.UnEquip();
        }

        /*
        public override void Dispose()
        {
            m_RendererObject.SetActive(true);
            base.Dispose();
        }
        */

        protected override void DischargeKeyAction()
        {
            base.DischargeKeyAction();
            m_PlayerInputController.SemiFire -= LongThrow;
            m_PlayerInputController.HeavyFire -= ShortThrow;
        }
    }
}
