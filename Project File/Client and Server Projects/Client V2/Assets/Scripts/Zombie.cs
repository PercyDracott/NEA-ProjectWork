using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using RiptideNetworking.Utils;

public class Zombie : MonoBehaviour
{
    public static Dictionary<ushort, Zombie> list = new Dictionary<ushort, Zombie>();

    public ushort Id { get; private set; }
    

    private void OnDestroy()
    {
        list.Remove(Id);
    }

    public void DespawnFromClient()
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.zombieDeath);
        message.AddUShort(Id);
        NetworkManager.Instance.Client.Send(message);
        //Destroy(list[Id].gameObject);
    }

    [MessageHandler((ushort)ServerToClientId.zombieSpawning)]
    private static void Spawn(Message message)
    {
        Zombie zombie = Instantiate(GameLogic.Instance.ZombiePrefab, message.GetVector3(), Quaternion.identity).GetComponent<Zombie>();
        zombie.Id = message.GetUShort();
        list.Add(zombie.Id, zombie);
    }

    [MessageHandler((ushort)ServerToClientId.zombiePosition)]
    private static void UpdatingZombiePosition(Message message)
    {
        list[message.GetUShort()].gameObject.transform.position = message.GetVector3();
    }

    [MessageHandler((ushort)ServerToClientId.zombieDeath)]
    private static void DespawnZombie(Message message)
    {
        ushort idOfDeath = message.GetUShort();
        Destroy(list[idOfDeath].gameObject);
        message.Release();
    }
}
