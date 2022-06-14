using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Services;
using System;
using Managers;
using Entities;

public class UIMain : MonoSingleton<UIMain>
{
    /* Function : Manager the Main City UI.*/

    //No1. Open Compoents
    public Text avatarName;
    public Text avatarLevle;
    public Slider avatarHPBar;
    public Slider avatarMPBar;
    public Text avatarHPValue;
    public Text avatarMPValue;
    public Image avatarImage;

    public UICreatureInfo targetUI;
    public UISkillSlots skillSlots;

    // Start is called before the first frame update
    protected override void OnStart()
    {
        //No.2 start UpdateAvatar() method
        this.UpdateAvatar();
        this.targetUI.gameObject.SetActive(false);
        BattleManager.Instance.OnTargetChanged += OnTargetChanged;

        User.Instance.OnCharacterInit += this.skillSlots.UpdateSkills;
        this.skillSlots.UpdateSkills();
    }


    // NO.3 update avatar ui
    void UpdateAvatar()
    {
        Character cha = User.Instance.CurrentCharacter;

        // this ui is User inforamtion
        this.avatarName.text = string.Format("{0} [ID : {1} ]", cha.Info.Name, cha.Define.TID);
        this.avatarLevle.text = cha.Info.Level.ToString();

        this.avatarHPBar.maxValue = cha.Attributes.MaxHP;
        this.avatarHPBar.value = cha.Attributes.HP ;
        this.avatarHPValue.text = string.Format("{0} / {1}", cha.Attributes.HP, cha.Attributes.MaxHP);

        this.avatarMPBar.maxValue = cha.Attributes.MaxMP;
        this.avatarMPBar.value = cha.Attributes.MP;
        this.avatarMPValue.text = string.Format("{0} / {1}", cha.Attributes.MP, cha.Attributes.MaxMP);

        this.avatarImage.overrideSprite = Resloader.Load<Sprite>(cha.Define.Icon);
    }

    // Update is called once per frame
    void Update()
    {

    }


    //// click UITest Button method
    //// to call UITest prefab
    //public void OnClickUITest()
    //{
    //    // get UITest prefab
    //    UITest test = UIManager.Instance.Show<UITest>();

    //    // set UITest prefab's title
    //    test.SetTitle("This is a test UI ！");

    //    // add listener to close evetn for UI Test prefab
    //    test.OnClose += Test_OnClose;
    //}

    //// method for preocessing the result of UITest
    //private void Test_OnClose(UIWindow sender, UIWindow.WindowResult result)
    //{
    //    MessageBox.Show("Click the dialog box : " + result, " Response Result ", MessageBoxType.Information);
    //}

    // click ui button show UIBag
    public void OnClickBag()
    {
        UIManager.Instance.Show<UIBag>();
    }

    // click equip button show character euqipment ui
    public void OnClickCharEquip()
    {
        UIManager.Instance.Show<UICharEquip>();
    }

    // click equip button show character euqipment ui
    public void OnClickQuest()
    {
        UIManager.Instance.Show<UIQuestSystem>();
    }

    // public void OnClickFriends()
    // {

    // }

    // public void OnClickGuild()
    // {

    // }

    //public void OnClickRide()
    //{
    //    UIManager.Instance.Show<UIRide>();
    //}

    public void OnClickSetting()
    {
        UIManager.Instance.Show<UISetting>();
    }

    public void OnClickSkill()
    {
        UIManager.Instance.Show<UISkill>();
    }

    // public void ShowTeamUI(bool show)
    // {

    // }

    private void OnTargetChanged(Entities.Creature target)
    {
        if(target != null)
        {
            if (!targetUI.isActiveAndEnabled) targetUI.gameObject.SetActive(true);
            targetUI.Target = target;
        }
        else
        {
            targetUI.gameObject.SetActive(false);
        }
    }
}
