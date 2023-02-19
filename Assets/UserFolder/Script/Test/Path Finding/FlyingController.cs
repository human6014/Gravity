using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingController : MonoBehaviour
{
    private AStarAgent _Agent;
    [SerializeField] private Transform _MoveToPoint;
    [SerializeField] private Animator _Anim;
    [SerializeField] private AnimationCurve _SpeedCurve;
    [SerializeField] private float _Speed;
    private void Start()
    {
        _Agent = GetComponent<AStarAgent>();
        StartCoroutine(Coroutine_MoveRandom());
    }

    private IEnumerator Coroutine_MoveRandom()
    {
        List<Point> freePoints = WorldManager.Instance.GetFreePoints();
        Point start = freePoints[Random.Range(0, freePoints.Count)];
        transform.position = start.WorldPosition;
        while (true)
        {
            Point p = freePoints[Random.Range(0, freePoints.Count)];

            _Agent.Pathfinding(p.WorldPosition);
            while (_Agent.Status != AStarAgentStatus.Finished)
            {
                yield return null;
            }
        }
    }

    private IEnumerator Coroutine_Animation()
    {
        _Anim.SetBool("Flying", true);
        while (_Agent.Status != AStarAgentStatus.Finished)
        {
            yield return null;
        }
        _Anim.SetBool("Flying", false);
    }
}