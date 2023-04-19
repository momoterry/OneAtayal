using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OnlineSaveLoad : MonoBehaviour
{
    //private const string urlRoot = "http://localhost/one/game/";
    private const string urlRoot = "http://yeshouse.tplinkdns.com/one/game/";
    private const string urlGetID = "getid.php";


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
}
