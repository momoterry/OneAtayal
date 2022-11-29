using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    public Transform MenuRoot;
    public Toggle toggleOpenAllLevel;

    static DebugMenu instance;

    protected bool isOpenAllLevel = false;

    public DebugMenu() : base()
    {
        if (instance != null)
            print("ERROR !! 超過一份 DebugMenu 存在 ");
        instance = this;
    }

    static public bool GetIsOpenAllLevel() { return instance.isOpenAllLevel; }

    private void Awake()
    {
        if (MenuRoot)
        {
            MenuRoot.gameObject.SetActive(false);
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

    protected void InitMenuValue()
    {
        if (toggleOpenAllLevel)
        {
            toggleOpenAllLevel.SetIsOnWithoutNotify(isOpenAllLevel);
        }
    }

    public void OnOpenMenu()
    {
        if (MenuRoot)
        {
            MenuRoot.gameObject.SetActive(true);
        }
        InitMenuValue();
    }

    public void OnCloseMenu()
    {
        if (MenuRoot)
        {
            MenuRoot.gameObject.SetActive(false);
        }
    }

    public void OnOpenAllLevel( bool value)
    {
        isOpenAllLevel = value;
    }
}
