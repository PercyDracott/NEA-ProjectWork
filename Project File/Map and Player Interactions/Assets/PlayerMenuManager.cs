using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenuManager : MonoBehaviour
{
    
    public GameObject PauseMenu;
    public GameObject DeathMenu;
    public GameObject Player;
    public bool hasAnyMenuOpen { get; private set; }

    bool hasDeathMenuOpen;
    bool hasPauseMenuOpen;

    // Start is called before the first frame update
    void Start()
    {
        hasDeathMenuOpen = false;
        hasPauseMenuOpen = false;
        hasAnyMenuOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    void TogglePauseMenu()
    {
        if (!hasDeathMenuOpen)
        {
            if (hasPauseMenuOpen)
            {
                PauseMenu.SetActive(false);
                hasAnyMenuOpen = false;
                hasPauseMenuOpen = false;
            }
            else
            {
                PauseMenu.SetActive(true);
                hasAnyMenuOpen = true;
                hasPauseMenuOpen = true;
            }

            

        }
        
    }

    public void Resume()
    {
        PauseMenu.SetActive(false);
        hasPauseMenuOpen = false;
        hasAnyMenuOpen = false;
        
    }

    public void Leave()
    {
        //Temporary Before Network Code
        Debug.Log("Leave Called");

        PauseMenu.SetActive(false);
        hasPauseMenuOpen = false;
        hasAnyMenuOpen = false;
    }

    public void EnableDeathScreen()
    {
        DeathMenu.SetActive(true);
        hasAnyMenuOpen = true;
        hasDeathMenuOpen = true;
    }

    public void Respawn()
    {
        DeathMenu.SetActive(false);
        hasAnyMenuOpen = false;
        hasDeathMenuOpen = false;
        FindObjectOfType<WorldEventManager>().PlayerRespawn(Player);
    }




}
