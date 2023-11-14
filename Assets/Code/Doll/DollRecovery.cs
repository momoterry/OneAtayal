using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�b���d�}�l�ɡA�N PlayerData �O���� Doll �l��X��
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

    protected DollInstanceData[] allDIs = null;

    protected string[] allDollIDs = null;

    protected FormationDollInfo[] allFormationDolls = null;
    protected int totalDollCount = 0;

    protected float waitTime = 0.1f;
    protected float stepTime = 0.15f;
    protected int currSpawn = 0;
    protected float timeLeft = 0;

    // Start is called before the first frame update
    void Start()
    {
        DollManager dm = GetComponent<DollManager>();
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
                    //const int FIX_MAX_WAVE = 8;
                    //int maxSpawnCount = (allDollIDs.Length - 1) / FIX_MAX_WAVE + 1;                                     //�C�@�i�̤j���ͦ���
                    //int maxSpawnWave = (allDollIDs.Length + FIX_MAX_WAVE - 1) % FIX_MAX_WAVE + 1;                       //�H�̤j�ƶq�ͦ����i���� (�e�X�i)
                    //int waveCount = currSpawn < (maxSpawnCount * maxSpawnWave) ? currSpawn / maxSpawnCount
                    //    : (currSpawn - (maxSpawnCount * maxSpawnWave)) / (maxSpawnCount - 1) + maxSpawnWave;            //�o�O�ĴX�i
                    //int spwanCount = currSpawn < (maxSpawnCount * maxSpawnWave) ? maxSpawnCount : maxSpawnCount - 1;    //�o�@�i�n�ͦ����ƶq
                    ////print("currSpawn: " + currSpawn + " maxSpawnWave: " + maxSpawnWave + "  waveCount: " + waveCount + "  spwanCount  " + spwanCount);
                    //for (int i=0; i<spwanCount; i++)
                    //    DoSpawnOneDoll(i, spwanCount);

                    SpawnOneWave();
                }
                break;
        }

        if (currPhase != nextPhase)
        {
            switch (nextPhase)
            {
                case Phase.FINISH:
                    DollManager dm = GetComponent<DollManager>();
                    break;
            }
        }

        currPhase = nextPhase;
    }

    protected void SpawnOneWave()
    {
        const int FIX_MAX_WAVE = 8;
        int maxSpawnCount = (totalDollCount - 1) / FIX_MAX_WAVE + 1;                                     //�C�@�i�̤j���ͦ���
        int maxSpawnWave = (totalDollCount + FIX_MAX_WAVE - 1) % FIX_MAX_WAVE + 1;                       //�H�̤j�ƶq�ͦ����i���� (�e�X�i)
        int waveCount = currSpawn < (maxSpawnCount * maxSpawnWave) ? currSpawn / maxSpawnCount
            : (currSpawn - (maxSpawnCount * maxSpawnWave)) / (maxSpawnCount - 1) + maxSpawnWave;            //�o�O�ĴX�i
        int spwanCount = currSpawn < (maxSpawnCount * maxSpawnWave) ? maxSpawnCount : maxSpawnCount - 1;    //�o�@�i�n�ͦ����ƶq
        //print("currSpawn: " + currSpawn + " maxSpawnWave: " + maxSpawnWave + "  waveCount: " + waveCount + "  spwanCount  " + spwanCount);
        for (int i = 0; i < spwanCount; i++)
            DoSpawnOneDoll(i, spwanCount);
    }

    //protected void SpawnOneWave()
    //{
    //    const int FIX_MAX_WAVE = 8;
    //    int maxSpawnCount = (allDollIDs.Length - 1) / FIX_MAX_WAVE + 1;                                     //�C�@�i�̤j���ͦ���
    //    int maxSpawnWave = (allDollIDs.Length + FIX_MAX_WAVE - 1) % FIX_MAX_WAVE + 1;                       //�H�̤j�ƶq�ͦ����i���� (�e�X�i)
    //    int waveCount = currSpawn < (maxSpawnCount * maxSpawnWave) ? currSpawn / maxSpawnCount
    //        : (currSpawn - (maxSpawnCount * maxSpawnWave)) / (maxSpawnCount - 1) + maxSpawnWave;            //�o�O�ĴX�i
    //    int spwanCount = currSpawn < (maxSpawnCount * maxSpawnWave) ? maxSpawnCount : maxSpawnCount - 1;    //�o�@�i�n�ͦ����ƶq
    //    //print("currSpawn: " + currSpawn + " maxSpawnWave: " + maxSpawnWave + "  waveCount: " + waveCount + "  spwanCount  " + spwanCount);
    //    for (int i = 0; i < spwanCount; i++)
    //        DoSpawnOneDoll(i, spwanCount);
    //}

    //void DoSpawnOneDoll( int batchIndex = 0, int batchTotal = 1)
    //{
    //    bool isDollInstance = false;
    //    string dollID = allDollIDs[currSpawn];
    //    if (dollID == "_DollInstance")
    //    {
    //        isDollInstance = true;
    //        DollInstanceData diData = allDIs[currSpawn];
    //        dollID = diData.baseDollID;
    //    }
    //    GameObject dollRef = GameSystem.GetDollData().GetDollRefByID(dollID);
    //    if (!dollRef)
    //    {
    //        print("Error!! There is no dollRef in ID: " + dollID);
    //        return;
    //    }

    //    Vector3 front = BattleSystem.GetInstance().GetPlayerController().GetDollManager().transform.forward;
    //    Vector3 right = BattleSystem.GetInstance().GetPlayerController().GetDollManager().transform.right;
    //    Doll theDollInRef = dollRef.GetComponent<Doll>();
    //    if (theDollInRef == null)
    //    {
    //        print("Error!! There is no Doll in dollRef !!");
    //        return;
    //    }
    //    switch (theDollInRef.positionType)
    //    {
    //        case DOLL_POSITION_TYPE.FRONT:
    //            front = front * 2.0f;
    //            break;
    //        case DOLL_POSITION_TYPE.MIDDLE:
    //            front = front * 1.0f;
    //            break;
    //        case DOLL_POSITION_TYPE.BACK:
    //            front = front * -2.0f;
    //            break;
    //    }

    //    float rightShift = -0.25f * (batchTotal - 1) + batchIndex * 0.5f;

    //    Vector3 pos = transform.position + front + right * rightShift;

    //    GameObject dollObj = null;
    //    if (isDollInstance)
    //    {
    //        DollInstanceData diData = allDIs[currSpawn];
    //        dollObj = DollInstance.SpawnDollFromData(diData, pos);
    //    }
    //    else
    //    {
    //        dollObj = BattleSystem.GetInstance().SpawnGameplayObject(dollRef, pos, false);
    //    }
    //    if (SpawnFX)
    //        BattleSystem.GetInstance().SpawnGameplayObject(SpawnFX, pos, false);
    //    Doll theDoll = dollObj.GetComponent<Doll>();

    //    if (!theDoll.TryJoinThePlayer())
    //    {
    //        print("Woooooooooops.......");
    //        return;
    //    }

    //    currSpawn++;
    //    if (currSpawn == allDollIDs.Length)
    //    {
    //        nextPhase = Phase.FINISH;
    //    }
    //}


    protected void DoSpawnOneDoll(int batchIndex = 0, int batchTotal = 1)
    {
        string dollID = allFormationDolls[currSpawn].dollID;
        GameObject dollRef = GameSystem.GetDollData().GetDollRefByID(dollID);
        if (!dollRef)
        {
            print("Error!! There is no dollRef in ID: " + dollID);
            return;
        }

        Vector3 front = BattleSystem.GetInstance().GetPlayerController().GetDollManager().transform.forward;
        Vector3 right = BattleSystem.GetInstance().GetPlayerController().GetDollManager().transform.right;
        switch (allFormationDolls[currSpawn].group)
        {
            case (int)DM_Dynamic.GROUP_TYPE.FRONT:
                front = front * 2.0f;
                break;
            case (int)DM_Dynamic.GROUP_TYPE.LEFT:
            case (int)DM_Dynamic.GROUP_TYPE.RIGHT:
                front = front * 1.0f;
                break;
            case (int)DM_Dynamic.GROUP_TYPE.BACK:
                front = front * -2.0f;
                break;
        }

        Doll theDollInRef = dollRef.GetComponent<Doll>();
        if (theDollInRef == null)
        {
            print("Error!! There is no Doll in dollRef !!");
            return;
        }


        float rightShift = -0.25f * (batchTotal - 1) + batchIndex * 0.5f;

        Vector3 pos = transform.position + front + right * rightShift;

        GameObject dollObj = null;
        dollObj = BattleSystem.GetInstance().SpawnGameplayObject(dollRef, pos, false);
        if (SpawnFX)
            BattleSystem.GetInstance().SpawnGameplayObject(SpawnFX, pos, false);
        Doll theDoll = dollObj.GetComponent<Doll>();
        //print(theDoll.ID + " Try Join: " + allFormationDolls[currSpawn].group + " -- " + allFormationDolls[currSpawn].index);

        if (!theDoll.TryJoinThePlayer(DOLL_JOIN_SAVE_TYPE.NONE, allFormationDolls[currSpawn].group, allFormationDolls[currSpawn].index))
        {
            print("Woooooooooops.......");
            return;
        }
        theDoll.joinSaveType = DOLL_JOIN_SAVE_TYPE.FOREVER; //TODO: �ݭn�Ϥ� Forever �٬O Battle

        currSpawn++;
        if (currSpawn == totalDollCount)
        {
            nextPhase = Phase.FINISH;
        }
    }

    protected void StartSpawn()
    {
        allFormationDolls = GameSystem.GetPlayerData().GetAllFormationDolls();


        if (allFormationDolls == null || allFormationDolls.Length == 0)
        {
            nextPhase = Phase.FINISH;
        }
        else
        {
            totalDollCount = allFormationDolls.Length;
            nextPhase = Phase.SPAWN;
        }

        timeLeft = 0;   //���W�ͲĤ@��

    }

    //void StartSpawn()
    //{
    //    /*Debug
    //    if (GameSystem.GetPlayerData().GetCurrDollNum() == 0)
    //    {
    //        GameSystem.GetPlayerData().AddUsingDoll("DollOne");
    //        GameSystem.GetPlayerData().AddUsingDoll("DollOne");
    //    }
    //    //*/

    //    allDIs = GameSystem.GetPlayerData().GetAllUsingDIs();
    //    int diLength = allDIs == null? 0 : allDIs.Length;

    //    string[] savedDolls = GameSystem.GetPlayerData().GetAllUsingDolls();
    //    int savedLength = savedDolls == null ? 0 : savedDolls.Length;
    //    string[] collectedDolls = ContinuousBattleManager.GetAllCollectedDolls();
    //    int collectedLength = collectedDolls == null ? 0 : collectedDolls.Length;
    //    allDollIDs = new string[diLength + savedLength + collectedLength];

    //    if (diLength > 0)
    //    {
    //        for (int i=0; i< diLength; i++)
    //        {
    //            allDollIDs[i] = "_DollInstance";        //MAGIC WORD !!
    //        }
    //    }
    //    if (savedLength > 0)
    //    {
    //        System.Array.Copy(savedDolls, 0, allDollIDs, diLength, savedLength);
    //    }
    //    if (collectedLength > 0)
    //    {
    //        System.Array.Copy(collectedDolls, 0, allDollIDs, diLength + savedLength, collectedLength);
    //    }


    //    //����
    //    //allDollIDs = new string[]{"DollStone", "DollStone", "DollOne", "DollOne", "DollFire", "DollFire" };

    //    if (allDollIDs == null || allDollIDs.Length == 0)
    //    {
    //        nextPhase = Phase.FINISH;
    //    }
    //    else
    //        nextPhase = Phase.SPAWN;

    //    timeLeft = 0;   //���W�ͲĤ@��

    //}

}
