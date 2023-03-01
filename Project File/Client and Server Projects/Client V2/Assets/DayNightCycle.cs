using RiptideNetworking;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using static UnityEditor.PlayerSettings;

public class DayNightCycle : MonoBehaviour
{


    //public float MoveY;
    //public float DayOrNightTime;

    //Vector3 StartPos;
    //float timeSinceActive;
    //float time;
    //public bool DayNightEnabled { get; set; }
    //[SerializeField] public GameObject thisLight;


    

    //Days will be 5 mins, Nights wll be 5 mins
    //void FixedUpdate()
    //{
    //    MovingTheLight();
    //}

    //void MovingTheLight()
    //{
    //    if ((MoveY > 0) && DayOrNightTime > 0 && DayNightEnabled)
    //    {
    //        timeSinceActive += Time.deltaTime;
    //        if (timeSinceActive > 2 * DayOrNightTime)
    //        {
    //            timeSinceActive -= 2 * DayOrNightTime;
    //        }


    //        if (timeSinceActive > DayOrNightTime)
    //        {
    //            time = timeSinceActive - DayOrNightTime;
    //            transform.position = new Vector3(StartPos.x, (StartPos.y + MoveY) - MoveY * (time / DayOrNightTime), StartPos.z);

    //        }
    //        else
    //        {
    //            time = timeSinceActive;
    //            transform.position = new Vector3(StartPos.x, StartPos.y + MoveY * (time / DayOrNightTime), StartPos.z);

    //        }

    //    }
    //}


    //public static void ClientLightPosition(Vector3 pos) { transform.position = pos; }

    

    //public bool isDay()
    //{
    //    if (timeSinceActive >= DayOrNightTime * 0.5 && timeSinceActive <= DayOrNightTime * 1.5) return false;
    //    else return true;
        
    //}

}
