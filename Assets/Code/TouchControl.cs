using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchControl : MonoBehaviour
{

    protected PlayerControllerBase thePC = null;

    protected bool isTouching = false;

    // Start is called before the first frame update
    void Start()
    {
        if (GameSystem.IsUseVpad())
        {
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (isTouching)
        {
            //print("isTouching !! " + thePC);
            Vector3 mPos = Input.mousePosition;
            Vector3 mWorldMousePos = Camera.main.ScreenToWorldPoint(mPos);
            mWorldMousePos.y = 0.0f;
            if (thePC)
            {
                if (GameSystem.IsUseDirectionControl())
                {
                    thePC.OnFacePosition(mWorldMousePos);
                }
                else
                {
                    thePC.OnMoveToPosition(mWorldMousePos);
                }
            }
        }
    }


    void OnBattleTouchDown(Vector3 point)
    {
        //print("TouchControl::OnBattleTouchDown!! " + point);
        thePC = BattleSystem.GetPC();
        if (thePC)
        {
            isTouching = true;
        }
    }

    void OnBattleTouchUp()
    {
        isTouching = false;
    }

}
