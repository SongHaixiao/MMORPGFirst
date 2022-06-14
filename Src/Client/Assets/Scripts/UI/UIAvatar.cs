using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAvatar : MonoBehaviour
{
    public Text Name;
    public Character character;
    public Slider HPBar;
    public Slider MPBar;
    public Text HPText;
    public Text MPText;
    public Image AvatarImage;
    public 


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        // update ui name bar per frame
        this.UpadteAvatarUI();
    }

    // update UINambeBar method
    void UpadteAvatarUI()
    {
        if (this.character == null) return;
        this.Name.text = string.Format("{0} Lv.{1}", character.Name, character.Info.Level);

        this.HPBar.maxValue = character.Attributes.MaxHP;
        this.HPBar.value = character.Attributes.HP;
        this.HPText.text = string.Format("{0} / {1}", character.Attributes.HP, character.Attributes.MaxHP);

        this.MPBar.maxValue = character.Attributes.MaxMP;
        this.MPBar.value = character.Attributes.MP;
        this.MPText.text = string.Format("{0} / {1}", character.Attributes.MP, character.Attributes.MaxMP);

        this.AvatarImage.overrideSprite = Resloader.Load<Sprite>(this.character.Define.Icon);
    }
}
