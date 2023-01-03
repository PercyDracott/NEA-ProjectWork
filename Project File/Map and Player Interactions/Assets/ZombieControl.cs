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
    DayNightCycle DayNightCycleInUse;
    GameObject DayNightManager;

    public bool playerInRange { get { return (Physics2D.OverlapCircle(transform.position, sightRange, playerLayer)); } }

    // Start is called before the first frame update
    void Start()
    {
        zombieRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        DayNightManager = GameObject.Find("DayNightCycleLight");
        DayNightCycleInUse = DayNightManager.GetComponent<DayNightCycle>();
    }

    // Update is called once per frame
    void Update()
    {
        ZombieWalkAnimations();
        FaceTowardsWalkingDirection();
    }

    private void FixedUpdate()
    {
        TakeDamageDuringDay();
        if (playerInRange)
        {
            //Debug.Log("InRange");
        }

    }

    void TakeDamageDuringDay()
    {
        if (DayNightCycleInUse.isDay())
        {
            TakeDamage(1);
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
        if (ZombieHealth - damage > 0)
        {
            ZombieHealth -= damage;

        }
        else Destroy(gameObject);
        
    }


}
