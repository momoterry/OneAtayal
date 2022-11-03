using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMDirBlender : BoxBlender
{
    public float angle_1 = 0.0f;
    public float angle_2 = 90.0f;
    //public bool blendHori = true;   //垂直改變為 false

    //protected BoxCollider areaBox;
    //protected PlayerControllerBase thePlayerIn = null;

    //protected float v1;
    //protected float v2;

    protected float angle;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    areaBox = GetComponent<BoxCollider>();
    //    if (!areaBox)
    //    {
    //        print("ERROR!! DMDirBlender need a BoxCollider !!!!!");
    //        return;
    //    }

    //    if (blendHori)
    //    {
    //        v1 = areaBox.transform.position.x + areaBox.center.x - ( areaBox.size.x * 0.5f );
    //        v2 = areaBox.transform.position.x + areaBox.center.x + (areaBox.size.x * 0.5f);
    //    }
    //    else
    //    {
    //        v1 = areaBox.transform.position.z + areaBox.center.z - (areaBox.size.z * 0.5f);
    //        v2 = areaBox.transform.position.z + areaBox.center.z + (areaBox.size.z * 0.5f);
    //    }

    //}

    //private float GetBlendRatio()
    //{
    //    float ratio = 0;
    //    if (thePlayerIn && areaBox)
    //    {
    //        float vo;
    //        if (blendHori)
    //        {
    //            vo = thePlayerIn.transform.position.x;
    //        }
    //        else
    //        {
    //            vo = thePlayerIn.transform.position.z;
    //        }

    //        ratio = (vo - v1) / (v2 - v1);
    //        //print(vo + "_" + v1 + "_" + v2);
    //        ratio = Mathf.Clamp(ratio, 0, 1.0f);
    //        //print(ratio);
    //    }
    //    return ratio;
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    if (thePlayerIn)
    //    {
    //        float ratio = GetBlendRatio();
    //        angle = angle_2 * ratio + angle_1 * (1 - ratio);
    //        //print("Angle:" + angle + "  ratio = " + ratio);
    //        if (thePlayerIn.GetDollManager())
    //        {
    //            thePlayerIn.GetDollManager().ForceSetDirection(angle);
    //        }
    //    }
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!other.CompareTag("Player"))
    //        return;

    //    thePlayerIn = other.GetComponent<PlayerControllerBase>();

    //    //print("DMDirBlender: Player In !!");

    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (!other.CompareTag("Player"))
    //        return;

    //    if (thePlayerIn)
    //    {
    //        float ratio = GetBlendRatio();
    //        angle = ratio > 0.5f ? angle_2 : angle_1;
    //        if (thePlayerIn.GetDollManager())
    //        {
    //            thePlayerIn.GetDollManager().ForceSetDirection(angle);
    //        }
    //    }
    //    thePlayerIn = null;
    //}

    protected override void ApplyBlendResult()
    {
        angle = ratio * angle_2 + angle_1 * (1 - ratio);
        if (thePlayerIn.GetDollManager())
        {
            thePlayerIn.GetDollManager().ForceSetDirection(angle);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (thePlayerIn)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(thePlayerIn.transform.position, thePlayerIn.transform.position + Quaternion.Euler(0, angle, 0) * Vector3.forward * 2.0f);
        }
    }

}
