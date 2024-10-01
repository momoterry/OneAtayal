using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTrigger : MonoBehaviour
{
    public GameObject moveTarget;
    public Vector3 moveVector;
    public float moveDuration = 1.0f;

    protected Vector3 vStartPosition;
    protected float timeLeft = -1.0f;

    protected void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0.0f )
            {
                timeLeft = 0.0f;


            }
            Vector3 pos = vStartPosition + moveVector * ((moveDuration - timeLeft) / moveDuration);
            moveTarget.transform.position = pos;
        }
    }

    public void OnTG(GameObject whoTG)
    {
        if (!moveTarget)
            return;

        vStartPosition = moveTarget.transform.position;
        timeLeft = moveDuration;
        //print("要開始動了喔 !!");
        whoTG.SendMessage("OnActionResult", true, SendMessageOptions.DontRequireReceiver);      //TODO: 改用 Trigger 的方式回應
    }
}