using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class SpawnController : MonoBehaviour
{
    public int spawnAmount;
    public GameObject DayNightManager;
    public GameObject WorldGenerator;
    public GameObject MobPrefab;
    
    [SerializeField]
    bool Day;
    
    GenerationScriptV2 GenerationScriptInUse;
    DayNightCycle DayNightCycleInUse;
    bool hasSpawnedMobs = false;
    

    // Start is called before the first frame update
    void Start()
    {
        GenerationScriptInUse = WorldGenerator.GetComponent<GenerationScriptV2>();
        DayNightCycleInUse = DayNightManager.GetComponent<DayNightCycle>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Day = DayNightCycleInUse.isDay();
        if (!DayNightCycleInUse.isDay() && GenerationScriptInUse.terrainGenerationComplete && !hasSpawnedMobs)
        {
            ushort ZombieID = 0;
            foreach (var positionElement in FindSpawnLocations())
            {
                FindObjectOfType<GameLogic>().CallZombieSpawn(positionElement, ZombieID);
                ZombieID++;
            }
            hasSpawnedMobs = true;
        }
        //if (DayNightCycleInUse.isDay() && hasSpawnedMobs) FindObjectOfType<Zombie>().DespawnAll();
        if (DayNightCycleInUse.isDay()) hasSpawnedMobs = false;
    }
       
    /// <summary>
    /// Finds valid spawn locations, and returns an array of Vector3
    /// </summary>
    /// <returns></returns>
    Vector3[] FindSpawnLocations()
    {
        Vector3[] spawnLocations = new Vector3[spawnAmount];
        for (int i = 0; i < spawnAmount; i++)
        {
            int tempxPos = UnityEngine.Random.Range(1, GenerationScriptInUse.worldWidth);
            spawnLocations[i] = new Vector3(tempxPos, GenerationScriptInUse.ReturnGroundPosition(tempxPos),0);
        }
        return spawnLocations;
    }
}
