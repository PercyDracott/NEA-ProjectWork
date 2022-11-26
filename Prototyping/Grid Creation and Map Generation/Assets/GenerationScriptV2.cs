using System;
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
    //[SerializeField] float CaveSeed;
    [SerializeField] float OreSeed;

    float Smoothness = 40;
    float OreThreshhold = 0.1f;
    //float CaveThreshhold = 0.7f;
    
    
    [SerializeField] TileBase GrassSoil;
    [SerializeField] TileBase Stone;
    [SerializeField] TileBase SoilBG;
    [SerializeField] TileBase StoneBG;
    [SerializeField] TileBase Ore;

    [SerializeField] Tilemap TestTileFG;
    [SerializeField] Tilemap TestTileBG;

    [SerializeField] public Slider SeedSlider;
    [SerializeField] public Slider StoneSlider;
    //[SerializeField] public Slider CaveSlider;

    [SerializeField] float RandomPercentFill;
    [SerializeField] int iterations;

    int[,] map;
    int[,] cavemap;
    

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
        //CaveSeed = CaveSlider.value;
        
        map = new int[worldWidth, worldHeight];
        OptimisedTerrainGeneration(map, worldWidth, worldHeight, GrassSoil, Stone);
        ApplyCaves(map);
    }

    public void Generation()
    {
        
        TestTileBG.ClearAllTiles();
        TestTileFG.ClearAllTiles();
        Seed = (float)UnityEngine.Random.Range(0.0f, 1000.0f);
        StoneSeed = (float)UnityEngine.Random.Range(0.0f, 1000.0f);
        //CaveSeed = (float)UnityEngine.Random.Range(0.02f, 0.05f);
        OreSeed = (float)UnityEngine.Random.Range(0.03f, 0.05f);

        map = new int[worldWidth, worldHeight];
        OptimisedTerrainGeneration(map, worldWidth, worldHeight, GrassSoil, Stone);
        ApplyCaves(map);

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
    public void OptimisedTerrainGeneration(int[,] WorldMap, int width, int height, TileBase GrassSoil, TileBase Stone)
    {
        int perlinNoiseSoil;
        int perlinNoiseStone;
        for (int x = 0; x < width; x++)
        {
            //Making the Soil
            perlinNoiseSoil = Mathf.RoundToInt(Mathf.PerlinNoise(x / Smoothness, Seed) * height / 5);
            perlinNoiseSoil += height / 3;
            //Debug.Log($"Soil Noise Value is {perlinNoiseSoil}");
            for (int y = 0; y < perlinNoiseSoil; y++)
            {
                WorldMap[x, y] = 1;
            }

            //Making the Stone
            perlinNoiseStone = Mathf.RoundToInt(Mathf.PerlinNoise(x / (Smoothness * 2), StoneSeed) * height / 8);
            perlinNoiseStone += height / 4;
            //Debug.Log($"Stone Noise Value is {perlinNoiseStone}");
            for (int y = 0; y < perlinNoiseStone; y++)
            {
                WorldMap[x, y] = 2;
            }
            
            //Cutting Cacves out and Rendering to Optimise the code using the same for loops
            for (int y = 0; y < perlinNoiseSoil; y++)
            {
                float OrePL = (Mathf.PerlinNoise((float)x * OreSeed, (float)y * OreSeed));

                if (WorldMap[x, y] == 1)
                {
                    TestTileFG.SetTile(new Vector3Int(x, y, 0), GrassSoil);
                    TestTileBG.SetTile(new Vector3Int(x, y, 0), SoilBG);
                }
                if (WorldMap[x, y] == 2)
                {
                    TestTileFG.SetTile(new Vector3Int(x, y, 0), Stone);
                    TestTileBG.SetTile(new Vector3Int(x, y, 0), StoneBG);
                }
                if (OrePL <= OreThreshhold)
                {
                    map[x, y] = 3;
                    TestTileFG.SetTile(new Vector3Int(x, y, 0), Ore);
                }

            }
            
        }
    }

    /// <summary>
    /// Made to Enable me to edit the Cave Generation without messing around with the other parts of the code, however this is less optimised as the loops have to be run in addition to the main generation.
    /// </summary>
    /// <param name="map"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void ApplyCaves(int[,] map)
    {
        //Version 1
        //for (int x = 0; x < width; x++)
        //{
        //    for (int y = 0; y < height; y++)
        //    {
        //        float CavePL = (Mathf.PerlinNoise((float)x * CaveSeed, (float)y * CaveSeed));
        //        if (CavePL >= CaveThreshhold)
        //        {
        //            map[x, y] = 0;
        //            TestTileFG.SetTile(new Vector3Int(x, y, 0), null);
        //        }
        //    }
        //}

        //Version 2 With Automata
        CaveGeneration();
        Automata(iterations);

        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                if (cavemap[x, y] == 0 && y != 0)
                {
                    TestTileFG.SetTile(new Vector3Int(x, y, 0), null);
                    map[x, y] = 0;
                }
            }
        }
    }

    public void CaveGeneration()
    {
        cavemap = new int[worldWidth, worldHeight];
        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                cavemap[x, y] = UnityEngine.Random.Range(0, 100) < RandomPercentFill ? 1 : 0;
            }
        }
    }

    public void Automata(int count)
    {
        for (int i = 1; i <= count; i++)
        {
            int[,] tempmap = (int[,])cavemap.Clone();
            //Debug.Log(tempmap.GetLength(0));
            //Debug.Log(tempmap.GetLength(1));

            for (int xs = 1; xs < worldWidth - 1; xs++)
            {
                for (int ys = 1; ys < worldHeight - 1; ys++)
                {
                    int neighbours = 0;
                    neighbours = GetNeighbours(tempmap, xs, ys, neighbours);
                    if (neighbours > 4)
                    {
                        cavemap[xs, ys] = 1;
                    }
                    else
                    {
                        cavemap[xs, ys] = 0;
                    }
                }
            }
        }
    }

    private int GetNeighbours(int[,] tempmap, int xs, int ys, int neighbours)
    {
        for (int x = xs - 1; x <= xs + 1; x++)
        {
            for (int y = ys - 1; y <= ys + 1; y++)
            {
                if (y <= worldHeight && x <= worldWidth)
                {
                    if (y != ys || x != xs)
                    {
                        //Debug.Log(y);
                        //Debug.Log(x);
                        if (tempmap[x, y] == 1)
                        {
                            neighbours++;
                        }
                    }
                }
            }
        }

        return neighbours;
    }

}