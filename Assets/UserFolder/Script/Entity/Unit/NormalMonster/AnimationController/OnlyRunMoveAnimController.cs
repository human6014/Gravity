using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Unit.Normal
{
    public class OnlyRunMoveAnimController : NormalMonsterAnimController
    {
        public override void PlayWalk(bool active) => base.PlayRun(active);
    }
}
