using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Test
{
    public class NormalZomibeAssetList : MonoBehaviour
    {
        [Serializable] public struct UrbanZomibe
        {
            public Material[] BodyMaterials;// = new Material[5];
            public Material[] TrousersMaterials;// = new Material[4];
            public Material[] HoodieMaterials;// = new Material[4];
            public Material[] TankTopMaterials;// = new Material[4];
        }
        [Header("Normal Zombie Materials")]
        [Space(12)] [Tooltip("보통 좀비")]
        [SerializeField] private UrbanZomibe urbanZombieStruct;

        [Serializable] public struct OldManZomibe
        {
            public Material[] BodySkinMaterials;// = new Material[2];
            public Material[] TrousersMaterials;// = new Material[2];
        }
        [Space(12)] [Tooltip("고령 좀비")]
        [SerializeField] private OldManZomibe oldManZomibeStruct;

        [Serializable] public struct WomenZombie
        {
            public Material[] BodySkinMaterials;// = new Material[2];
            public Material[] HairMaterials;// = new Material[2];
        }
        [Space(12)] [Tooltip("여성 좀비")]
        [SerializeField] private WomenZombie womenZombieStruct;

        [Serializable] public struct BigZombie
        {
            public Material[] SkinMaterials;// = new Material[4];
            public Material[] ClothesMaterials;// = new Material[4];
        }
        [Space(12)] [Tooltip("뚱뚱 좀비")]
        [SerializeField] private BigZombie bigZombieStruct;

        [Serializable] public struct GiantZombie
        {
            public Material[] BodyMaterials;// = new Material[4];
            public Material[] LowerBodyMaterials;// = new Material[4];
            public Material[] TshirtMaterials;// = new Material[4];
            public Material[] TankTopMaterials;// = new Material[4];
            public Material[] LegsMaterials;// = new Material[4];
            public Transform[] HeadTypes;// = new Transform[4];
            //HeadTypes 혼자 Transform
        }
        [Space(12)] [Tooltip("거대 좀비")]
        [SerializeField] private GiantZombie giantZombieStruct;

        public UrbanZomibe UrbanZombieStruct { get => urbanZombieStruct; private set => urbanZombieStruct = value; }
        public OldManZomibe OldManZomibeStruct { get => oldManZomibeStruct; private set => oldManZomibeStruct = value; }
        public WomenZombie WomenZombieStruct { get => womenZombieStruct; private set => womenZombieStruct = value; }
        public BigZombie BigZombieStruct { get => bigZombieStruct; private set => bigZombieStruct = value; }
        public GiantZombie GiantZombieStruct { get => giantZombieStruct; private set => giantZombieStruct = value; }
    }
}
