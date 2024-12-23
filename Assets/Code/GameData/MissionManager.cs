using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class MissionData
{
    public string Title;
    public string ObjectiveText;
    //public string scene;
    public string sceneText;
    public string rewardText;
    public ContinuousBattleDataBase[] battles;
    public enum TYPE
    {
        NORMAL,     //��¨��쩳
        BOSS,       //�����ؼ�
        EXPLORE,
    }
    public TYPE type;
    public enum SCALE
    {
        LARGE,
        MEDIUM,
        SMALL,
    }
    public SCALE scale;
    public int dollLimit;

    [System.Serializable]
    public class RewardData
    {
        public int Monney = 1000;
        [System.Serializable]
        public class RewardItemDef
        {
            public string ITEM_ID;
            public int num_min;
            public int num_max;
        }
        public RewardItemDef[] items;
    }
    public RewardData rewardData;
}

public class MissionManager : GlobalSystemBase
{
    public ContinuousMORoomPathData[] DemoBattleData;   //���ծɥ�

    static MissionManager instance;
    static public MissionManager GetInstance() { return instance; }

    // ====== ���U���ȶ}���ɪ� CallBack
    public delegate void MissionManagerCB( MissionData mission);

    static public void AddAcceptMissionCB(MissionManagerCB cb, bool isRemove = false)
    {
        instance._AddAcceptMissionCB(cb, isRemove);
    }
    static public void AddCancelMissionCB(MissionManagerCB cb, bool isRemove = false)
    {
        instance._AddCancelMissionCB(cb, isRemove);
    }

    public override void InitSystem()
    {
        if (instance != null)
            print("ERROR !! �W�L�@�� MissionManager �s�b ... ");
        instance = this;
        base.InitSystem();
    }

    static public List<MissionData> GenerateMissions() 
    {
        return instance._GenerateMissions();
    }

    static public string GetScaleText(MissionData.SCALE scale)
    {
        switch (scale)
        {
            case MissionData.SCALE.SMALL:
                return "�p";
            case MissionData.SCALE.MEDIUM:
                return "��";
            case MissionData.SCALE.LARGE:
                return "�j";
        }
        return "XX";
    }

    static public MissionData GetCurrMission()
    {
        return instance._GetCurrMission();
    }

    static public void AcceptMission(MissionData mission)
    {
        instance._AcceptMission(mission);
    }
    static public void CancelCurrMission()
    {
        instance._CancelCurrMission();
    }

    static public void CompleteCurrMission()        //���Ȧ��\�����A�o����y (���٨S�u�����v����)
    {
        print("MissionManager.CompleteCurrMission()");
        instance._CompleteCurrMission();
    }

    static public void FinishCurrMission()          //���ެO���\�P�_�A���ȡu�����v
    {
        print("MissionManager.FinishCurrMission()");
        instance._FinishCurrMission();
    }

    static public void StartCurrMission()
    {
        instance._StartCurrMission();
    }

    static public void StartMission(MissionData mission)
    {
        ContinuousBattleManager.StartNewBattle(mission.battles);

        string sceneName = mission.battles[0].scene;
        BattleSystem.GetInstance().OnGotoScene(sceneName, "");
    }

    //================================================================

    protected MissionData currMission = null;

    protected void _AcceptMission(MissionData mission)
    {
        if (currMission != null)
        {
            One.ERROR("AcceptMission ���~�A�w�s�b Mission: " + mission.Title);
        }
        currMission = mission;
        foreach (MissionManagerCB cb in acceptCBs)
        {
            cb(currMission);
        }
    }

    protected MissionData _GetCurrMission() { return currMission; }

    protected void _StartCurrMission()
    {
        if (currMission == null)
        {
            One.ERROR("StartCurrMission ���~�A�S��������������");
            return;
        }
        StartMission(currMission);
    }


