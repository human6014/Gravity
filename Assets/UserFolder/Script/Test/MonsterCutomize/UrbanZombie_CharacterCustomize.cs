using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
	[RequireComponent(typeof(UrbanZombie_AssetsList))]
	public class UrbanZombie_CharacterCustomize : MonoBehaviour
	{
		private bool tanktopOld;

		private Transform hoodieT;
		private Transform tanktopT;
		private Transform bodyToHideT;
		private Transform bodyExposedHandT;
		private Transform bodyExposedTrouserT;
		private Transform headT_A;
		private Transform headT_B;

		private UrbanZombie_AssetsList materialsList;

		public enum FaceType
		{
			V1,
			V2
		}

		public enum BodySkin
		{
			V1,
			V2,
			V3,
			V4,
			V5
		}

		public enum TrousersSkin
		{
			V1,
			V2,
			V3,
			V4
		}

		public enum TankTopSkin
		{
			None,
			V1,
			V2,
			V3,
			V4
		}
		public enum HoodieSkin
		{
			None,
			V1,
			V2,
			V3,
			V4
		}

		public FaceType faceType;
		public BodySkin bodyType;
		public TrousersSkin trousersType;
		public TankTopSkin tanktopType;
		public HoodieSkin hoodieType;

        private void Awake()
        {
            
        }

        public void charCustomize(int body, int trousers, int tanktop, int hoodie, int head)
		{
			materialsList = GetComponent<UrbanZombie_AssetsList>();
			
			hoodieT = transform.Find("Geo/Hoodie");
			tanktopT = transform.Find("Geo/TankTop");
			bodyToHideT = transform.Find("Geo/Body_ToHide");
			bodyExposedHandT = transform.Find("Geo/BodyExposed/Hands");
			bodyExposedTrouserT = transform.Find("Geo/BodyExposed/Trousers");

			headT_A = transform.Find("Geo/BodyExposed/HeadA");
			headT_B = transform.Find("Geo/BodyExposed/HeadB");

			// Body_Exposed hands
			foreach(Transform child in bodyExposedHandT)
            {
				Renderer skinRend = child.GetComponent<Renderer>();
				skinRend.material = materialsList.BodyMaterials[body];
			}

			foreach (Transform child in bodyExposedTrouserT)
			{
				Renderer skinRend = child.GetComponent<Renderer>();
				skinRend.material = materialsList.TrousersMaterials[trousers];
			}

			// Body_Exposed HeadA
			foreach (Transform child in headT_A)
			{
				Renderer skinRend = child.GetComponent<Renderer>();
				skinRend.material = materialsList.BodyMaterials[body];
			}

			// Body_Exposed HeadB
			foreach (Transform child in headT_B)
            {
				Renderer skinRend = child.GetComponent<Renderer>();
				skinRend.material = materialsList.BodyMaterials[body];
			}

			//Head Type
			headT_A.gameObject.SetActive(head != 1);
			headT_B.gameObject.SetActive(head == 1);

			//BodyToHide
			Material[] mat;
			foreach (Transform child in bodyToHideT)
            {
				Renderer skinRend = child.GetComponent<Renderer>();
				mat = new Material[2];
				mat[0] = materialsList.BodyMaterials[body];
				mat[1] = materialsList.TrousersMaterials[trousers];

				skinRend.materials = mat;
			}

			// Hoodie
			if (hoodie < 1)
			{
				hoodieT.gameObject.SetActive(false);
				bodyToHideT.gameObject.SetActive(true);
			}
			else
			{
				hoodieT.gameObject.SetActive(true);
				bodyToHideT.gameObject.SetActive(false);
				foreach(Transform child in hoodieT)
                {
					Renderer skinRend = child.GetComponent<Renderer>();
					skinRend.material = materialsList.HoodieMaterials[hoodie - 1];
				}

				if (tanktopOld)
				{
					tanktopT.gameObject.SetActive(false);
					tanktopType = 0;
					tanktopOld = false;
					tanktop = 0;
				}
			}

			// TankTop
			if (tanktop < 1) tanktopT.gameObject.SetActive(false);
			else
			{
				tanktopT.gameObject.SetActive(true);
				bodyToHideT.gameObject.SetActive(true);
				foreach(Transform child in tanktopT)
                {
					Renderer skinRend = child.GetComponent<Renderer>();
					skinRend.material = materialsList.TankTopMaterials[tanktop - 1];
				}
				tanktopOld = true;
				hoodieT.gameObject.SetActive(false);
				hoodieType = 0;
			}
		}

		void OnValidate()
		{
			charCustomize((int)bodyType, (int)trousersType, (int)tanktopType, (int)hoodieType, (int)faceType);
		}
	}
}
