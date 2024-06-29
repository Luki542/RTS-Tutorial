using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    const string animatorSpeed = "Speed",
    animateorAlive = "Alive",
    animatorAttack = "Attack";

    public Transform target;
    NavMeshAgent nav;
    Animator animator;


    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        if(target)
        {
            nav.SetDestination(target.position);   
        }

        Animate();

    }

    protected virtual void Animate()
    {
        var speedVector = nav.velocity;
        speedVector.y = 0;
        float speed = speedVector.magnitude;
        animator.SetFloat(animatorSpeed, speed);
    }
}
