using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public GameObject Sword;
    public GameObject AttackPoint;
    public float AttackRate;
    public int AttackDamage;
    public LayerMask NPCLayer;
    

    Animator animator;
    float timeSinceAttack;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = Sword.GetComponentInChildren<Animator>();
        Debug.DrawLine(Sword.transform.position, new Vector2(Sword.transform.position.x + 1, Sword.transform.position.y + 2));
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceAttack += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && GetComponent<BlockInteractions>().hasSword && timeSinceAttack > AttackRate && GetComponent<BlockInteractions>().MouseInRange())
        {
            timeSinceAttack = 0;
            animator.Play("SwordSwing");
            FindObjectOfType<AudioManager>().Play("Sword Swing");
            Attack();
            
        }
        
    }

    private void FixedUpdate()
    {
        if (GetComponent<BlockInteractions>().hasSword)
        {
            Sword.GetComponent<SpriteRenderer>().enabled = true;
        }
        else Sword.GetComponent<SpriteRenderer>().enabled = false;
    }

    void Attack()
    {
        Collider2D[] hitEnemies = (Physics2D.OverlapCapsuleAll(AttackPoint.transform.position, new Vector2(1, 2), CapsuleDirection2D.Vertical, 0, NPCLayer));
        //Debug.Log(hitEnemies.Length);
        foreach(Collider2D hits in hitEnemies)
        {
            hits.GetComponent<ZombieControl>().TakeDamage(AttackDamage,true);
            hits.GetComponent<ZombieControl>().ApplyKnockback(transform.position);
        }
                  
        
    }

    



}
