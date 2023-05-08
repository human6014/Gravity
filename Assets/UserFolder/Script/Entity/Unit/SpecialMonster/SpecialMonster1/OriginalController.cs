using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OriginalController : MonoBehaviour
{
    // Player movement script
    [SerializeField] private SP1AnimationController animationController;
    private float MoveSpeed { get; } = 9f;
    private float RotSpeed { get; } = 60f;

    void Update()
    {
        // Handle keyboard control
        // This loop competes with AdjustBodyTransform() in LegController script to properly postion the body transform

        float ws = Input.GetAxis("Vertical") * -MoveSpeed * Time.deltaTime;
        float ad = Input.GetAxis("Horizontal") * -MoveSpeed * Time.deltaTime;

        //transform.Translate(ad, 0, ws);

        if (Input.GetKey(KeyCode.Q))
        {
            //transform.Rotate(0, -RotSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            //transform.Rotate(0, RotSpeed * Time.deltaTime, 0);
        }
        else if (ws != 0 || ad != 0)
        {
            //animationController.SetWalk();
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            animationController.SetBiteAttack();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            animationController.SetClawsAttack();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            animationController.SetSpitVenom();
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            animationController.SetRoar();
        }
    }
}