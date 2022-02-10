using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Battle_HUD : MonoBehaviour
{
    // Start is called before the first frame update

    // HP / MP
    public Slider hpBar;
    public Slider mpBar;
    public Text hpText;
    public Text mpText;

    //���d��T
    public Text levelText;

    //���ѵe��
    public GameObject failMenu;

    //��~
    public Text PotionNumText;
    public Text PotionMaxText;

    //�ޯ�
    public Text AttackText;

    void Start()
    {
        failMenu.SetActive(false);
        hpBar.value = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //�B�z��L
        if (Input.GetKeyDown("1"))
        {
            OnButtonPotion();
        }

    }

    public void SetPlayerInfo( float hp, float maxHp, float mp, float maxMP, float Attack)
    {
        hpBar.value = hp / maxHp;
        mpBar.value = mp / maxMP;

        hpText.text = hp.ToString("F0") + " / " + maxHp.ToString("F0");
        mpText.text = mp.ToString("F0") + " / " + maxMP.ToString("F0");

        AttackText.text = "ATK: " + Attack.ToString("F1");
    }

    public void SetLevelText(string text)
    {
        levelText.text = text;
    }

    public void OnStartFailUI()
    {
        failMenu.SetActive(true) ;
    }

    public void OnFailRetry()
    {
        print("�A�Ӥ@�� !!!");
        failMenu.SetActive(false);
        BattleSystem.GetInstance().OnLevelRestart();
    }

    public void OnFailBackToStartMenu()
    {
        print("�Q�^�D���r�A�������r");
        failMenu.SetActive(false);
        BattleSystem.GetInstance().OnBackToStartMenu();
    }

    public void SetPotionNum( int num, int maxNum)
    {
        if (PotionNumText)
            PotionNumText.text = "x " + num.ToString();
        if (PotionMaxText)
            PotionMaxText.gameObject.SetActive(num == maxNum);
    }

    public void OnButtonPotion()
    {
        BattleSystem.GetInstance().OnUsePotion();
    }
}
