using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashFX : MonoBehaviour
{
    // Start is called before the first frame update
    public float LifeTime = 0.2f;


    private float myTime = 0.0f;

    void Start()
    {
        myTime = LifeTime;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y); //�� Y �ȳ]�wZ

        myTime -= Time.deltaTime;
        if ( myTime <= 0.0f)
        {
            Destroy(gameObject);
        }
    }
}
