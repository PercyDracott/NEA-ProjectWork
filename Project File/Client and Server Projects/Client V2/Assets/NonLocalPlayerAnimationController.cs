using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonLocalPlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GetComponent<Rigidbody2D>().velocity.x != 0)
        {
            animator.Play("PlayerWalk");
        }
        else animator.Play("PlayerIdle");

        void FaceTowardsWalkingDirection()
        {
            if (rb.velocity.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            if (rb.velocity.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }
}
