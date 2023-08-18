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

        public void OnRainParticle()
        {
            m_RainParticle.transform.rotation = GravityManager.GetCurrentGravityRotation();
            m_GravityManager.SyncRotatingTransform.Add(m_RainParticle.transform);
            m_RainParticle.Play();
        }

        public void OffRainParticle()
        {
            m_GravityManager.SyncRotatingTransform.Remove(m_RainParticle.transform);
            m_RainParticle.Stop();
        }

        public async Task FogDensityChange(float targetDensity, float changeTime)
        {
            float elapsedTime = 0;
            float currentDensity = RenderSettings.fogDensity;
            while (elapsedTime < changeTime) {
                elapsedTime += Time.deltaTime;
                RenderSettings.fogDensity = Mathf.Lerp(currentDensity, targetDensity, elapsedTime / changeTime);
                await Task.Yield();
            }
            RenderSettings.fogDensity = targetDensity;
        }
    }
}
