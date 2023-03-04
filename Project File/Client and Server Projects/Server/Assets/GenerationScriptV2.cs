using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


public class GenerationScriptV2 : MonoBehaviour
{
    // Start is called before the first frame update
    public int worldWidth = 1024;
    public int worldHeight = 256;
    public bool terrainGenerationComplete { get; private set; }


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
    [SerializeField] TileBase Log;
    [SerializeField] TileBase Leaf;
    [SerializeField] TileBase Plank;

    public Tilemap TestTileFG;
    public Tilemap TestTileBG;

    [SerializeField] public Slider SeedSlider;
    [SerializeField] public Slider StoneSlider;
    //[SerializeField] public Slider CaveSlider;

    [SerializeField] float RandomPercentFill;
    [SerializeField] int iterations;
    [SerializeField] int TreePopulation;

    string worldName;

    byte[,] map;
    byte[,] cavemap;
    

    void Start()
    {
        worldWidth = 1024;
        worldHeight = 256;

        //terrainGenerationComplete = Generation();
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        Debug.Log("Reloading");
    //        Generation();
    //    }
    public void SetWorldName(string name) { worldName = name; }
        
    public int BreakBlock(int x, int y)
    {
        //Debug.Log($"breakblock called, position {x}, {y}");
        
        
        if (!(map[x, y] == 8 || map[x, y] == 9 || map[x, y] == 0))
        {
            TestTileFG.SetTile(new Vector3Int(x, y, 0), null);
            TestTileFG.RefreshTile(new Vector3Int(x, y, 0));
            int block = map[x,y];
            if (map[x, y] == 1)
            {
                map[x, y] = 8;
            }
            else if (map[x, y] == 2 || map[x, y] == 3)
            {
                map[x, y] = 9;
            }
            else map[x, y] = 0;
            FindObjectOfType<AudioManager>().Play("Block Break");
            return block;
        }
        return 0;
    }

    public int ReturnTypeOfBlock(int x, int y)
    {
        if (!(map[x, y] == 8 || map[x, y] == 9 || map[x, y] == 0))
        {
            int block = map[x, y];
            return block;
        }
        return 0;
    }

    public bool BuildBlock(int block,int x, int y)
    {
        //Debug.Log($"buildblock called, position {x}, {y}");
        if ((map[x-1,y] != 0 || map[x + 1, y] != 0 || map[x, y - 1] != 0 || map[x, y + 1] != 0) && (map[x,y] == 0 || map[x, y] == 8 || map[x, y] == 9))
        {
            map[x, y] = (byte)block;
            TileBase placing = Plank;
            switch (block) 
            {
                case 1: 
                    placing = GrassSoil;
                    break;
                case 2:
                    placing = Stone;
                    break;
                case 4:
                    placing = Log;
                    break;
                case 5:
                    placing = Leaf;
                    break;
                case 6:
                    placing = Plank;
                    break;
                default:
                    return false;
                    
            }

            TestTileFG.SetTile(new Vector3Int(x, y, 0), placing);
            TestTileFG.RefreshTile(new Vector3Int(x, y, 0));
            FindObjectOfType<AudioManager>().Play("Block Place");
            return true;
        }
        return false;
    }

    public void ServerUpdatingBlock(int block, int x, int y)
    {
        map[x, y] = (byte)block;
        TileBase placing = Plank;
        switch (block)
        {
            case 0:
                placing = null;
                break;
            case 1:
                placing = GrassSoil;
                break;
            case 2:
                placing = Stone;
                break;
            case 4:
                placing = Log;
                break;
            case 5:
                placing = Leaf;
                break;
            case 6:
                placing = Plank;
                break;
            case 8:
                placing = SoilBG;
                break;
            case 9:
                placing = StoneBG;
                break;
            default:
                break;

        }
        Debug.Log($"At Position {x}:{y}, Map is now {map[x,y]}");
        if (block == 8 || block == 9)
        {
            TestTileBG.SetTile(new Vector3Int(x, y, 0), placing);
            TestTileFG.SetTile(new Vector3Int(x, y, 0), null);
            TestTileBG.RefreshTile(new Vector3Int(x, y, 0));
            TestTileFG.RefreshTile(new Vector3Int(x, y, 0));
        }
        else
        {
            TestTileFG.SetTile(new Vector3Int(x, y, 0), placing);
            TestTileFG.RefreshTile(new Vector3Int(x, y, 0));
            TestTileBG.RefreshTile(new Vector3Int(x, y, 0));
        }
        //Debug.Log(TestTileFG.GetTile(new Vector3Int(x, y, 0)).name);

    }

    //public void BuildBlock(int block, int x, int y)
    //{
    //    map[x, y] = block;

    //    TestTileFG.SetTile(new Vector3Int(x, y, 0), null);
    //}

    public string CurrentWorldName() { return (worldName); }

    public void GenerateFromSlider()
    {
        Seed = SeedSlider.value;
        StoneSeed = StoneSlider.value;
        //CaveSeed = CaveSlider.value;
        
        map = new byte[worldWidth, worldHeight];
        OptimisedTerrainGeneration(map, worldWidth, worldHeight, GrassSoil, Stone);
        ApplyCaves(map);
        AddTrees(TreePopulation, TestTileFG, Log, Leaf);
        Renderer(map, TestTileFG, TestTileBG);
    }

    public bool Generation(string saveName)
    {
        worldName = saveName;
        TestTileBG.ClearAllTiles();
        TestTileFG.ClearAllTiles();
        Seed = (float)UnityEngine.Random.Range(0.0f, 1000.0f);
        StoneSeed = (float)UnityEngine.Random.Range(0.0f, 1000.0f);
        //CaveSeed = (float)UnityEngine.Random.Range(0.02f, 0.05f);
        OreSeed = (float)UnityEngine.Random.Range(0.03f, 0.05f);

        map = new byte[worldWidth, worldHeight];

        if (IsFromSave())
        {
            //GenerateSaveFile();
            LoadMap();
            SaveMap();
        }
        else
        {
            OptimisedTerrainGeneration(map, worldWidth, worldHeight, GrassSoil, Stone);
            ApplyCaves(map);
            AddTrees(TreePopulation, TestTileFG, Log, Leaf);
        }
        
        Renderer(map, TestTileFG, TestTileBG);
        GenerateSaveFile();
        SaveMap();


        terrainGenerationComplete = true;
        return true;
        //map = Generate2DArray(worldWidth, worldHeight);
        //cavemap = GenerateCaveMap(worldWidth, worldHeight);
        //map = TerrainGeneration(map);
        //Renderer(map, GrassSoil, Stone, TestTileMap);
    }

    public byte[,] SendMap()
    {
        return map;
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

    public void Renderer(byte[,] MapToRender, Tilemap FG, Tilemap BG)
    {
        FG.ClearAllTiles();
        BG.ClearAllTiles();
        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                if (MapToRender[x, y] == 1)
                {
                    FG.SetTile(new Vector3Int(x, y, 0), GrassSoil);
                    BG.SetTile(new Vector3Int(x, y, 0), SoilBG);
                }
                if (MapToRender[x, y] == 2)
                {
                    FG.SetTile(new Vector3Int(x, y, 0), Stone);
                    BG.SetTile(new Vector3Int(x, y, 0), StoneBG);
                }
                if (MapToRender[x, y] == 3)
                {
                    FG.SetTile(new Vector3Int(x, y, 0), Ore);
                    BG.SetTile(new Vector3Int(x, y, 0), StoneBG);
                }
                if (MapToRender[x,y] == 4)
                {
                    FG.SetTile(new Vector3Int(x, y, 0), Log);
                }
                if (MapToRender[x, y] == 5)
                {
                    FG.SetTile(new Vector3Int(x, y, 0), Leaf);
                }
                if (MapToRender[x, y] == 6)
                {
                    FG.SetTile(new Vector3Int(x, y, 0), Plank);
                }
                if (MapToRender[x,y] == 8)
                {
                    BG.SetTile(new Vector3Int(x, y, 0), SoilBG);
                }
                if (MapToRender[x, y] == 9)
                {
                    BG.SetTile(new Vector3Int(x, y, 0), StoneBG);
                }
            }
        }
    }

