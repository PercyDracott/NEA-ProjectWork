using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEngine;
using UnityEngine.Rendering.Universal;
//using static UnityEditor.Progress;

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
        int count = 1;
        foreach (var item in messages)
        {
            chat.text += "\n" + count.ToString() + " : " + item;
            count++;
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
