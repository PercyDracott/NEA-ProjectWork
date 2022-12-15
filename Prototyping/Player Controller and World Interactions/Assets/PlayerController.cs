using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float MoveX;
    public float speed;
    public float jump;
    public bool OnTheGround;
    public float checkradius;
    public CapsuleCollider2D PlayerCC;
    public Rigidbody2D PlayerRB;
    public LayerMask groundLayer;
   

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

        if (Input.GetKey(KeyCode.Space) && OnGround())
        {
            PlayerRB.AddForce(new Vector3(0.0f, jump, 0.0f));
        }

    }

    bool OnGround()
    {
        Vector3 checkPos = new Vector3(transform.position.x, transform.position.y - PlayerCC.bounds.extents.y, 0);
        return Physics2D.OverlapCircle(checkPos, checkradius, groundLayer);
    }
}
