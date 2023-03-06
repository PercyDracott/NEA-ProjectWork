using RiptideNetworking;
using RiptideNetworking.Utils;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    textChat,
}

public enum ClientToServerId : ushort
{
    name = 1,
    updatePlayerPosition,
    updateServerMap,
    zombieDeath,
    updateTextChat,
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

    public Server Server { get; private set; }
    [SerializeField] private ushort port;
    [SerializeField] private ushort maxClientCount;

    //[SerializeField] private TMP_InputField ipField;
    [SerializeField] private TMP_InputField portField;
    [SerializeField] private Button CurrentServerState;
    
    


    private void Awake()
    {
        instance = this;
        Application.runInBackground = true;
    }

    private void Start()
    {
        //Application.targetFrameRate = 60;
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        Server = new Server();
        //Server.Start(port, maxClientCount);
        Server.ClientDisconnected += PlayerLeft;
        //Server.ClientConnected += SendMap;
        
    }

    private void FixedUpdate()
    {
        Server.Tick();
        if (Server.IsRunning) CurrentServerState.GetComponent<Image>().color = Color.green; 
        else CurrentServerState.GetComponent<Image>().color = Color.red;
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        Destroy(Player.list[e.Id].gameObject);
    }

    public void StartFromButtons()
    {
        if (!string.IsNullOrEmpty(portField.text))
        {
            port = Convert.ToUInt16(portField.text);
            //port = Convert.ToUInt16(ipField.text);
            Server.Start(port, maxClientCount);
            
        }
        

        
    }

    public void StopFromButton()
    {
        Server.Stop();
        
    }
    
    //[MessageHandler((ushort)ClientToServerId.updatePlayerPosition)]
    //private static void PlayerPos(Message message)
    //{
    //    Debug.Log("Received Call");

    //}









}
