using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DollLayoutUIBase : MonoBehaviour
{
    protected DollManager theDM;
    public bool IsMenuActive() { return gameObject.activeSelf; }
    public virtual void OpenMenu()  { gameObject.SetActive(true); }
    public virtual void CloseMenu() { gameObject.SetActive(false); }
    public virtual void SetupDollManager(DollManager dm) { theDM = dm; }
}

public class DollLayoutDynamic : DollLayoutUIBase
{

}