    void GenerateSaveFile()
    {

        if (!System.IO.File.Exists($"WorldSaves/{worldName}"))
        {
            Directory.CreateDirectory($"WorldSaves/{worldName}");
        }
    }

    bool IsFromSave()
    {
        if (System.IO.File.Exists($"WorldSaves/{worldName}")) return true;
        return false;
        
    }


    
    public void SaveMap()
    {
        using (StreamWriter sw = new StreamWriter($"WorldSaves/{worldName}/worldSave.txt"))
        {
            //sw.WriteLine(worldHeight);
            //sw.WriteLine(worldWidth);
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    sw.Write(map[x, y]);
                }
                sw.WriteLine();
            }
        }
    }

    public void LoadMap()
    {
        int y = 0;
        
        using (StreamReader sr = new StreamReader($"WorldSaves/{worldName}/worldSave.txt"))
        {
            string value;
            
            while ((value = sr.ReadLine()) != null)
            {
                Debug.Log(Convert.ToInt16(value[1]));
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    map[x, y] = (byte)(Convert.ToInt16(value[x]) - 48);
                }
                y++;
            }
        }
        Renderer(map, TestTileFG, TestTileBG);
    }

    /// <summary>
    /// More optimised version of the functions above using a shaired for loop to reduce the number of calls.
    /// </summary>
    /// <param name="worldWidth"></param>
    /// <param name="worldHeight"></param>
    /// <param name="GrassSoil"></param>
    /// <param name="Stone"></param>
    /// <param name="TestTileMap"></param>
    public void OptimisedTerrainGeneration(byte[,] WorldMap, int width, int height, TileBase GrassSoil, TileBase Stone)
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
    public void ApplyCaves(byte[,] map)
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
                int topofworld = Mathf.RoundToInt(Mathf.PerlinNoise(x / Smoothness, Seed) * worldHeight / 5);
                topofworld += worldHeight / 3;
                if (cavemap[x, y] == 0 && y != 0 && y != topofworld-1 && y != topofworld-2)
                {
                    //estTileFG.SetTile(new Vector3Int(x, y, 0), null);
                    if (map[x,y] == 1)
                    {
                        map[x, y] = 8;
                    }
                    if (map[x, y] == 2 || map[x, y] == 3)
                    {
                        map[x, y] = 9;
                    }

                }
            }
        }
    }

    public void CaveGeneration()
    {
        cavemap = new byte[worldWidth, worldHeight];
        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                cavemap[x, y] = (byte)(UnityEngine.Random.Range(0, 100) < RandomPercentFill ? 1 : 0);
            }
        }
    }

    public void Automata(int count)
    {
        for (int i = 1; i <= count; i++)
        {
            byte[,] tempmap = (byte[,])cavemap.Clone();
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



    private int GetNeighbours(byte[,] tempmap, int xs, int ys, int neighbours)
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

    public void AddTrees(int density, Tilemap tilemap, TileBase Log, TileBase Leaf)
    {
        if (density >= worldWidth/6)
        {
            density = worldWidth / 6;
        }

        int tempx = 0;
        int[] positionHistory = new int[density];
        //positionHistory[0] = 0;

        for (int i = 0; i < density; i++)
        {
            bool collisionAvoidance = true;
            while (collisionAvoidance)
            {
                int holdingvariable = UnityEngine.Random.Range(2, worldWidth - 2);
                collisionAvoidance = false;

                for (int j = 0; j <= i; j++)
                {
                    if (Mathf.Abs(holdingvariable - positionHistory[j]) < 6)
                    {
                        //Debug.Log(Mathf.Abs(holdingvariable - positionHistory[j]));

                        collisionAvoidance = true;
                    }
                }
                if (!collisionAvoidance)
                {
                    tempx = holdingvariable;
                }
            }
            positionHistory[i] = tempx;
            
            //Debug.Log($"tempx is {tempx}");
            int tempy = worldHeight-1;
            while (map[tempx,tempy] == 0)
            {
                tempy--;
            }
            tempy++;
            //Debug.Log($"tempy is {tempy}");

            map[tempx,tempy] = 4;
            map[tempx, tempy + 1] = 4;
            map[tempx, tempy + 2] = 5;
            map[tempx, tempy + 3] = 5;
            map[tempx, tempy + 4] = 5;
            map[tempx + 1, tempy + 2] = 5;
            map[tempx - 1, tempy + 2] = 5;
            map[tempx + 1, tempy + 3] = 5;
            map[tempx - 1, tempy + 3] = 5;
            map[tempx + 1, tempy + 4] = 5;
            
        }
    }

    public Vector3Int PlayerSpawnPoint()
    {
        Debug.Log("called");
        Debug.Log(worldHeight);
        Debug.Log(worldWidth);

        int tempy = worldHeight - 1;
        while (map[worldWidth/2, tempy] == 0)
        {
            tempy--;
        }
        tempy += 2;
        Debug.Log($"{worldWidth / 2}, {tempy}");
        return new Vector3Int(worldWidth / 2, tempy, 0);
    }

    public int ReturnGroundPosition(int xPos)
    {
        if (xPos-1 >= worldWidth) return worldHeight;
        int tempy = worldHeight - 1;
        while (map[xPos-1, tempy] == 0)
        {
            tempy--;
        }
        tempy += 5;
        return tempy;
    }

}