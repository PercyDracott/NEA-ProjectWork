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
    [SerializeField] float RandomPercentFill;
    [SerializeField] TileBase TestTile;
    [SerializeField] Tilemap TestTileMap;
    [SerializeField] int Iterations;
    int[,] map;


    public void Generation()
    {
        map = new int[worldWidth, worldHeight];
        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                map[x, y] = Random.Range(0, 100) < RandomPercentFill ? 1 : 0;
            }
        }
    }

    public void Automata(int[,] map, int count)
    {
        for (int i = 1; i <= count; i++)
        {
            int[,] tempmap = (int[,])map.Clone();
            //Debug.Log(tempmap.GetLength(0));
            //Debug.Log(tempmap.GetLength(1));

            for (int xs = 1; xs < worldWidth-1; xs++)
            {
                for (int ys = 1; ys < worldHeight-1; ys++)
                {
                    int neighbours = 0;
                    neighbours = GetNeighbours(tempmap, xs, ys, neighbours);
                    if (neighbours > 4)
                    {
                        map[xs, ys] = 1;
                    }
                    else
                    {
                        map[xs, ys] = 0;
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

    //int neighbours()
    //{
    //    return map.GetLength(0);
    //}

    public void Renderer(int[,] map, TileBase TestTile , Tilemap TestTileMap)
    {
        for (int x = 0; x < worldWidth; x++)
        {
            for (int y = 0; y < worldHeight; y++)
            {
                if (x == 0 || y == 0 || x == worldWidth - 1 || y == worldHeight - 1)
                {
                    TestTileMap.SetTile(new Vector3Int(x, y, 0), TestTile);
                }
                else if (map[x,y] == 1)
                {
                    TestTileMap.SetTile(new Vector3Int(x, y, 0), TestTile);
                }
            }
        }
    }

    private void Start()
    {
        TestTileMap.ClearAllTiles();
        Generation();
        Automata(map, Iterations);
        Renderer(map, TestTile, TestTileMap);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
