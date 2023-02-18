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

    [MessageHandler((ushort)ClientToServerId.name)]
    private static void Name(ushort fromClientId, Message message)
    {
        Spawn(fromClientId, message.GetString());
    }

    
}
