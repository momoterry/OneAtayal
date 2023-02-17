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
//#if !TOUCH_MOVE
        if (GameSystem.IsUseVpad())
        {
            this.enabled = false;
        }
//#endif
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(1))
        //{
        //    Vector3 mPos = Input.mousePosition;
        //    Vector3 mWorldMousePos = Camera.main.ScreenToWorldPoint(mPos);
        //    mWorldMousePos.z = 0.0f;

        //    PlayerController thePC = BattleSystem.GetInstance().GetPlayerController();
        //    if (thePC)
        //    {
        //        //thePC.OnAttackToward(mWorldMousePos);
        //    }
        //}

        if (isTouching)
        {
            //print("isTouching !! " + thePC);
            Vector3 mPos = Input.mousePosition;
            Vector3 mWorldMousePos = Camera.main.ScreenToWorldPoint(mPos);
            mWorldMousePos.y = 0.0f;
            if (thePC)
            {
                thePC.OnMoveToPosition(mWorldMousePos);
            }
        }
    }

    //void OnMouseDown()
    //{
    //    if (EventSystem.current.IsPointerOverGameObject())
    //    {
    //        //ªí¥Ü«ö¨ì UI
    //        return;
    //    }

    //    //print("Mouse Down !!");

    //    thePC = BattleSystem.GetPC();
    //    if (thePC)
    //    {
    //        //thePC.OnMoveToPosition(mWorldMousePos);
    //        isTouching = true;
    //    }
    //}

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

    //private void OnMouseUp()
    //{
    //    print("Mouse Up !!");
    //    isTouching = false;
    //}

    //
}
