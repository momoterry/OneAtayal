using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //���F�z�L Scene �۰ʥ[�� GameSystem

public class GameSystem : MonoBehaviour
{
    public PlayerData thePlayerData;

    //TODO: �o�������ӧ�� PlayerData ��
    private GameObject playerCharacterRef = null;

    //Skill ���� //TODO: �o�������ӧ�� PlayerData ��
    private Dictionary<string, SkillBase> skillMap = new Dictionary<string, SkillBase>(); 

    static private GameSystem instance;

    public GameSystem() : base()
    {
        print("GameSystem : �ڳQ�ЫؤF!!!");
        if (instance != null)
            print("ERROR !! �W�L�@�� Game System �s�b ");
        instance = this;
        print("GameSystem �Ыا���");
    }

    private void Awake()
    {
        //print("�ڳQ����F");
    }

    static public GameSystem GetInstance()
    {
        //print("GetInstance() ���G = " + instance);
        return instance;
    }

    static public void Ensure()
    {
        if (instance == null)
        {
            print("�٨S���Ы� GameSystem�A�ݭn�[�� Scene: GameSystem!!");
            SceneManager.LoadScene("Global", LoadSceneMode.Additive);
            //print("�[����, instace =" + instance);
        }
    }

    static public PlayerData GetPlayerData()
    {
        if (!instance || !instance.thePlayerData)
        {
            print("ERROR !!! GameSystem �䤣�� PlayerData !!" + instance);
            return null;
        }
        return instance.thePlayerData;
    }

    public void SetPlayerCharacterRef( GameObject objRef)
    {
        playerCharacterRef = objRef;
    }

    public GameObject GetPlayerCharacterRef()
    {
        return playerCharacterRef;
    }

    public void SetPlayerSkillRef( string skillStr, SkillBase skillRef)
    {
        if (skillMap.ContainsKey(skillStr))
            skillMap[skillStr] = skillRef;
        else
            skillMap.Add(skillStr, skillRef);
    }

    public SkillBase GetPlayerSkillRef(string skillStr)
    {
        if (skillMap.ContainsKey(skillStr))
            return skillMap[skillStr];
        else
            return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 300;  //�j�� Android �}�� !!

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
