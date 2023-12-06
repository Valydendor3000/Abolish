using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    int keys;
    public Transform Player;
    public GameObject TeleportTo;

    public void OnTriggerEnter(Collider collision)
    {
        if(collision.CompareTag("Teleporter") && keys == 3)
        {
            Player.transform.position = TeleportTo.transform.position;
        }
    }
    public void AddKey()
    {
        keys++;
    }
}
