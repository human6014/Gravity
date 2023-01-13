using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CustomizingAssetList : MonoBehaviour
{
    [Serializable]
    public struct MaterialsStruct
    {
        [Header("Parts material info")]
        public Material[] partMaterials;
    }

    [Serializable]
    public struct NormalZomibeComponentsStruct
    {
        [Header("Parts list && info")]
        public MaterialsStruct[] materials;
    }

    [Header("Normalzombie customizing factor")]
    [SerializeField] private NormalZomibeComponentsStruct[] normalZomibeMaterials;

    public MaterialsStruct[] GetUnitMaterial(EnumType.NoramlMonsterType monsterType) => normalZomibeMaterials[(int)monsterType].materials;
}

