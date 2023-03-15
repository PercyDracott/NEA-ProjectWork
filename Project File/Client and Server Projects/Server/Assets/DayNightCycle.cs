using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    

    public float MoveY;
    public float DayOrNightTime;
    
    Vector3 StartPos;
    float timeSinceActive;
    float time;
    public bool DayNightEnabled { get; set; }


    void Start()
    {
        StartPos = transform.position;
        
    }

    // Days will be 5 mins, Nights wll be 5 mins
    void FixedUpdate()
    {
        if (FindObjectOfType<NetworkManager>().Server.IsRunning) MovingTheLight();
    }

    /// <summary>
    /// Moves the light source within the scene in a periodic vertical wave.
    /// </summary>
    void MovingTheLight()
    {
        if ((MoveY > 0) && DayOrNightTime > 0 && DayNightEnabled)
        {
            timeSinceActive += Time.deltaTime;
            if (timeSinceActive > 2 * DayOrNightTime)
            {
                timeSinceActive -= 2 * DayOrNightTime;
            }


            if (timeSinceActive > DayOrNightTime)
            {
                time = timeSinceActive - DayOrNightTime;
                transform.position = new Vector3(StartPos.x, (StartPos.y + MoveY) - MoveY * (time / DayOrNightTime), StartPos.z);
                
            }
            else
            {
                time = timeSinceActive;
                transform.position = new Vector3(StartPos.x, StartPos.y + MoveY * (time / DayOrNightTime), StartPos.z);
                
            }

        }
    }

    /// <summary>
    /// Returns true if the light is far enough away for the ground to be dark.
    /// </summary>
    /// <returns></returns>
    public bool isDay()
    {
        if (timeSinceActive >= DayOrNightTime * 0.5 && timeSinceActive <= DayOrNightTime * 1.5) return false;
        else return true;
        
    }
    /// <summary>
    /// Returns the position of the light
    /// </summary>
    /// <returns></returns>
    public Vector3 ReturnLightPosition() { return transform.position; }

}
