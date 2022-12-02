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

        public GameObject GetNowFloor()
        {
            return floorObject[(int)GravitiesManager.type];
        }
    }
}
