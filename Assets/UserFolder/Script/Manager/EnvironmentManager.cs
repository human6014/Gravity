using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Manager 
{
    public class EnvironmentManager : MonoBehaviour
    {
        [Header("Rain Particle")]
        [SerializeField] private ParticleSystem m_RainParticle;

        [Header("Road Wetness Controll")]
        [SerializeField] private Renderer[] m_RoadRenderer;

        [SerializeField] [Range(0, 2)] private float m_WetnessMin = 0.8f;
        [SerializeField] [Range(0, 2)] private float m_WetnessMax = 1.8f;

        [SerializeField] [Range(0, 1)] private float m_RippleSpeedMin = 0.1f;
        [SerializeField] [Range(0, 1)] private float m_RippleSpeedMax = 0.75f;

        [SerializeField] [Range(0, 1)] private float m_SplashMin = 0.1f;
        [SerializeField] [Range(0, 1)] private float m_SplashMax = 0.7f;

        private MaterialPropertyBlock m_MaterialPropertyBlock;

        private const float m_InitialWetness = 0.8f;
        private const float m_InitialRippleSpeed = 0.1f;
        private const float m_InitialSplash = 0.1f;

        private const string m_Wetness = "Vector1_64252772";
        private const string m_RippleSpeed = "Vector1_C9C81CCF";
        private const string m_Splash = "Vector1_24B8AC3D";


        public ParticleSystem RainParticle 
        { 
            get => m_RainParticle; 
            private set => m_RainParticle = value;
        }

        private void Awake()
        {
            m_MaterialPropertyBlock = new MaterialPropertyBlock();
        }

        [ContextMenu("Rain on")]
        public void OnRainParticle()
        {
            m_RainParticle.transform.rotation = GravityManager.GetCurrentGravityRotation();
            m_RainParticle.Play();
        }

        [ContextMenu("Rain off")]
        public void OffRainParticle()
        {
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

        public IEnumerator RoadWetnessChange(float changeTime, bool isASC)
        {
            float elapsedTime = 0;
            float t;
            m_RoadRenderer[0].GetPropertyBlock(m_MaterialPropertyBlock);

            float startWetness;
            float startRippleSpeed;
            float startSplash;

            float endWetness;
            float endRippleSpeed;
            float endSplash;
            if (isASC)
            {
                startWetness = m_WetnessMin;
                startRippleSpeed = m_RippleSpeedMin;
                startSplash = m_SplashMin;

                endWetness = m_WetnessMax;
                endRippleSpeed = m_RippleSpeedMax;
                endSplash = m_SplashMax;
            }
            else
            {
                startWetness = m_WetnessMax;
                startRippleSpeed = m_RippleSpeedMax;
                startSplash = m_SplashMax;

                endWetness = m_WetnessMin;
                endRippleSpeed = m_RippleSpeedMin;
                endSplash = m_SplashMin;
            }


            float newWetness;
            float newRippleSpeed;
            float newSplash;
            while (elapsedTime < changeTime)
            {
                elapsedTime += Time.deltaTime;
                t = elapsedTime / changeTime;

                newWetness = Mathf.Lerp(startWetness, endWetness, t);
                newRippleSpeed = Mathf.Lerp(startRippleSpeed, endRippleSpeed, t);
                newSplash = Mathf.Lerp(startSplash, endSplash, t);

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
            //m_SpawnManager.GetStageTime
            StartCoroutine(RoadWetnessChange(15, true));
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
