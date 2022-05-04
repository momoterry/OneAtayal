using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneScroller : MonoBehaviour
{
    // Start is called before the first frame update
    protected float startPos = 40.0f;
    protected float endPos = -40.0f;

    protected float scrollSpeed = 10.0f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 thePos = transform.position;
        thePos.z -= Time.deltaTime * scrollSpeed;
        if (thePos.z < endPos)
        {
            thePos.z = startPos;
        }
        transform.position = thePos;
    }
}
