using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using RiptideNetworking.Utils;
//using static UnityEditor.Progress;

public class Zombie : MonoBehaviour
{
    public static Dictionary<ushort, Zombie> list = new Dictionary<ushort, Zombie>();

    public ushort Id { get; private set; }


    private void OnDestroy()
    {
        list.Remove(Id);
    }


    public void SendId(ushort id)
    {
        Zombie zombie = GetComponent<Zombie>();
        zombie.Id = id;
        list.Add(id, zombie);
    }

    private void FixedUpdate()
    {
        SendPositionToClients();
    }

    private void SendPositionToClients()
    {
        Message message = Message.Create(MessageSendMode.unreliable, (ushort)ServerToClientId.zombiePosition);
        message.AddUShort(Id);
        message.AddVector3(list[Id].gameObject.transform.position);
        
        NetworkManager.Instance.Server.SendToAll(message);
    }

    public void Despawn()
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.zombieDeath);
        message.AddUShort(Id);
        NetworkManager.Instance.Server.SendToAll(message);
        Destroy(list[Id].gameObject);
    }

    [MessageHandler((ushort)ClientToServerId.zombieDeath)]
    private static void ZombieKilledByPlayer(ushort fromClientId, Message message)
    {
        ushort idOfDeath = message.GetUShort();

        Message message2 = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.zombieDeath);
        message2.AddUShort(idOfDeath);
        NetworkManager.Instance.Server.SendToAll(message2);
        Destroy(list[idOfDeath].gameObject);
    }

}
