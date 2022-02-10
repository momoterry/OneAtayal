using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControl : MonoBehaviour
{
    //public PlayerController thePC; //TODO:  need system to get PC

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mPos = Input.mousePosition;
            Vector3 mWorldMousePos = Camera.main.ScreenToWorldPoint(mPos);
            mWorldMousePos.z = 0.0f;

            PlayerController thePC = BattleSystem.GetInstance().GetPlayerController();
            if (thePC)
            {
                thePC.OnAttackToward(mWorldMousePos);
            }
        }
    }

    void OnMouseDown()
    {
        Vector3 mPos = Input.mousePosition;
        Vector3 mWorldMousePos = Camera.main.ScreenToWorldPoint(mPos);
        mWorldMousePos.z = 0.0f;

        PlayerController thePC = BattleSystem.GetInstance().GetPlayerController();
        if (thePC)
        {
            thePC.OnMoveToPosition(mWorldMousePos);
        }
    }

    //
}
