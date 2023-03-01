using RiptideNetworking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private static GameLogic instance;
    public bool gameIsRunning { get; set; }


    public static GameLogic Instance
    {
        get => instance;
        private set
        {
            if (instance == null) instance = value;
            else if (instance != value)
            {
                Debug.Log($"{nameof(GameLogic)} instance already exists, destroying new");
                Destroy(value);
            }
        }
    }

    public GameObject PlayerPrefab => playerPrefab;

    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;

    private void Awake()
    {
        Instance = this;
        
    }

    float timeSinceActive = 0f;
    private void FixedUpdate()
    {
        timeSinceActive += Time.deltaTime;
        if (timeSinceActive > 1.0f && FindObjectOfType<NetworkManager>().Server.IsRunning)
        {
            RiptideNetworking.Message message = Message.Create(MessageSendMode.unreliable, (ushort)ServerToClientId.lightPosition);
            message.AddVector3(FindObjectOfType<DayNightCycle>().ReturnLightPosition());
            NetworkManager.Instance.Server.SendToAll(message);
            //Debug.Log("light Pos sent");
            timeSinceActive = 0f;
        }
    }

    

}
