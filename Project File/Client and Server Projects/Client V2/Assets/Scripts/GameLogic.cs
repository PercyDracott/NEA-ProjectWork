using RiptideNetworking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private static GameLogic instance;
    private static GameObject worldLight;

    /// <summary>
    /// Creates a Singleton
    /// </summary>
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

    /// <summary>
    /// Called before the object is loaded
    /// </summary>
    private void Awake()
    {
        instance = this;
        worldLight = FindObjectOfType<DayNightCycle>().gameObject;
    }

    /// <summary>
    /// Enable indirect access to the gameobjects, for security reasons
    /// </summary>
    public GameObject LocalPlayerPrefab => localPlayerPrefab;
    public GameObject PlayerPrefab => playerPrefab;
    //public GameObject ZombiePrefab => zombiePrefab;


    /// <summary>
    /// Serialised Fields are used to make the variables show up in the Unity Editor while remaining private
    /// </summary>
    [Header("Prefabs")]
    [SerializeField] private GameObject localPlayerPrefab;
    [SerializeField] private GameObject playerPrefab;

    

    /// <summary>
    /// Message Handler for moving the light source within the scene
    /// </summary>
    /// <param name="message"></param>
    [MessageHandler((ushort)ServerToClientId.lightPosition)]
    private static void ClientLightPosition(Message message)
    {
        worldLight.transform.position = (message.GetVector3());
        //Debug.Log("light pos received");
    }

    /// <summary>
    /// Enable indirect access to the gameobjects, for security reasons
    /// </summary>
    public GameObject ZombiePrefab => zombiePrefab;

    
    [SerializeField] private GameObject zombiePrefab;
    
}
