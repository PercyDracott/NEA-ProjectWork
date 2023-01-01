using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlockInteractions : MonoBehaviour
{
    public GameObject MapManagerObject;
    [SerializeField]
    Vector2 mousePos;
    public int block;
    public int[] inventory = new int[10];
    public int range;

    [SerializeField]
    public bool hasAxe { get; private set; }
    [SerializeField]
    public bool hasSword { get; private set; }

    [SerializeField]
    float timemining;
    Vector2 mousePostionOnCall;


    void Start()
    {
        //transform.position = MapManagerObject.GetComponent<GenerationScriptV2>().PlayerSpawnPoint();
        
    }
   

    // Update is called once per frame
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //if (Input.GetMouseButton(0)) Break();
        
        if (Input.GetMouseButton(1) && block == 0 && MouseInRange()) TimedBreaking();
        if (Input.GetMouseButtonDown(1) && block != 0 && MouseInRange()) Build(block);

        
    }

    void Break()
    {
        //Debug.Log(MapManagerObject.GetComponent<GenerationScriptV2>().ReturnTypeOfBlock((int)System.Math.Truncate(mousePos.x), (int)System.Math.Truncate(mousePos.y)));
        inventory[MapManagerObject.GetComponent<GenerationScriptV2>().BreakBlock((int)System.Math.Truncate(mousePos.x), (int)System.Math.Truncate(mousePos.y))]++;
    }

    void TimedBreaking()
    {
        if (timemining == 0) mousePostionOnCall = new Vector2((int)System.Math.Truncate(mousePos.x), (int)System.Math.Truncate(mousePos.y));
        if (mousePostionOnCall == new Vector2((int)System.Math.Truncate(mousePos.x), (int)System.Math.Truncate(mousePos.y)))
        {
            timemining += Time.deltaTime;
            //Debug.Log(timemining);
            if (timemining > AmountofTimetoBreak())
            {
                Break();
                //Debug.Log("Mined");
                timemining = 0;
            }
        }
        else timemining = 0;
    }

    float AmountofTimetoBreak() 
    {
        float timetobreak;
        switch (MapManagerObject.GetComponent<GenerationScriptV2>().ReturnTypeOfBlock((int)System.Math.Truncate(mousePos.x), (int)System.Math.Truncate(mousePos.y))) 
        {
            case (1):
                timetobreak = 0.6f;
                break;
            case (2):
                timetobreak = 1.8f;
                break;
            case (3):
                timetobreak = 1.8f;
                break;
            case (4):
                timetobreak = 1f;
                break;
            case (5):
                timetobreak = 0.2f;
                break;
            case (6):
                timetobreak = 1f;
                break;
            default:
                timetobreak = 1f;
                break;
        }
        if (hasAxe)
        {
            return timetobreak / 2;
        }
        else return timetobreak;

    }

    void Build(int BlockPassed)
    {
        if (inventory[BlockPassed] > 0 && MapManagerObject.GetComponent<GenerationScriptV2>().BuildBlock(BlockPassed, (int)System.Math.Truncate(mousePos.x), (int)System.Math.Truncate(mousePos.y)))
        {
            inventory[BlockPassed]--;
        }

    }

    public void HudInput(int selectedBlock)
    {
        block = selectedBlock;
    }

    public string QuantityinInventory()
    {
        if (block == 0)
        {
            return "";
        }
        else return inventory[block].ToString();
        
        
    }

    public bool RequestToCraft(int item)
    {
        if (item == 0 && !hasAxe)
        {
            if (inventory[2] >= 12 && inventory[3] >= 12 && inventory[6] >= 8)
            {
                inventory[2] -= 12;
                inventory[3] -= 12;
                inventory[6] -= 8;
                hasAxe = true;
                return true;
            }
        }
        if (item == 1 && !hasSword)
        {
            if (inventory[2] >= 12 && inventory[3] >= 10 && inventory[6] >= 4)
            {
                inventory[2] -= 12;
                inventory[3] -= 10;
                inventory[6] -= 4;
                hasSword = true;
                return true;
            }
        }
        if (item == 2)
        {
            if (inventory[4] >= 1)
            {
                inventory[4] -= 1;
                inventory[6] += 4;
                return true;
            }
        }
        return false;
    }

    public bool MouseInRange()
    {
        if (Mathf.Sqrt((float)(System.Math.Pow((mousePos.x - transform.position.x), 2) + System.Math.Pow((mousePos.y - transform.position.y), 2))) < range) return true;
        else return false;
    }

}
