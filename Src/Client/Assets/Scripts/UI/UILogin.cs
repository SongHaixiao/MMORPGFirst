using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Services;
using SkillBridge.Message;

public class UILogin : MonoBehaviour
{
    /* Function : Manage the login .*/

    /* open components for unity editor*/
    public InputField Account;
    public InputField Password;
    public Button ButtonLogin;
    public Button ButtonRegister;


    // Start is called before the first frame update
    void Start()
    {
        // listener of the login event from logic layer ( UserService.cs ) 
        UserService.Instance.OnLogin = OnLogin;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // method for login button is clicked
    public void OnClickLogin()
    {
        // check the text of Account and Passord whether is unavailable

        if(string.IsNullOrEmpty(this.Account.text))
        {
            MessageBox.Show("Please input account !");
            return;
        }

        if(string.IsNullOrEmpty(this.Password.text))
        {
            MessageBox.Show("Please input password !");
            return ;
        }

        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);

        // tell the logical layer login's information
        UserService.Instance.SendLogin(this.Account.text, this.Password.text);
    }

    // login operation for login event
    void OnLogin(Result result, string message)
    {
        if(result == Result.Success)
        {
            // login successed
            MessageBox.Show("Login Successed ! Prepare to selected Character !"
                + message, " Tip ", MessageBoxType.Information);

            // loading character selection scene
            SceneManager.Instance.LoadScene("CharacterScene");
            SoundManager.Instance.PlaySound(SoundDefine.Music_Select);
        }
        else
        {
            // login failed
            MessageBox.Show(message,"Error!",MessageBoxType.Error);
        }
    }

}
