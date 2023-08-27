using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Unit
{
    public class OldManCustomize : CustomizableScript
    {
        [Header("Original Model")]
        [SerializeField] private Transform bodyT;

        [Header("RagDoll Model")]
        [SerializeField] private Transform bodyT_Doll;

        private readonly int bodyTypeLength = 2;
        private readonly int trouserTypeLength = 2;

        private int bodyType;
        private int trouserType;

        protected override void RandNum()
        {
            bodyType = Random.Range(0, bodyTypeLength);
            trouserType = Random.Range(0, trouserTypeLength);
        }

        public override void Customizing(ref CustomizingAssetList.MaterialsStruct[] materialStructs)
        {
            RandNum();

            ChangeParts(ref materialStructs, bodyT);
            ChangeParts(ref materialStructs, bodyT_Doll);
        }

        private void ChangeParts(ref CustomizingAssetList.MaterialsStruct[] materialStructs, Transform body)
        {
            Renderer skinnedRenderer;
            Material[] mat = new Material[2];

            foreach (Transform child in bodyT)
            {
                skinnedRenderer = child.GetComponent<Renderer>();

                mat[0] = materialStructs[0].partMaterials[bodyType];
                mat[1] = materialStructs[1].partMaterials[trouserType];

                skinnedRenderer.materials = mat;
            }
        }
    }
}

