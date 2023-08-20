using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Manager 
{
    public class EnvironmentManager : MonoBehaviour
    {
        [SerializeField] private ParticleSystem m_RainParticle;
        [SerializeField] private Renderer[] m_RoadRenderer;

        [SerializeField] [Range(0, 2)] private float m_WetnessMin = 0.6f;
        [SerializeField] [Range(0, 2)] private float m_WetnessMax = 1.8f;

        [SerializeField] [Range(0, 1)] private float m_RippleSpeedMin = 0.1f;
        [SerializeField] [Range(0, 1)] private float m_RippleSpeedMax = 0.75f;

        [SerializeField] [Range(0, 1)] private float m_SplashMin = 0.4f;
        [SerializeField] [Range(0, 1)] private float m_SplashMax = 0.7f;

        private MaterialPropertyBlock m_MaterialPropertyBlock;
        private GravityManager m_GravityManager;

        private const float m_InitialWetness = 0.8f;
        private const float m_InitialRippleSpeed = 0.1f;
        private const float m_InitialSplash = 0.1f;

        private const string m_Wetness = "Vector1_64252772";
        private const string m_RippleSpeed = "Vector1_C9C81CCF";
        private const string m_Splash = "Vector1_24B8AC3D";

        private void Awake()
        {
            m_GravityManager = FindObjectOfType<GravityManager>();
            m_MaterialPropertyBlock = new MaterialPropertyBlock();
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

        public IEnumerator MaterialPropertyChange(float changeTime, bool isASC)
        {
            float elapsedTime = 0;
            float t;
            m_RoadRenderer[0].GetPropertyBlock(m_MaterialPropertyBlock);

            //float startWetness = m_MaterialPropertyBlock.GetFloat(m_Wetness);
            //float startRippleSpeed = m_MaterialPropertyBlock.GetFloat(m_RippleSpeed);
            //float startSplash = m_MaterialPropertyBlock.GetFloat(m_Splash);

            float endWetness = isASC ? m_WetnessMax : m_WetnessMin;
            float endRippleSpeed = isASC ? m_RippleSpeedMax : m_RippleSpeedMin;
            float endSplash = isASC ? m_SplashMax : m_SplashMin;

            float newWetness;
            float newRippleSpeed;
            float newSplash;
            while (elapsedTime < changeTime)
            {
                elapsedTime += Time.deltaTime;
                t = elapsedTime / changeTime;

                newWetness = Mathf.Lerp(m_InitialWetness, endWetness, t);
                newRippleSpeed = Mathf.Lerp(m_InitialRippleSpeed, endRippleSpeed, t);
                newSplash = Mathf.Lerp(m_InitialSplash, endSplash, t);

                foreach (Renderer r in m_RoadRenderer)
                {
                    m_MaterialPropertyBlock.SetFloat(m_Wetness, newWetness);
                    m_MaterialPropertyBlock.SetFloat(m_RippleSpeed, newRippleSpeed);
                    m_MaterialPropertyBlock.SetFloat(m_Splash, newSplash);
                    r.SetPropertyBlock(m_MaterialPropertyBlock);
                }

                yield return null;
            }

            foreach (Renderer r in m_RoadRenderer)
            {
                m_MaterialPropertyBlock.SetFloat(m_Wetness, endWetness);
                m_MaterialPropertyBlock.SetFloat(m_RippleSpeed, endRippleSpeed);
                m_MaterialPropertyBlock.SetFloat(m_Splash, endSplash);
                r.SetPropertyBlock(m_MaterialPropertyBlock);
            }
        }

        [ContextMenu("WetChange")]
        private void RoadWetnessChange()
        {
            StartCoroutine(MaterialPropertyChange(15, true));
        }

        private void ChangeRoadShaderValue(string propertyName, float value)
        {
            foreach (Renderer r in m_RoadRenderer)
            {
                m_MaterialPropertyBlock.SetFloat(propertyName, value);

                r.SetPropertyBlock(m_MaterialPropertyBlock);
            }
        }
    }
}
