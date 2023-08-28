using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller.Player.Utility
{
    [System.Serializable]
    public class WeaponControlledBob : CurveControlledBob
    {
        [SerializeField] private PlayerWeaponState m_CanState;

        public override bool CanState(PlayerWeaponState playerWeaponState)
            => m_CanState == playerWeaponState;
    }
}
