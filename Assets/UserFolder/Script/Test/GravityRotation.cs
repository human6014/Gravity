using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
using EnumType;

public class GravityRotation : MonoBehaviour
{
    public bool IsChanging { get; private set; } = false;
    private int currentGravityKeyInput = 1;
    private readonly KeyCode[] gravityChangeInput =
    {
            KeyCode.Z,
            KeyCode.X,
            KeyCode.C
    };

    private void Update()
    {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");
        for (int i = 0; i < gravityChangeInput.Length; i++)
        {
            if (Input.GetKeyDown(gravityChangeInput[i]))
            {
                currentGravityKeyInput = i;
            }
        }

        if (wheelInput != 0 && !GravityManager.IsGravityChanging)
        {
            IsChanging = true;
            GravityManager.gravityDirection = (GravityDirection)currentGravityKeyInput;
            GravityManager.GravityChange(Mathf.FloorToInt(wheelInput * 10));
            transform.Rotate(new Vector3(180,0,0));
        }
        Debug.Log(GravityManager.currentGravityType);
        GravityManager.CompleteGravityChanging();
    }

}
