using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OnlineSaveLoad : MonoBehaviour
{
    //private const string urlRoot = "http://localhost/one/game/";
    private const string urlRoot = "http://yeshouse.tplinkdns.com/one/game/";
    private const string urlGetID = "getid.php";
    private const string urlSaveGame = "savegame.php";
    private const string urlLoadGame = "loadgame.php";

    private const string ONLINE_ERROR_PREFIX = "ERROR";


    public string GetNewID()
    {
        string url = urlRoot + urlGetID;
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.timeout = 5;
        request.SendWebRequest();

        while (!request.isDone)
        {
            // 等待請求完成
        }

        if (request.result == UnityWebRequest.Result.Success)
        {

            string id = request.downloadHandler.text;
            //Debug.Log("獲得的新 ID 是：" + id);
            return id;
        }
        else
        {
            print(request.error);
            print("ERROR!!!! OnlineSaveLoad::GetNewID 失敗 ....");
            return "";
        }
    }

    public string LoadGameData(string game_ID)
    {
        string url = urlRoot + urlLoadGame + "?game_id=" + game_ID;
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.timeout = 10;

        request.SendWebRequest();

        while (!request.isDone)
        {
            // 等待請求完成
        }
        string resultStr = "";
        if (request.result == UnityWebRequest.Result.Success)    
        {
            resultStr = request.downloadHandler.text;
            if (resultStr.StartsWith(ONLINE_ERROR_PREFIX))
            {
                //為錯誤訊息
                print("OnlineSaveLoad::LoadGameDat " + game_ID);
                resultStr = "";
            }
            else
            {
                print("OnlineSaveLoad::LoadGameData 回傳成功!! " + game_ID);
            }
        }
        else
        {
            print("ERROR: OnlineSaveLoad::LoadGameData UnityWebRequest 失敗");
            print(request.error);
        }
        request.Dispose();
        return resultStr;
    }

    public void SaveGameData(string game_ID, string dataStr)
    {
        string url = urlRoot + urlSaveGame;
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        // 將進度數據作為參數添加到請求中
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
            // 等待請求完成
        }

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("進度數據已成功保存到伺服器");
            print("PhP 回傳資訊:\n" + request.downloadHandler.text);
        }

        request.Dispose();
    }
}
