using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    public GameObject Sword;
    public float AttackRate;

    Animator animator;
    float timeSinceAttack;
    bool animationNotPlayed = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = Sword.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceAttack += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && GetComponent<BlockInteractions>().hasSword && timeSinceAttack > AttackRate)
        {
            TimedAttack();
            
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

    void TimedAttack()
    {
        timeSinceAttack = 0;
        animator.Play("SwordSwing");
        FindObjectOfType<AudioManager>().Play("Sword Swing");
        //PlayAnimation();
    }

    void PlayAnimation()
    {
        if (animationNotPlayed)
        {
            
            
        }
    }

   
}
