using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class GiantZombie_CharacterCustomize : MonoBehaviour
    {
        private Transform tshirtT;
        private Transform tanktopT;
        private Transform bodyToHideT;
        private Transform bodyExposedT;
        private Transform trousersT;
        private Transform footL_T;

        private GiantZombie_AssetsList materialsList;

        public enum FaceType
        {
            FaceV1,
            FaceV2,
            FaceV3,
            FaceV4
        }

        public enum BodySkin
        {
            BodyV1,
            BodyV2,
            BodyV3,
            BodyV4
        }

        public enum TrousersSkin
        {
            TrousersV1,
            TrousersV2,
            TrousersV3,
            TrousersV4
        }

        public enum TankTopSkin
        {
            None,
            TankTopV1,
            TankTopV2,
            TankTopV3,
            TankTopV4
        }

        public enum TshirtSkin
        {
            None,
            TshirtV1,
            TshirtV2,
            TshirtV3,
            TshirtV4
        }

        public FaceType faceType;
        public BodySkin bodySkin;
        public TrousersSkin trousersSkin;
        public TankTopSkin tanktopSkin;
        public TshirtSkin tshirtSkin;

        public void charCustomize(int body, int trousers, int tanktop, int tshirt, int head)
        {
            materialsList = GetComponent<GiantZombie_AssetsList>();

            tshirtT = transform.Find("Giant_GRP/Tshirt");
            tanktopT = transform.Find("Giant_GRP/TankTop");
            trousersT = transform.Find("Giant_GRP/Trousers");
            bodyToHideT = transform.Find("Giant_GRP/Giant_Zombie_SECTIONS/Torso");
            bodyExposedT = transform.Find("Giant_GRP/Giant_Zombie_SECTIONS/Arms");
            footL_T = transform.Find("Giant_GRP/Giant_Zombie_SECTIONS/Foot_L");

            for (int i = 0; i <= 3; i++)
                materialsList.HeadTypes[i].gameObject.SetActive(false);

            materialsList.HeadTypes[head].gameObject.SetActive(true);

            Renderer skinRend;
            foreach (Transform child in materialsList.HeadTypes[head])
            {
                skinRend = child.GetComponent<Renderer>();
                skinRend.material = materialsList.BodyMaterials[body];
            }

            // Body_Exposed arms
            foreach(Transform child in bodyExposedT)
            {
                skinRend = child.GetComponent<Renderer>();
                skinRend.material = materialsList.BodyMaterials[body];
            }


            //Torso to hide
            foreach(Transform child in bodyToHideT)
            {
                skinRend = child.GetComponent<Renderer>();
                skinRend.material = materialsList.BodyMaterials[body];
            }

            foreach(Transform child in footL_T)
            {
                skinRend = child.GetComponent<Renderer>();
                skinRend.material = materialsList.LegsMaterials[body];
            }

            //Trousers
            foreach(Transform child in trousersT)
            {
                skinRend = child.GetComponent<Renderer>();
                skinRend.material = materialsList.LowerBodyMaterials[trousers];
            }


            // Tshirt
            if (tshirt < 1)
            {
                tshirtT.gameObject.SetActive(false);
                bodyToHideT.gameObject.SetActive(true);
            }
            else
            {
                tshirtT.gameObject.SetActive(true);
                bodyToHideT.gameObject.SetActive(false);

                foreach(Transform child in tshirtT)
                {
                    skinRend = child.GetComponent<Renderer>();
                    skinRend.material = materialsList.TshirtMaterials[tshirt - 1];
                }
            }

            // TankTop
            // Tshirt가 우선권이 있음
            if (tanktop < 1 || tshirt > 0)
            {
                tanktopT.gameObject.SetActive(false);
                tanktopSkin = 0;
            }
            else
            {
                tanktopT.gameObject.SetActive(true);
                bodyToHideT.gameObject.SetActive(true);

                foreach(Transform child in tanktopT)
                {
                    skinRend = child.GetComponent<Renderer>();
                    skinRend.material = materialsList.TankTopMaterials[tanktop - 1];
                }

            }
        }

        void OnValidate()
        {
            charCustomize((int)bodySkin, (int)trousersSkin, (int)tanktopSkin, (int)tshirtSkin, (int)faceType);
        }
    }
}
