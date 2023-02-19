using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;
using System;

public enum ServerToClientId : ushort
{
    playerSpawned = 1,
    map,
}

public enum ClientToServerId : ushort
{
    name = 1,
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
    //[SerializeField] private string ip;
    //[SerializeField] private ushort port;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        Client = new Client();

        Client.Connected += DidConnect;
        Client.ConnectionFailed += FailedToConnect;
        Client.Disconnected += DidDisconnect;
        Client.ClientDisconnected += PlayerLeft;
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
        
        Client.Connect($"{ip}:{port}");
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        Destroy(Player.list[e.Id]);
    }

    private void DidConnect(object sender, EventArgs e)
    {
        UIManager.Instance.SendName();

    }

    private void FailedToConnect(object sender, EventArgs e)
    {
        UIManager.Instance.BackToMain();
    }

    private void DidDisconnect(object sender, EventArgs e)
    {
        UIManager.Instance.BackToMain();
    }

}
