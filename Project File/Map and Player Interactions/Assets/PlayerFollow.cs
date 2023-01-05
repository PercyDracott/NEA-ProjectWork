using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    public Camera CamOnPlayer;
    public float offsetx;
    public float offsety;
    
    
    Vector3 offset;
    

    // Start is called before the first frame update
    void Start()
    {
        CamOnPlayer = GetComponent<Camera>();
        offset = new Vector3(offsetx, offsety,-10);
    }

    // Update is called once per frame
    void Update()
    {
        
        //CamOnPlayer.transform.position = transform.position + offset;
        
    }

}
