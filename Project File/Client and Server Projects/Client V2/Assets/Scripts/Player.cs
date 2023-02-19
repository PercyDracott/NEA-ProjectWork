using System.Collections;
using RiptideNetworking;
using RiptideNetworking.Utils;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();

    public ushort Id { get; private set; }
    public bool IsLocal { get; private set; }
    public string userName { get; private set; }

    private void OnDestroy()
    {
        list.Remove(Id);
    }

    public static void Spawn(ushort id, string username, Vector3 position)
    {
        Player player;
        if (id == NetworkManager.Instance.Client.Id)
        {
            player = Instantiate(GameLogic.Instance.LocalPlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = true;
        }
        else
        {
            player = Instantiate(GameLogic.Instance.LocalPlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = false;
        }
        player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        player.Id = id;
        player.userName = username;
        list.Add(id, player);
    }

    private static void CallMapBuild(int xlength, int ylength, int[] mapAs1DArray)
    {
        int[,] map = new int[xlength, ylength];
        //int tempy = 0;
        for (int y = 0; y < ylength; y++)
        {
            for (int x = 0; x < xlength; x++)
            {
                map[x, y] = mapAs1DArray[x];                
            }
        }
        FindObjectOfType<GenerationScriptV2>().Renderer(map);
        FindObjectOfType<GenerationScriptV2>().setMap(map);
    }

    [MessageHandler((ushort)(ServerToClientId.playerSpawned))]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetVector3());
    }


    [MessageHandler((ushort)(ServerToClientId.map))]
    private static void GettingMapFromServer(Message message)
    {
        CallMapBuild(message.GetInt(), message.GetInt(), message.GetInts());
    }

    

}
