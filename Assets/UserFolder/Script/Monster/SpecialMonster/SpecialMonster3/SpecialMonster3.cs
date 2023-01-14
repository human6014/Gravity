using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialMonster3 : MonoBehaviour
{
    Boids boids;
    Animator animator;
    private void Awake()
    {
        boids = GetComponent<Boids>();
        //animator = GetComponent<Animator>();
    }

    void Start()
    {
        boids.GenerateBoidMonster(500);
    }
}