    protected void _CancelCurrMission()
    {
        if (currMission == null)
        {
            One.ERROR("CancelCurrMission ���~�A�S��������������");
        }
        foreach (MissionManagerCB cb in cancelCBs) 
        {
            cb(currMission);
        }

        currMission = null;
    }


    protected void _FinishCurrMission()
    {
        if (currMission == null)
        {
            One.ERROR("FinishCurrMission ���~�A�S��������������");
        }
        currMission = null;
    }


    //���ȧ����o����y�A�]���ٯd�b���d�������Y�A�����٨S Finish
    protected void _CompleteCurrMission()
    {
        if (currMission == null)
        {
            One.ERROR("CompleteCurrMission ���~�A�S��������������");
            return;
        }
        SystemUI.ShowMessageBox(null, "�o����y���� " + currMission.rewardData.Monney);
        GameSystem.GetPlayerData().AddMoney(currMission.rewardData.Monney);
        for (int i=0; i<currMission.rewardData.items.Length; i++)
        {
            int itemNum = Random.Range(currMission.rewardData.items[i].num_min, currMission.rewardData.items[i].num_max+1);
            GameSystem.GetPlayerData().AddItem(currMission.rewardData.items[i].ITEM_ID, itemNum);
            print("�[�J�F " + currMission.rewardData.items[i].ITEM_ID + " " + itemNum + "��");
        }
    }


    protected List<MissionData> _GenerateMissions()
    {
        MissionData.SCALE[] scales = {MissionData.SCALE.SMALL,MissionData.SCALE.MEDIUM,MissionData.SCALE.LARGE,};
        MissionData.TYPE[] types = {MissionData.TYPE.EXPLORE, MissionData.TYPE.NORMAL, MissionData.TYPE.BOSS };
        int[] dollLimits = { 10, 15, 20 };
        string[] titles = {"�a�}����","�F�z�j�|��","�a�������v" };
        string[] ObjectiveTexts = { "��F�a�}�`�B", "�M���ĤH","���� BOSS" };
        string[] scenes = { "DemoRoomPath", "RoomPathDesert", "RoomPathDungeon" };
        string[] sceneTexts = { "�a�}","�F�z","�a��" };
        string[] rewardTexts = {"����","�F�ۯ���","���_��" };



        List < MissionData > missions = new List < MissionData >();
        for (int i = 0; i < 3; i ++) 
        {
            MissionData data = new();
            data.Title = titles[i];
            data.type = types[i];// MissionData.TYPE.EXPLORE;
            //data.scene = scenes[i];
            //data.ObjectiveText = "��F�a�}�`�B";
            data.ObjectiveText = ObjectiveTexts[i];
            data.sceneText = sceneTexts[i];
            data.rewardText = rewardTexts[i];
            data.scale = scales[i];
            data.dollLimit = dollLimits[i];
            data.battles = new ContinuousBattleDataBase[1];
            data.battles[0] = DemoBattleData[i];
            missions.Add(data);
        }
        return missions;
    }

    //==================== �B�z���ȱ����B�����ɪ��欰��
    protected List<MissionManagerCB> acceptCBs = new List<MissionManagerCB>();
    protected List<MissionManagerCB> cancelCBs = new List<MissionManagerCB>();
    protected void _AddAcceptMissionCB(MissionManagerCB cb, bool isRemove = false)
    {
        if (!isRemove)
        {
            acceptCBs.Add(cb);
        }
        else
        {
            if (!acceptCBs.Remove(cb))
            {
                One.ERROR("MissionManager Remove AcceptCB ERROR");
            }
            else
                print("���� Accept CB ���\");
        }
    }
    protected void _AddCancelMissionCB(MissionManagerCB cb, bool isRemove = false)
    {
        if (!isRemove)
        {
            cancelCBs.Add(cb);
        }
        else
        {
            if (!cancelCBs.Remove(cb))
            {
                One.ERROR("MissionManager Remove Cancel CB ERROR");
            }
            else
                print("���� Cancel CB ���\");
        }
    }
}
