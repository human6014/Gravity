using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
public class FlyingMonster : MonoBehaviour, IMonster
{
    Transform target;
    [SerializeField] LayerMask obstacleLayer;

    Transform cachedTransform;
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
        cachedTransform = GetComponent<Transform>();
        //rigidbody = GetComponent<Rigidbody>();
    }
    void Update()
    {
        target = AIManager.PlayerTransfrom;
        targetPos = new Vector3(target.position.x, target.position.y + 5, target.position.z);
        targetMovementPos = targetPos - cachedTransform.position;
        //���� �̵��� ��ġ (�÷��̾�� y�� 5���� ���� but �߷� �ٲٸ� �̻���)
        targetRot = (target.transform.position + target.transform.up * 1.5f) - cachedTransform.position;
        targetRotNomal = targetRot.normalized;
        targetDistance = Vector3.Distance(cachedTransform.position, target.transform.position);
        
        rotationModX1 = Quaternion.AngleAxis(-90, cachedTransform.up);
        rotationModY1 = Quaternion.AngleAxis(-90, cachedTransform.right);

        rotationModX2 = Quaternion.AngleAxis(90, cachedTransform.up);
        rotationModY2 = Quaternion.AngleAxis(90, cachedTransform.right);

        obstacleRay[0] = new Ray(cachedTransform.position, cachedTransform.right + cachedTransform.forward);
        obstacleRay[1] = new Ray(cachedTransform.position, cachedTransform.up + cachedTransform.forward);
        obstacleRay[2] = new Ray(cachedTransform.position, -cachedTransform.right + cachedTransform.forward);
        obstacleRay[3] = new Ray(cachedTransform.position, -cachedTransform.up + cachedTransform.forward);

        if (!isCollision)
        {
            for (int i = 0; i < obstacleRay.Length; i++)
            {
                if (Physics.Raycast(cachedTransform.position, obstacleRay[i].direction, out RaycastHit hit, rayRange, obstacleLayer))
                {
                    Debug.DrawRay(cachedTransform.position, obstacleRay[i].direction, Color.red);
                    float obstacleDistance = Vector3.Distance(hit.point, cachedTransform.position);
                    if (obstacleDistance < 0.5f) cachedTransform.position += hit.normal * 0.5f;
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
            Quaternion.Angle(newRotation, cachedTransform.rotation) > 5f || 
            !Physics.Raycast(cachedTransform.position, targetRot, obstacleLayer))
            cachedTransform.position += Time.deltaTime * velocity * cachedTransform.forward;      
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
        nowRotation = cachedTransform.rotation;
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
        Quaternion _nowRot = cachedTransform.rotation;
        while (true)
        {
            cachedTransform.rotation = Quaternion.Lerp(_nowRot, newRotation, elapsedTime / rotTime);
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
