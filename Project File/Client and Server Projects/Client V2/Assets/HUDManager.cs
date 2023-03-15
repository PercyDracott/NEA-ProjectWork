//using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    
    public int scrollPosition = 1;
    [SerializeField]
    public GameObject Player;
    public UnityEngine.UI.Image Icon;
    public Sprite axeIcon, emptyIcon;
    public Sprite[] BlockIcons = new Sprite[6];
    public TextMeshProUGUI quantityText;
    int[] blockKey = { 0 ,1, 2, 4, 5, 6 };

    public GameObject PauseDeathMenuManager;
    public GameObject craftMenu;
    bool craftMenuShowing;

    // Start is called before the first frame update
    void Start()
    {
        //Temporary Code
        //Player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {

        if (!PauseDeathMenuManager.GetComponent<PlayerMenuManager>().hasAnyMenuOpen)
        {
            MouseScroll();
            SendtoPlayer();
            QuantityText();
        }
        
        //Debug.Log(block);
        //Debug.Log("running");
    }
    /// <summary>
    /// Changes the icon for the Axe tool if one has been crafted
    /// </summary>
    void FixedUpdate()
    {
        if (!PauseDeathMenuManager.GetComponent<PlayerMenuManager>().hasAnyMenuOpen)
        {
            if (PauseDeathMenuManager.GetComponent<PlayerMenuManager>().hasAnyMenuOpen) return;
            if (Player.GetComponent<BlockInteractions>().hasAxe) BlockIcons[0] = axeIcon;
            else BlockIcons[0] = emptyIcon;

            craftMenu.SetActive(craftMenuShowing);
        }
        
    }

    /// <summary>
    /// Controls which icon appears in the hud
    /// </summary>
    void MouseScroll()
    {            
        if (Mathf.RoundToInt(Input.GetAxisRaw("Mouse ScrollWheel") * 10) == 1) HudUP();
        if (Mathf.RoundToInt(Input.GetAxisRaw("Mouse ScrollWheel") * 10) == -1) HudDown();
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
        FindObjectOfType<AudioManager>().Play("Hud Interact");
    }

    public void HudDown()
    {
        //Debug.Log("Down Called");
        scrollPosition += -1;
        scrollPosition = (scrollPosition % blockKey.Length);
        FindObjectOfType<AudioManager>().Play("Hud Interact");
    }

    /// <summary>
    /// Returns a sprite to the itemInHand Script passing the item currently selected in the HUD
    /// </summary>
    /// <returns></returns>
    public Sprite PasstoHand()
    {
        return BlockIcons[Mathf.Abs(scrollPosition)];
    }

    /// <summary>
    /// Shows the Quanitity of block in in the inventory
    /// </summary>
    void QuantityText()
    {
        quantityText.text = (Player.GetComponent<BlockInteractions>().QuantityinInventory());
    }

    /// <summary>
    /// Toggles the crafting screen
    /// </summary>
    public void ToggleCraftMenu()
    {
        craftMenuShowing = !craftMenuShowing;
        FindObjectOfType<AudioManager>().Play("Hud Interact");
    }

    public void CraftRequestAxe()
    {
        if (Player.GetComponent<BlockInteractions>().RequestToCraft(0)) FindObjectOfType<AudioManager>().Play("Craft Success");
        else FindObjectOfType<AudioManager>().Play("Craft Fail");
    }

    public void CraftRequestSword()
    {
        if (Player.GetComponent<BlockInteractions>().RequestToCraft(1)) FindObjectOfType<AudioManager>().Play("Craft Success");
        else FindObjectOfType<AudioManager>().Play("Craft Fail");

    }

    public void CraftRequestPlank()
    {
        if (Player.GetComponent<BlockInteractions>().RequestToCraft(2)) FindObjectOfType<AudioManager>().Play("Craft Success");
        else FindObjectOfType<AudioManager>().Play("Craft Fail");
    }


}
