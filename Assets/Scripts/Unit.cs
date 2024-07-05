using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    const string animatorSpeed = "Speed",
    animatorAlive = "Alive",
    animatorAttack = "Attack";

    public float healthPercent { get { return hp / hpMax; } }

    public Transform target;
    NavMeshAgent nav;
    Animator animator;

    [SerializeField]
    private float hp, hpMax = 100;
    [SerializeField]
    GameObject hpBarPrefab;


    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        hp = hpMax;
        Instantiate(hpBarPrefab, transform);
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
        animator.SetBool(animatorAlive, hp > 0);
    }
}
