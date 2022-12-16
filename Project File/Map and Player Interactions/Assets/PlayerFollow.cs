using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    public Transform player;
    public float offset;
    public float offsety;
    
    
    Vector3 PlayerPos;
    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        PlayerPos = new Vector3(player.position.x - offset, player.position.y - offsety, -10);
        transform.position = PlayerPos;
    }

}
