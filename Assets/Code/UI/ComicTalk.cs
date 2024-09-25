using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComicTalk : MonoBehaviour
{
    static ComicTalk instance = null;

    //public ComicTalkItem theItem;
    public ComicTalkItem itemRef;
    public int maxItem = 3;

    protected List<ComicTalkItem> items = new List<ComicTalkItem>();
    protected int currItemIndex = 0;

    void Awake()
    {
        if (instance != null)
        {
            One.ERROR("ComicTalk 超過一個實體存在 !!!!");
        }
        instance = this;

        for (int i = 0; i < maxItem; i++)
        {
            GameObject o = Instantiate(itemRef.gameObject, transform);
            ComicTalkItem item = o.GetComponent<ComicTalkItem>();
            items.Add(item);
            item.Init();
            o.SetActive(false);
        }
    }


    public static void StartTalk(string str, GameObject talker, float timeDuration)
    {
        instance._StartTalk(str, talker, timeDuration);
    }

    protected ComicTalkItem GetOneItem()
    {
        ComicTalkItem item = items[currItemIndex];
        currItemIndex++;
        if (currItemIndex >= items.Count)
            currItemIndex = 0;
        return item;
    }

    protected void _StartTalk(string str, GameObject talker, float timeDuration)
    {
        //talkUI.obj.SetActive(true);
        //talkUI.textUI.text = str;
        //float minWidth = Mathf.Ceil(talkUI.textUI.preferredWidth) + 8.0f;
        //float minHeight = Mathf.Ceil(talkUI.textUI.preferredHeight) + 8.0f;
        //talkUI.bg.rectTransform.sizeDelta = new Vector2(minWidth, minHeight);

        ComicTalkItem item = GetOneItem();
        item.StartTalk(str, talker, new Vector3(0.5f, 0.5f, 0.0f), timeDuration);
    }

}
