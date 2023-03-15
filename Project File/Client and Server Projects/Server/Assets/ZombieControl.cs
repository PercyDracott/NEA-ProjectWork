using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieControl : MonoBehaviour
{
    public int sightRange;

    public LayerMask playerLayer;
    public LayerMask groundLayer;

    public GameObject attackPoint;
    public float attackDamage;
    public float attackRate;

    public float knockbackStrength;
    public float knockbackDuration;
    public float walkSpeed;
    public float jumpHeight = 6;
    float jumpForce;

    int ZombieHealth = 10;
    public bool OnTheGround;
    public bool InSight;
    public bool InRange;

    Rigidbody2D zombieRB;
    Animator animator;
    DayNightCycle DayNightCycleInUse;
    GameObject DayNightManager;
    
    float patrolRange;
    float timeSinceActive;
    float time;

    bool hasBeenKnockedBack;
    float timeSinceKB;
    float timeSinceAttack;

    /// <summary>
    /// These are physics overlaps, that return a boolean. If an object on the given layer is within the given range
    /// </summary>
    public bool playerInRange { get { return (Physics2D.OverlapCircle(transform.position, sightRange, playerLayer)); } }
    public bool OnGround { get { return Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - 1.1f), 0.1f, groundLayer); } }
    public bool playerInAttackRange { get { return Physics2D.OverlapCapsule(attackPoint.transform.position, new Vector2(1, 2), CapsuleDirection2D.Vertical, 0, playerLayer); } }
    
    // Start is called before the first frame update
    /// <summary>
    /// This defines many of the components on the zombie gameobject into code
    /// </summary>
    void Start()
    {
        zombieRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        DayNightManager = GameObject.Find("DayNightCycleLight");
        DayNightCycleInUse = DayNightManager.GetComponent<DayNightCycle>();
        patrolRange = UnityEngine.Random.Range(1, 20);
        RandomStartingDirection();
        Physics.IgnoreLayerCollision(gameObject.layer, gameObject.layer);
        timeSinceAttack = attackRate;

    }

    /// <summary>
    /// Picks a randomm starting direction for the zombie, so when they initally patrol they all start walking in different directions
    /// </summary>
    void RandomStartingDirection()
    {
        int temp = Mathf.RoundToInt(UnityEngine.Random.Range(0, 1));
        if (temp == 0) transform.localScale = new Vector2(-1, 0);
        if (temp == 1) transform.localScale = new Vector2(1, 0);


    }
    
    /// <summary>
    /// Used to control the behaviour of each zombie
    /// </summary>
    private void FixedUpdate()
    {
        OnTheGround = OnGround;
        InSight = playerInRange;
        InRange = playerInAttackRange;
        //Debug.DrawLine(new Vector2(transform.position.x, transform.position.y - 1.1f), new Vector2(transform.position.x, transform.position.y - 1.2f));
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y - 0.5f), new Vector2(transform.localScale.x,0), Color.red);
        Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + 0.5f), new Vector2(transform.localScale.x, 0), Color.red);
        
        ZombieWalkAnimations();
        TakeDamageDuringDay();
        JumpUpByOneBlock();
        if (playerInRange)
        {
            FaceTowardsPlayer(playerInRangePosition());
            //if (playerInAttackRange) AttackManager();
            if (!playerInAttackRange) zombieRB.velocity = new Vector2(walkSpeed * transform.localScale.x,zombieRB.velocity.y);
        }
        else IdlePatrol();

        //if (hasBeenKnockedBack)
        //{
        //    timeSinceKB += Time.deltaTime;
        //    if (timeSinceKB >= knockbackDuration) hasBeenKnockedBack = false;
        //}
    }

    //void Attack()
    //{
    //    Collider2D[] hitPlayers = (Physics2D.OverlapCapsuleAll(attackPoint.transform.position, new Vector2(1, 2), CapsuleDirection2D.Vertical, 0, playerLayer));
    //    //Debug.Log(hitEnemies.Length);
    //    foreach (Collider2D hits in hitPlayers)
    //    {
    //        hits.GetComponent<HealthManager>().TakeDamage((int)attackDamage);
            
    //    }


    //}

    //void AttackManager()
    //{
    //    timeSinceAttack += Time.deltaTime;
    //    if (timeSinceAttack > attackRate)
    //    {
    //        timeSinceAttack = 0;
    //        Attack();
    //    }
    //}

    /// <summary>
    /// Zombies local scale is changed so that they are facing a player when in range
    /// </summary>
    /// <param name="playerPos"></param>
    void FaceTowardsPlayer(Vector2 playerPos)
    {
        if (transform.position.x < playerPos.x) transform.localScale = new Vector3(1, 1, 1);
        if (transform.position.x > playerPos.x) transform.localScale = new Vector3(-1, 1, 1);
        Debug.DrawLine(transform.position, playerPos, Color.red);
    }
    
    /// <summary>
    /// Returns a Vector2 of the closest player within its sight range, even if there's many players in range.
    /// </summary>
    /// <returns></returns>
    public Vector2 playerInRangePosition()
    {
        Vector2 closePlayer = new Vector2(0, 0);
        float previousDistance = sightRange;
        if (playerInRange)
        {
            Collider2D[] players = (Physics2D.OverlapCircleAll(transform.position, sightRange, playerLayer));
            float currentDistance;
            foreach (var item in players)
            {
                currentDistance = (Mathf.Sqrt((float)(System.Math.Pow((item.transform.position.x - transform.position.x), 2) + System.Math.Pow((item.transform.position.y - transform.position.y), 2))));
                if (currentDistance < previousDistance)
                {
                    closePlayer = item.transform.position;
                    previousDistance = currentDistance;
                }
            }
        }
        return closePlayer;
    }

    /// <summary>
    /// The zombies will walk in an alternating direction for a random amount of time, as defined in Start()
    /// </summary>
    void IdlePatrol()
    {
        float lengthOfTime = walkSpeed * patrolRange;
        if ((patrolRange > 0) && !playerInRange)
        {
            FaceTowardsWalkingDirection();
            timeSinceActive += Time.deltaTime;
            if (timeSinceActive > 2 * lengthOfTime)
            {
                timeSinceActive -= 2 * lengthOfTime;
            }


            if (timeSinceActive > lengthOfTime)
            {
                time = timeSinceActive - lengthOfTime;
                zombieRB.velocity = new Vector2(walkSpeed * transform.localScale.x, zombieRB.velocity.y);

            }
            else
            {
                time = timeSinceActive;
                zombieRB.velocity = new Vector2(walkSpeed * transform.localScale.x, zombieRB.velocity.y);

            }
        }
        timeSinceActive = 0;
    }

    /// <summary>
    /// If the zombie detects a block infront of itself that is only one block tall, it will jump over it.
    /// </summary>
    void JumpUpByOneBlock()
    {
        if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.5f), new Vector2(transform.localScale.x, 0), 0.5f, groundLayer) && !Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.5f), new Vector2(transform.localScale.x, 0), 0.5f, groundLayer) && OnGround)
        {
            jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * zombieRB.gravityScale));
            zombieRB.AddForce(new Vector3(0.0f, jumpForce, 0.0f), ForceMode2D.Impulse);
            //Debug.Log($"Jump Called, {jumpForce}");
        }

    }

    /// <summary>
    /// Faces the zombie towards its velocity.
    /// </summary>
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

    /// <summary>
    /// Despawns during the daytime
    /// </summary>
    void TakeDamageDuringDay()
    {
        if (DayNightCycleInUse.isDay())
        {
            GetComponent<Zombie>().Despawn();
        }
    }

    /// <summary>
    /// Simple animation trigger
    /// </summary>
    void ZombieWalkAnimations()
    {
        if (zombieRB.velocity.x != 0)
        {
            animator.Play("ZombieWalk");
        }
        else animator.Play("ZombieIdle");
    }
     
    /// <summary>
    /// Unused on the server side
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="isFromPlayer"></param>
    public void TakeDamage(int damage, bool isFromPlayer)
    {
        if (isFromPlayer)
        {
            FindObjectOfType<AudioManager>().Play("Mob Damage");
            
        }
        if (ZombieHealth - damage > 0)
        {
            ZombieHealth -= damage;

        }
        else Destroy(gameObject);
        
    }

    //public void ApplyKnockback(Vector2 attackPos)
    //{
    //    hasBeenKnockedBack = true;
    //    timeSinceKB = 0;
    //    Vector2 direction = ((Vector2)transform.position - attackPos).normalized;
    //    zombieRB.AddForce(direction * knockbackStrength, ForceMode2D.Impulse);
    //    Debug.Log(hasBeenKnockedBack);
    //}
}
