using RiptideNetworking;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public static UIManager Instance
    {
        get => instance;
        private set
        {
            if (instance == null) instance = value;
            else if (instance != value)
            {
                Debug.Log($"{nameof(UIManager)} instance already exists, destroying new");
                Destroy(value);
            }
        }
    }

    [Header("Connect")]
    [SerializeField] private GameObject connectUI;
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField ipField;
    [SerializeField] private TMP_InputField portField;

    private void Awake()
    {
        instance = this;
    }

    public void ConnectClicked()
    {
        usernameField.interactable = false;
        connectUI.SetActive(false);
        NetworkManager.Instance.Connect(ipField.text, System.Convert.ToUInt16(portField.text));
    }

    public void BackToMain()
    {
        usernameField.interactable = true;
        connectUI.SetActive(true);
    }

    public void SendName()
    {
        RiptideNetworking.Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.name);
        message.AddString(usernameField.text);
        NetworkManager.Instance.Client.Send(message);
    }
}
