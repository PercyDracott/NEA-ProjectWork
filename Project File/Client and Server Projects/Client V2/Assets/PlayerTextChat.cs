using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTextChat : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_InputField input;
       

    public void Send()
    {
        FindObjectOfType<TextChatManger>().AddToChat(input.text);
        FindObjectOfType<AudioManager>().Play("Hud Interact");
        //GetComponent<Player>().UpdateTextChat(input.text);
        GetComponentInParent<Player>().UpdateTextChat(input.text);
    }

    
}
