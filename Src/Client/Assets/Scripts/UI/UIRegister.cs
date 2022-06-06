using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Services;
using SkillBridge.Message;

public class UIRegister : MonoBehaviour
{
    /* Function : Manage the user register.*/

    /* Open Components to unity editor. */
    public InputField Account;
    public InputField Password;
    public InputField PasswordConfirm;
    public Button ButtonRegister;

    public GameObject LoginPanel;
    void Start()
    {
        // accpet register event in logic layer
        UserService.Instance.OnRegister = this.OnRegister;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    // click register button method
    public void OnClickRegister()
    {
        // check the message inputed
        //  if components are unavailable，give the hit message

        if(string.IsNullOrEmpty(this.Account.text))
        {
            MessageBox.Show("Please input account!");
            return;
        }
        
        if(string.IsNullOrEmpty(this.Password.text))
        {
            MessageBox.Show("Please input password!");
            return;
        }

        if(string.IsNullOrEmpty(this.PasswordConfirm.text))
        {
            MessageBox.Show("Please confirm your password!");
            return;
        }

        if(this.Password.text != this.PasswordConfirm.text)
        {
            MessageBox.Show("Two passwords are different! Check your passwords!");
            return;
        }

        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        // inform logic layer to rigister
        UserService.Instance.SendRegister(this.Account.text, this.Password.text);
    }

    // register event's operation in UI Layer
    void OnRegister(Result result, string message)
    {
        if (result == Result.Success)
        {
            //登录成功，进入角色选择
            MessageBox.Show("Register is successed !", "Tip", MessageBoxType.Information).OnYes = this.CloseRegister;
        }
        else
            MessageBox.Show(message, "Error !", MessageBoxType.Error);
    }

    void CloseRegister()
    {
        this.gameObject.SetActive(false);
        LoginPanel.SetActive(true);
    }
}
