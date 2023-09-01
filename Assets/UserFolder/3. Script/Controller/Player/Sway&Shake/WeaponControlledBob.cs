using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller.Player.Utility
{
    [System.Serializable]
    public class WeaponControlledBob : CurveControlledBob
    {
        [SerializeField] private PlayerWeaponState m_CanState;
        [SerializeField] private bool m_CanCombinationState = true;

        public bool CanCombinationState { get => m_CanCombinationState; }

        public override bool CanState(PlayerWeaponState playerWeaponState)
            => m_CanState == playerWeaponState;
    }
}
