using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hp_BarHandler : MonoBehaviour
{
    public GameObject barRef;
    public float barHeight = 1.0f;

    private GameObject myBarObj;
    private HP_Bar myBar;
    private HpBar_PA myBarPA;

    // Start is called before the first frame update
    void Start()
    {
#if XZ_PLAN
        myBarObj = Instantiate(barRef, Vector3.zero, Quaternion.Euler(90.0f, 0, 0), null);
#else
        myBarObj = Instantiate(barRef, Vector3.zero, Quaternion.identity, null);
#endif
        myBar = myBarObj.GetComponent<HP_Bar>();
        if (myBar)
            myBar.SetValue(50.0f, 100.0f);
        myBarPA = myBarObj.GetComponent<HpBar_PA>();
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
#if XZ_PLAN
        pos.z += barHeight;
#else
        pos.y += barHeight;
#endif
        if (myBar)
            myBar.SetWorldPosition(pos);
        if (myBarPA)
            myBarPA.SetWorldPosition(pos);
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
        if (myBarPA)
            myBarPA.SetValue(hp, hpMax);
    }

    public void SetMP( float mp, float mpMax)
    {
        if (myBarPA)
            myBarPA.SetMPValue(mp, mpMax);
    }

    public void OnEnable()
    {
        if (myBarObj)
            myBarObj.SetActive(true);
    }

    private void OnDisable()
    {
        if (myBarObj)
            myBarObj.SetActive(false);
    }

}
