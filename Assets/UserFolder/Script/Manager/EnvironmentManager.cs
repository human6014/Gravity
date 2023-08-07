using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Manager {
    public class EnvironmentManager : MonoBehaviour
    {
        [SerializeField] private ParticleSystem m_RainParticle;

        private GravityManager m_GravityManager;

        private void Awake()
        {
            m_GravityManager = FindObjectOfType<GravityManager>();
        }

        [ContextMenu("Rain On")]
        public void OnRainParticle()
        {
            m_RainParticle.transform.rotation = GravityManager.GetCurrentGravityRotation();
            m_GravityManager.SyncRotatingTransform.Add(m_RainParticle.transform);
            m_RainParticle.gameObject.SetActive(true);
            //Particle Play로 변경해야함
        }

        public async Task FogDensityChange(float targetDensity, float changeTime)
        {
            float startTime = Time.time;
            float elapsedTime = 0;
            float currentDensity = RenderSettings.fogDensity;
            while (elapsedTime < changeTime) {
                elapsedTime = Time.time - startTime;
                RenderSettings.fogDensity = Mathf.Lerp(currentDensity, targetDensity, elapsedTime / changeTime);
                await Task.Yield();
            }
            RenderSettings.fogDensity = targetDensity;
        }
    }
}
