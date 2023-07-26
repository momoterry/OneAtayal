using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class DollSkillButton : MonoBehaviour
{
    public Image icon;

    public void SetIcon(Sprite iconSprite)
    {
        if (icon)
        {
            icon.sprite = iconSprite;
        }
    }

    public void Bind(UnityAction act)
    {
        Button button = gameObject.GetComponent<Button>();
        if (button)
        {
            button.onClick.AddListener(act);
        }
    }

    public void UnBind()
    {
        Button button = gameObject.GetComponent<Button>();
        if (button)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
