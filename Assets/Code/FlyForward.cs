using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyForward : MonoBehaviour
{
    public float flySpeed = 3.0f;

    protected DollManager theDM;

    private void Start()
    {
        theDM = BattleSystem.GetPC().GetDollManager();
    }

    void Update()
    {
        transform.position += Vector3.forward * flySpeed * Time.deltaTime;
        theDM.transform.position = transform.position;
    }
}
