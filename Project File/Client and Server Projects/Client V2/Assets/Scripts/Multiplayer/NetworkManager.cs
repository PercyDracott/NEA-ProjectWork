using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;
using System;
using TMPro;
//using UnityEditor.Experimental.GraphView;

public enum ServerToClientId : ushort
{
    playerSpawned = 1,
    map,
    syncNonLocalPosition,
    syncMapUpdate,
    lightPosition,
    zombieSpawning,
    zombiePosition,
    zombieDeath,
}

public enum ClientToServerId : ushort
{
    name = 1,
    updatePlayerPosition,
    updateServerMap,
    zombieDeath,
    
}


public class NetworkManager : MonoBehaviour
{
    private static NetworkManager instance;

    public static NetworkManager Instance
    {
        get => instance;
        private set
        {
            if (instance == null) instance = value;
            else if (instance != value)
            {
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying new");
                Destroy(value);
            }
        }
    }

    public Client Client { get; private set; }
    public TextChatManger DebugText;
    //[SerializeField] private string ip;
    //[SerializeField] private ushort port;
    
    private void Awake()
    {
        instance = this;
        Application.runInBackground = true;
    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        Client = new Client();

        Client.Connected += DidConnect;
        Client.ConnectionFailed += FailedToConnect;
        Client.Disconnected += DidDisconnect;
        Client.ClientDisconnected += PlayerLeft;
        //Client.Disconnect += CalledLeave;
    }

    private void FixedUpdate()
    {
        Client.Tick();

    }

    private void OnApplicationQuit()
    {
        Client.Disconnect();
    }

    public void Connect(string ip, ushort port)
    {
        //DebugText.AddToChat($"Attemping Connection on {ip}:{port}, at {Time.realtimeSinceStartup}");
        Client.Connect($"{ip}:{port}");
    }



    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        Destroy(Player.list[e.Id]);
    }

    private void DidConnect(object sender, EventArgs e)
    {
        UIManager.Instance.SendName();
        DebugText.AddToChat($"Connected at {Time.realtimeSinceStartup}");
    }

    private void FailedToConnect(object sender, EventArgs e)
    {
        DebugText.AddToChat($"Failed To Connect at {Time.realtimeSinceStartup}");
        UIManager.Instance.BackToMain();
    }

    private void DidDisconnect(object sender, EventArgs e)
    {

        DebugText.AddToChat($"Disconnected at {Time.realtimeSinceStartup}");
        UIManager.Instance.BackToMain();
        foreach (Player players in Player.list.Values)
        {
            Destroy(players.gameObject);
        }
        UIManager.Instance.BackToMain();


    }

    public void CalledLeave()
    {
        Client.Disconnect();
        DebugText.AddToChat($"Player Called Left at {Time.realtimeSinceStartup}");
        Debug.Log("Disconnect Called");
        foreach (Player players in Player.list.Values)
        {
            Destroy(players.gameObject);
        }
        UIManager.Instance.BackToMain();
    }

}
