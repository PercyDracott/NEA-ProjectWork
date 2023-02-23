using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonLocalPlayerAnimationController : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GetComponent<Rigidbody2D>().velocity.x != 0)
        {
            animator.Play("PlayerWalk");
        }
        else animator.Play("PlayerIdle");
    }
}