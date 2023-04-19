using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OnlineSaveLoad : MonoBehaviour
{
    private const string urlRoot = "http://localhost/one/game/";
    //private const string urlRoot = "http://yeshouse.tplinkdns.com/one/game/";
    private const string urlGetID = "getid.php";
    private const string urlSaveGame = "savegame.php";


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
            Debug.Log("獲得的新 ID 是：" + id);
            return id;
        }
        else
        {
            Debug.Log(request.error);
            return "";
        }
    }

    public void SaveGameData(string game_ID, string dataStr)
    {
        //string the_ID = GameSystem.GetInstance().GetID();
        //string progressData = "我現在很厲害喔，總共有 " + GameSystem.GetPlayerData().GetAllUsingDolls().Length + " 個巫靈!!";
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
