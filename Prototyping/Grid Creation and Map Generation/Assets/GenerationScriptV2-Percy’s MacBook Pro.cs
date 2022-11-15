using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


public class GenerationScriptV2 : MonoBehaviour
{
    // Start is called before the first frame update
    public int worldWidth;
    public int worldHeight;
    [SerializeField] float Seed;
    [SerializeField] float StoneSeed;
    [SerializeField] float CaveSeed;
    [SerializeField] float Smoothness;
    [SerializeField] float CaveThreshhold;
    
    
    [SerializeField] TileBase GrassSoil;
    [SerializeField] TileBase Stone;
    [SerializeField] Tilemap TestTileMap;

    [SerializeField] public Slider SeedSlider;
    [SerializeField] public Slider StoneSlider;
    [SerializeField] public Slider CaveSlider;
    public Camera MainCamera;
    //public Texture2D TestTexure;

    //int[,] map;
    //int[,] cavemap;

    void Start()
    {
        Generation();
        
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        Debug.Log("Reloading");
    //        Generation();
    //    }
        
    //}

    public void GenerateFromSlider()
    {
        Seed = SeedSlider.value;
        StoneSeed = StoneSlider.value;
        CaveSeed = CaveSlider.value;

        TestTileMap.ClearAllTiles();
        OptimisedTerrainGeneration(worldWidth, worldHeight, GrassSoil, Stone, TestTileMap);
    }

    public void Generation()
    {
        TestTileMap.ClearAllTiles();
        Seed = (float)UnityEngine.Random.Range(0.0f, 1000.0f);
        StoneSeed = (float)UnityEngine.Random.Range(0.0f, 1000.0f);
        CaveSeed = (float)UnityEngine.Random.Range(0.03f, 0.06f);

        OptimisedTerrainGeneration(worldWidth, worldHeight, GrassSoil, Stone, TestTileMap);

        //map = Generate2DArray(worldWidth, worldHeight);
        //cavemap = GenerateCaveMap(worldWidth, worldHeight);
        //map = TerrainGeneration(map);
        //Renderer(map, GrassSoil, Stone, TestTileMap);
    }

    //public int[,] Generate2DArray(int worldWidth, int worldHeight)
    //{
    //    int[,] map = new int[worldWidth, worldHeight];
    //    for (int x = 0; x < worldWidth; x++)
    //    {
    //        for (int y = 0; y < worldHeight; y++)
    //        {
    //            map[x, y] = 0;
    //        }
    //    }
    //    return map;
    //}

    //public int[,] GenerateCaveMap(int worldWidth, int worldHeight)
    //{
    //    int[,] cavemap = new int[worldWidth, worldHeight];
    //    TestTexure = new Texture2D(worldWidth, worldHeight);

    //    for (int x = 0; x < worldWidth; x++)
    //    {
    //        for (int y = 0; y < worldHeight; y++)
    //        {
    //            float v = Mathf.PerlinNoise((float)x *0.05f, (float)y *0.05f);
    //            TestTexure.SetPixel(x, y, new Color(v, v, v));
    //            if (v >= CaveThreshhold)
    //            {
    //                cavemap[x, y] = 0;
    //            }
    //            else
    //            {
    //                cavemap[x, y] = 1;
    //            }
    //            TestTexure.Apply();
    //        }
    //    }
    //    return cavemap;
    //}
    
    ///// <summary>
    ///// Mathf.PerlinNoise(x,y) -- Takes the X and Y coordinates and produces a smooth random value between 0 and 1
    ///// </summary>
    ///// <param name="map"></param>
    ///// <returns></returns>
    //public int[,] TerrainGeneration(int[,] map)
    //{
    //    int perlinNoise;
    //    for (int x = 0; x < worldWidth; x++)
    //    {
    //        perlinNoise = Mathf.RoundToInt(Mathf.PerlinNoise(x / Smoothness, Seed) * worldHeight / 2);
    //        perlinNoise += worldHeight / 2;
    //        for (int y = 0; y < perlinNoise; y++)
    //        {
    //            map[x, y] = 1;
    //        }
    //    }

    //    perlinNoise = 0;
    //    for (int x = 0; x < worldWidth; x++)
    //    {
    //        perlinNoise = Mathf.RoundToInt(Mathf.PerlinNoise(x / (Smoothness*2), StoneSeed) * worldHeight / 2);
    //        perlinNoise += (worldHeight / 4);
    //        for (int y = 0; y < perlinNoise; y++)
    //        {
    //            map[x, y] = 2;
    //        }
    //    }



    //    return map;
    //}

    //public void Renderer(int[,] map, TileBase GrassSoil, TileBase Stone, Tilemap TestTileMap)
    //{
    //    for (int x = 0; x < worldWidth; x++)
    //    {
    //        for (int y = 0; y < worldHeight; y++)
    //        {
    //            if (map[x, y] == 1 && cavemap[x,y] == 1)
    //            {
    //                TestTileMap.SetTile(new Vector3Int(x, y, 0), GrassSoil);
    //            }
    //            if (map[x, y] == 2 && cavemap[x, y] == 1)
    //            {
    //                TestTileMap.SetTile(new Vector3Int(x, y, 0), Stone);
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// More optimised version of the functions above using a shaired for loop to reduce the number of calls.
    /// </summary>
    /// <param name="worldWidth"></param>
    /// <param name="worldHeight"></param>
    /// <param name="GrassSoil"></param>
    /// <param name="Stone"></param>
    /// <param name="TestTileMap"></param>
    public void OptimisedTerrainGeneration(int worldWidth, int worldHeight, TileBase GrassSoil, TileBase Stone, Tilemap TestTileMap)
    {
        int[,] map = new int[worldWidth, worldHeight];
        int perlinNoise;

        for (int x = 0; x < worldWidth; x++)
        {
            //Making the Soil
            perlinNoise = Mathf.RoundToInt(Mathf.PerlinNoise(x / Smoothness, Seed) * worldHeight / 2);
            perlinNoise += worldHeight / 2;
            for (int y = 0; y < perlinNoise; y++)
            {
                map[x, y] = 1;
            }

            //Making the Stone
            perlinNoise = Mathf.RoundToInt(Mathf.PerlinNoise(x / (Smoothness * 2), StoneSeed) * worldHeight / 2);
            perlinNoise += (worldHeight / 4);
            for (int y = 0; y < perlinNoise; y++)
            {
                map[x, y] = 2;
            }
            
            //Cutting Cacves out and Rendering to Optimise the code using the same for loops
            for (int y = 0; y < worldHeight; y++)
            {
                float v = Mathf.PerlinNoise((float)x * CaveSeed, (float)y * CaveSeed);
                if (v >= CaveThreshhold)
                {
                    map[x, y] = 0;
                }
                if (map[x, y] == 1)
                {
                    TestTileMap.SetTile(new Vector3Int(x, y, 0), GrassSoil);
                }
                if (map[x, y] == 2)
                {
                    TestTileMap.SetTile(new Vector3Int(x, y, 0), Stone);
                }
                
            }
        }
    }

}
