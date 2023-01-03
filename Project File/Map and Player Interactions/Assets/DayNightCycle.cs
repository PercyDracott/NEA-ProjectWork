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


    void Start()
    {
        StartPos = transform.position;
        
    }

    // Days will be 5 mins, Nights wll be 5 mins
    void FixedUpdate()
    {
        MovingTheLight();
    }

    void MovingTheLight()
    {
        if ((MoveY > 0) && DayOrNightTime > 0)
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

    public bool isDay()
    {
        if (timeSinceActive >= DayOrNightTime * 0.5 && timeSinceActive <= DayOrNightTime * 1.5) return false;
        else return true;
        
    }

}
