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
    


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //if (Input.GetMouseButton(0)) Break();
        if (Input.GetMouseButton(1) && block == 0) Break(); 
        if (Input.GetMouseButton(1) && block != 0) Build(block);

        void Break()
        {
            inventory[MapManagerObject.GetComponent<GenerationScriptV2>().BreakBlock((int)System.Math.Truncate(mousePos.x), (int)System.Math.Truncate(mousePos.y))]++;
        }

        void Build(int BlockPassed)
        {
            if (inventory[BlockPassed] > 0 && MapManagerObject.GetComponent<GenerationScriptV2>().BuildBlock(BlockPassed, (int)System.Math.Truncate(mousePos.x), (int)System.Math.Truncate(mousePos.y)))
            {
                inventory[BlockPassed]--;
            }
            
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

}
