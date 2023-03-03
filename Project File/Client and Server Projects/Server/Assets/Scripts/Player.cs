using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();

    public ushort Id { get; private set; }
    public string userName { get; private set; }



    public void OnDestroy()
    {
        list.Remove(Id);
    }

    public static void Spawn(ushort id, string username)
    {
        foreach (Player otherPlayers in list.Values)
        {
            otherPlayers.SendSpawn(id);
        }



        Player player = Instantiate(GameLogic.Instance.PlayerPrefab, FindObjectOfType<GenerationScriptV2>().PlayerSpawnPoint(), Quaternion.identity).GetComponent<Player>();
        player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        player.Id = id;
        player.userName = string.IsNullOrEmpty(username) ? "Guest" : username;

        player.SendMap(player.Id);
        player.SendSpawn();
        list.Add(id, player);
    }

    private void SendSpawn()
    {
        RiptideNetworking.Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.playerSpawned);

        NetworkManager.Instance.Server.SendToAll(AddSpawnData(message));
    }

    private Message AddSpawnData(Message message)
    {
        message.AddUShort(Id);
        message.AddString(userName);
        message.AddVector3(FindObjectOfType<GenerationScriptV2>().PlayerSpawnPoint());
        return message;
    }

    private void SendSpawn(ushort toClientId)
    {
        RiptideNetworking.Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.playerSpawned);

        NetworkManager.Instance.Server.Send(AddSpawnData(message), toClientId);
    }

    private void SendMap(ushort toClientId)
    {
        byte[,] maptoSend = FindObjectOfType<GenerationScriptV2>().SendMap();
        

        //List<int> TempList = new List<int>();
        //int[] mapAs1DArray;
        //for (int y = 0; y < maptoSend.GetLength(0); y++)
        //{
        //    for (int x = 0; x < maptoSend.GetLength(1); x++)
        //    {
        //        TempList.Add(maptoSend[y, x]);
        //    }
        //}
        ////int[] temporaryarray = new int[maptoSend.GetLength(0)];
        ////temporaryarray[0] = maptoSend[i];
        //mapAs1DArray = TempList.ToArray();

        //message.AddInt(maptoSend.GetLength(0));
        //message.AddInt(maptoSend.GetLength(1));
        ////message.AddInts(mapAs1DArray, false, true);
        ///
        for (short y = 0; y < maptoSend.GetLength(1); y++)
        {
            //message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.map);
            Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.map);
            byte[] tempXS = new byte[maptoSend.GetLength(0)];
            for (int x = 0; x < maptoSend.GetLength(0); x++)
            {
                tempXS[x] = maptoSend[x, y];
            }
            //
            //Debug.LogAssertion(y);
            message.AddShort(y);
            message.AddInt(FindObjectOfType<GenerationScriptV2>().worldWidth);
            message.AddBytes(tempXS, false, true);
            //Debug.Log("sending map to client");
            NetworkManager.Instance.Server.Send(message, toClientId);
        }



        //Debug.Log("sending map to client");
        //NetworkManager.Instance.Server.Send(message, toClientId);

    }

    private void FixedUpdate()
    {
        SyncNonLocalPlayers();
    }

    private void SyncNonLocalPlayers()
    {
        foreach (Player players in list.Values)
        {
            Message message = Message.Create(MessageSendMode.unreliable, (ushort)ServerToClientId.syncNonLocalPosition);
            message.AddUShort(players.Id);
            message.AddVector3(players.gameObject.transform.position);
            NetworkManager.Instance.Server.SendToAll(message);
        }
    }

    private static void UpdatePlayerMaps(byte block, int xPos, int yPos, ushort originPlayer)
    {
        
        foreach (Player players in list.Values)
        {
            if (players.Id != originPlayer)
            {
                RiptideNetworking.Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.syncMapUpdate);
                message.AddInt(xPos);
                message.AddInt(yPos);
                message.AddByte(block);
                NetworkManager.Instance.Server.Send(message, players.Id);
            }
        }
    }

    [MessageHandler((ushort)(ClientToServerId.updatePlayerPosition))]
    private static void PlayerPos(ushort fromClientId, Message message)
    {
        Vector3 positionFromMessage = message.GetVector3();
        //ushort playerIdFromMessage = message.GetUShort();
        //Debug.Log($"CALLED POSITION UPDATE, {positionFromMessage.x}:{positionFromMessage.y}");
        list[fromClientId].gameObject.transform.position = positionFromMessage;
        message.Release();

    }

    [MessageHandler((ushort)(ClientToServerId.name))]
    private static void Name(ushort fromClientId, Message message)
    {
        Spawn(fromClientId, message.GetString());
    }

    [MessageHandler((ushort)ClientToServerId.updateServerMap)]
    private static void UpdateMap(ushort fromClientId, Message message)
    {
        //Vector2 BlockPos = message.GetVector2();
        int xPos = message.GetInt();
        int yPos = message.GetInt();
        byte block = message.GetByte();
        //Debug.Log($"Block Update Called at {(int)BlockPos.x}:{(int)BlockPos.y} for {block}");
        FindObjectOfType<GenerationScriptV2>().ServerUpdatingBlock(block, xPos, yPos);
        message.Release();
        UpdatePlayerMaps(block, xPos, yPos, fromClientId);
    }




}




