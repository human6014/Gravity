using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
using EnumType;

public class Parabola : MonoBehaviour
{
    [Header("Parabola")]
    [SerializeField] private LineRenderer m_LineRenderer;
    //[SerializeField] float height = 10;

    private Transform m_NavMeshTransform;

    private void Awake() => m_NavMeshTransform = transform.GetChild(0);

    public float GetHeight(float heightRatio, Transform target, out Vector3 targetVector, ref Vector3 groundDirection, float destinationDist = 0)
    {
        Vector3 direction = target.position - m_NavMeshTransform.position;
        float dir = 0;
        switch (GravityManager.m_CurrentGravityType)
        {
            case GravityType.xUp:
            case GravityType.xDown:
                groundDirection = new(0, direction.y, direction.z);
                dir = direction.x;
                break;

            case GravityType.yUp: //Init
            case GravityType.yDown:
                groundDirection = new(direction.x, 0, direction.z);
                dir = direction.y;
                break;

            case GravityType.zUp:
            case GravityType.zDown:
                groundDirection = new(direction.x, direction.y, 0);
                dir = direction.z;
                break;
        }
        targetVector = new(groundDirection.magnitude - destinationDist, dir * -GravityManager.GravityDirectionValue, 0);
        return Mathf.Max(0.01f, targetVector.y + targetVector.magnitude / heightRatio);
    }

#if UNITY_EDITOR
    public void DrawPath(Vector3 direction, float v0, float angle, float time, float step)
    {
        step = Mathf.Max(0.01f, step);
        m_LineRenderer.positionCount = (int)(time / step) + 2;
        int count = 0;
        for (float i = 0; i < time; i += step)
        {
            float x = v0 * i * Mathf.Cos(angle);
            float y = v0 * i * Mathf.Sin(angle) - 0.5f * Physics.gravity.magnitude * Mathf.Pow(i, 2);
            m_LineRenderer.SetPosition(count, m_NavMeshTransform.position + direction * x - GravityManager.GravityVector * y);

            count++;
        }
        float xFinal = v0 * time * Mathf.Cos(angle);
        float yFinal = v0 * time * Mathf.Sin(angle) - 0.5f * Physics.gravity.magnitude * Mathf.Pow(time, 2);
        m_LineRenderer.SetPosition(count, m_NavMeshTransform.position + direction * xFinal - GravityManager.GravityVector * yFinal);
    }
#endif

    private float QuadraticEquation(float a, float b, float c, float sign) =>
        (-b + sign * Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);

    public void CalculatePathWithHeight(Vector2 targetPos, float h, out float v0, out float angle, out float time)
    {
        float g = Physics.gravity.magnitude;

        float a = (-0.5f * g);
        float b = Mathf.Sqrt(2 * g * h);
        float c = -targetPos.y;

        float tplus = QuadraticEquation(a, b, c, 1);
        float tmin = QuadraticEquation(a, b, c, -1);

        time = tplus > tmin ? tplus : tmin;
        angle = Mathf.Atan(b * time / targetPos.x);
        v0 = b / Mathf.Sin(angle);
    }
}
