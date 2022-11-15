using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject player;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    public void Start()
    {
        Vector3 randomPos = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY),0);
        PhotonNetwork.Instantiate(player.name,randomPos, Quaternion.identity);
    }

}
