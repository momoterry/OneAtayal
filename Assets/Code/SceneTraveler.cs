using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTraveler : MonoBehaviour
{
    static public void GotoScene(string sceneName, string entraceName)
    {
        SceneManager.LoadScene(sceneName);
        BattleSystem bs = BattleSystem.GetInstance();
        print("SceneTraveler GotoScene Done....Entrace: " + entraceName);
        print("SceneTraveler GotoScene Done....BattleSystem: " + bs.name);
    }
}
