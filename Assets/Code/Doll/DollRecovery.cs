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
        DollManager dm = GetComponent<DollManager>();
        //if (dm)
        //    dm.SetIsWaitDollRecovery(true);
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
                    const int FIX_MAX_WAVE = 8;
                    int maxSpawnCount = (allDollIDs.Length - 1) / FIX_MAX_WAVE + 1;
                    int maxSpawnWave = (allDollIDs.Length + FIX_MAX_WAVE - 1) % FIX_MAX_WAVE + 1;
                    int waveCount = currSpawn < (maxSpawnCount * maxSpawnWave) ? currSpawn / maxSpawnCount
                        : (currSpawn - (maxSpawnCount * maxSpawnWave)) / (maxSpawnCount - 1) + maxSpawnWave;
                    int spwanCount = currSpawn < (maxSpawnCount * maxSpawnWave) ? maxSpawnCount : maxSpawnCount - 1;
                    //int spwanCount = (allDollIDs.Length - currSpawn) / 4 + 1;
                    //print("currSpawn: " + currSpawn + " maxSpawnWave: " + maxSpawnWave + "  waveCount: " + waveCount + "  spwanCount  " + spwanCount);
                    for (int i=0; i<spwanCount; i++)
                        DoSpawnOneDoll(i, spwanCount);
                }
                break;
        }

        if (currPhase != nextPhase)
        {
            switch (nextPhase)
            {
                case Phase.FINISH:
                    DollManager dm = GetComponent<DollManager>();
                    //if (dm)
                    //{
                    //    print("Finish!!");
                    //    dm.SetIsWaitDollRecovery(false);
                    //}
                    break;
            }
        }

        currPhase = nextPhase;
    }

    void DoSpawnOneDoll( int batchIndex = 0, int batchTotal = 1)
    {
        string dollID = allDollIDs[currSpawn];
        GameObject dollRef = GameSystem.GetDollData().GetDollRefByID(dollID);
        if (!dollRef)
        {
            print("Error!! There is no dollRef in ID: " + dollID);
            return;
        }

        Vector3 front = BattleSystem.GetInstance().GetPlayerController().GetDollManager().transform.forward;
        Vector3 right = BattleSystem.GetInstance().GetPlayerController().GetDollManager().transform.right;
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

        float rightShift = -0.25f * (batchTotal - 1) + batchIndex * 0.5f;

        Vector3 pos = transform.position + front + right * rightShift;

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
            GameSystem.GetPlayerData().AddUsingDoll("DollOne");
            GameSystem.GetPlayerData().AddUsingDoll("DollOne");
            //GameSystem.GetPlayerData().AddUsingDoll("DollBlackStone");
            //GameSystem.GetPlayerData().AddUsingDoll("DollOne");
            //GameSystem.GetPlayerData().AddUsingDoll("DollOne");
            //GameSystem.GetPlayerData().AddUsingDoll("DollOne");
            //GameSystem.GetPlayerData().AddUsingDoll("DollOne");

            //GameSystem.GetPlayerData().AddUsingDoll("DollAce");
            //GameSystem.GetPlayerData().AddUsingDoll("DollAce");
            //GameSystem.GetPlayerData().AddUsingDoll("DollAce");
            //GameSystem.GetPlayerData().AddUsingDoll("DollAce");



            //GameSystem.GetPlayerData().AddUsingDoll("DollWhiteFire");
            //GameSystem.GetPlayerData().AddUsingDoll("DollWhiteFire");
            //GameSystem.GetPlayerData().AddUsingDoll("DollWhiteFire");
            //GameSystem.GetPlayerData().AddUsingDoll("DollWhiteFire");


            //GameSystem.GetPlayerData().AddUsingDoll("DollFire");
            //GameSystem.GetPlayerData().AddUsingDoll("DollFire");
            //GameSystem.GetPlayerData().AddUsingDoll("DollLeaf");
            //GameSystem.GetPlayerData().AddUsingDoll("DollLeaf");
            //GameSystem.GetPlayerData().AddUsingDoll("DollLeaf");
            //GameSystem.GetPlayerData().AddUsingDoll("DollLeaf");

        }
        //*/

        DollInstanceData[] DIs = GameSystem.GetPlayerData().GetAllUsingDIs();
        if (DIs != null)
        {
            print("DollRecovery: 所有的 DIs ");
            for (int i = 0; i < DIs.Length; i++)
            {
                print(DIs[i].fullName);
            }
        }
        else
            print("沒有任何 DIs ");


        string[] savedDolls = GameSystem.GetPlayerData().GetAllUsingDolls();
        int savedLength = savedDolls == null ? 0 : savedDolls.Length;
        string[] collectedDolls = ContinuousBattleManager.GetAllCollectedDolls();
        int collectedLength = collectedDolls == null ? 0 : collectedDolls.Length;
        allDollIDs = new string[savedLength + collectedLength];

        if (savedLength > 0)
        {
            System.Array.Copy(savedDolls, 0, allDollIDs, 0, savedLength);
        }
        if (collectedLength > 0)
        {
            System.Array.Copy(collectedDolls, 0, allDollIDs, savedLength, collectedLength);
        }


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
