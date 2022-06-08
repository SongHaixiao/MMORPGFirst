﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIMessageBox : MonoBehaviour 
{
    /* Function : the API for message box UI*/

    /* Open Components to unity editor*/
    public Text title;
    public Text message;
    public Image[] icons;
    public Button buttonYes;
    public Button buttonNo;
    public Button buttonClose;

    public Text buttonYesTitle;
    public Text buttonNoTitle;

    public UnityAction OnYes;
    public UnityAction OnNo;
    

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // initialization
    public void Init(string title, string message, MessageBoxType type = MessageBoxType.Information, string btnOK = "", string btnCancel = "")
    {
        // tiele message is available，fill in the information
        if (!string.IsNullOrEmpty(title)) this.title.text = title;
        this.message.text = message; 
        this.icons[0].enabled = type == MessageBoxType.Information;
        this.icons[1].enabled = type == MessageBoxType.Confirm;
        this.icons[2].enabled = type == MessageBoxType.Error;

        if (!string.IsNullOrEmpty(btnOK)) this.buttonYesTitle.text = title;
        if (!string.IsNullOrEmpty(btnCancel)) this.buttonNoTitle.text = title;

        this.buttonYes.onClick.AddListener(OnClickYes); // add click yes event
        this.buttonNo.onClick.AddListener(OnClickNo);  // add click no event

        // when message type is confirm, active the NO button
        this.buttonNo.gameObject.SetActive(type == MessageBoxType.Confirm);

        if(type == MessageBoxType.Error)
            SoundManager.Instance.PlaySound(SoundDefine.SFX_Message_Error);
        else
            SoundManager.Instance.PlaySound(SoundDefine.SFX_Message_Info);
    }

    // click button yes event
    void OnClickYes()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Confirm);
        Destroy(this.gameObject);
        if (this.OnYes != null)
            this.OnYes();
    }

    // click button no event
    void OnClickNo()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Close);
        Destroy(this.gameObject);
        if (this.OnNo != null)
            this.OnNo();
    }
}
