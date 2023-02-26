using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TextChatManger : MonoBehaviour
{
    // Start is called before the first frame update
    public int chatLength;
    public Queue<string> messages;
    public TMP_Text chat;

    void Start()
    {
        messages = new Queue<string>(chatLength);
        //chat = GetComponent<TextMeshPro>();
        UpdateChat();
    }

    // Update is called once per frame
    void UpdateChat()
    {
        chat.text = "";
        foreach (var item in messages)
        {
            chat.text += "\n" + item;
        }
    }

    public void AddToChat(string Message)
    {
        if (messages.Count == chatLength)
        {
            messages.Dequeue();
            messages.Enqueue(Message);
        }
        else messages.Enqueue(Message);
        UpdateChat();
    }
}
