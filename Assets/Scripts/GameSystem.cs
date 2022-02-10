using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //為了透過 Scene 自動加載 GameSystem

public class GameSystem : MonoBehaviour
{
    private GameObject playerCharacterRef = null;

    static private GameSystem instance;

    public GameSystem() : base()
    {
        print("GameSystem : 我被創建了!!!");
        if (instance != null)
            print("ERROR !! 超過一份 Game System 存在 ");
        instance = this;
        print("GameSystem 創建完成");
    }

    private void Awake()
    {
        //print("我被喚醒了");
    }

    static public GameSystem GetInstance()
    {
        //print("GetInstance() 結果 = " + instance);
        return instance;
    }

    static public void Ensure()
    {
        if (instance == null)
        {
            //print("還沒有創建 GameSystem，需要加載 Scene: GameSystem!!");
            SceneManager.LoadScene("Global", LoadSceneMode.Additive);
            //print("加載完, instace =" + instance);
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
