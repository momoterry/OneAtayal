using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashFX : MonoBehaviour
{
    // Start is called before the first frame update
    public float LifeTime = 0.2f;
    public bool randomHoriFlip = false;
    public bool randomVertiFlip = false;


    private float myTime = 0.0f;

    void Start()
    {
        myTime = LifeTime;
        if (randomHoriFlip && Random.Range(0.0f, 1.0f) > 0.5f)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        if (randomVertiFlip && Random.Range(0.0f, 1.0f) > 0.5f)
        {
            transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y); //¥Î Y ­È³]©wZ

        myTime -= Time.deltaTime;
        if ( myTime <= 0.0f)
        {
            Destroy(gameObject);
        }
    }

}
