using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hp_BarHandler : MonoBehaviour
{
    public GameObject barRef;
    public float barHeight = 1.0f;

    private GameObject myBarObj;
    private HP_Bar myBar;

    // Start is called before the first frame update
    void Start()
    {
        myBarObj = Instantiate(barRef, Vector3.zero, Quaternion.identity, null);
        myBar = myBarObj.GetComponent<HP_Bar>();
        myBar.SetValue(50.0f, 100.0f);
        SetBarPosition();
    }

    // Update is called once per frame
    void Update()
    {
        SetBarPosition();
    }

    private void SetBarPosition()
    {
        Vector3 pos = transform.position;
        pos.y += barHeight;
        if (myBar)
            myBar.SetWorldPosition(pos);
    }

    private void OnDestroy()
    {
        if (myBarObj)
        {
            Destroy(myBarObj);
        }
    }

    public void SetHP( float hp, float hpMax)
    {
        if (myBar)
            myBar.SetValue(hp, hpMax);
    }
}
