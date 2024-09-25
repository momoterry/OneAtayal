using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComicTalk : MonoBehaviour
{
    static ComicTalk instance = null;

    //[System.Serializable]
    //public class OneTalk
    //{
    //    public GameObject obj;
    //    public Text textUI;
    //    public Image bg;
    //}
    //public OneTalk talkUI;

    public ComicTalkItem theItem;

    void Awake()
    {
        if (instance != null)
        {
            One.ERROR("ComicTalk 超過一個實體存在 !!!!");
        }
        instance = this;
    }


    public static void StartTalk(string str, GameObject talker, float timeDuration)
    {
        instance._StartTalk(str, talker, timeDuration);
    }

    protected void _StartTalk(string str, GameObject talker, float timeDuration)
    {
        //talkUI.obj.SetActive(true);
        //talkUI.textUI.text = str;
        //float minWidth = Mathf.Ceil(talkUI.textUI.preferredWidth) + 8.0f;
        //float minHeight = Mathf.Ceil(talkUI.textUI.preferredHeight) + 8.0f;
        //talkUI.bg.rectTransform.sizeDelta = new Vector2(minWidth, minHeight);
        theItem.StartTalk(str, talker, new Vector3(1.0f, 0.0f, 0.5f), timeDuration);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
