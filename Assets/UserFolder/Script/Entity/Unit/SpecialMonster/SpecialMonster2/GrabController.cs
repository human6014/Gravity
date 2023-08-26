using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using System;

public class GrabController : MonoBehaviour
{
    [System.Serializable]
    public class TentacleParts
    {
        public Transform[] m_GrabTentacles;
        public PathCreator m_PathCreator;
        public Transform m_StartTransform;

        [Space(12)]
        public Vector3 m_StartAnchorPointInterpolation;
        public Vector3 m_StartControlPointInterpolation;
        public Vector3 m_EndControlPointInterpolation;
        public Vector3 m_EndAnchorPointInterpolation;

        [HideInInspector] public Vector3[] m_OriginalPosition;
        [HideInInspector] public Quaternion[] m_OriginalRotation;
        [HideInInspector] public Vector3[] m_OriginalScale;
    }

    [SerializeField] private TentacleParts[] m_TentacleParts;

    private float m_GrabAttachedTime;
    private readonly float m_ScaleSize = 7;

    public Action AttachedToPlayer { get; set; }
    public bool IsGrabbing { get; set; }        //촉수가 갔다 오는 시간까지 포함해서 true
    public bool IsAttachedPlayer { get; set; }  //플레이어한테 촉수가 Grab됐을때만 true

    private void Awake()
    {
        Transform tentacleTransform = GameObject.Find("SP2TentaclesPath").transform;

        int partsCount = 0;
        foreach (Transform t in tentacleTransform)
            m_TentacleParts[partsCount++].m_PathCreator = t.GetComponent<PathCreator>();

        foreach (TentacleParts tp in m_TentacleParts)
        {
            tp.m_OriginalPosition = new Vector3[tp.m_GrabTentacles.Length];
            tp.m_OriginalRotation = new Quaternion[tp.m_GrabTentacles.Length];
            tp.m_OriginalScale = new Vector3[tp.m_GrabTentacles.Length];
            for (int i = 0; i < tp.m_OriginalPosition.Length; i++)
            {
                tp.m_OriginalPosition[i] = tp.m_GrabTentacles[i].localPosition;
                tp.m_OriginalRotation[i] = tp.m_GrabTentacles[i].localRotation;
                tp.m_OriginalScale[i] = tp.m_GrabTentacles[i].localScale;
            }
        }
    }

    public void Init(float grabAttachedTime)
    {
        m_GrabAttachedTime = grabAttachedTime;
    }

    public void SetBezierPath()
    {
        Transform startTransform;
        Vector3 rightVector;
        float rightAngle;
        int count = 0;

        foreach (TentacleParts tp in m_TentacleParts)
        {
            startTransform = tp.m_StartTransform;
            rightVector = count >= 3 ? startTransform.right : -startTransform.right;
            rightAngle = count >= 3 ? 90 : -90;
            if (count < 3) tp.m_PathCreator.bezierPath.FlipNormals = true;

            tp.m_PathCreator.EditorData.ResetBezierPath(Vector3.zero);

            tp.m_PathCreator.bezierPath.MovePoint(0, startTransform.position +
                rightVector * tp.m_StartAnchorPointInterpolation.x + 
                startTransform.forward * tp.m_StartAnchorPointInterpolation.y +
                startTransform.up * tp.m_StartAnchorPointInterpolation.z);

            tp.m_PathCreator.bezierPath.MovePoint(1, startTransform.position +
                rightVector * tp.m_StartControlPointInterpolation.x +
                startTransform.forward * tp.m_StartControlPointInterpolation.z + 
                startTransform.up * tp.m_StartControlPointInterpolation.y);

            tp.m_PathCreator.bezierPath.MovePoint(3, Manager.AI.AIManager.PlayerTransform.position + 
                rightVector * tp.m_EndAnchorPointInterpolation.x +
                startTransform.forward * tp.m_EndAnchorPointInterpolation.y + 
                startTransform.up * tp.m_EndAnchorPointInterpolation.z);

            tp.m_PathCreator.bezierPath.MovePoint(2, tp.m_PathCreator.bezierPath.GetPoint(3) +
                rightVector * tp.m_EndControlPointInterpolation.x +
                startTransform.forward * tp.m_EndControlPointInterpolation.z + 
                startTransform.up * tp.m_EndControlPointInterpolation.y);

            
            count++;

            if (IsAttachedPlayer) ImmediateGrabStart(rightAngle, tp);
            else StartCoroutine(GrabStartCoroutine(m_GrabAttachedTime, rightAngle, tp));
        }
        if(!IsAttachedPlayer) Invoke(nameof(GrabDamage), m_GrabAttachedTime);
    }

    private void GrabDamage() => AttachedToPlayer?.Invoke();

