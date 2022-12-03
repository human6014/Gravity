using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoidSetting", menuName = "Scriptable Object/BoidSettings", order = int.MaxValue)]
public class BoidsScriptable : ScriptableObject
{
    [Header("Speed Options")]
    [Tooltip("이동 속도")]
    public Vector2 speedRange;

    [Header("Required Weight Options")]
    [Tooltip("무리와 가까워질 힘")]
    [Range(0, 10)]
    public float cohesionWeight = 1;
    [Tooltip("무리와 같은 방향으로 갈 힘")]
    [Range(0, 10)]
    public float alignmentWeight = 1;
    [Tooltip("무리에서 분리될 힘")]
    [Range(0, 10)]
    public float separationWeight = 1;

    [Header("Additional Weight Options")]
    [Tooltip("타겟 추적할 강도")]
    [Range(0, 50)]
    public float targetWeight = 5;
    [Tooltip("중심 주변 머무르는 강도")]
    [Range(0, 100)]
    public float boundsWeight = 10;
    [Tooltip("장애물 회피할 강도")]
    [Range(0, 100)]
    public float obstacleWeight = 30;
    [Tooltip("본래 값으로 돌아갈 강도")]
    [Range(0, 10)]
    public float egoWeight = 1;

    [Header("Neighbour")]
    [Tooltip("장애물 회피 거리")]
    public float obstacleDistance = 5;
    [Tooltip("정면에서 이웃 탐지할 각도")]
    public float FOVAngle = 120;
    [Tooltip("최대 탐지할 이웃 개체")]
    public float maxNeighbourCount = 5;
    [Tooltip("이웃 개체 탐지 거리")]
    public float neighbourDistance = 5;

    [Header("ETC")]
    [Tooltip("같은 Boid개체 레이어")]
    public LayerMask boidUnitLayer;
    [Tooltip("장애물 레이어")]
    public LayerMask obstacleLayer;
}