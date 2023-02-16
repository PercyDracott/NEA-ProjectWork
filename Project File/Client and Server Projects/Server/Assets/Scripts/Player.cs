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
        Player player = Instantiate(GameLogic.Instance.PlayerPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity).GetComponent<Player>();
        player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        player.Id = id;
        player.userName = string.IsNullOrEmpty(username) ? "Guest" : username;

        list.Add(id, player);
    }

    private void SendSpawn()
    {
        RiptideNetworking.Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.playerSpawned);
        message.AddUShort(Id);
        message.AddString(userName);
        message.A
    }

    [MessageHandler((ushort)ClientToServerId.name)]
    private static void Name(ushort fromClientId, Message message)
    {
        Spawn(fromClientId, message.GetString());
    }

    
}
