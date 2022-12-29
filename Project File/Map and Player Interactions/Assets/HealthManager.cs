using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Image healthBar;
    public int playerHealth = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        healthBar.rectTransform.localScale = new Vector3((float)playerHealth / 10, 1, 1);
    }

    public void TakeDamage(int damage)
    {
        if (playerHealth - damage >= 0)
        {
            playerHealth = playerHealth - damage;
        }
        else playerHealth = 0;
        FindObjectOfType<AudioManager>().Play("Take Damage");
    }

    public void FallDamage(float distance)
    {
        Debug.Log(Mathf.RoundToInt(distance / 4));
        if (distance > 6) TakeDamage(Mathf.RoundToInt(distance / 4));
        
    }
}
