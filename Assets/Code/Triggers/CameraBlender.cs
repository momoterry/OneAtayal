using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraBlender : BoxBlender
{
    protected float SizeAdd_1 = 0;
    protected float SizeAdd_2 = 2.0f;

    protected Vector3 OffsetAdd_1 = Vector3.zero;
    protected Vector3 OffsetAdd_2 = new Vector3(2.0f, 0, -2.0f);

    protected Camera theCamera;
    protected BattleCamera theBattleCamera;

    protected float originalSize;
    protected Vector3 originalOffset;

    protected override void Start()
    {
        base.Start();

        print("CameraBlender::Start");
        theCamera = Camera.main;
        originalSize = theCamera.orthographicSize;
        theBattleCamera = theCamera.GetComponent<BattleCamera>();
        if (theBattleCamera)
        {
            originalOffset = theBattleCamera.targetOffset;
        }

    }

    //protected override void OnStartBlend()
    //{
    //    if (theCamera.scaledPixelHeight < theCamera.scaledPixelWidth)
    //    {
    //        SizeAdd_1 = SizeAdd_2 = 0;
    //        OffsetAdd_2 = OffsetAdd_1 = Vector3.zero;
    //    }
    //}

    //protected override void OnEndBlend()
    //{
    //    print("CameraBlender::OnEndBlend");
    //}

    protected override void ApplyBlendResult()
    {
        float sizeAdd = ratio * SizeAdd_2 + SizeAdd_1 * (1 - ratio);
        Vector3 OffsetAdd = ratio * OffsetAdd_2 + OffsetAdd_1 * (1 - ratio);

        if (theCamera.scaledPixelHeight < theCamera.scaledPixelWidth)
        {
            sizeAdd = 0;
            OffsetAdd = Vector3.zero;
        }

        theCamera.orthographicSize = originalSize + sizeAdd;
        if (theBattleCamera)
        {
            theBattleCamera.targetOffset = originalOffset + OffsetAdd;
        }

    }
}
