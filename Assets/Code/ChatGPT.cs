using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ChatGPT : MonoBehaviour
{
    //public Text theText;
    [TextArea(2, 10)]
    public string prompt = "你好";
    public string apiKey = "your_api_key_here";
    public int maxToken = 100;
    private string url = "https://api.openai.com/v1/completions";

    public delegate void ChatResultCallback(string result);

    protected ChatResultCallback chatCB;
    protected bool isWaiting = false;

    //void Start()
    //{
    //    //StartCoroutine(SendMessageToGPT(prompt));
    //}

    public void StartChat(ChatResultCallback _chatCB)
    {
        if (isWaiting)
        {
            print("ERROR!!!! 還沒處理完上一個對話 !!");
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
            //print("沒有成功.........");
            //Debug.LogError(request.error);
            if (chatCB != null)
            {
                chatCB("....說不出話來....");
            }
        }
        else
        {
            //print("成功了喲.........");
            //print(request.downloadHandler.text);
            var jsonResponse = JsonUtility.FromJson<GPTResponse>(request.downloadHandler.text);
            //Debug.Log("Response: " + jsonResponse.choices[0].text);

            //if (theText)
            //{
            //    theText.text = jsonResponse.choices[0].text;
            //}
            if (chatCB != null)
            {
                chatCB(jsonResponse.choices[0].text);
            }
        }
        isWaiting = false;
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