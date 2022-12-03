using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoidSetting", menuName = "Scriptable Object/BoidSettings", order = int.MaxValue)]
public class BoidsScriptable : ScriptableObject
{
    [Header("Speed Options")]
    [Tooltip("�̵� �ӵ�")]
    public Vector2 speedRange;

    [Header("Required Weight Options")]
    [Tooltip("������ ������� ��")]
    [Range(0, 10)]
    public float cohesionWeight = 1;
    [Tooltip("������ ���� �������� �� ��")]
    [Range(0, 10)]
    public float alignmentWeight = 1;
    [Tooltip("�������� �и��� ��")]
    [Range(0, 10)]
    public float separationWeight = 1;

    [Header("Additional Weight Options")]
    [Tooltip("Ÿ�� ������ ����")]
    [Range(0, 50)]
    public float targetWeight = 5;
    [Tooltip("�߽� �ֺ� �ӹ����� ����")]
    [Range(0, 100)]
    public float boundsWeight = 10;
    [Tooltip("��ֹ� ȸ���� ����")]
    [Range(0, 100)]
    public float obstacleWeight = 30;
    [Tooltip("���� ������ ���ư� ����")]
    [Range(0, 10)]
    public float egoWeight = 1;

    [Header("Neighbour")]
    [Tooltip("��ֹ� ȸ�� �Ÿ�")]
    public float obstacleDistance = 5;
    [Tooltip("���鿡�� �̿� Ž���� ����")]
    public float FOVAngle = 120;
    [Tooltip("�ִ� Ž���� �̿� ��ü")]
    public float maxNeighbourCount = 5;
    [Tooltip("�̿� ��ü Ž�� �Ÿ�")]
    public float neighbourDistance = 5;

    [Header("ETC")]
    [Tooltip("���� Boid��ü ���̾�")]
    public LayerMask boidUnitLayer;
    [Tooltip("��ֹ� ���̾�")]
    public LayerMask obstacleLayer;
}