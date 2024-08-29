using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCamera : MonoBehaviour
{
    public Vector3 targetOffset;

    protected float SizeAdjustRatioByScreen = 1.0f;   //因為螢幕解析度而調整   CameraSize
    protected float SizeAdjustByMap = 0f;         //因為關卡需要而調整     CameraSize
    protected float DefaultCameraSize = 10.0f;
    protected Camera theCamera;

    public void SetSizeAdjustRatioByScreen(float ratio)
    {
        SizeAdjustRatioByScreen = ratio;
        SetCameraSize();
    }

    public void SetSizeAdjustByMap(float adjust)
    {
        SizeAdjustByMap = adjust;
        SetCameraSize();
    }

    void Awake()
    {
        theCamera = GetComponent<Camera>();
        DefaultCameraSize = theCamera.orthographicSize;
        SetCameraSize();
    }

    protected void SetCameraSize()
    {
        theCamera.orthographicSize = (DefaultCameraSize + SizeAdjustByMap) * SizeAdjustRatioByScreen;
    }

    // Update is called once per frame
    void Update()
    {

        GameObject thePlayer = BattleSystem.GetInstance().GetPlayer();
        if (thePlayer)
        {
            Vector3 newPos = thePlayer.transform.position + targetOffset;
#if XZ_PLAN
            newPos.y = transform.position.y;
#else
            newPos.z = transform.position.z;
#endif

            //TODO Smooth move
            transform.position = newPos;
        }
    }
}
