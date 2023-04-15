using Scriptable;
using System.Collections;
using System.Collections.Generic;
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
        private Transform m_MainCamera;
        private Manager.ObjectPoolManager.PoolingObject m_PoolingObject;

        private bool m_IsThrowing;

        private int m_TempHasCount = 1;

        protected override void Awake()
        {
            base.Awake();
            m_ThrowingWeaponSound = (ThrowingWeaponSoundScriptable)base.m_WeaponSoundScriptable;
            m_ThrowingWeaponStat = (ThrowingWeaponStatScriptable)base.m_WeaponStatScriptable;
            m_MainCamera = Camera.main.transform;
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
            m_PlayerInputController.SemiFire += LongThrow;
            m_PlayerInputController.HeavyFire += ShortThrow;
        }

        private void LongThrow()
        {
            if (m_IsThrowing) return;
            m_ArmAnimator.SetTrigger("Long Throw");
            m_EquipmentAnimator.SetTrigger("Long Throw");
            StartCoroutine(PlayThrowSound(true));
        }

        private void ShortThrow()
        {
            if (m_IsThrowing) return;
            m_ArmAnimator.SetTrigger("Short Throw");
            m_EquipmentAnimator.SetTrigger("Short Throw");
            StartCoroutine(PlayThrowSound(false));
        }

        private IEnumerator PlayThrowSound(bool isLong)
        {
            m_IsThrowing = true;
            WeaponSoundScriptable.DelaySoundClip[] playingSound = m_ThrowingWeaponSound.throwSound;
            for (int i = 0; i < playingSound.Length; i++)
            {
                yield return new WaitForSeconds(playingSound[i].delayTime);
                m_AudioSource.PlayOneShot(playingSound[i].audioClip);
            }
            m_RendererObject.SetActive(false);
            Throw(isLong);
        }

        private void Throw(bool isLong)
        {
            if(Physics.Raycast(m_MainCamera.position, m_MainCamera.forward, out RaycastHit hit, 300f, m_ThrowingWeaponStat.m_AttackableLayer))
            {
                SetThrowVector(isLong, hit.point, out Vector3 forceToAdd, out Vector3 TorquToAdd);

                Explosible poolable = (Explosible)m_PoolingObject.GetObject(false);

                poolable.gameObject.SetActive(true);
                poolable.Init(m_PoolingObject, m_SpawnPos.position, m_MainCamera.rotation);
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
            forceToAdd = (hitPoint - m_SpawnPos.position).normalized * forwardForce + m_MainCamera.up * upwardForce;
            TorquToAdd = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
        }

        public override void Dispose()
        {
            m_RendererObject.SetActive(true);
            base.Dispose();
        }

        protected override void DischargeKeyAction()
        {
            m_PlayerInputController.SemiFire -= LongThrow;
            m_PlayerInputController.HeavyFire -= ShortThrow;
        }
    }
}
