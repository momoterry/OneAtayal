using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//在關卡開始時，將 PlayerData 記錄的 Doll 召喚出來
public class DollRecovery : MonoBehaviour
{
    public GameObject SpawnFX;
    protected enum Phase
    {
        NONE,
        WAIT,
        SPAWN,
        FINISH,
    }
    Phase currPhase = Phase.NONE;
    Phase nextPhase = Phase.WAIT;

    protected string[] allDollIDs = null;

    protected float waitTime = 0.1f;
    protected float stepTime = 0.15f;
    protected int currSpawn = 0;
    protected float timeLeft = 0;

    // Start is called before the first frame update
    void Start()
    {
        nextPhase = Phase.WAIT;
        timeLeft = waitTime;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currPhase)
        {
            case Phase.WAIT:
                timeLeft -= Time.deltaTime;
                if (timeLeft <= 0)
                {
                    StartSpawn();
                }
                break;
            case Phase.SPAWN:
                timeLeft -= Time.deltaTime;
                if (timeLeft <= 0)
                {
                    timeLeft += stepTime;
                    DoSpawnOneDoll();
                }
                break;
        }
        currPhase = nextPhase;
    }

    void DoSpawnOneDoll()
    {
        string dollID = allDollIDs[currSpawn];
        GameObject dollRef = GameSystem.GetPlayerData().GetDollRefByID(dollID);
        if (!dollRef)
        {
            print("Error!! There is no dollRef in ID: " + dollID);
            return;
        }

        Vector3 front = BattleSystem.GetInstance().GetPlayerController().GetDollManager().transform.forward;
        Doll theDollInRef = dollRef.GetComponent<Doll>();
        if (theDollInRef == null)
        {
            print("Error!! There is no Doll in dollRef !!");
            return;
        }
        switch (theDollInRef.positionType)
        {
            case DOLL_POSITION_TYPE.FRONT:
                front = front * 2.0f;
                break;
            case DOLL_POSITION_TYPE.MIDDLE:
                front = front * 1.0f;
                break;
            case DOLL_POSITION_TYPE.BACK:
                front = front * -2.0f;
                break;
        }

        Vector3 pos = transform.position + front;

        GameObject dollObj = BattleSystem.GetInstance().SpawnGameplayObject(dollRef, pos, false);
        if (SpawnFX)
            BattleSystem.GetInstance().SpawnGameplayObject(SpawnFX, pos, false);
        Doll theDoll = dollObj.GetComponent<Doll>();

        if (!theDoll.TryJoinThePlayer())
        {
            print("Woooooooooops.......");
            return;
        }

        currSpawn++;
        if (currSpawn == allDollIDs.Length)
        {
            nextPhase = Phase.FINISH;
        }
    }

    void StartSpawn()
    {
        /*Debug
        if (GameSystem.GetPlayerData().GetCurrDollNum() == 0)
        {
            GameSystem.GetPlayerData().AddUsingDoll("DollBlackStone");
            GameSystem.GetPlayerData().AddUsingDoll("DollBlackStone");
            GameSystem.GetPlayerData().AddUsingDoll("DollBlackStone");
            //GameSystem.GetPlayerData().AddUsingDoll("DollBlackStone");
            //GameSystem.GetPlayerData().AddUsingDoll("DollOne");
            //GameSystem.GetPlayerData().AddUsingDoll("DollOne");
            //GameSystem.GetPlayerData().AddUsingDoll("DollOne");
            //GameSystem.GetPlayerData().AddUsingDoll("DollOne");

            //GameSystem.GetPlayerData().AddUsingDoll("DollAce");
            //GameSystem.GetPlayerData().AddUsingDoll("DollAce");
            //GameSystem.GetPlayerData().AddUsingDoll("DollAce");
            //GameSystem.GetPlayerData().AddUsingDoll("DollAce");



            GameSystem.GetPlayerData().AddUsingDoll("DollWhiteFire");
            GameSystem.GetPlayerData().AddUsingDoll("DollWhiteFire");
            //GameSystem.GetPlayerData().AddUsingDoll("DollWhiteFire");
            //GameSystem.GetPlayerData().AddUsingDoll("DollWhiteFire");


            //GameSystem.GetPlayerData().AddUsingDoll("DollFire");
            //GameSystem.GetPlayerData().AddUsingDoll("DollFire");
            GameSystem.GetPlayerData().AddUsingDoll("DollLeaf");
            GameSystem.GetPlayerData().AddUsingDoll("DollLeaf");
            //GameSystem.GetPlayerData().AddUsingDoll("DollLeaf");
            //GameSystem.GetPlayerData().AddUsingDoll("DollLeaf");

        }
        //*/

        allDollIDs = GameSystem.GetPlayerData().GetAllUsingDolls();

        //測試
        //allDollIDs = new string[]{"DollStone", "DollStone", "DollOne", "DollOne", "DollFire", "DollFire" };

        if (allDollIDs == null || allDollIDs.Length == 0)
        {
            nextPhase = Phase.FINISH;
        }
        else
            nextPhase = Phase.SPAWN;

        timeLeft = 0;   //馬上生第一隻

    }

}
