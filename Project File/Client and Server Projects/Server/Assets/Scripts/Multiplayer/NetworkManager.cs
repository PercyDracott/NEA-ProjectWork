using RiptideNetworking;
using RiptideNetworking.Utils;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Enumberators used in message headers to direct it to the correct Message Handler
/// </summary>
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
    /// <summary>
    /// A Singleton spawned in on the first action of the scene, a singleton being a class that allows only a single instance of itself to be created and gives access to that created instance.
    /// </summary>
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

    /// <summary>
    /// Serialised fields show up in the unity editor window without being public variables, and are therefore protected.
    /// </summary>
    public Server Server { get; private set; }
    [SerializeField] private ushort port;
    [SerializeField] private ushort maxClientCount;

    //[SerializeField] private TMP_InputField ipField;
    [SerializeField] private TMP_InputField portField;
    [SerializeField] private Button CurrentServerState;
    
    

    /// <summary>
    /// Runs before being loaded.
    /// </summary>
    private void Awake()
    {
        instance = this;
        Application.runInBackground = true;
    }

    /// <summary>
    /// Runs on its first frame of existance, Methods are added to the Servers.ClientDisconnected function so they are also run on the event
    /// </summary>
    private void Start()
    {
        //Application.targetFrameRate = 60;
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        Server = new Server();
        //Server.Start(port, maxClientCount);
        Server.ClientDisconnected += PlayerLeft;
        //Server.ClientConnected += SendMap;
        
    }

    /// <summary>
    /// Runs 60 times per second.
    /// </summary>
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

    /// <summary>
    /// Remopves the player from the scene if they leave.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        Destroy(Player.list[e.Id].gameObject);
    }

    /// <summary>
    /// Used to start a server from the UI within the scene
    /// </summary>
    public void StartFromButtons()
    {
        if (!string.IsNullOrEmpty(portField.text))
        {
            port = Convert.ToUInt16(portField.text);
            //port = Convert.ToUInt16(ipField.text);
            Server.Start(port, maxClientCount);
            
        }
        

        
    }

    /// <summary>
    /// Used as a function accessible to the stop button.
    /// </summary>
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
