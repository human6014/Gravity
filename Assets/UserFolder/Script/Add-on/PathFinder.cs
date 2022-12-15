using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathFinder : MonoBehaviour
{
    [SerializeField] private GameObject target;

    private NavMeshAgent navMeshAgent;
    private LineRenderer lineRenderer;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.startWidth = lineRenderer.endWidth = 0.1f;
        lineRenderer.material.color = Color.blue;
        lineRenderer.enabled = false;
    }

    public void MakePath()
    {
        lineRenderer.enabled = true;
        StartCoroutine(MakePathCoroutine());
    }

    IEnumerator MakePathCoroutine()
    {
        navMeshAgent.SetDestination(target.transform.position);
        lineRenderer.SetPosition(0, this.transform.position);

        while (Vector3.Distance(this.transform.position, target.transform.position) > 0.1f)
        {
            lineRenderer.SetPosition(0, this.transform.position);

            DrawPath();

            yield return null;
        }

        lineRenderer.enabled = false;
    }

    void DrawPath()
    {
        int length = navMeshAgent.path.corners.Length;

        lineRenderer.positionCount = length;
        for (int i = 1; i < length; i++)
            lineRenderer.SetPosition(i, navMeshAgent.path.corners[i]);
    }
}