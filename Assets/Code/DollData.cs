using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollData : MonoBehaviour
{
    public GameObject[] DollRefs;
    // Start is called before the first frame update

    protected Dictionary<string, GameObject> theMapping = new Dictionary<string, GameObject>();


    public GameObject GetDollRefByID( string dollID )
    { 
        if (!theMapping.ContainsKey(dollID))
        {
            print("ERROR!!! Invalid dollID: " + dollID);
            return null;
        }
        return theMapping[dollID];
    }

    void Start()
    {
        foreach ( GameObject doObj in DollRefs)
        {
            Doll d = doObj.GetComponent<Doll>();
            if (d)
            {
                theMapping.Add(d.ID, doObj);
            }
        }
    }

}
