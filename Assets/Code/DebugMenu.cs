using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    public Transform MenuRoot;
    public Toggle toggleOpenAllLevel;

    static DebugMenu instance;

    protected bool isLevelFree = false;

    static public bool GetIsLevelFree() { return instance.isLevelFree; }

    private void Awake()
    {
        if (instance != null)
            print("ERROR !! 超過一份 DebugMenu 存在 ");
        instance = this;

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
            toggleOpenAllLevel.SetIsOnWithoutNotify(isLevelFree);
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

    public void OnLevelFree( bool value)
    {
        isLevelFree = value;
    }
    public void OnClearAllMainLevels()
    {
        GameSystem.GetLevelManager().DebugClearAllMainLevels();
    }
}
