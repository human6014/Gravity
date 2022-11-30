using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingMonster : MonoBehaviour, IMonster
{
    [SerializeField] Transform target;
    [SerializeField] LayerMask obstacleLayer;

    Transform myTransform;
    //Rigidbody rigidbody;

    Quaternion rotationModX1;
    Quaternion rotationModY1;
    Quaternion rotationModX2;
    Quaternion rotationModY2;

    Quaternion nowRotation;
    Quaternion newRotation;

    Vector3 targetPos;
    Vector3 targetMovementPos;
    Vector3 targetRot;
    Vector3 targetRotNomal;

    Ray[] obstacleRay = new Ray[4];

    float targetDistance;
    float attackRange = 15f;
    float rayRange = 1.5f;
    float obstacleRotTime = 0.2f;
    float normalRotTime = 0.3f;
    float velocity = 3;
    bool isCollision;
    private void Start()
    {
        myTransform = GetComponent<Transform>();
        //rigidbody = GetComponent<Rigidbody>();
    }
    void Update()
    {
        targetPos = new Vector3(target.position.x, target.position.y + 5, target.position.z);
        targetMovementPos = targetPos - myTransform.position;
        //���� �̵��� ��ġ (�÷��̾�� y�� 5���� ���� but �߷� �ٲٸ� �̻���)
        targetRot = (target.transform.position + target.transform.up * 1.5f) - myTransform.position;
        targetRotNomal = targetRot.normalized;
        targetDistance = Vector3.Distance(myTransform.position, target.transform.position);
        
        rotationModX1 = Quaternion.AngleAxis(-90, myTransform.up);
        rotationModY1 = Quaternion.AngleAxis(-90, myTransform.right);

        rotationModX2 = Quaternion.AngleAxis(90, myTransform.up);
        rotationModY2 = Quaternion.AngleAxis(90, myTransform.right);

        obstacleRay[0] = new Ray(myTransform.position, myTransform.right + myTransform.forward);
        obstacleRay[1] = new Ray(myTransform.position, myTransform.up + myTransform.forward);
        obstacleRay[2] = new Ray(myTransform.position, -myTransform.right + myTransform.forward);
        obstacleRay[3] = new Ray(myTransform.position, -myTransform.up + myTransform.forward);

        if (!isCollision)
        {
            for (int i = 0; i < obstacleRay.Length; i++)
            {
                if (Physics.Raycast(myTransform.position, obstacleRay[i].direction, out RaycastHit hit, rayRange, obstacleLayer))
                {
                    Debug.DrawRay(myTransform.position, obstacleRay[i].direction, Color.red);
                    float obstacleDistance = Vector3.Distance(hit.point, myTransform.position);
                    if (obstacleDistance < 0.5f) myTransform.position += hit.normal * 0.5f;
                    isCollision = true;
                    StartCoroutine(LerpRotate(obstacleRotTime));
                    Rotate(i);
                    break;
                }
            }
            //if (isCollision)
        }

        if(!isCollision)
        {
            newRotation = Quaternion.LookRotation(targetRotNomal);
            StartCoroutine(LerpRotate(normalRotTime));
        }
        if(targetDistance > attackRange || 
            Quaternion.Angle(newRotation, myTransform.rotation) > 5f || 
            !Physics.Raycast(myTransform.position, targetRot, obstacleLayer))
            myTransform.position += Time.deltaTime * velocity * myTransform.forward;      
        /*
         * ���� �̵��� �̰Ŷ� �ٸ�
         * ����� �Ϸ��� targetMovementPos.nomalized�� �ؾ���
         * ���� �� ������Ʈ�� cs��
         * mesh�� �ִ� ������Ʈ cs�� �и��� �θ� �ڽ� ���·� ����
         * �ٶ󺸴� ���� �� ������ �̵��� ��ġ���� ���� �����ؾ���
         * ���� �̵��� ��ġ�� ���ϸ� �ش� cs���� forwardó���ϸ� �� ��
         */
    }
    /*
     * ���� �浹 �Ͼ�� ��� ����
     * ���� �浹 �Ͼ ��� ���� �浹 ������ �������� �̻���
     * 
     */
    void Rotate(int _keyIndex)
    {
        nowRotation = myTransform.rotation;
        switch (_keyIndex)
        {
            case 0:
                newRotation = nowRotation * rotationModX1;
                break;
            case 1:
                newRotation = nowRotation * rotationModY1;
                break;
            case 2:
                newRotation = nowRotation * rotationModX2;
                break;
            case 3:
                newRotation = nowRotation * rotationModY2;
                break;
        }
    }
    IEnumerator LerpRotate(float _rotTime)
    {
        float elapsedTime = 0;
        float rotTime = _rotTime;
        Quaternion _nowRot = myTransform.rotation;
        while (true)
        {
            myTransform.rotation = Quaternion.Lerp(_nowRot, newRotation, elapsedTime / rotTime);
            if (elapsedTime > rotTime) break;
            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        isCollision = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, transform.right + transform.forward);
        Gizmos.DrawRay(transform.position, -transform.right + transform.forward);
        Gizmos.DrawRay(transform.position, transform.up + transform.forward);
        Gizmos.DrawRay(transform.position, -transform.up + transform.forward);
    }
}
