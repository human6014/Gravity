using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetChanger : MonoBehaviour
{
    RaycastHit hit;
    public GameObject player;

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.red);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                transform.position = hit.point;
                player.GetComponent<PathFinder>().MakePath();
            }
        }
    }
}