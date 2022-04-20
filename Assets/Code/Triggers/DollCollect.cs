using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollCollect : MonoBehaviour
{
    public GameObject dollObject;

    protected Doll theDoll;
    // Start is called before the first frame update
    void Start()
    {
        //if (dollObject)
        //theDoll = dollObject.GetComponent<Doll>();
        theDoll = GetComponentInChildren<Doll>();
        if (theDoll == null || theDoll.transform.parent != gameObject.transform)
        {
            print("ERROR !!!! There is no Doll as child of DollCollct !!");
        }
    }


    void OnTG(GameObject whoTG)
    {
        //print("DollCollect OnTG");


        //�^�� ActionTrigger �O�_���\
        bool actionResult = theDoll.TryJoinThePlayer();
        whoTG.SendMessage("OnActionResult", actionResult);
        if (actionResult)
        {
            //����
            theDoll.transform.SetParent(null);
            //�ۧڧR��?
        }
    }
}
