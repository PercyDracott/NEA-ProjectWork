using RiptideNetworking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private static GameLogic instance;
    private static GameObject worldLight;

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
    private void Awake()
    {
        instance = this;
        worldLight = FindObjectOfType<DayNightCycle>().gameObject;
    }

    public GameObject LocalPlayerPrefab => localPlayerPrefab;
    public GameObject PlayerPrefab => playerPrefab;

    [Header("Prefabs")]
    [SerializeField] private GameObject localPlayerPrefab;
    [SerializeField] private GameObject playerPrefab;

    

    [MessageHandler((ushort)ServerToClientId.lightPosition)]
    private static void ClientLightPosition(Message message)
    {
        worldLight.transform.position = (message.GetVector3());
        //Debug.Log("light pos received");
    }
}
