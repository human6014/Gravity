using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrbanZombie_CharacterCustomize : MonoBehaviour
{
	private int headTyp;
	private int bodyTyp;
	private int trousersTyp;
	private int tanktopTyp;
	private int hoodieTyp;
	

	private bool tanktopOld;
	private Transform hoodieT;
	private Transform tanktopT;
	private Transform bodyToHideT;
	private Transform bodyExposedT;

	private Transform headT_A;
	private Transform headT_B;


	private UrbanZombie_AssetsList materialsList;

	private SkinnedMeshRenderer skinnedMeshRenderer;

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

	public void charCustomize(int body, int trousers, int tanktop, int hoodie, int head)
	{
		materialsList = gameObject.GetComponent<UrbanZombie_AssetsList>();
		Material[] mat;
		hoodieT = transform.Find("Geo/Hoodie");
		tanktopT = transform.Find("Geo/TankTop");
		bodyToHideT = transform.Find("Geo/Body_ToHide");
		bodyExposedT = transform.Find("Geo/BodyExposed/");

		headT_A = transform.Find("Geo/BodyExposed/HeadA");
		headT_B = transform.Find("Geo/BodyExposed/HeadB");

		// Body_Exposed hands
		for (int i = 0; i <= 3; i++)
		{
			Transform curSub = bodyExposedT.Find("Hands_LOD" + i);
			Renderer skinRend = curSub.GetComponent<Renderer>();
			skinRend.material = materialsList.BodyMaterials[body];
		}

		// Body_Exposed Trousers
		for (int i = 0; i <= 3; i++)
		{
			Transform curSub = bodyExposedT.Find("Trousers_LOD" + i);
			Renderer skinRend = curSub.GetComponent<Renderer>();
			skinRend.material = materialsList.TrousersMaterials[trousers];
		}

		// Body_Exposed HeadA
		for (int i = 0; i <= 3; i++)
		{
			Transform curSub = headT_A.Find("HeadA_LOD" + i);

			Renderer skinRend = curSub.GetComponent<Renderer>();
			skinRend.material = materialsList.BodyMaterials[body];
		}

		// Body_Exposed HeadB
		for (int i = 0; i <= 3; i++)
		{
			Transform curSub = headT_B.Find("HeadB_LOD" + i);

			Renderer skinRend = curSub.GetComponent<Renderer>();
			skinRend.material = materialsList.BodyMaterials[body];
		}

		//Head Type
		if(head == 1)
        {
			headT_A.gameObject.SetActive(false);
			headT_B.gameObject.SetActive(true);
        }
        else
        {
			headT_B.gameObject.SetActive(false);
			headT_A.gameObject.SetActive(true);
		}

		//BodyToHide
		for (int i = 0; i <= 3; i++)
		{
			Transform curSub = transform.Find("Geo/Body_ToHide/Body_ToHide_LOD" + i);

			Renderer skinRend = curSub.GetComponent<Renderer>();
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
			for (int i = 0; i <= 3; i++)
			{
				Transform curSub = hoodieT.Find("Hoodie_LOD" + i);
				Renderer skinRend = curSub.GetComponent<Renderer>();
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
		if (tanktop < 1)
		{
			tanktopT.gameObject.SetActive(false);
		}
		else
		{
			tanktopT.gameObject.SetActive(true);
			bodyToHideT.gameObject.SetActive(true);
			for (int i = 0; i <= 3; i++)
			{
				Transform curSub = tanktopT.Find("TankTop_LOD" + i);
				Renderer skinRend = curSub.GetComponent<Renderer>();
				skinRend.material = materialsList.TankTopMaterials[tanktop - 1];

				tanktopOld = true;
			}
			hoodieT.gameObject.SetActive(false);
			hoodieType = 0;
		}
	}

	void OnValidate()
	{
		//code for In Editor customize
		headTyp = (int)faceType;
		bodyTyp = (int)bodyType;
		trousersTyp = (int)trousersType;
		tanktopTyp = (int)tanktopType;
		hoodieTyp = (int)hoodieType;
		
		charCustomize(bodyTyp, trousersTyp, tanktopTyp, hoodieTyp, headTyp);
	}
}
