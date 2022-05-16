using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIRegister : MonoBehaviour
{
    /* Function : Manage the user register.*/

    /* Open Components to unity editor. */
    public InputField Account;
    public InputField Password;
    public InputField PasswordConfirm;
    public Button ButtonRegister;

    // Start is called before the first frame update
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

        // inform logic layer to rigister
        UserService.Instance.SendRegister(this.Account.text, this.Password.text);
    }

    // register event's operation in UI Layer
    void OnRegister(Result result, string msg)
    {
        // MessageBox.Show(string.Format("Result : {0} ! {1} ", result, msg));
        Debug.LogFormat("Result : {0} ! {1} ", result, msg);
    }
}
