using System.Collections;
using RiptideNetworking;
using RiptideNetworking.Utils;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();

    public ushort Id { get; private set; }
    public bool IsLocal { get; private set; }
    public string userName { get; private set; }

    [SerializeField] private GenerationScriptV2 Generation;

    private void OnDestroy()
    {
        list.Remove(Id);
    }

    private void FixedUpdate()
    {
        PassingPlayerPositionToServer();
    }

    private void PassingPlayerPositionToServer()
    {
        if (Id == NetworkManager.Instance.Client.Id)
        {
            //Debug.Log("positon Update called");
            RiptideNetworking.Message message = Message.Create(MessageSendMode.unreliable, (ushort)ClientToServerId.updatePlayerPosition);
            message.AddVector3(list[NetworkManager.Instance.Client.Id].gameObject.transform.position);
            //message.AddUShort(Id);
            NetworkManager.Instance.Client.Send(message);
        }
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

    //private static void CallMapBuild(int xlength, int ylength, int[] mapAs1DArray)
    //{
    //    int[,] map = new int[xlength, ylength];
    //    //int tempy = 0;
    //    for (int y = 0; y < ylength; y++)
    //    {
    //        for (int x = 0; x < xlength; x++)
    //        {
    //            map[x, y] = mapAs1DArray[x];                
    //        }
    //    }
    //    FindObjectOfType<GenerationScriptV2>().Renderer(map);
    //    FindObjectOfType<GenerationScriptV2>().setMap(map);
    //}

    public void SendBlockUpdateToServer(byte x, byte y, int block)
    {
        RiptideNetworking.Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.updateServerMap);
        message.AddByte(x);
        message.AddByte(y);
        message.AddInt(block);
        NetworkManager.Instance.Client.Send(message);
    }

    private static void CallMapBuild(byte[] mapSlice, short mapLayer)
    {
        FindObjectOfType<GenerationScriptV2>().setMapByLayer(mapSlice, mapLayer);
    }

    [MessageHandler((ushort)(ServerToClientId.playerSpawned))]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetVector3());
    }


    [MessageHandler((ushort)(ServerToClientId.map))]
    private static void GettingMapFromServer(Message message)
    {

        //
        short Layer = message.GetShort();
        int lengthofByteArray = message.GetInt();
        byte[] ByteArray = new byte[lengthofByteArray];
        message.GetBytes(lengthofByteArray, ByteArray);
        //short LayerInteger = message.GetShort();
        //Debug.Log(LayerInteger);
        //Debug.LogAssertion(Layer);
        Debug.Log("Received Call From Server To Build Map");
        
        CallMapBuild(ByteArray, Layer);
        message.Release();
       
    }

    [MessageHandler((ushort)(ServerToClientId.syncNonLocalPosition))]
    private static void SyncingPlayers(Message message)
    {
        ushort playerID = message.GetUShort();
        Vector3 playerPosition = message.GetVector3();
        if (playerID != NetworkManager.Instance.Client.Id)
        {
            Player.list[playerID].gameObject.transform.position = playerPosition;
        }        
        message.Release();

    }

    [MessageHandler((ushort)(ServerToClientId.syncMapUpdate))]
    private static void SyncingMaps(Message message)
    {
        byte xPos = message.GetByte();
        byte yPos = message.GetByte();
        int block = message.GetInt();
        //Debug.Log($"Block Update Called at {(int)BlockPos.x}:{(int)BlockPos.y} for {block}");
        FindObjectOfType<WorldEventManager>().GetComponentInChildren<GenerationScriptV2>().ServerUpdatingBlock(block, xPos, yPos);
        message.Release();
        //UpdatePlayerMaps(block, xPos, yPos, fromClientId);

    }



}
