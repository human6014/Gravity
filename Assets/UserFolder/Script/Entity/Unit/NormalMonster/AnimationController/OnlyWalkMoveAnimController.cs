using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Unit.Normal
{
    public class OnlyWalkMoveAnimController : NormalMonsterAnimController
    {
        public override void PlayRun(bool active) => base.PlayWalk(active);
    }
}
