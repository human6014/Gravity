using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "CrossHairSetting", menuName = "Scriptable Object/CrossHairSettings", order = int.MaxValue - 4)]
    public class CrossHairScripatble : ScriptableObject
    {
        public Sprite [] crossHairSprite;
    }
}
