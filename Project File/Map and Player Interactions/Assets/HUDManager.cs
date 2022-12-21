using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    
    public int scrollPosition = 1;
    public GameObject Player;
    public UnityEngine.UI.Image Icon;
    public Sprite[] BlockIcons = new Sprite[6];
    int[] blockKey = { 0 ,1, 2, 4, 5, 6 };



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MouseScroll();
        SendtoPlayer();
        //Debug.Log(block);
        //Debug.Log("running");
    }

    void MouseScroll()
    {

        scrollPosition += (Mathf.RoundToInt(Input.GetAxisRaw("Mouse ScrollWheel") * 10));
        scrollPosition = (scrollPosition % blockKey.Length);
        
    }

    void SendtoPlayer()
    {
        Player.GetComponent<BlockInteractions>().HudInput(blockKey[Mathf.Abs(scrollPosition)]);
        Icon.sprite = BlockIcons[Mathf.Abs(scrollPosition)];
    }
    
    public void HudUP()
    {
        //Debug.Log("Up Called");
        scrollPosition += 1;
        scrollPosition = (scrollPosition % blockKey.Length);
    }

    public void HudDown()
    {
        //Debug.Log("Down Called");
        scrollPosition += -1;
        scrollPosition = (scrollPosition % blockKey.Length);
    }
       
}
