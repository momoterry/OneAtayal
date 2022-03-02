using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 targetOffset;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        GameObject thePlayer = BattleSystem.GetInstance().GetPlayer();
        if (thePlayer)
        {
            Vector3 newPos = thePlayer.transform.position + targetOffset;
            newPos.z = transform.position.z;

            //TODO Smooth move
            transform.position = newPos;
        }
    }
}
