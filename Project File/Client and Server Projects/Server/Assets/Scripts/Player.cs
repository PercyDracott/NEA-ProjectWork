using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

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



        Player player = Instantiate(GameLogic.Instance.PlayerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity).GetComponent<Player>();
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
        message.AddVector3(transform.position);
        return message;
    }

    private void SendSpawn(ushort toClientId)
    {
        RiptideNetworking.Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.playerSpawned);

        NetworkManager.Instance.Server.Send(AddSpawnData(message), toClientId);
    }

    private void SendMap(ushort toClientId)
    {
        int[,] maptoSend = FindObjectOfType<GenerationScriptV2>().SendMap();
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.map);

        List<int> TempList = new List<int>();
        int[] mapAs1DArray;
        for (int y = 0; y < maptoSend.GetLength(0); y++)
        {
            for (int x = 0; x < maptoSend.GetLength(1); x++)
            {
                TempList.Add(maptoSend[y, x]);
            }
        }
        //int[] temporaryarray = new int[maptoSend.GetLength(0)];
        //temporaryarray[0] = maptoSend[i];
        mapAs1DArray = TempList.ToArray();

        message.AddInt(maptoSend.GetLength(0));
        message.AddInt(maptoSend.GetLength(1));
        message.AddInts(mapAs1DArray, false, true);

        NetworkManager.Instance.Server.Send(message, toClientId);

    }

    [MessageHandler((ushort)ClientToServerId.name)]
    private static void Name(ushort fromClientId, Message message)
    {
        Spawn(fromClientId, message.GetString());
    }


    
}
