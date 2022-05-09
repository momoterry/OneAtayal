using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneScroll : MonoBehaviour
{
    // Start is called before the first frame update
    public float startPos = 40.0f;
    public float endPos = -40.0f;

    public float scrollSpeed = 8.0f;



    public GameObject childGameplayRef;
    public bool isInitGameplay = false;

    //TODO: ²¾µ¹ ScrollController
    public bool addBattleDifficultyWhenEnd = false;

    protected GameObject childGameplay = null;

    void Start()
    {
        if (isInitGameplay)
        {
            SetupGameplay();
        }
    }

    // Update is called once per frame
    void UpdateOld()
    {
        Vector3 thePos = transform.position;
        thePos.z -= Time.deltaTime * scrollSpeed;
        bool isReset = false;
        if (thePos.z < endPos)
        {
            thePos.z = startPos;
            isReset = true;
        }
        transform.position = thePos;
        print(gameObject.name + " : " + thePos);

        if (isReset)
        {
            ClearGameplay();
            if (BattleSystem.GetInstance().IsDuringBattle())
            {
                if (addBattleDifficultyWhenEnd)
                {
                    BattleSystem.GetInstance().OnAddLevelDifficulty();
                }
                SetupGameplay();
            }
        }
    }


    public void ClearGameplay()
    {
        if (childGameplay)
        {
            Destroy(childGameplay);
        }
    }

    public void SetupGameplay()
    {
        if (childGameplayRef)
        {
            childGameplay = Instantiate(childGameplayRef, transform.position, Quaternion.Euler(90, 0, 0), transform);
        }
    }
}
