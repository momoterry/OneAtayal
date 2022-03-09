using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderAdjust : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
    }
}
