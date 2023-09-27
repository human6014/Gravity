using System;
using System.Collections;
using UnityEngine;

namespace Controller.Player.Utility
{
    [Serializable]
    public class FOVKick
    {
        private readonly WaitForEndOfFrame waitForEndOfFrame = new();
        private Camera Camera;                           

        public AnimationCurve IncreaseCurve;
        
        public float FOVIncrease = 3f;                  // the amount the field of view increases when going into a run
        public float TimeToIncrease = 1f;               // the amount of time the field of view will increase over
        public float TimeToDecrease = 1f;               // the amount of time the field of view will take to return to its original size
        private float m_OriginalFOV;

        public void Setup(Camera camera)
        {
            CheckStatus(camera);

            Camera = camera;
            m_OriginalFOV = camera.fieldOfView;
        }

        private void CheckStatus(Camera camera)
        {
            if (camera == null) throw new Exception("FOVKick camera is null, please supply the camera to the constructor");
            if (IncreaseCurve == null) throw new Exception("FOVKick Increase curve is null, please define the curve for the field of view kicks");
        }

        public void ChangeCamera(Camera camera) => Camera = camera;


        public IEnumerator FOVKickUp()
        {
            float t = Mathf.Abs((Camera.fieldOfView - m_OriginalFOV) / FOVIncrease);
            while (t < TimeToIncrease)
            {
                Camera.fieldOfView = m_OriginalFOV + (IncreaseCurve.Evaluate(t / TimeToIncrease) * FOVIncrease);
                t += Time.deltaTime;
                yield return waitForEndOfFrame;
            }
        }

        public IEnumerator FOVKickDown()
        {
            float t = Mathf.Abs((Camera.fieldOfView - m_OriginalFOV) / FOVIncrease);
            while (t > 0)
            {
                Camera.fieldOfView = m_OriginalFOV + (IncreaseCurve.Evaluate(t / TimeToDecrease) * FOVIncrease);
                t -= Time.deltaTime;
                yield return waitForEndOfFrame;
            }
            //make sure that fov returns to the original size
            Camera.fieldOfView = m_OriginalFOV;
        }
    }
}
