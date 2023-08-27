using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationRotate : MonoBehaviour
{
    private void Update() 
        => transform.rotation = Quaternion.LookRotation(Vector3.forward,Vector3.up);
}
