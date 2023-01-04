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
            foreach (var positionElement in FindSpawnLocations())
            {
                Instantiate(MobPrefab, positionElement, Quaternion.identity);
                
            }
            hasSpawnedMobs = true;
        }
        if (DayNightCycleInUse.isDay()) hasSpawnedMobs = false;
    }
       

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
