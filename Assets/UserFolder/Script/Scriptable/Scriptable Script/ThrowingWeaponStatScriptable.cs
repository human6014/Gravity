using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable.Equipment
{

    [CreateAssetMenu(fileName = "ThrowingWeaponStatSetting", menuName = "Scriptable Object/ThrowingWeaponStatSettings", order = int.MaxValue - 10)]
    public class ThrowingWeaponStatScriptable : WeaponStatScriptable
    {
        [Space(15)]
        [Header("Child")]
        [Header("Long throw force")]
        public float longThrowForwardForce;
        public float longThrowUpwardForce;

        [Header("Short throw force")]
        public float shortThrowForwardForce;
        public float shortThrowUpwardForce;

        public int initHavingBullet;
    }
}
