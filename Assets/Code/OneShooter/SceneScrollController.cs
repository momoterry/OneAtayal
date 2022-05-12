using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneScrollController : MonoBehaviour
{
    // Start is called before the first frame update
    public float scrollSpeed = 8.0f;
    public SceneScroll[] SceneScrollArray;
    public float vEnd = -40.0f;
    public float scrollLength = 40.0f;
    public bool autoStart = true;

    protected float vRefPoint;
    protected float totalLength;
    protected int scrollCount;

    protected bool isScroll = false;
    protected bool[] scrollFlags; //每個 Scroll 是否已經經過 vEnd

    void Start()
    {
        Reset();
        if (autoStart)
            isScroll = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isScroll)
        {
            vRefPoint -= Time.deltaTime * scrollSpeed;
            //print("REF===== "+vRefPoint);
            for (int i=0; i<scrollCount; i++)
            {
                float vPos = vRefPoint - (float)(scrollCount - i - 1) * scrollLength;
                bool isReset = false;
                if (vPos <= vEnd)
                {
                    vPos += totalLength;
                    if (!scrollFlags[i])
                    {
                        SceneScrollArray[i].ClearGameplay();
                        isReset = true;
                        scrollFlags[i] = true;
                    }
                }

                //捲動區塊
                Vector3 newPos = SceneScrollArray[i].transform.position;
                newPos.z = vPos;
                SceneScrollArray[i].gameObject.transform.position = newPos;

                //重啟新的一輪 Gameplay
                if (isReset && BattleSystem.GetInstance().IsDuringBattle())
                {
                    SceneScrollArray[i].SetupGameplay();
                }
            }

            if (vRefPoint <= vEnd)
            {
                //print("============== Reset Ref ==============");
                vRefPoint += totalLength;
                ResetFlags();
                if (BattleSystem.GetInstance().IsDuringBattle())
                {
                    BattleSystem.GetInstance().OnAddLevelDifficulty();
                }
            }
        }
    }

    private void ResetFlags()
    {
        for (int i=0; i<scrollCount; i++)
        {
            scrollFlags[i] = false;
        }
    }

    public void Reset()
    {
        scrollCount = SceneScrollArray.Length;
        if (scrollCount <= 0)
            return;

        scrollFlags = new bool[scrollCount];
        ResetFlags();

        totalLength = scrollLength * (float)scrollCount;
        vRefPoint = vEnd + totalLength;
    }
}
