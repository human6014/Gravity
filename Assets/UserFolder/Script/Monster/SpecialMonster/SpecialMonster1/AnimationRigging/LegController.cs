using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegController : MonoBehaviour
{
    [SerializeField] private NavTrace navTrace;
    [SerializeField] private Transform bodyTransform; //Body 위치
    [SerializeField] private Leg[] legs;
    
    private float maxTipWait = 1.5f; //값이 작으면 발 위치 고정 시간이 길어짐 0.7f

    private bool preJump = false;
    private bool isNavOn = true;
    private bool isJump = false;
    private bool readySwitchOrder = false;
    private bool stepOrder = true;

    private readonly float bodyHeightBase = 0;   //body 높이 1.3f
    private readonly float posAdjustRatio = 0.05f;  //body 위치 조정 강도
    private readonly float rotAdjustRatio = 0.75f;   //body 회전 조정 강도
    
    public bool GetIsNavOn() => isNavOn;
    public void SetPreJump(bool _preJump) => preJump = _preJump;
    //public void SetMaxTipWait(float _maxTipWait) => maxTipWait = _maxTipWait;
    private void Start() => StartCoroutine(AdjustBodyTransform());

    private void FixedUpdate()
    {
        // If tip is not in current order but it's too far from target position, Switch the order
        for (int i = 0; i < legs.Length; i++)
        {
            if (legs[i].TipDistance > maxTipWait)
            {
                stepOrder = i % 2 == 0;
                break;
            }
        }

        // Ordering steps
        foreach (Leg leg in legs)
        {
            leg.Movable = stepOrder;
            stepOrder = !stepOrder;
        }

        int index = stepOrder ? 0 : 1;

        // If the opposite foot step completes, switch the order to make a new step
        if (readySwitchOrder && !legs[index].Animating)
        {
            stepOrder = !stepOrder;
            readySwitchOrder = false;
        }

        if (!readySwitchOrder && legs[index].Animating) readySwitchOrder = true;
    }
    Vector3 bodyPos;
    Vector3 bodyUp;
    Vector3 bodyForward;
    Vector3 bodyRight;
    Quaternion bodyRotation;
    /// <summary>
    /// 다리에 대한 몸체 위치, 각도 계산 (Couroutine)
    /// </summary>
    /// <returns></returns>
    private IEnumerator AdjustBodyTransform()
    {
        while (true)
        {
            Vector3 tipCenter = Vector3.zero;
            bodyUp = Vector3.zero;

            // Collect leg information to calculate body transform
            foreach (Leg leg in legs)
            {
                tipCenter += leg.TipPos;
                bodyUp += leg.TipUpDir + leg.RaycastTipNormal;
            }

            if (Physics.Raycast(bodyTransform.position, bodyTransform.up * -1, out RaycastHit hit, 30.0f))
                bodyUp += hit.normal;

            bodyUp.Normalize();

            // calc transform
            // Interpolate postition from old to new
            if (!isJump && !preJump)
            {
                tipCenter /= legs.Length;

                bodyPos = tipCenter + bodyUp * bodyHeightBase;
                navTrace.ProceduralPosition = Vector3.Lerp(bodyTransform.position, bodyPos, posAdjustRatio);
                //bodyTransform.position = Vector3.Lerp(bodyTransform.position, bodyPos, posAdjustRatio);
            }

            // calc rotation
            // Calculate new body axis
            bodyRight = Vector3.Cross(bodyUp, bodyTransform.forward);
            bodyForward = Vector3.Cross(bodyRight, bodyUp).normalized;

            // Interpolate rotation from old to new
            bodyRotation = Quaternion.LookRotation(bodyForward, bodyUp);
            navTrace.ProceduralForwardAngle = bodyForward;
            navTrace.ProceduralUpAngle = bodyUp;
            //bodyTransform.rotation = Quaternion.Slerp(bodyTransform.rotation, bodyRotation, rotAdjustRatio);
            yield return null;
        }
    }

    /// <summary>
    /// 점프 입력시 this + children setter
    /// </summary>
    /// <param name="_isJump">점프 == true, 점프 안함 == false</param>
    public void Jump(bool _isJump)
    {
        isJump = _isJump;
        isNavOn = !_isJump;
        foreach (Leg leg in legs)
        {
            leg.SetIsJump(isJump);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(bodyPos, bodyPos + bodyRight);
        Gizmos.DrawLine(bodyPos, bodyPos + bodyUp);
        Gizmos.DrawLine(bodyPos, bodyPos + bodyForward);
    }
}
