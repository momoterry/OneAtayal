using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class ChatGPT : MonoBehaviour
{
    [TextArea(2, 10)]
    public string prompt = "�A�n";
    protected string apiKey = "";
    public int maxToken = 100;
    private string url = "https://api.openai.com/v1/completions";

    public delegate void ChatResultCallback(string result);

    protected ChatResultCallback chatCB;
    protected bool isWaiting = false;

    private string key = "cQeThWmZq4t7w!z%";

    void Start()
    {
        //string message = "�ڬO�@�Ӧn�_�_";

        //byte[] encrypted = OneUtility.EncryptString(message, key);
        //string enStr = System.Convert.ToBase64String(encrypted);
        //string myEnStr = "aDa54kBkHErQjK93J5AAXmkTcKjor/GsDitXB/a6FVI=";
        //print(enStr);
        //print("�ѥX�ӴN�O: " + OneUtility.DecryptString(myEnStr, key));

        StartCoroutine(GetOpenAPIKey());
    }

    public IEnumerator GetOpenAPIKey()
    {
        string url = "http://yeshouse.tplinkdns.com/one/k.txt";
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string keyword = www.downloadHandler.text;
            Debug.Log("API Key: " + " ������\ !!");
            apiKey = OneUtility.DecryptString(keyword, key);
        }
        else
        {
            Debug.Log("Error: " + www.error);
        }

        www.Dispose();
    }

    public void StartChat(ChatResultCallback _chatCB)
    {
        if (isWaiting)
        {
            print("ERROR!!!! �٨S�B�z���W�@�ӹ�� !!");
            return;
        }
        isWaiting = true;
        chatCB = _chatCB;
        StartCoroutine(SendMessageToGPT(prompt));
    }


    IEnumerator SendMessageToGPT(string message)
    {
        var request = new UnityWebRequest(url, "POST");
        var gptRequest = new GPTRequest(message);
        gptRequest.max_tokens = maxToken;
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(gptRequest));
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            //print("�S�����\.........");
            Debug.LogError(request.error);
            if (chatCB != null)
            {
                chatCB("....�����X�ܨ�....");
            }
        }
        else
        {

            var jsonResponse = JsonUtility.FromJson<GPTResponse>(request.downloadHandler.text);

            if (chatCB != null)
            {
                chatCB(jsonResponse.choices[0].text);
            }
        }
        isWaiting = false;

        request.Dispose();
    }

    [System.Serializable]
    public class GPTRequest
    {
        public string model = "text-davinci-003";
        public string prompt;
        public int max_tokens = 50;
        public float temperature = 0.7f;
        public int top_p = 1;
        public float frequency_penalty = 0;
        public float presence_penalty = 0;

        public GPTRequest(string message)
        {
            prompt = message;
        }
    }

    [System.Serializable]
    public class GPTResponse
    {
        public List<Choice> choices;
    }

    [System.Serializable]
    public class Choice
    {
        public string text;
    }
}