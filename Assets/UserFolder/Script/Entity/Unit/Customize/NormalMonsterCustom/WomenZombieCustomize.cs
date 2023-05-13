using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Unit
{
    public class WomenZombieCustomize : CustomizableScript
    {
        [Header("Original Model")]
        [SerializeField] private Transform bodyT;

        [Header("RagDoll Model")]
        [SerializeField] private Transform bodyT_RagDoll;

        private readonly int bodyTypeLength = 2;
        private readonly int hairTypeLength = 2;

        private int bodyType;
        private int hairType;

        protected override void RandNum()
        {
            bodyType = Random.Range(0, bodyTypeLength);
            hairType = Random.Range(0, hairTypeLength);
        }

        public override void Customizing(ref CustomizingAssetList.MaterialsStruct[] materialsStructs)
        {
            RandNum();

            ChangeParts(ref materialsStructs, bodyT);
            ChangeParts(ref materialsStructs, bodyT_RagDoll);
        }

        private void ChangeParts(ref CustomizingAssetList.MaterialsStruct[] materialsStructs, Transform body)
        {
            Renderer skinnedRenderer;
            Material[] mat = new Material[2];

            foreach (Transform child in body)
            {
                skinnedRenderer = child.GetComponent<Renderer>();

                mat[0] = materialsStructs[0].partMaterials[bodyType];
                mat[1] = materialsStructs[1].partMaterials[hairType];

                skinnedRenderer.materials = mat;
            }
        }
    }
}

