using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller.Player.Utility
{
    [System.Serializable]
    public class BehaviorControlledBob : CurveControlledBob
    {
        [SerializeField] private PlayerBehaviorState m_CanState;
        [SerializeField] private bool m_IsControlledExternal;

        public bool IsControlledExternal { get => m_IsControlledExternal; }

        public override bool CanState(PlayerBehaviorState playerBehaviorState)
            => m_CanState == playerBehaviorState;
    }
}
