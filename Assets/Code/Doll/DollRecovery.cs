using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//在關卡開始時，將 PlayerData 記錄的 Doll 召喚出來
public class DollRecovery : MonoBehaviour
{
    protected enum Phase
    {
        NONE,
        WAIT,
        SPAW,
        FINISH,
    }
    Phase currPhase = Phase.NONE;
    Phase nextPhase = Phase.WAIT;

    protected float waitTime = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        nextPhase = Phase.WAIT;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currPhase)
        {
            case Phase.WAIT:
                waitTime -= Time.deltaTime;
                if (waitTime <= 0)
                {
                    DoSpawnAll();
                    nextPhase = Phase.FINISH;
                }
                break;
        }
        currPhase = nextPhase;
    }

    void DoSpawnDoneDoll(string dollID)
    {
        //print(">>>>>> Recover Doll: " + dollID);
        GameObject dollRef = GameSystem.GetPlayerData().GetDollRefByID(dollID);
        if (!dollRef)
        {
            print("Error!! There is no dollRef in ID: " + dollID);
            return;
        }

        Vector3 pos = transform.position + Vector3.forward * 2.0f;

        GameObject dollObj = BattleSystem.GetInstance().SpawnGameplayObject(dollRef, pos, false);

        Doll theDoll = dollObj.GetComponent<Doll>();
        if (theDoll == null)
        {
            print("Error!! There is no Doll in dollRef !!");
            Destroy(dollObj);
            return;
        }

        if (!theDoll.TryJoinThePlayer())
        {
            print("Woooooooooops.......");
            return;
        }
    }

    void DoSpawnAll()
    {
        string[] allDollIDs = GameSystem.GetPlayerData().GetAllUsingDolls();

        if (allDollIDs != null)
        {
            for (int i= 0; i<allDollIDs.Length; i++)
            {
                DoSpawnDoneDoll(allDollIDs[i]);
            }
        }
    }

}
