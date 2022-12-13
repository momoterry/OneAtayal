using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SPAnimationClip
{
    public Sprite[] sprites;
    public float FPS = 4.0f;
}

public class SPAnimator : MonoBehaviour
{
    public SPAnimationClip IdleUp;
    public SPAnimationClip IdleDown;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