    private IEnumerator GrabStartCoroutine(float time, float rightAngle, TentacleParts tp)
    {
        IsGrabbing = true;

        float elapsedTime = 0;
        float amount;
        float ySize = tp.m_PathCreator.path.length / tp.m_GrabTentacles.Length * 9f;

        for (int i = 0; i < tp.m_GrabTentacles.Length; i++)
        {
            tp.m_GrabTentacles[i].localScale = new Vector3(m_ScaleSize, ySize, m_ScaleSize);
        }

        while (elapsedTime <= time)
        {
            elapsedTime += Time.deltaTime;

            for (int i = 0; i < tp.m_GrabTentacles.Length; i++)
            {
                amount = (1 + i) / (float)tp.m_GrabTentacles.Length * (elapsedTime / time);
                tp.m_GrabTentacles[i].SetPositionAndRotation(
                    tp.m_PathCreator.path.GetPointAtTime(amount),
                    tp.m_PathCreator.path.GetRotation(amount) * Quaternion.AngleAxis(-90, Vector3.up) * Quaternion.AngleAxis(rightAngle, Vector3.forward));
            }

            yield return null;
        }
        
        IsAttachedPlayer = true;
    }

    private void ImmediateGrabStart(float rightAngle, TentacleParts tp)
    {
        float amount;
        float ySize = tp.m_PathCreator.path.length / tp.m_GrabTentacles.Length * 9f;

        for (int i = 0; i < tp.m_GrabTentacles.Length; i++)
        {
            amount = (1 + i) / (float)tp.m_GrabTentacles.Length;

            tp.m_GrabTentacles[i].SetPositionAndRotation(
                tp.m_PathCreator.path.GetPointAtTime(amount),
                tp.m_PathCreator.path.GetRotation(amount) * Quaternion.AngleAxis(-90, Vector3.up) * Quaternion.AngleAxis(rightAngle, Vector3.forward));

            tp.m_GrabTentacles[i].localScale = new Vector3(m_ScaleSize, ySize, m_ScaleSize);
        }
    }

    //수정 하도록
    public IEnumerator GrabEndCoroutine(float time)
    {
        IsAttachedPlayer = false;

        float elapsedTime = 0;
        float t;

        Vector3[][] initialPositions = new Vector3[m_TentacleParts.Length][];
        Quaternion[][] initialRotations = new Quaternion[m_TentacleParts.Length][];
        Vector3[][] initialScales = new Vector3[m_TentacleParts.Length][];

        for (int i = 0; i < m_TentacleParts.Length; i++)
        {
            initialPositions[i] = new Vector3[m_TentacleParts[i].m_GrabTentacles.Length];
            initialRotations[i] = new Quaternion[m_TentacleParts[i].m_GrabTentacles.Length];
            initialScales[i] = new Vector3[m_TentacleParts[i].m_GrabTentacles.Length];
            for (int j = 0; j < m_TentacleParts[i].m_GrabTentacles.Length; j++)
            {
                initialPositions[i][j] = m_TentacleParts[i].m_GrabTentacles[j].localPosition;
                initialRotations[i][j] = m_TentacleParts[i].m_GrabTentacles[j].localRotation;
                initialScales[i][j] = m_TentacleParts[i].m_GrabTentacles[j].localScale;
            }
        }

        while (elapsedTime <= time)
        {
            elapsedTime += Time.deltaTime;
            t = Mathf.Clamp01(elapsedTime / time);

            for (int i = 0; i < m_TentacleParts.Length; i++)
            {
                for (int j = 0; j < m_TentacleParts[i].m_OriginalPosition.Length; j++)
                {
                    m_TentacleParts[i].m_GrabTentacles[j].localPosition =
                        Vector3.Lerp(initialPositions[i][j], m_TentacleParts[i].m_OriginalPosition[j], t);

                    m_TentacleParts[i].m_GrabTentacles[j].localRotation =
                        Quaternion.Slerp(initialRotations[i][j], m_TentacleParts[i].m_OriginalRotation[j], t);

                    m_TentacleParts[i].m_GrabTentacles[j].localScale =
                        Vector3.Lerp(initialScales[i][j], m_TentacleParts[i].m_OriginalScale[j], t);
                }
            }

            yield return null;
        }

        IsGrabbing = false;
    }

    public void ImmediateGrabEnd()
    {
        foreach (TentacleParts tp in m_TentacleParts)
        {
            for (int i = 0; i < tp.m_OriginalPosition.Length; i++)
            {
                tp.m_GrabTentacles[i].localPosition = tp.m_OriginalPosition[i];
                tp.m_GrabTentacles[i].localRotation = tp.m_OriginalRotation[i];
                tp.m_GrabTentacles[i].localScale = tp.m_OriginalScale[i];
            }
        }

        IsAttachedPlayer = false;
    }

    public void Dispose()
    {
        StopAllCoroutines();
        if (IsAttachedPlayer) ImmediateGrabEnd();
    }

    private void OnDrawGizmosSelected()
    {
        foreach(TentacleParts tp in m_TentacleParts)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(tp.m_StartTransform.position, tp.m_StartTransform.position+tp.m_StartTransform.forward * 3);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(tp.m_StartTransform.position, tp.m_StartTransform.position + tp.m_StartTransform.right * 3);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(tp.m_StartTransform.position, tp.m_StartTransform.position + tp.m_StartTransform.up * 3);
        }
    }
}
