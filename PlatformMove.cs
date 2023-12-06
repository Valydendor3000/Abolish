using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMove : MonoBehaviour
{
    [SerializeField] GameObject[] waypoints;
    int currentwaypointIndex = 0;

    [SerializeField] float speed = 1f;

    void Update()
    {
        if(Vector3.Distance(transform.position, waypoints[currentwaypointIndex].transform.position) < .1f)
        {
            currentwaypointIndex++;
            if(currentwaypointIndex >= waypoints.Length)
            {
                currentwaypointIndex = 0;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentwaypointIndex].transform.position, speed * Time.deltaTime);
    }
}
