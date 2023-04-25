using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OnlineSaveLoad : MonoBehaviour
{
    //private const string urlRoot = "http://localhost/one/oaserver/";
    private const string urlRoot = "http://yeshouse.tplinkdns.com/one/oaserver/";
    private const string urlGetID = "getid.php";
    private const string urlSaveGame = "savegame.php";
    private const string urlLoadGame = "loadgame.php";
    private const string urlSetNickName = "setnickname.php";

    private const string urlGAME_ID = "game_id";
    private const string urlNICK_NAME = "nickname";
    private const string ONLINE_ERROR_PREFIX = "ERROR";

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

    public string LoadGameData(string game_ID)
    {
        string url = urlRoot + urlLoadGame + "?" + urlGAME_ID  + "=" + game_ID;
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


    public bool SetNickName( string game_ID, string nickName)
    {
        bool setResult = false;
        string url = urlRoot + urlSetNickName + "?" + urlGAME_ID + "=" + game_ID + "&"+ urlNICK_NAME +"=" + nickName;
        print("SetNickName url = " + url);
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.timeout = 10;

        request.SendWebRequest();

        while (!request.isDone)
        {
            // ���ݽШD����
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            string resultStr = request.downloadHandler.text;
            if (resultStr.StartsWith(ONLINE_ERROR_PREFIX))
            {
                //�����~�T��
                print("OnlineSaveLoad::SetNickName " + nickName);
                setResult = false;
            }
            else
            {
                print("OnlineSaveLoad::SetNickName �^�Ǧ��\!! " + nickName);
                setResult = true;
            }
        }
        else
        {
            print("ERROR: OnlineSaveLoad::SetNickName UnityWebRequest ����");
            print(request.error);
        }
        request.Dispose();
        return setResult;
    }

}
