using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Services;
using System;

public class UIMain : MonoSingleton<UIMain>
{
    /* Function : Manager the Main City UI.*/

    //No1. Open Compoents
    public Text avatarName;
    public Text avatarLevle;

    // Start is called before the first frame update
    protected override void OnStart()
    {
        //No.2 start UpdateAvatar() method
        this.UpdateAvatar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // NO.3 update avatar ui
    void UpdateAvatar()
    {
        // this ui is User inforamtion
        this.avatarName.text = string.Format("{0}", User.Instance.CurrentCharacter.Name);
        this.avatarLevle.text = User.Instance.CurrentCharacter.Level.ToString();
    }

    // click back button to go back to character selection scene
    public void BackToCharacterSelection()
    {
        SceneManager.Instance.LoadScene("CharacterScene");
        UserService.Instance.SendGameLeave();
    }
    
    // click UITest Button method
    // to call UITest prefab
    public void OnClickUITest()
    {
        // get UITest prefab
        UITest test = UIManager.Instance.Show<UITest>();

        // set UITest prefab's title
        test.SetTitle("This is a test UI ！");

        // add listener to close evetn for UI Test prefab
        test.OnClose += Test_OnClose;
    }

    // method for preocessing the result of UITest
    private void Test_OnClose(UIWindow sender, UIWindow.WindowResult result)
    {
        MessageBox.Show("Click the dialog box : " + result, " Response Result ", MessageBoxType.Information);
    }

    // click ui button show UIBag
    public void OnClickBag()
    {
        UIManager.Instance.Show<UIBag>();
    }

    // clicl equip button show character euqipment ui
    public void OnClickCharEquip()
    {
        UIManager.Instance.Show<UICharEquip>();
    }
}
