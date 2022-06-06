using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISetting:UIWindow
{
    public void ExitToCharSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");
        Service.UserSerivce.Instance.SendGameLeave();
    }

    public void ExitGame()
    {
         Service.UserSerivce.Instance.SendGameLeave(true);
    }
}