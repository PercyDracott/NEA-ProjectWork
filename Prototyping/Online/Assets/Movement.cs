using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Movement : MonoBehaviour
{
    public float moveX;
    public float moveY;
    public float playerSpeed;
    PhotonView view;
    // Start is called before the first frame update
    private void Start()
    {
        view = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            moveX = Input.GetAxis("Horizontal");
            moveY = Input.GetAxis("Vertical");
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(moveX * playerSpeed, moveY * playerSpeed);
        }
    }
}
