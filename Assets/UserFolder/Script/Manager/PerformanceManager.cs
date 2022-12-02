using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Manager
{
    public class PerformanceManager : MonoBehaviour {

        [SerializeField] Text uiText;
        private int lastFrameIndex;
        private float[] frameDeltaTimeArray;
        private float total;
        void Awake() => frameDeltaTimeArray = new float[50];

        void Update()
        {
            frameDeltaTimeArray[lastFrameIndex] = Time.deltaTime;
            lastFrameIndex = (lastFrameIndex + 1) % frameDeltaTimeArray.Length;

            uiText.text = Mathf.RoundToInt(CalculateFPS()).ToString();
        }

        float CalculateFPS()
        {
            total = 0f;
            foreach(float deltaTime in frameDeltaTimeArray) total += deltaTime;
            return frameDeltaTimeArray.Length / total;
        }
    }
}
