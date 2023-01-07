using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldEventManager : MonoBehaviour
{
    public bool EnableDayNightCycle = true;
    public GameObject PlayerPreFab;
    //public GameObject MapManager;

    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<GenerationScriptV2>().Generation();
        Instantiate(PlayerPreFab, GetComponentInChildren<GenerationScriptV2>().PlayerSpawnPoint(), Quaternion.identity);
        Debug.Log("finito");

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetComponentInChildren<DayNightCycle>().DayNightEnabled = EnableDayNightCycle;
    }

    public void PlayerDeath(GameObject Player)
    {
        Player.GetComponentInChildren<PlayerMenuManager>().EnableDeathScreen();
        Player.GetComponent<BlockInteractions>().EmptyInventory();
        Player.transform.position = new Vector3(-1000, 0, 0);
        
    }

    public void PlayerRespawn(GameObject Player)
    {
        Player.transform.position = GetComponentInChildren<GenerationScriptV2>().PlayerSpawnPoint();
    }

    public void SaveAll()
    {
        GetComponentInChildren<GenerationScriptV2>().SaveMap();
        //Add Player Saving
    }
}
