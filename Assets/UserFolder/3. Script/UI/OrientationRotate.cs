using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationRotate : MonoBehaviour
{
    private void Update()
    {
        transform.forward = Vector3.forward;
        transform.up = Vector3.up;
    }
}
