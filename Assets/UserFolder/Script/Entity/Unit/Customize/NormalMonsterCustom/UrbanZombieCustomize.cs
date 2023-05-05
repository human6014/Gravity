using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Entity.Unit
{
    public class UrbanZombieCustomize : CustomizableScript
    {
        private Transform hoodieT;
        private Transform tanktopT;
        private Transform bodyToHideT;
        private Transform bodyExposedHandT;
        private Transform bodyExposedTrouserT;
        private Transform headT_A;
        private Transform headT_B;

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
        private void Awake()
        {
            hoodieT = transform.Find("Geo/Hoodie");
            tanktopT = transform.Find("Geo/TankTop");
            bodyToHideT = transform.Find("Geo/Body_ToHide");
            bodyExposedHandT = transform.Find("Geo/Hands");
            bodyExposedTrouserT = transform.Find("Geo/Trousers");

            headT_A = transform.Find("Geo/HeadA");
            headT_B = transform.Find("Geo/HeadB");
        }

        protected override void RandNum()
        {
            bodyType = Random.Range(0, bodyTypeLength);
            trouserType = Random.Range(0, trouserTypeLength);
            tanktopType = Random.Range(-1, tanktopTypeLength);
            hoodieType = Random.Range(-1, hoodieTypeLength);
            headType = Random.Range(0, faceTypeLength);
        }

        public override void Customizing(ref CustomizingAssetList.MaterialsStruct[] materialsStructs)
        {
            RandNum();
            Renderer skinRend;
            Material[] mat;
            foreach (Transform child in bodyExposedHandT)
            {
                skinRend = child.GetComponent<Renderer>();
                skinRend.material = materialsStructs[0].partMaterials[bodyType];
            }

            foreach (Transform child in bodyExposedTrouserT)
            {
                skinRend = child.GetComponent<Renderer>();
                skinRend.material = materialsStructs[1].partMaterials[trouserType];
            }

            if(headType == 0)
            {
                // Body_Exposed HeadA
                foreach (Transform child in headT_A)
                {
                    skinRend = child.GetComponent<Renderer>();
                    skinRend.material = materialsStructs[0].partMaterials[bodyType];
                }
            }
            else
            {
                // Body_Exposed HeadB
                foreach (Transform child in headT_B)
                {
                    skinRend = child.GetComponent<Renderer>();
                    skinRend.material = materialsStructs[0].partMaterials[bodyType];
                }
            }
            //Head Type
            headT_A.gameObject.SetActive(headType != 1);
            headT_B.gameObject.SetActive(headType == 1);

            //BodyToHide
            foreach (Transform child in bodyToHideT)
            {
                skinRend = child.GetComponent<Renderer>();
                mat = new Material[2];
                mat[0] = materialsStructs[0].partMaterials[bodyType];
                mat[1] = materialsStructs[1].partMaterials[trouserType];

                skinRend.materials = mat;
            }

            // TankTop
            if (tanktopType < 1) tanktopT.gameObject.SetActive(false);
            else
            {
                tanktopT.gameObject.SetActive(true);
                bodyToHideT.gameObject.SetActive(true);
                hoodieT.gameObject.SetActive(false);
                foreach (Transform child in tanktopT)
                {
                    skinRend = child.GetComponent<Renderer>();
                    skinRend.material = materialsStructs[3].partMaterials[tanktopType - 1];
                }
                return;
            }

            // Hoodie
            if (hoodieType < 1)
            {
                hoodieT.gameObject.SetActive(false);
                bodyToHideT.gameObject.SetActive(true);
            }
            else
            {
                hoodieT.gameObject.SetActive(true);
                bodyToHideT.gameObject.SetActive(false);
                foreach (Transform child in hoodieT)
                {
                    skinRend = child.GetComponent<Renderer>();
                    skinRend.material = materialsStructs[2].partMaterials[hoodieType - 1];
                }
                tanktopT.gameObject.SetActive(false);
            }
        }
    }
}

