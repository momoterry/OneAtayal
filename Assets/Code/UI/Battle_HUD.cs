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
    //public Image[] SkillIcons;
    public SkillButton[] SkillButtons;

    //�����n�����
    public VPad theVPad;


    void Start()
    {
        failMenu.SetActive(false);
        if (hpBar)
            hpBar.value = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //�B�z��L
        //if (Input.GetKeyDown("1"))
        //{
        //    OnButtonPotion();
        //}

    }

    //public void SetSkillIcon(Sprite sprite, int index)
    //{
    //    if (index < 0 || index >= SkillIcons.Length)
    //        return;

    //    //print("SetSkillIcon " + index + "   " + sprite);

    //    if (SkillIcons[index])
    //    {
    //        SkillIcons[index].sprite = sprite;
    //    }
    //}

    public SkillButton GetSkillButton(int index)
    {
        if (index < 0 || index >= SkillButtons.Length)
            return null;

        return SkillButtons[index];
    }

    public void SetPlayerInfo( float hp, float maxHp, float mp, float maxMP, float Attack)
    {
        if (hpBar)
            hpBar.value = hp / maxHp;
        if (mpBar)
            mpBar.value = mp / maxMP;

        if (hpText)
            hpText.text = hp.ToString("F0") + " / " + maxHp.ToString("F0");
        if (mpText)
            mpText.text = mp.ToString("F0") + " / " + maxMP.ToString("F0");

        if (AttackText)
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
        //print("�A�Ӥ@�� !!!");
        failMenu.SetActive(false);
        BattleSystem.GetInstance().OnLevelRestart();
    }

    public void OnFailBackToStartMenu()
    {
        //print("�Q�^�D���r�A�������r");
        failMenu.SetActive(false);
        BattleSystem.GetInstance().OnBackToStartMenu();
    }

    public void OnBackToScene()
    {
        //print("�^���w���d");
        failMenu.SetActive(false);
        BattleSystem.GetInstance().OnBackPrevScene();
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
