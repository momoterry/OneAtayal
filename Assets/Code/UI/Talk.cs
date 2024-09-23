using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talk : MonoBehaviour
{
    public int MaxSentences = 4;
    public float TimePerSentence = 2.0f;
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
        newSentenceObj.timeLeft = TimePerSentence;
        sentenceList.Add(newSentenceObj);
        if (sentenceList.Count > MaxSentences) 
        {
            sentenceList.RemoveRange(MaxSentences, sentenceList.Count - MaxSentences);
        }

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
        //foreach (SentenceObject o in sentenceList)
        for (int i=0; i<sentenceList.Count; i++)
        {
            if (i != 0)
                allText += "\n";
            allText += (sentenceList[i].text);
        }

        if (theTextMesh)
            theTextMesh.text = allText;

        //print("MakeText\n" + allText);
    }

}
