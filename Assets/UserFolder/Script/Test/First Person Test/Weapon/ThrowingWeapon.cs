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
        [SerializeField] private Explosive m_Explosive;

        [SerializeField] private ThrowingWeaponSoundScriptable m_ThrowingWeaponSound;
        [SerializeField] private ThrowingWeaponStatScriptable m_ThrowingWeaponStat;

        [SerializeField] private Transform m_SpawnPos;
        [SerializeField] private bool m_IsBounce;

        private Transform m_MainCamera;
        private Manager.ObjectPoolManager.PoolingObject m_PoolingObject;
        protected override void Awake()
        {
            base.Awake();
            m_MainCamera = Camera.main.transform;
        }

        private void Start()
        {
            AssignPooling();
        }

        public override void Init()
        {
            base.Init();
            AssignKeyAction();
        }

        private void AssignPooling()
        {
            m_PoolingObject = Manager.ObjectPoolManager.Register(m_Explosive, transform);
            m_PoolingObject.GenerateObj(3);
        }

        private void AssignKeyAction()
        {
            m_PlayerInputController.SemiFire += LongThrow;
            m_PlayerInputController.HeavyFire += ShortThrow;
        }

        private void LongThrow()
        {
            m_ArmAnimator.SetTrigger("Long Throw");
            m_EquipmentAnimator.SetTrigger("Long Throw");
            StartCoroutine(PlayThrowSound());
        }

        private void ShortThrow()
        {
            m_ArmAnimator.SetTrigger("Short Throw");
            m_EquipmentAnimator.SetTrigger("Short Throw");
            StartCoroutine(PlayThrowSound());
        }

        private IEnumerator PlayThrowSound()
        {
            WeaponSoundScriptable.DelaySoundClip[] playingSound = m_ThrowingWeaponSound.throwSound;
            for (int i = 0; i < playingSound.Length; i++)
            {
                yield return new WaitForSeconds(playingSound[i].delayTime);
                m_AudioSource.PlayOneShot(playingSound[i].audioClip);
            }
            Throw();
        }

        private void Throw()
        {
            if(Physics.Raycast(m_MainCamera.position, m_MainCamera.forward, out RaycastHit hit, 300f, m_AttackableLayer))
            {
                Explosive poolable = (Explosive)m_PoolingObject.GetObject(false);
                GameObject obj = Instantiate(poolable.gameObject, m_SpawnPos.position, m_MainCamera.rotation);
                obj.SetActive(true);
                obj.TryGetComponent(out Rigidbody throwingRigid);

                Vector3 forceToAdd = (hit.point - m_SpawnPos.position).normalized * m_ThrowingWeaponStat.longThrowForwardForce
                    + m_MainCamera.up * m_ThrowingWeaponStat.longThrowUpwardForce;
                throwingRigid.AddForce(forceToAdd, ForceMode.Impulse);

                poolable.Init(m_PoolingObject);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            DischargeKeyAction();
        }

        private void DischargeKeyAction()
        {
            m_PlayerInputController.SemiFire -= LongThrow;
            m_PlayerInputController.HeavyFire -= ShortThrow;
        }
    }
}
