
using UnityEngine;
using UnityEngine.Rendering;

public class BFX_DemoTest : MonoBehaviour
{
    public bool InfiniteDecal;
    public Light DirLight;
    public GameObject BloodAttach;
    public GameObject[] BloodFX;
    public Vector3 direction;

    private int m_EffectIndex;
    private int m_ActiveBloods;
    private Camera m_MainCamera;

    private void Awake()
    {
        m_MainCamera = GetComponent<Camera>();
    }

    private Transform GetNearestObject(Transform hit, Vector3 hitPos)
    {
        float closestPos = 100f;
        Transform closestBone = null;
        var childs = hit.GetComponentsInChildren<Transform>();

        foreach (var child in childs)
        {
            var dist = Vector3.Distance(child.position, hitPos);
            if (dist < closestPos)
            {
                closestPos = dist;
                closestBone = child;
            }
        }

        var distRoot = Vector3.Distance(hit.position, hitPos);
        if (distRoot < closestPos)
        {
            closestPos = distRoot;
            closestBone = hit;
        }
        return closestBone;
    }



    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = m_MainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))//Layer추가
            {
                // var randRotation = new Vector3(0, Random.value * 360f, 0);
                // var dir = CalculateAngle(Vector3.forward, hit.normal);
                float angle = Mathf.Atan2(hit.normal.x, hit.normal.z) * Mathf.Rad2Deg + 180;

                //var effectIdx = Random.Range(0, BloodFX.Length);
                if (m_EffectIndex == BloodFX.Length) m_EffectIndex = 0;

                var instance = Instantiate(BloodFX[m_EffectIndex], hit.point, Quaternion.Euler(0, angle + 90, 0));
                m_EffectIndex++;
                m_ActiveBloods++;
                var settings = instance.GetComponent<BFX_BloodSettings>();
                //settings.FreezeDecalDisappearance = InfiniteDecal;
                //settings.LightIntensityMultiplier = DirLight.intensity;

                var nearestBone = GetNearestObject(hit.transform.root, hit.point);
                if (nearestBone != null)
                {
                    var attachBloodInstance = Instantiate(BloodAttach);
                    var bloodT = attachBloodInstance.transform;
                    bloodT.position = hit.point;
                    bloodT.localRotation = Quaternion.identity;
                    bloodT.localScale = Vector3.one * Random.Range(0.75f, 1.2f);
                    bloodT.LookAt(hit.point + hit.normal, direction);
                    bloodT.Rotate(90, 0, 0);
                    bloodT.transform.parent = nearestBone;
                    //Destroy(attachBloodInstance, 20);
                }

                // if (!InfiniteDecal) Destroy(instance, 20);
            }
        }
    }


    public float CalculateAngle(Vector3 from, Vector3 to)
    {

        return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;

    }

}
