using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Image healthBar;
    public int playerHealth = 10;
    public float regenTime;
    float timeWhenDamage;
    float healthToGain;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        healthBar.rectTransform.localScale = new Vector3((float)playerHealth / 10, 1, 1);
        HealthRegeneration();
    }

    public void TakeDamage(int damage)
    {
        if (playerHealth - damage > 0)
        {
            playerHealth -= damage;
            timeWhenDamage = Time.time;
        }
        else PlayerDeath();
        FindObjectOfType<AudioManager>().Play("Take Damage");
    }

    public void FallDamage(float distance)
    {
        // Debug.Log(Mathf.RoundToInt(distance / 4));
        if (distance > 6) TakeDamage(Mathf.RoundToInt(distance / 2));
        
    }

    void HealthRegeneration()
    {
        if (playerHealth < 10 && playerHealth != 0)
        {
            if (Time.time - timeWhenDamage > regenTime)
            {
                healthToGain += Time.deltaTime;
                playerHealth += Mathf.RoundToInt(healthToGain);
            }
            else healthToGain = 0;
        }        
    }

    void PlayerDeath()
    {
        playerHealth = 10;
        FindObjectOfType<WorldEventManager>().PlayerDeath(gameObject);
    }
}
