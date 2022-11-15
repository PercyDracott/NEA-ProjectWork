using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Generator : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] int worldWidth;
    [SerializeField] int worldHeight;
    [SerializeField] float Seed;
    [SerializeField] float Smoothness;
    [SerializeField] TileBase TestTile;
    [SerializeField] Tilemap TestTileMap;

    int[,] map;

    void Start()
    {
        Generation();
    }

    void Generation()
    {
        TestTileMap.ClearAllTiles();
        Seed = (float)UnityEngine.Random.Range(0.0f,5.0f);
        map = GenerateNoiseTexture(worldWidth, worldHeight, true);
        map = TerrainGeneration(map);
        Renderer(map, TestTile, TestTileMap);
    }

    public int[,] GenerateNoiseTexture(int worldWidth, int worldHeight, bool empty)
    {
        int[,] map = new int[worldWidth, worldHeight];
        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                if (empty) map[x, y] = 0;
                else map[x, y] = 1;                        
            }
        }
        return map;
    }

    public int[,] TerrainGeneration(int[,] map)
    {
        int perlinNoise;
        for(int x = 0; x < worldWidth; x++)
        {
            perlinNoise = Mathf.RoundToInt(Mathf.PerlinNoise(x / Smoothness, Seed) * worldHeight / 2);
            perlinNoise += worldHeight / 2;
            for (int y = 0; y < perlinNoise; y++)
            {
                map[x, y] = 1;
            }
        }
        return map;
    }

    public void Renderer(int[,] map, TileBase TestTile , Tilemap TestTileMap)
    {
        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                if (map[x,y] == 1)
                {
                    TestTileMap.SetTile(new Vector3Int(x, y, 0), TestTile);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Generation();
        }
    }
}
