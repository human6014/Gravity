using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : MonoBehaviour
{
    // Self
    [SerializeField] private LegController legController;

    [Header("Rays")]
    [Tooltip("��ü -> �ϴ� ���� ����")]
    [SerializeField] private Transform downRayOrigin;
    [Tooltip("��ü -> ���� ���� ����")]
    [SerializeField] private Transform forwardRayOrigin;//rayOrigin : ������ ��ġ(���� ��)
    [Tooltip("���� -> ��ü ���� ����")]
    [SerializeField] private Transform backRayOrigin;

    [Header("ikTarget")]
    [SerializeField] private Transform ikTarget;        //ikTarget : �� �ٷ� �� ����(�̵� ��)

    [Header("Details")]
    [Tooltip("�� ���� ����")]
    [SerializeField] private float tipMoveDist = 2;
    [Tooltip("�� ���� �ִϸ��̼� �̵� �ð�")]
    [SerializeField] private float tipAnimationTime = 0.25f; //�� ������ �ð� 0.15f
    [Tooltip("����� �� ��ġ ����")]
    [SerializeField] private float ikOffset = 2f;  // �� �� ���� 1.0f
    [Tooltip("���� ���� ����")]
    [SerializeField] private float maxFowardRayDist = 2f; //���� ����     3

    private readonly float maxDownRayDist = 10f; // �ϴ� ����    7
    private readonly float maxBackRayDist = 10f; // ���� -> ��ü ����    7
    private readonly float tipMaxHeight = 1.5f;  //���� �� ���� ���� 0.2f
    private readonly float tipAnimationFrameTime = 0.02f;    // 1 / 60.0f
    private readonly float tipPassOver = 0.55f / 2.0f;   //0.55f/2.0f

    private float interporation;
    private bool isJump;

    Vector3 right;
    Vector3 tipDirVec;
    //�߿��� ������� ����

    public void SetIsJump(bool _isJump) => isJump = _isJump;
    public Vector3 TipPos { get; private set; }
    //TipPos : �� ��ġ
    public Vector3 TipUpDir { get; private set; }
    public Vector3 RaycastTipPos { get; private set; }
    //RaycastTipPos : ���� ���� �� �ִ� ������ �� ��ġ
    public Vector3 RaycastTipNormal { get; private set; }
    //RaycastTipNormal : �ش� �� ������Ʈ�� ���� ����
    public bool Animating { get; private set; } = false;
    public bool Movable { get; set; } = false;
    public float TipDistance { get; private set; }
    //�ڱ� �� ��ġ�� ray���� �ɸ� �� ��ġ�� �Ÿ�

    private void Awake() => TipPos = ikTarget.position;

    private void Start() => ikTarget.SetPositionAndRotation(TipPos + legController.BodyTransform.up.normalized * ikOffset,
                            Quaternion.LookRotation(TipPos - ikTarget.position) * Quaternion.Euler(180, 0, 0));

    RaycastHit hit;
    private void FixedUpdate()
    {
        // Calculate the tip target position
        //if (!IsIKEnable) return;
        if (forwardRayOrigin && Physics.Raycast(forwardRayOrigin.position, forwardRayOrigin.forward.normalized, out hit, maxFowardRayDist, legController.LayerMask))
        {
            RaycastTipPos = hit.point;
            RaycastTipNormal = hit.normal;
            interporation = ikOffset - 3;
        }
        else if (backRayOrigin && Physics.Raycast(backRayOrigin.position, backRayOrigin.forward.normalized * -1, out hit, maxBackRayDist, legController.LayerMaskBack))
        {
            RaycastTipPos = hit.point;
            RaycastTipNormal = hit.normal;
            interporation = 0;
        }
        else if (downRayOrigin && Physics.Raycast(downRayOrigin.position, downRayOrigin.up.normalized * -1, out hit, maxDownRayDist, legController.LayerMask))
        {
            RaycastTipPos = hit.point;
            RaycastTipNormal = hit.normal;
            interporation = 0;
        }
        else
        {
            TipPos = RaycastTipPos = downRayOrigin.position + -maxDownRayDist * legController.BodyTransform.up.normalized;
            interporation = 0;
            UpdateIKTargetTransform();
            return;
        }

        TipDistance = (RaycastTipPos - TipPos).magnitude;

        // If the distance gets too far, animate and move the tip to new position
        if (!Animating && (TipDistance > tipMoveDist && Movable))
        {
            StartCoroutine(AnimateLeg()); //�������� �ٸ�(��) �ִϸ�����(������)
        }
    }

    private IEnumerator AnimateLeg()
    {
        Animating = true;

        float timer = 0.0f;
        float animTime, tipAcceleration;

        Vector3 startingTipPos = TipPos;
        tipDirVec = RaycastTipPos - TipPos;
        tipDirVec += tipDirVec.normalized * tipPassOver;
        

        right = Vector3.Cross(legController.BodyTransform.up, tipDirVec.normalized).normalized;
        TipUpDir = Vector3.Cross(tipDirVec.normalized, right);
        //tipUpVec : ������⿡���� �߿��� �� ����
        
        while (timer < tipAnimationTime + tipAnimationFrameTime)
        {
            animTime = legController.SpeedCurve.Evaluate(timer / tipAnimationTime);

            // If the target is keep moving, apply acceleration to correct the end point
            tipAcceleration = Mathf.Max((RaycastTipPos - startingTipPos).magnitude / tipDirVec.magnitude, 1);

            TipPos = startingTipPos + animTime * tipAcceleration * tipDirVec; // Forward direction of tip vector
            TipPos += legController.HeightCurve.Evaluate(animTime) * tipMaxHeight * TipUpDir; // Upward direction of tip vector

            UpdateIKTargetTransform();
            yield return null;

            timer += tipAnimationFrameTime;
        }
        Animating = false;
    }

    private void UpdateIKTargetTransform() =>
        ikTarget.SetPositionAndRotation(TipPos + legController.BodyTransform.up * ikOffset + legController.BodyTransform.forward * -interporation,
        Quaternion.LookRotation(TipPos - ikTarget.position) * Quaternion.Euler(90, 0, 0));


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        /*
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(RaycastTipPos, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(TipPos, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(TipPos, RaycastTipPos);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(ikTarget.position, 0.1f);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(TipPos, TipPos + tipDirVec);

        Gizmos.color = Color.black;
        Gizmos.DrawLine(TipPos, TipPos + TipUpDir);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(downRayOrigin.position, downRayOrigin.up.normalized * -maxDownRayDist);
        if (forwardRayOrigin)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(forwardRayOrigin.position, forwardRayOrigin.forward.normalized * maxFowardRayDist);
        }
        if (backRayOrigin)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(backRayOrigin.position, backRayOrigin.forward.normalized * -maxBackRayDist);
        }
        */
    }
#endif
}
