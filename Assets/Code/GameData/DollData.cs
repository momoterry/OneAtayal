using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DollInfo
{
    public GameObject objRef;
    public string dollName;
}

public class DollData : MonoBehaviour
{
    public GameObject[] DollRefs;
    public DollInfo[] DollInfos;
    // Start is called before the first frame update

    protected Dictionary<string, GameObject> theMapping = new Dictionary<string, GameObject>();

    protected Dictionary<string, DollInfo> theDollMapping = new Dictionary<string, DollInfo>();


    public GameObject GetDollRefByID( string dollID )
    {
        //if (!theMapping.ContainsKey(dollID))
        //{
        //    print("ERROR!!! Invalid dollID: " + dollID);
        //    return null;
        //}
        //return theMapping[dollID];
        if (!theDollMapping.ContainsKey(dollID))
        {
            print("ERROR!!! Invalid dollID: " + dollID);
            return null;
        }
        return theDollMapping[dollID].objRef;
    }

    public DollInfo GetDollInfoByID(string dollID)
    {
        if (!theDollMapping.ContainsKey(dollID))
        {
            print("ERROR!!! Invalid dollID: " + dollID);
            return null;
        }
        return theDollMapping[dollID];
    }

    void Awake()
    {
        //foreach ( GameObject doObj in DollRefs)
        //{
        //    Doll d = doObj.GetComponent<Doll>();
        //    if (d)
        //    {
        //        theMapping.Add(d.ID, doObj);
        //    }
        //}
        foreach (DollInfo dInfo in DollInfos)
        {
            if (dInfo != null && dInfo.objRef)
            {
                Doll d = dInfo.objRef.GetComponent<Doll>();
                theDollMapping.Add(d.ID, dInfo);
            }
        }
    }

}
