using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconBlender : MonoBehaviour
{
    public Sprite iconA;
    public Sprite iconB;
    public SpriteRenderer target;
    public Material mat;
    // Start is called before the first frame update
    void Start()
    {
        target.sprite = OneUtility.BlendSprite(iconA, iconB, mat);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
