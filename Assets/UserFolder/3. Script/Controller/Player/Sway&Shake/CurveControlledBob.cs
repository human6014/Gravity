using System;
using UnityEngine;

namespace Controller.Player.Utility
{
    [Serializable]
    public class CurveControlledBob
    {
        [SerializeField] public string m_InspectorText = "";
        [SerializeField] private float m_HorizontalBobRange = 0.1f;
        [SerializeField] private float m_VerticalBobRange = 0.1f;

        [SerializeField] private AnimationCurve m_BobcurveX = 
            new(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f),
                new Keyframe(1f, 0f), new Keyframe(1.5f, -1f),
                new Keyframe(2f, 0f));

        [SerializeField] private AnimationCurve m_BobcurveY =
            new(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f),
                new Keyframe(1f, 0f), new Keyframe(1.5f, -1f),
                new Keyframe(2f, 0f));

        private Vector3 m_OriginalPosition;

        private float m_BobBaseInterval;

        private float m_CyclePositionX;
        private float m_CyclePositionY;
        
        private float m_XTime;
        private float m_YTime;
        
        public void Setup(Transform tr, float bobBaseInterval)
        {
            m_OriginalPosition = tr.localPosition;
            m_BobBaseInterval = bobBaseInterval;

            m_XTime = m_BobcurveX[m_BobcurveX.length - 1].time;
            m_YTime = m_BobcurveY[m_BobcurveY.length - 1].time;
        }

        public virtual bool CanState(PlayerBehaviorState playerBehaviorState) => false;
        
        public virtual bool CanState(PlayerWeaponState playerWeaponState)  => false;
        
        public Vector3 DoMoveHeadBob(float speed = 1)
        {
            float xPos = m_OriginalPosition.x + (m_BobcurveX.Evaluate(m_CyclePositionX) * m_HorizontalBobRange);
            float yPos = m_OriginalPosition.y + (m_BobcurveY.Evaluate(m_CyclePositionY) * m_VerticalBobRange);

            m_CyclePositionX += ((speed * Time.deltaTime) / m_BobBaseInterval);
            m_CyclePositionY += ((speed * Time.deltaTime) / m_BobBaseInterval);

            if (m_CyclePositionX > m_XTime) m_CyclePositionX -= m_XTime;
            if (m_CyclePositionY > m_YTime) m_CyclePositionY -= m_YTime;

            return new Vector3(xPos, yPos, 0f);
        }
    }
}
