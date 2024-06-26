using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    public Transform MenuRoot;
    public Toggle toggleOpenAllLevel;

    static DebugMenu instance;

    protected bool isLevelFree = false;
    protected bool isDebugBattle = false;

    static public bool IsLevelFree() { return instance.isLevelFree; }
    static public bool IsDebugBattle() { return instance.isDebugBattle; }

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

    protected void InitMenuValue()
    {
        if (toggleOpenAllLevel)
        {
            toggleOpenAllLevel.SetIsOnWithoutNotify(isLevelFree);
        }
    }

    protected bool isMenuOn = false;
    public void OnToggleDebugMenu()
    {
        isMenuOn = !isMenuOn;
        if (isMenuOn)
        {
            OnOpenMenu();
        }
        else
        {
            OnCloseMenu();
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

    public void OnDebugBattle(bool value)
    {
        isDebugBattle = value;
        print("OnDebugBattle " + value);
    }

    public void OnShowFPS(bool value)
    {
        FPS fps = GetComponentInChildren<FPS>(true);
        if (fps)
        {
            fps.gameObject.SetActive(value);
        }
    }

    public void OnBackLevel()
    {
        if (BattleSystem.GetInstance())
        {
            BattleSystem.GetInstance().OnBackPrevScene();
            OnCloseMenu();
        }
    }

    public void OnLvUp()
    {
        if (BattleSystem.GetPC())
        {
            CharacterData mData = BattleSystem.GetPC().theCharData;
            mData.AddExp(mData.GetExpMax());
        }
    }

    public void OnAddMoney()
    {
        GameSystem.GetPlayerData().AddMoney(10000);
    }

    public void OnAddBattlePoints()
    {
        if (BattlePlayerData.GetInstance())
        {
            BattlePlayerData.GetInstance().AddBattleLVPoints(10);
        }
    }

    public void OnBattleStat()
    {
        if (BattleStat.GetInstance())
        {
            BattleStat.GetInstance().DebugPrintAll();
        }
    }

    public void OnResetBattleStat()
    {
        if (BattleStat.GetInstance())
        {
            BattleStat.GetInstance().DebugClearAll();
        }
    }

    public void OnGotoNextBattle() 
    {
        ContinuousBattleManager.GotoNextBattle();
        ContinuousBattleDataBase nextBattle = ContinuousBattleManager.GetCurrBattleData();
        if (nextBattle != null)
        {
            BattleSystem.GetInstance().OnGotoScene(nextBattle.scene);
        }
        else
        {
            BattleSystem.GetInstance().OnBackPrevScene();
        }
        OnCloseMenu();
    }

    public void OnShowLog()
    {
        SystemUI.ShowLOG();
        OnCloseMenu();
    }

    //public void OnSaveToServerTest()
    //{
    //    //StartCoroutine(TestSaveProgressToServer());
    //    GameSystem.GetInstance().LoadOnlineAgain();
    //}

    //IEnumerator TestSaveProgressToServer()
    //{
    //    string filename = "testsave.txt";
    //    string progressData = "我現在很厲害喔，總共有 " + GameSystem.GetPlayerData().GetAllUsingDolls().Length + " 個巫靈!!";
    //    //string url = "http://localhost/one/saveprogress.php";
    //    string url = "http://yeshouse.tplinkdns.com/one/save/saveprogress.php";     //TODO: 從 GameSystem 拿
    //    UnityWebRequest request = new UnityWebRequest(url, "POST");

    //    // 將進度數據作為參數添加到請求中
    //    WWWForm form = new WWWForm();
    //    form.AddField("filename", filename);
    //    form.AddField("progress", progressData);
    //    request.uploadHandler = new UploadHandlerRaw(form.data);
    //    request.downloadHandler = new DownloadHandlerBuffer();
    //    request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

    //    yield return request.SendWebRequest();

    //    if (request.result == UnityWebRequest.Result.ConnectionError ||
    //        request.result == UnityWebRequest.Result.ProtocolError)
    //    {
    //        Debug.Log(request.error);
    //    }
    //    else
    //    {
    //        Debug.Log("進度數據已成功保存到伺服器");
    //        print("PhP 回傳資訊:\n" + request.downloadHandler.text);
    //    }

    //    request.Dispose();
    //}

}
