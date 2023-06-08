using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;


public class WayPointMover : MonoBehaviour
{
    public float speed = 10.0f;
    //public float turnSpeed = 30.0f;
    public float closeDis = 0.25f;
    public float flowRate = 0.5f;

    public Vector3[] wayPoints;

    protected Vector3 startPos;
    protected int currentWaypointIndex = 0;

    protected Vector3 currDirection;

    protected float flowSpeed = 8.0f;       //TODO: 想辦法透過系統抓到真正「流速」

    private void Awake()
    {
        startPos = transform.position;
        //print(startPos);
        if (wayPoints.Length > 0)
        {
            transform.position = startPos + wayPoints[0];
            currentWaypointIndex++;
        }
        if (wayPoints.Length > 1)
            currDirection = (wayPoints[1] - wayPoints[0]).normalized;

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
            if (distance < closeDis)
            {
                //print("Got Point: " + currentWaypointIndex);
                currentWaypointIndex++;
                if (currentWaypointIndex >= wayPoints.Length)
                    return;
            }

            //Vector3 toDirection = (startPos + wayPoints[currentWaypointIndex] - transform.position).normalized;
            //float targetObjAngle = Vector3.Angle(currDirection, toDirection);
            //float turnStep = turnSpeed * Time.deltaTime;
            //if (targetObjAngle < turnStep)
            //{
            //    currDirection = toDirection;
            //}
            //else 
            //{
            //    currDirection = Vector3.RotateTowards(currDirection, toDirection, turnStep * Mathf.Deg2Rad, 0);
            //}
            //transform.position += currDirection * speed * Time.deltaTime;

            float flowAddRate = (1.0f - (startPos + wayPoints[currentWaypointIndex] - transform.position).normalized.x * flowRate * flowSpeed / speed);
            transform.position = Vector3.MoveTowards(transform.position, startPos + wayPoints[currentWaypointIndex], flowAddRate * speed * Time.deltaTime);

        }
    }
}
