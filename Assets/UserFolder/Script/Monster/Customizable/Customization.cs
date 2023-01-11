using System;
using System.Collections.Generic;
using UnityEngine;

public class Customization : MonoBehaviour
{
    [Serializable]
    public struct MaterialsStruct
    {
        [Header("Parts material info")]
        public Material[] partMaterials;
    }

    [Serializable]
    public struct NormalZomibeComponentsStruct
    {
        [Header("Parts list && info")]
        public MaterialsStruct[] materials;
        public Transform[] transforms;
    }

    [Header("Normalzombie customizing factor")]
    public NormalZomibeComponentsStruct[] normalZomibeMaterials;

    MaterialsStruct[] currentMaterials;
    private List<int> randNumList = new();
    public void Start()
    {
        // 예시
        // Material ma = normalZomibeMaterials[0].materials[0];
    }

    private void RandNum()
    {
        randNumList.Clear();
        for (int i = 0; i < currentMaterials.Length; i++)
        {
            randNumList.Add(UnityEngine.Random.Range(0,currentMaterials[i].partMaterials.Length));
        }
    }
    
    //인스펙터 enum 개수와 인스펙터 material 개수가 같음
    public void Customize(NormalMonster unit)
    {
        int iCount = 0;
        int jCount = 0;
        int monsterType = (int)unit.GetMonsterType();
        currentMaterials = normalZomibeMaterials[monsterType].materials;
        RandNum();
        //Geo의 자식 Transform

        //enum 길이
        //int count = Enum.GetValues(typeof(NormalZomibeMaterials)).Length;

        Renderer renderer;
        Material[] cachedMaterials;
        foreach (Transform child in unit.transform.GetChild(0))
        {
            //cachedMaterials = currentMaterials[iCount].partMaterials;
            //Debug.Log(currentMaterials[iCount].partMaterials);
            foreach (Transform childLOD in child)
            {
                //Debug.Log(childLOD);
                //renderer = childLOD.GetComponent<Renderer>();
                //renderer.material = cachedMaterials[jCount];
                jCount++;
            }
            iCount++;
            jCount = 0;
        }
        /*
        ransform[] child;
        Transform childLOD;
        Renderer renderer;
        Material[] cachedMaterials;
        for (int i = 1; i < partTransforms.Length; i++)
        {
            Debug.Log(partTransforms[i].name);
            child = partTransforms[i].GetComponentsInChildren<Transform>(true);
            cachedMaterials = currentMaterials[i - 1].partMaterials;
            for (int j = 1; j < child.Length; j++)
            {
                //Debug.Log(child[j]);
                childLOD = child[j];
                renderer = childLOD.GetComponent<Renderer>();
                renderer.material = cachedMaterials[j - 1];
            }
        }
        */
    }
}
