using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSE_Gen : MapGeneratorBase
{
    public GameObject scrollScene;
    //public float vStart = 0;
    public float vSceneLength = 40.0f;
    public float vSceneEnd = -40.0f;

    public GameObject[] gameplayRefs;
    public SceneScrollController theSSController;
    public GameObject initGameRef;

    protected GameObject[] roomArray;
    protected GameObject initGamePlay;

    void Start()
    {
        roomArray = new GameObject[gameplayRefs.Length];
    }

    void ClearAll()
    {
        for (int i=0; i<roomArray.Length; i++)
        {
            if (roomArray[i])
            {
                Destroy(roomArray[i]);
                roomArray[i] = null;
            }
        }

        if (initGamePlay)
        {
            Destroy(initGamePlay);
        }
    }

    public override void BuildAll(int buildLevel = 1)
    {
        ClearAll();

        base.BuildAll(buildLevel);

        if (gameplayRefs.Length <= 0 || !theSSController)
            return;

        Quaternion rm = Quaternion.Euler(90, 0, 0);
        Vector3 pos = new Vector3(0, 0, 0);

        if (initGameRef)
        {
            initGamePlay = Instantiate(initGameRef, pos, rm, null);
        }

        float startPos = vSceneLength * (float)gameplayRefs.Length;

        theSSController.SceneScrollArray = new SceneScroll[gameplayRefs.Length];
        for (int i=0; i<gameplayRefs.Length; i++)
        {
            if (scrollScene) 
            {
                GameObject newRoom = Instantiate(scrollScene, pos, rm, null);
                if (newRoom)
                {
                    //³]©w SceneScroller
                    SceneScroll newSS = newRoom.GetComponent<SceneScroll>();
                    if (newSS)
                    {
                        //newSS.endPos = vSceneEnd;
                        //newSS.startPos = startPos;
                        newSS.isInitGameplay = (i != 0);
                        //newSS.addBattleDifficultyWhenEnd = (i == gameplayRefs.Length - 1);

                        newSS.childGameplayRef = gameplayRefs[i];
                        //newSS.scrollSpeed = 0;
                    }
                    theSSController.SceneScrollArray[i] = newSS;

                    roomArray[i] = newRoom;
                }
                pos.z += vSceneLength;
            }
            
        }

        theSSController.Reset();
    }
}
