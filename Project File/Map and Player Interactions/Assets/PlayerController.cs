using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MoveX;
    public float speed;
    public float jumpHeight;
    public bool OnTheGround;
    public float checkradius;
    public CapsuleCollider2D PlayerCC;
    public Rigidbody2D PlayerRB;
    public LayerMask groundLayer;

    float jumpForce;
   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        OnTheGround = OnGround();


        MoveX = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(MoveX, 0, 0);
        PlayerRB.AddForce(movement * speed);

        jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * PlayerRB.gravityScale));

        

    }

    bool OnGround()
    {
        Vector3 checkPos = new Vector3(transform.position.x, transform.position.y - PlayerCC.bounds.extents.y, 0);
        return Physics2D.OverlapCircle(checkPos, checkradius, groundLayer);
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space) && OnGround())
        {
            PlayerRB.AddForce(new Vector3(0.0f, jumpForce, 0.0f), ForceMode2D.Impulse);
        }
    }
}
