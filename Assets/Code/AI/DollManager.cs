using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollManager : MonoBehaviour
{
    public Transform[] DollSlots;

    private int slotNum = 0;
    private Doll[] dolls;

    public Transform AddOneDoll(Doll doll)
    {
        for (int i=0; i<slotNum; i++)
        {
            if ( dolls[i] == null && DollSlots[i] != null)
            {
                dolls[i] = doll;
                return DollSlots[i];
            }
        }
        return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform tm in DollSlots)
        {
            if (tm == null)
            {
                print("Error !!! Empty Slot !!");
            }
        }
        slotNum = DollSlots.Length;
        dolls = new Doll[slotNum];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
