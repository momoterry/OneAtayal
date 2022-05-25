using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackRoot : MonoBehaviour
{
    public Transform[] slots;

    public Transform GetSlot( int slotIndex = 0)
    {

        if (slotIndex < slots.Length && slots[slotIndex]!= null)
        {
            return slots[slotIndex];
        }
        print("ERROR!!!! TrackRoot Invalid slot for Index" + gameObject.name + " Slot " + slotIndex);
        return transform;
    }

    public void OnAnimationDoneAndDestroy()
    {
        Destroy(gameObject);
    }
}
