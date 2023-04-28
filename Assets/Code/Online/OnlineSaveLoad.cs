using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OnlineSaveLoad : MonoBehaviour
{
    //private const string urlRoot = "http://localhost/one/oaserver/";
    public const string urlRoot = "http://yeshouse.tplinkdns.com/one/oaserver/";
    public const string urlGetID = "getid.php";
    public const string urlSaveGame = "savegame.php";
    public const string urlLoadGame = "loadgame.php";
    public const string urlSetNickName = "setnickname.php";
    public const string urlCheckID = "checkidnickname.php";
    public const string urlRetrieveAccount = "retrieveaccount.php";

    public const string urlGAME_ID = "game_id";
    public const string urlNICK_NAME = "nickname";
    public const string ONLINE_ERROR_PREFIX = "ERROR";

    public static string GetUrlRoot() { return urlRoot; }

    public string GetNewID()
    {
        string url = urlRoot + urlGetID;
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.timeout = 5;
        request.SendWebRequest();

        string id = "";
        while (!request.isDone)
        {
            // ���ݽШD����
        }

        if (request.result == UnityWebRequest.Result.Success)
        {

            id = request.downloadHandler.text;
            //Debug.Log("��o���s ID �O�G" + id);
            //return id;
        }
        else
        {
            print(request.error);
            print("ERROR!!!! OnlineSaveLoad::GetNewID ���� ....");
            //return "";
        }
        request.Dispose();
        return id;
    }

    public bool CheckID(string game_id, string nickname)
    {
        string url = urlRoot + urlCheckID + "?" + urlGAME_ID + "=" + game_id + "&" + urlNICK_NAME + "=" + nickname;
        print(url);
        string result = GetRequest(url);

        return result.StartsWith("SUCCESS");
    }

    public string LoadGameData(string game_ID)
    {
        string url = urlRoot + urlLoadGame + "?" + urlGAME_ID + "=" + game_ID;
        print("LoadGameData url = " + url);
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.timeout = 10;

        request.SendWebRequest();

        while (!request.isDone)
        {
            // ���ݽШD����
        }
        string resultStr = "";
        if (request.result == UnityWebRequest.Result.Success)
        {
            resultStr = request.downloadHandler.text;
            if (resultStr.StartsWith(ONLINE_ERROR_PREFIX))
            {
                //�����~�T��
                print("OnlineSaveLoad::LoadGameDat " + game_ID);
                resultStr = "";
            }
            else
            {
                print("OnlineSaveLoad::LoadGameData �^�Ǧ��\!! " + game_ID);
            }
        }
        else
        {
            print("ERROR: OnlineSaveLoad::LoadGameData UnityWebRequest ����");
            print(request.error);
        }
        request.Dispose();
        return resultStr;
    }

    public void SaveGameData(string game_ID, string dataStr)
    {
        string url = urlRoot + urlSaveGame;
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        // �N�i�׼ƾڧ@���ѼƲK�[��ШD��
        WWWForm form = new WWWForm();
        form.AddField("game_id", game_ID);
        form.AddField("game_data", dataStr);
        request.uploadHandler = new UploadHandlerRaw(form.data);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        request.timeout = 10;
        request.SendWebRequest();

        while (!request.isDone)
        {
            // ���ݽШD����
        }

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


    public bool SetNickName(string game_ID, string nickName)
    {
        string url = urlRoot + urlSetNickName + "?" + urlGAME_ID + "=" + game_ID + "&" + urlNICK_NAME + "=" + nickName;
        print("SetNickName url = " + url);

        string str = GetRequest(url);
        return !str.StartsWith("ERROR");
    }

    public string GetIDByNickName(string nickname)
    {
        string url = urlRoot + urlRetrieveAccount + "?" + urlNICK_NAME + "=" + nickname;
        print("GetIDByNickName url = " + url);
        string str = GetRequest(url);
        return str;
    }

    //�q�� Get �禡
    public static string GetRequest(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.timeout = 5;
        www.SendWebRequest();
        while (!www.isDone)
        {}
        string restulStr = ONLINE_ERROR_PREFIX;
        if (www.result == UnityWebRequest.Result.Success)
        {
            restulStr = www.downloadHandler.text;
        }
        else
        {
            print(ONLINE_ERROR_PREFIX + " " + url);
            print("OnlineSaveLoad ERROR!!" + www.error);
        }
        www.Dispose();
        return restulStr;
    }

    //�D�P�B��
    public delegate void GetRequestCallBack(string returnMsg);

    public static void GetRequestAsync(string url, GetRequestCallBack cb)
    {
        OnlineSaveLoad osl = GameSystem.GetInstance().theOnlineSaveLoad;
        if (osl)
        {
            osl._GetRequestAsync(url, cb);
        }
    }

    protected void _GetRequestAsync(string url, GetRequestCallBack cb)
    {
        StartCoroutine(GetRequestCorotine(url, cb));
    }

    public IEnumerator GetRequestCorotine(string url, GetRequestCallBack cb)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.timeout = 5;
        yield return www.SendWebRequest();

        string restulStr = ONLINE_ERROR_PREFIX;
        if (www.result == UnityWebRequest.Result.Success)
        {
            restulStr = www.downloadHandler.text;
        }
        else
        {
            print(ONLINE_ERROR_PREFIX + " : " + url);
            print("GetRequestCorotine ERROR!!" + www.error);
            restulStr += www.error;
        }
        cb(restulStr);
        www.Dispose();
    }

    // ====================== �D�P�B���U�� Online �{��
}
