using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Scriptable.Monster;

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

    public ref MaterialsStruct[] GetUnitMaterial(NoramlMonsterType monsterType) 
        => ref normalZomibeMaterials[(int)monsterType].materials;
}

