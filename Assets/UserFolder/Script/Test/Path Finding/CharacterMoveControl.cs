using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager.AI;

[RequireComponent(typeof(AStarAgent))]
public class CharacterMoveControl : MonoBehaviour
{
    private float timer = 0f;
    private bool isArrive = false;
    private AStarAgent _Agent;
    private float attackRange = 8;
    [SerializeField] private Transform moveToPoint;

    private void Start()
    {
        _Agent = GetComponent<AStarAgent>();
        StartCoroutine(Coroutine_MoveToTarget());
    }

    private void Update()
    {
        /*
        timer += Time.deltaTime;
        if (timer>=0.3f)
        {
            if (moveToPoint.position == oldPos) return;
            timer = 0;
            AStarAgentStatus status = _Agent.Pathfinding(moveToPoint.position);
            oldPos = moveToPoint.position;
            Debug.Log(status);
        }
        */
    }

    private IEnumerator Coroutine_MoveToTarget()
    {
        while (true)
        {
            _Agent.Pathfinding(moveToPoint.position + moveToPoint.up * 5);
            while (_Agent.Status == AStarAgentStatus.Invalid)
            {
                isArrive = true;
                yield return new WaitForSeconds(0.5f);
                _Agent.Pathfinding(moveToPoint.position + moveToPoint.up * 5);
            }
            isArrive = false;
            while (_Agent.Status != AStarAgentStatus.Finished)
            {
                //이동중일때
                yield return null;
            }
            yield return null;
        }
    }

    private void FixedUpdate()
    {
        if (isArrive)
        {
            Vector3 dir = (AIManager.PlayerTransfrom.position + AIManager.PlayerTransfrom.up * 0.5f - transform.position).normalized;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir, -Manager.GravitiesManager.GravityVector), 0.1f);
        }
    }
}
