using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MA : MonoBehaviour
{
    public SpriteRenderer[] theSprites = null;
    // Start is called before the first frame update


    protected Color[] oldColors;
    protected float timeMA = 0;

    void Start()
    {
        if (theSprites.Length == 0)
        {
            theSprites = GetComponentsInChildren<SpriteRenderer>();
        }

        oldColors = new Color[theSprites.Length];
        for (int i=0; i<theSprites.Length; i++)
        {
            oldColors[i] = theSprites[i].color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timeMA > 0)
        {
            timeMA -= Time.deltaTime;
            if (timeMA <= 0)
            {
                for (int i = 0; i < theSprites.Length; i++)
                {
                    theSprites[i].color = oldColors[i];
                }
            }
        }
    }

    void OnDamage(Damage theDamage)
    {
        foreach (SpriteRenderer sr in theSprites)
        {
            sr.color = Color.red;
        }
        timeMA = 0.05f;
    }
}
