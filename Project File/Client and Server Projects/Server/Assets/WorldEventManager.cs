using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class WorldEventManager : MonoBehaviour
{
    public bool EnableDayNightCycle = true;
    public GameObject PlayerPreFab;
    [SerializeField] private TMP_InputField worldNameField;
    //public GameObject MapManager;

    // Start is called before the first frame update
    void Start()
    {
        
        //Instantiate(PlayerPreFab, GetComponentInChildren<GenerationScriptV2>().PlayerSpawnPoint(), Quaternion.identity);
        //Debug.Log("finito");

    }

    public void GenerateWorld()
    {
        //GetComponentInChildren<GenerationScriptV2>().SetWorldName(PassingVariables.worldName);
        //Debug.Log(PassingVariables.worldName);
        GetComponentInChildren<GenerationScriptV2>().Generation(worldNameField.text);
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

        //Finding All the Players in the scene
        GameObject[] players;
        players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var item in players)
        {
            item.GetComponent<BlockInteractions>().SavePlayerState(GetComponentInChildren<GenerationScriptV2>().CurrentWorldName());
        }
        
    }
}
