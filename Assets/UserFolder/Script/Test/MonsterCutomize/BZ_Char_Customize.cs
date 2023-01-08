using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class BZ_Char_Customize : MonoBehaviour
    {
        private Renderer skinRend;
        private BZ_AssetsList materialsList;

        public enum BodyType
        {
            V1,
            V2,
            V3,
            V4
        }

        public enum ShirtType
        {
            V1,
            V2,
            V3,
            V4,
            No
        }

        public enum ShortsType
        {
            V1,
            V2,
            V3,
            V4
        }

        public BodyType bodyType;
        public ShirtType shirtType;
        public ShortsType shortsType;

        public void charCustomize(int body, int top, int bottom)
        {
            materialsList = gameObject.GetComponent<BZ_AssetsList>();
            Material[] mat;

            // Set BodyType, BottomType
            Transform curSub = gameObject.transform.Find("Geo/BigZombie");
            foreach (Transform child in curSub)
            {
                skinRend = child.gameObject.GetComponent<Renderer>();
                
                mat = new Material[2];
                mat[0] = materialsList.ClothesMaterials[bottom];
                mat[1] = materialsList.SkinMaterials[body];
                skinRend.materials = mat;
            }

            // Set ShirtType
            curSub = gameObject.transform.Find("Geo/BigZombieShirt");
            foreach (Transform child in curSub)
            {
                if (top != 4)
                {
                    curSub.gameObject.SetActive(true);
                    skinRend = child.GetComponent<Renderer>();

                    skinRend.material = materialsList.ClothesMaterials[top];
                }
                else curSub.gameObject.SetActive(false);
            }
        }

        void OnValidate()
        {
            charCustomize((int)bodyType, (int)shirtType, (int)shortsType);
        }
    }
}
