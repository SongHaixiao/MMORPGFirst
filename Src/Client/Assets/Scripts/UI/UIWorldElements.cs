using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Services;

public class UIWorldElements : MonoBehaviour
{
    /* Function : Manager the Main City UI.*/

    //No1. Open Compoents
    public Text avatarName;
    public Text avatarLevle;
        
    // Start is called before the first frame update
    void Start()
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
        this.avatarName.text = string.Format("{0}{1}", User.Instance.CurrentCharacter.Name, User.Instance.CurrentCharacter.Id);
        this.avatarLevle.text = User.Instance.CurrentCharacter.Level.ToString();
    }

    // click back button to go back to character selection scene
    public void BackToCharacterSelection()
    {
        SceneManager.Instance.LoadScene("CharacterScene");
        UserService.Instance.SendGameLeave();
    }
}
