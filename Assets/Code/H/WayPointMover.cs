using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WayPointMover : MonoBehaviour
{
    public float speed = 10.0f;

    public Vector3[] wayPoints;

    protected Vector3 startPos;
    protected int currentWaypointIndex = 0;

    private void Awake()
    {
        startPos = transform.position;
        print(startPos);
        if (wayPoints.Length > 0)
            transform.position = startPos + wayPoints[0];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWaypointIndex < wayPoints.Length)
        {
            float distance = Vector3.Distance(transform.position, startPos + wayPoints[currentWaypointIndex]);
            if (distance < 0.1f)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= wayPoints.Length)
                    return;
            }
            transform.position = Vector3.MoveTowards(transform.position, startPos + wayPoints[currentWaypointIndex], speed * Time.deltaTime);
        }
    }
}
