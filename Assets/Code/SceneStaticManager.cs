using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneStaticManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //��Ҧ����� Z �]���M Y �P��
        for ( int i=0; i<transform.childCount; i++)
        {
            Transform tm = transform.GetChild(i);
            tm.position = new Vector3(tm.position.x, tm.position.y, tm.position.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
