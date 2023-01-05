using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEventManager : MonoBehaviour
{
    public bool EnableDayNightCycle = true;
    public GameObject Player;
    public GameObject MapManager;

    // Start is called before the first frame update
    void Start()
    {
        MapManager.GetComponent<GenerationScriptV2>().Generation();
        Instantiate(Player, MapManager.GetComponent<GenerationScriptV2>().PlayerSpawnPoint(), Quaternion.identity);
        Debug.Log("finito");

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetComponentInChildren<DayNightCycle>().DayNightEnabled = EnableDayNightCycle;
    }
}
