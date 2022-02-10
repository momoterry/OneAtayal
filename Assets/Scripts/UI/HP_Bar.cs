using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HP_Bar : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider barSlider;

    private RectTransform barTransform;

    void Start()
    {
        barTransform = barSlider.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetWorldPosition( Vector3 wPos)
    {
        if (!barTransform)
            barTransform = barSlider.GetComponent<RectTransform>();
        Vector3 uiPos = Camera.main.WorldToScreenPoint(wPos);
        uiPos.z = 0;
        barTransform.position = uiPos;
    }

    public void SetValue( float hp, float hpMax)
    {
        barSlider.value = hp / hpMax;
    }
}
