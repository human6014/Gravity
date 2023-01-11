using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
	public class OldManCharCustomize : MonoBehaviour
	{
		public enum BodySkin
		{
			V1 = 0,
			V2 = 1,
		}

		public enum TrousersSkin
		{
			V1 = 0,
			V2 = 1,
		}


		public BodySkin bodySkin;
		public TrousersSkin trousersSkin;
		OldManAssetList materialsList;
		Renderer skinnedRenderer;

		public void charCustomize(int body, int trousers)
		{
			materialsList = GetComponent<OldManAssetList>();
			Material[] mat = new Material[2];
			Transform curSub = transform.Find("Geo/OldManZombie_One");

			foreach (Transform child in curSub)
            {
				skinnedRenderer = child.GetComponent<Renderer>();

				mat[0] = materialsList.BodySkinMaterials[body];
				mat[1] = materialsList.TrousersMaterials[trousers];

				skinnedRenderer.materials = mat;
			}
		}
		void OnValidate()
		{
			charCustomize((int)bodySkin, (int)trousersSkin);
		}
	}
}
