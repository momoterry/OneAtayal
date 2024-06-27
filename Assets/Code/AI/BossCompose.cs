using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCompose : MonoBehaviour
{
    public EnemyBeta theBoss;

    [System.Serializable]
    public class ModuleData
    {
        public GameObject moduelObject;
        public SkillBase skillRef;
    }

    public ModuleData[] headModuels;
    public ModuleData[] bodyModuels;

    public int debugHeadIndex = -1;
    public int debugBodyIndex = -1;

    protected void RandomCompose()
    {
        int headIndex = debugHeadIndex < 0 ? Random.Range(0, headModuels.Length) : debugHeadIndex;
        int bodyIndex = debugBodyIndex < 0 ? Random.Range(0, bodyModuels.Length) : debugBodyIndex;

        for (int i=0; i < headModuels.Length; i++)
        {
            headModuels[i].moduelObject.SetActive(i == headIndex);
        }
        for (int i = 0; i < bodyModuels.Length; i++)
        {
            bodyModuels[i].moduelObject.SetActive(i == bodyIndex);
        }

        if (theBoss)
        {
            theBoss.normalSkillRef = headModuels[headIndex].skillRef;
            theBoss.bigOneSkillRef = bodyModuels[bodyIndex].skillRef;
        }
    }

    void Awake()
    {
        RandomCompose();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
