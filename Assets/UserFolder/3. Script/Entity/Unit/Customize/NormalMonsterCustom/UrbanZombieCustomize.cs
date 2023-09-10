using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Entity.Unit
{
    public class UrbanZombieCustomize : CustomizableScript
    {
        [Header("Original Model")]
        [SerializeField] private Transform hoodieT;
        [SerializeField] private Transform tanktopT;
        [SerializeField] private Transform bodyToHideT;
        [SerializeField] private Transform bodyExposedHandT;
        [SerializeField] private Transform bodyExposedTrouserT;
        [SerializeField] private Transform headT_A;
        [SerializeField] private Transform headT_B;

        [Header("RagDoll Model")]
        [SerializeField] private Transform hoodieT_Doll;
        [SerializeField] private Transform tanktopT_Doll;
        [SerializeField] private Transform bodyToHideT_Doll;
        [SerializeField] private Transform bodyExposedHandT_Doll;
        [SerializeField] private Transform bodyExposedTrouserT_Doll;
        [SerializeField] private Transform headT_A_Doll;
        [SerializeField] private Transform headT_B_Doll;

        private readonly int bodyTypeLength = 5;
        private readonly int trouserTypeLength = 4;
        private readonly int tanktopTypeLength = 5;
        private readonly int hoodieTypeLength = 5;
        private readonly int faceTypeLength = 2;

        private int bodyType = 0;
        private int trouserType = 0;
        private int tanktopType = 0;
        private int hoodieType = 0;
        private int headType = 0;

        protected override void RandNum()
        {
            bodyType = Random.Range(0, bodyTypeLength);
            trouserType = Random.Range(0, trouserTypeLength);
            tanktopType = Random.Range(-1, tanktopTypeLength);
            hoodieType = Random.Range(-1, hoodieTypeLength);
            headType = Random.Range(0, faceTypeLength);
        }

        public override void Customizing(ref CustomizingAssetList.MaterialsStruct[] materialStructs)
        {
            RandNum();

            ChangeParts(ref materialStructs, bodyExposedHandT, bodyExposedTrouserT, 
                headT_A, headT_B, bodyToHideT, tanktopT, hoodieT);
            ChangeParts(ref materialStructs, bodyExposedHandT_Doll, bodyExposedTrouserT_Doll, 
                headT_A_Doll, headT_B_Doll, bodyToHideT_Doll, tanktopT_Doll, hoodieT_Doll);
        }

        private void ChangeParts(ref CustomizingAssetList.MaterialsStruct[] materialStructs, 
            Transform bodyExposedHand, Transform bodyExposedTrouser, Transform head_A, Transform head_B, Transform bodyToHide,
            Transform tanktop, Transform hoodie)
        {
            Renderer skinRend;
            Material[] mat;
            foreach (Transform child in bodyExposedHand)
            {
                skinRend = child.GetComponent<Renderer>();
                skinRend.sharedMaterial = materialStructs[0].partMaterials[bodyType];
            }

            foreach (Transform child in bodyExposedTrouser)
            {
                skinRend = child.GetComponent<Renderer>();
                skinRend.sharedMaterial = materialStructs[1].partMaterials[trouserType];
            }

            if (headType == 0)
            {
                // Body_Exposed HeadA
                foreach (Transform child in head_A)
                {
                    skinRend = child.GetComponent<Renderer>();
                    skinRend.sharedMaterial = materialStructs[0].partMaterials[bodyType];
                }
            }
            else
            {
                // Body_Exposed HeadB
                foreach (Transform child in head_B)
                {
                    skinRend = child.GetComponent<Renderer>();
                    skinRend.sharedMaterial = materialStructs[0].partMaterials[bodyType];
                }
            }
            //Head Type
            head_A.gameObject.SetActive(headType != 1);
            head_B.gameObject.SetActive(headType == 1);

            //BodyToHide
            foreach (Transform child in bodyToHide)
            {
                skinRend = child.GetComponent<Renderer>();
                mat = new Material[2];
                mat[0] = materialStructs[0].partMaterials[bodyType];
                mat[1] = materialStructs[1].partMaterials[trouserType];

                skinRend.sharedMaterials = mat;
            }

            // TankTop
            if (tanktopType < 1) tanktop.gameObject.SetActive(false);
            else
            {
                tanktop.gameObject.SetActive(true);
                bodyToHide.gameObject.SetActive(true);
                hoodie.gameObject.SetActive(false);
                foreach (Transform child in tanktop)
                {
                    skinRend = child.GetComponent<Renderer>();
                    skinRend.sharedMaterial = materialStructs[3].partMaterials[tanktopType - 1];
                }
                return;
            }

            // Hoodie
            if (hoodieType < 1)
            {
                hoodie.gameObject.SetActive(false);
                bodyToHide.gameObject.SetActive(true);
            }
            else
            {
                hoodie.gameObject.SetActive(true);
                bodyToHide.gameObject.SetActive(false);
                foreach (Transform child in hoodie)
                {
                    skinRend = child.GetComponent<Renderer>();
                    skinRend.sharedMaterial = materialStructs[2].partMaterials[hoodieType - 1];
                }
                tanktop.gameObject.SetActive(false);
            }
        }
    }
}

