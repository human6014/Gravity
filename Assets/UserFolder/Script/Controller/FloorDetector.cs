using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

namespace Detector
{
    public class FloorDetector : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] floorObject;
        public void Start()
        {
            AIManager.FloorDetector = this;
        }
        public GameObject GetNowFloor()
        {
            return floorObject[(int)GravitiesManager.currentGravityType];
        }
    }
}
