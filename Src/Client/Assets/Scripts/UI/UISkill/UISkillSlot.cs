using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Battle;
using Common.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Common.Battle;
using Managers;

public class UISkillSlot : MonoBehavior, IPointerClickHandler
{
    public Image icon;
    public Image overlay;
    public Text cdText;

    Skill skill;

    // Use this for initialization
    void Start()
    {
        overlay.enabled = false;
        cdText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.skill == null) return;
        if (this.skill.CD > 0)
        {
            if (!overlay.enabled) overlay.enabled = true;
            if (!cdText.enabled) cdText.enabled = true;

            overlay.fillAmount = this.skill.CD / this.skill.Define.CD;
            this.cdText.text = ((int)Math.Ceiling(this.skill.CD)).ToString();
        }
        else
        {
            if (overlay.enabled) overlay.enabled = false;
            if (this.cdText.enabled) this.cdText.enabled = false;
        }
    }

    public void OnPositionSelected(Vector3 pos)
    {
        BattleManager.Instance.CurrentPosition = GameObjectTool.WorldToLogicN(pos);
        this.CastSkill();
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        if(this.skill.Define.CastTarget == Common.Battle.TargetType.Position)
        {
            TargetSelector.ShowSelector(User.Instance.CurrentCharacter.position, this.skill.Define.CastRange, this.skill.Define.AOERange);
            return;
        }

        CastSkill();
    }

    private void CastSkill()
    { 
        SkillResult result = this.skill.CanCast(BattleManager.Instance.CurrentTarget);
        switch (result)
        {
            case SkillResult.InvalidTarget:
                MessageBox.Show("Skill : [ " + this.skill.Define.Name + " ] invalid target !");
                return;

            case SkillResult.OutOfMP:
                MessageBox.Show("Skill : [ " + this.skill.Define.Name + " ] MP is not enough !");
                return;

            case SkillResult.Cooldown:
                MessageBox.Show("Skill : [ " + this.skill.Define.Name + " ] is cooling down !");
                return;

            case SkillResult.OutOfRange:
                MessageBox.Show("Skill : [ " + this.skill.Define.Name + " ] is out of range !");
                return;
        }

        BattleManager.Instance.CastSkill(this.skill);
    }

    internal void SetSkill(Skill value)
    {
        this.skill = value;
        if(this.icon != null)
        {
            this.icon.overrideSprite = Resloader.Load<Sprite>(this.skill.Define.Icon);
            this.icon.SetAllDirty();
            
        }
    }
}