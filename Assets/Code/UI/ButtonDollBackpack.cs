using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDollBackpack : MonoBehaviour
{
    public Image icon;
    public Text numText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetIcon(Sprite iconSprite)
    {
        if (icon)
        {
            icon.sprite = iconSprite;
        }
    }

    public void SetNum( int num)
    {
        numText.text = "" + num;
    }

    public void OnButtonDown()
    {

    }
}
