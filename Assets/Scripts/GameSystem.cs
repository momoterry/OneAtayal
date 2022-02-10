using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //���F�z�L Scene �۰ʥ[�� GameSystem

public class GameSystem : MonoBehaviour
{
    private GameObject playerCharacterRef = null;

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
            //print("�٨S���Ы� GameSystem�A�ݭn�[�� Scene: GameSystem!!");
            SceneManager.LoadScene("Global", LoadSceneMode.Additive);
            //print("�[����, instace =" + instance);
        }
    }

    public void SetPlayerCharacterRef( GameObject objRef)
    {
        playerCharacterRef = objRef;
    }

    public GameObject GetPlayerCharacterRef()
    {
        return playerCharacterRef;
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
