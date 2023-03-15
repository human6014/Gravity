using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
using EnumType;

namespace Contoller.Player.Utility
{
    public class GravityRotation : MonoBehaviour
    {
        private const float ROTATETIME = 1;

        public void GravityChange(int gravityKeyInput, float mouseScroll)
        {
            GravityManager.gravityDirection = (GravityDirection)gravityKeyInput;
            GravityManager.GravityChange(Mathf.FloorToInt(mouseScroll * 10));
            StartCoroutine(GravityRotate());
        }

        private IEnumerator GravityRotate()
        {
            Quaternion currentRotation = transform.rotation;
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / ROTATETIME;
                transform.rotation = Quaternion.Lerp(currentRotation, GravityManager.GetGravityNormalRotation(), t);
                yield return null;
            }
            GravityManager.CompleteGravityChanging();
        }
    }
}
