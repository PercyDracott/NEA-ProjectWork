using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieControl : MonoBehaviour
{
    public int sightRange;
    public LayerMask playerLayer;
    int ZombieHealth = 10;

    Rigidbody2D zombieRB;
    Animator animator;

    public bool playerInRange { get { return (Physics2D.OverlapCircle(transform.position, sightRange, playerLayer)); } }

    // Start is called before the first frame update
    void Start()
    {
        zombieRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ZombieWalkAnimations();
        FaceTowardsWalkingDirection();
    }

    private void FixedUpdate()
    {
        if (playerInRange)
        {
            //Debug.Log("InRange");
        }

    }

    void ZombieWalkAnimations()
    {
        if (zombieRB.velocity.x != 0)
        {
            animator.Play("ZombieWalk");
        }
        else animator.Play("ZombieIdle");
    }

    void FaceTowardsWalkingDirection()
    {
        if (zombieRB.velocity.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        if (zombieRB.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void TakeDamage(int damage)
    {
        FindObjectOfType<AudioManager>().Play("Take Damage");
        if (ZombieHealth - damage >= 0)
        {
            ZombieHealth -= damage;

        }
        else Destroy(gameObject);
        
    }
}
