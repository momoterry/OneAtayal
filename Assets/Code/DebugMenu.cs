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

    static public bool GetIsLevelFree() { return instance.isLevelFree; }

    private void Awake()
    {
        if (instance != null)
            print("ERROR !! �W�L�@�� DebugMenu �s�b ");
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


    public void OnSaveToServerTest()
    {
        StartCoroutine(TestSaveProgressToServer());
    }

    IEnumerator TestSaveProgressToServer()
    {
        string filename = "testsave.txt";
        string progressData = "�ڲ{�b�ܼF�`��A�`�@�� " + GameSystem.GetPlayerData().GetAllUsingDolls().Length + " �ӧ��F!!";
        string url = "http://localhost/one/saveprogress.php";
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        // �N�i�׼ƾڧ@���ѼƲK�[��ШD��
        WWWForm form = new WWWForm();
        form.AddField("filename", filename);
        form.AddField("progress", progressData);
        request.uploadHandler = new UploadHandlerRaw(form.data);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("�i�׼ƾڤw���\�O�s����A��");
            print("PhP �^�Ǹ�T:\n" + request.downloadHandler.text);
        }

        request.Dispose();
    }

}
