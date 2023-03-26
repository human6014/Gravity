using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scriptable
{
    [CreateAssetMenu(fileName = "GunInfoSetting", menuName = "Scriptable Object/GunInfoSettings", order = int.MaxValue - 5)]
    public class GunInfoScriptable : ScriptableObject
    {
        public AudioClip[] fireSound;
        public AudioClip[] ReloadSound;
        public AudioClip[] emptySound;
        public AudioClip[] changeModeSound;
    }
}
