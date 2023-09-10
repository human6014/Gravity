using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Unit
{
    public class BigZomibeCustomize : CustomizableScript
    {
        private readonly int bodyTypeLength = 4;
        private readonly int shirtTypeLength = 5;
        private readonly int trouserTypeLength = 4;

        private int bodyType;
        private int shirtType;
        private int trouserType;

        [Header("Original Model")]
        [SerializeField] private Transform bodyT;
        [SerializeField] private Transform shirtT;

        [Header("RagDoll Model")]
        [SerializeField] private Transform bodyT_RagDoll;
        [SerializeField] private Transform shirtT_RagDoll;

        protected override void RandNum()
        {
            bodyType = Random.Range(0, bodyTypeLength);
            shirtType = Random.Range(0, shirtTypeLength);
            trouserType = Random.Range(0, trouserTypeLength);
        }

        public override void Customizing(ref CustomizingAssetList.MaterialsStruct[] materialStructs)
        {
            RandNum();

            ChangeParts(ref materialStructs, bodyT, shirtT);
            ChangeParts(ref materialStructs, bodyT_RagDoll, shirtT_RagDoll);
        }

        private void ChangeParts(ref CustomizingAssetList.MaterialsStruct[] materialStructs, Transform body, Transform shirt)
        {
            Renderer skinRend;
            Material[] mat = new Material[2];
            
            foreach (Transform child in body)
            {
                skinRend = child.GetComponent<Renderer>();

                mat[0] = materialStructs[1].partMaterials[trouserType];
                mat[1] = materialStructs[0].partMaterials[bodyType];
                skinRend.sharedMaterials = mat;
            }

            if (shirtType < 1) shirt.gameObject.SetActive(false);
            else
            {
                shirt.gameObject.SetActive(true);
                foreach (Transform child in shirt)
                {
                    skinRend = child.GetComponent<Renderer>();
                    skinRend.sharedMaterial = materialStructs[0].partMaterials[shirtType - 1];
                }
            }
        }
    }
}

