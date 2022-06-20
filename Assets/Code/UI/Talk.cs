using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talk : MonoBehaviour
{

    public TextMesh theTextMesh;

    //==== Protected Members 
    protected class SentenceObject
    {
        public string text;
        public float timeLeft;
    }
    protected List<SentenceObject> sentenceList = new List<SentenceObject>();

    protected float updateTime = 0;

    //==== Public ====
    public void AddSentence(string sentence)
    {
        SentenceObject newSentenceObj = new SentenceObject();
        newSentenceObj.text = sentence;
        newSentenceObj.timeLeft = 2.0f;
        sentenceList.Add(newSentenceObj);

        MakeText();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (theTextMesh)
            theTextMesh.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        updateTime += Time.deltaTime;
        if (updateTime < 0.2f)
            return;
        updateTime = 0;

        bool makeText = false;
        foreach (SentenceObject o in sentenceList)
        {
            o.timeLeft -= 0.2f;
            if (o.timeLeft <= 0)
            {
                sentenceList.Remove(o);
                makeText = true;
                //print("移除!!");
                break;  //TODO: 更好的作法? 能一次多個移除的
            }
        }
        if (makeText)
            MakeText();
    }

    // ==== protected ====
    
    void MakeText()
    {
        string allText = "";
        foreach (SentenceObject o in sentenceList)
        {
            allText += ( o.text + "\n");
        }

        if (theTextMesh)
            theTextMesh.text = allText;

        //print("MakeText\n" + allText);
    }

}
