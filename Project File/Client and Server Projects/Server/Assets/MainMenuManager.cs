using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField userNameInput;
    [SerializeField] private TMP_InputField worldNameInput;
    [SerializeField] private TMP_InputField iPInput;
    
    public void Create()
    {
        PassingVariables.playerID = userNameInput.text;
        PassingVariables.worldName = worldNameInput.text;
        FindObjectOfType<AudioManager>().Play("Hud Interact");
        //SceneManager.LoadScene("RockAndCaves");
        FindObjectOfType<WorldEventManager>().GenerateWorld(false, worldNameInput.text);
    }

    public void Load()
    {
        FindObjectOfType<WorldEventManager>().GenerateWorld(true, worldNameInput.text);
    }
}
