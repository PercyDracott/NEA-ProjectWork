using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class usernameText : MonoBehaviour
{
    [SerializeField] private TMP_Text userName;

    public void ApplyUserName(string name) { userName.text = name; }
}
