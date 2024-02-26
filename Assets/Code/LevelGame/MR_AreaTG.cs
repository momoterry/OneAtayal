using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MR_AreaTG : MR_Node
{
    public GameObject[] TriggerTargets;
    public float Width = ROOM_RELATIVE_SIZE;
    public float Height = ROOM_RELATIVE_SIZE;
    public bool triggerOnce = true;
    private bool isTriggered = false;

    protected BoxCollider col = null;

    public override void OnSetupByRoom(MazeGameManager.RoomInfo room)
    {
        base.OnSetupByRoom(room);

        Width *= widthRatio;
        Height *= heightRatio;
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider>();
        }
        col.size = new Vector3 (Width, Height, Height);
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && isTriggered == false)
        {
            //print("Player In !!");
            foreach (GameObject o in TriggerTargets)
            {
                if (o)
                    o.SendMessage("OnTG", other.gameObject);
            }
            if (triggerOnce)
            {
                isTriggered = true;
                enabled = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(Width, 2.0f, Height));
    }
}
