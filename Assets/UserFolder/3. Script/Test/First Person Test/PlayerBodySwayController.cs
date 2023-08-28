using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Controller.Player.Utility
{
    public class PlayerBodySwayController : MonoBehaviour
    {
        [SerializeField] private float m_LerpedSpeed = 25;

        [SerializeField] private BehaviorControlledBob[] m_BehaviorControlledBobs;
        [SerializeField] private WeaponControlledBob[] m_WeaponControlledBobs;

        private FirstPersonController m_FirstPersonController;
        private PlayerState m_PlayerState;

        private void Awake()
        {
            m_PlayerState = transform.root.GetComponent<PlayerData>().PlayerState;
            m_FirstPersonController = transform.root.GetComponent<FirstPersonController>();

            foreach (BehaviorControlledBob bcb in m_BehaviorControlledBobs)
                bcb.Setup(transform, 5);
            foreach(WeaponControlledBob wcb in m_WeaponControlledBobs)
                wcb.Setup(transform, 5);
        }

        private void FixedUpdate()
        {
            Vector3 additionalPos = Vector3.zero;

            foreach(BehaviorControlledBob bcb in m_BehaviorControlledBobs)
            {
                if (bcb.CanState(m_PlayerState.PlayerBehaviorState))
                {
                    if (bcb.IsControlledExternal) additionalPos += bcb.DoMoveHeadBob(m_FirstPersonController.BobSpeed());
                    else additionalPos += bcb.DoMoveHeadBob(3);
                    break;
                }
            }

            foreach (WeaponControlledBob wcb in m_WeaponControlledBobs)
            {
                if (wcb.CanState(m_PlayerState.PlayerWeaponState))
                {
                    additionalPos += wcb.DoMoveHeadBob(3);
                    break;
                }
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition + additionalPos, Time.deltaTime * m_LerpedSpeed);
            //transform.localPosition += additionalPos;
        }
    }
}
