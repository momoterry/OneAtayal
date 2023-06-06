using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HBackGroundScroller : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject scrollTarget;
    public float scrollSpeed = 8.0f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scrollTarget.transform.position += scrollSpeed * Vector3.left * Time.deltaTime;
    }
}
