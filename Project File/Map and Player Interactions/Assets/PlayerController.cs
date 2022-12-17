using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveX;
    public float speed;
    public float jumpHeight;
    public bool OnTheGround;
    public float checkradius;
    public CapsuleCollider2D PlayerCC;
    public Rigidbody2D PlayerRB;
    public LayerMask groundLayer;
    Animator animator;

    float jumpForce;
    bool facingRight = true;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        OnTheGround = OnGround();
        xMovePlayer();
        PlayerWalkAnimation();


        jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * PlayerRB.gravityScale));
        if (Input.GetKey(KeyCode.Space) && OnGround())
        {
            PlayerRB.AddForce(new Vector3(0.0f, jumpForce, 0.0f), ForceMode2D.Impulse);
        }


    }

    void xMovePlayer()
    {
        PlayerDirection();
        moveX = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(moveX, 0);
        if (OnGround()) PlayerRB.velocity = movement * speed;
        else if (!OnGround() && moveX != 0) PlayerRB.AddForce(new Vector2(transform.localScale.x, 0));
    }

    void PlayerDirection()
    {
        
        if (moveX < 0.0f && facingRight)
        {
            FlipPlayer();
        }
        else if (moveX > 0.0f && !facingRight)
        {
            FlipPlayer();
        }
    }

    void FlipPlayer()
    {
        facingRight = !facingRight;
        Vector2 localScale = gameObject.transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    bool OnGround()
    {
        Vector3 checkPos = new Vector3(transform.position.x, transform.position.y - PlayerCC.bounds.extents.y, 0);
        return Physics2D.OverlapCircle(checkPos, checkradius, groundLayer);
    }

    void PlayerWalkAnimation()
    {
        if (moveX != 0)
        {
            animator.Play("PlayerWalk");
        }
        else animator.Play("PlayerIdle");
    }

    
}
