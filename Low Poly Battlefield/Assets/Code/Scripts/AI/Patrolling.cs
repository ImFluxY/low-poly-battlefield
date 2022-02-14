using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrolling : MonoBehaviour
{
    [SerializeField]
    private Transform[] points;
    [SerializeField]
    private float speed;

    private Animator animator;
    private int current;

    private void Start()
    {
        animator = GetComponent<Animator>();
        current = 0;
    }

    private void Update()
    {
        if(transform.position != points[current].position)
        {
            transform.position = Vector3.MoveTowards(transform.position, points[current].position, speed * Time.deltaTime);
            animator.SetFloat("Locomotion", 1, 1, Time.deltaTime);
        }
        else
        {
            animator.SetFloat("Locomotion", 0, 1, Time.deltaTime);
            current = (current + 1) % points.Length;
        }
    }
}
