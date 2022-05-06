using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneScroller : MonoBehaviour
{
    // Start is called before the first frame update
    public float startPos = 40.0f;
    public float endPos = -40.0f;

    public float scrollSpeed = 8.0f;

    public GameObject childGameplayRef;
    public bool isInitGameplay = false;
    public bool addBattleDifficultyWhenEnd = false;

    protected GameObject childGameplay = null;

    void Start()
    {
        if (isInitGameplay)
        {
            ResetGameplay();
        }
    }

    // Update is called once per frame
    void Update()
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

        if (isReset)
        {
            ResetGameplay();
        }
    }

    void ResetGameplay()
    {
        if (addBattleDifficultyWhenEnd)
        {
            BattleSystem.GetInstance().OnAddLevelDifficulty();
        }
        if (childGameplay)
        {
            Destroy(childGameplay);
        }
        childGameplay = Instantiate(childGameplayRef, transform.position, Quaternion.Euler(90, 0, 0), transform);
    }
}
