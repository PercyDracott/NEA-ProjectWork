using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Progress;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();

    public ushort Id { get; private set; }
    public string userName { get; private set; }


    /// <summary>
    /// Removes the player fromteh dictionary if they are destroyed within the scene
    /// </summary>
    public void OnDestroy()
    {
        list.Remove(Id);
    }

    /// <summary>
    /// Called From within a message, by a Client attemptiing to join the server. Adds the player to the Dictionary, along with defining its' identifiers. This is also sent to all other players connected to spawn a Non-Local player
    /// </summary>
    /// <param name="id"></param>
    /// <param name="username"></param>
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
        player.SendMobs(player.Id);
        player.SendSpawn();
        list.Add(id, player);
    }

    /// <summary>
    /// SendSpawn and the Override if a variable is passed in are used to send the spawn location of a new player to the new client or to all existing clients.
    /// </summary>
    private void SendSpawn()
    {
        RiptideNetworking.Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.playerSpawned);

        NetworkManager.Instance.Server.SendToAll(AddSpawnData(message));
    }
        

    private void SendSpawn(ushort toClientId)
    {
        RiptideNetworking.Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.playerSpawned);

        NetworkManager.Instance.Server.Send(AddSpawnData(message), toClientId);
    }

    /// <summary>
    /// This adds the Id, username and a Vector3 position for the new player to be spawned at.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private Message AddSpawnData(Message message)
    {
        message.AddUShort(Id);
        message.AddString(userName);
        message.AddVector3(FindObjectOfType<GenerationScriptV2>().PlayerSpawnPoint());
        return message;
    }

    /// <summary>
    /// Used to send the spawning position of new mobs in a scene, in addition to tellin the client to spawn mobs.
    /// </summary>
    /// <param name="toClientId"></param>
    private void SendMobs(ushort toClientId)
    {
        foreach (var item in Zombie.list.Values)
        {
            Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.zombieSpawning);
            message.AddVector3(item.gameObject.transform.position);
            message.AddUShort(item.Id);
            NetworkManager.Instance.Server.Send(message, toClientId);
        }
    }

    /// <summary>
    /// Sends the Map file from the Server to the client, layer by layer due to the capacity limit of messages.
    /// </summary>
    /// <param name="toClientId"></param>
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

    /// <summary>
    /// Runs 60 times per second.
    /// </summary>
    private void FixedUpdate()
    {
        SyncNonLocalPlayers();
    }

    /// <summary>
    /// Sends the position of all players connected along with their respective Id's to all players.
    /// </summary>
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

    /// <summary>
    /// Sends updates to the map to all but the origional editor of the map
    /// </summary>
    /// <param name="block"></param>
    /// <param name="xPos"></param>
    /// <param name="yPos"></param>
    /// <param name="originPlayer"></param>
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

    /// <summary>
    /// Message handler handles changing the position of a clients Local player on the Server.
    /// </summary>
    /// <param name="fromClientId"></param>
    /// <param name="message"></param>
    [MessageHandler((ushort)(ClientToServerId.updatePlayerPosition))]
    private static void PlayerPos(ushort fromClientId, Message message)
    {
        Vector3 positionFromMessage = message.GetVector3();
        //ushort playerIdFromMessage = message.GetUShort();
        //Debug.Log($"CALLED POSITION UPDATE, {positionFromMessage.x}:{positionFromMessage.y}");
        list[fromClientId].gameObject.transform.position = positionFromMessage;
        message.Release();

    }

    /// <summary>
    /// Sent from the client upon connecting
    /// </summary>
    /// <param name="fromClientId"></param>
    /// <param name="message"></param>
    [MessageHandler((ushort)(ClientToServerId.name))]
    private static void Name(ushort fromClientId, Message message)
    {
        Spawn(fromClientId, message.GetString());
    }

    /// <summary>
    /// Updates the Servers map from a message from a Client.
    /// </summary>
    /// <param name="fromClientId"></param>
    /// <param name="message"></param>
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

    /// <summary>
    /// Updates the Text Chat of all clients but the origin of the chat.
    /// </summary>
    /// <param name="fromClientId"></param>
    /// <param name="message"></param>
    [MessageHandler((ushort)ClientToServerId.updateTextChat)]
    private static void SendUpdatesToPlayers(ushort fromClientId, Message message)
    {
        string text = message.GetString();

        foreach (var item in list.Values)
        {
            if (item.Id != fromClientId)
            {
                Message message2 = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.textChat);
                message2.AddUShort(item.Id);
                message2.AddString(text);
                
                NetworkManager.Instance.Server.Send(message2, item.Id);
            }
        }
    }

  


}




