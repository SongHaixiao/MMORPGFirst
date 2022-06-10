using Common.Data;
using Models;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    /* Function : Control the behavi of NPC.*/

    // open compoents
    public int NpcID;


    // private attributes
    SkinnedMeshRenderer renderer;
    Animator anim;
    Color orignColor;

    private bool inInteractive = false;

    NpcDefine npc;

    NpcQuestStatus questStatus;

    // Start is called before the first frame update
    void Start()
    {
        // get Skinned Mesah Render
        this.renderer = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();

        // get Animator
        this.anim = this.gameObject.GetComponent<Animator>();

        // set orign color
        this.orignColor = renderer.sharedMaterial.color;

        // get this npc's data from from NPCManager
        this.npc = NPCManager.Instance.GetNpcDefine(this.NpcID);

        this.StartCoroutine(Actions());

        RefreshNpcStatus();
        QuestManager.Instance.OnQuestStatusChanged += OnQuestStatusChanged;

    }

    void OnQuestStatusChanged(Quest quest)
    {
        this.RefreshNpcStatus();
    }

    private void RefreshNpcStatus()
    {
        questStatus = QuestManager.Instance.GetQuestStatusByNpc(this.NpcID);
        UIWorldElementManager.Instance.AddNpcQuestStatus(this.transform, questStatus);
    }

    private void OnDestroy()
    {
        QuestManager.Instance.OnQuestStatusChanged -= OnQuestStatusChanged;
        if (UIWorldElementManager.Instance != null)
            UIWorldElementManager.Instance.RemoveNpcQuestStatus(this.transform);
    }

    // set actions
    IEnumerator Actions()
    {
        while(true)
        {
            // is insteractive, wait 2s
            if(inInteractive)
            {
                yield return new WaitForSeconds(2f);
            }

            // is not interactive, wait 5s ~ 10s
            else
                yield return new WaitForSeconds(Random.Range(5f, 10f));

            // set relax state
            this.Relax();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // set Relax state
    void Relax()
    {
        this.anim.SetTrigger("Relax");
    }

    // interact
    void Interactive()
    {
        // if -> check to avoid repeat click
        if(!inInteractive)
        {
            // non interactive state, active interactive
            inInteractive = true;
            StartCoroutine(DoInteractive());
        }
    }

    // do interactive operation
    IEnumerator DoInteractive()
    {
        // firstly make npc towards to player
        yield return FaceToPlayer();

        // set Talk state
        if(NPCManager.Instance.Interactive(npc))
        {
            this.anim.SetTrigger("Talk");
        }

        // waite 3s
        yield return new WaitForSeconds(3f);

        // cancel interactive
        inInteractive = false;
    }

    // npc towards to player
    IEnumerator FaceToPlayer()
    {
        Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized;
        while (Mathf.Abs(Vector3.Angle(this.gameObject.transform.forward, faceTo)) > 5)
        {
            this.gameObject.transform.forward = Vector3.Lerp(this.gameObject.transform.forward, faceTo, Time.deltaTime * 5f);
            yield return null;
        }
    }

    // do interact when mouse down
    private void OnMouseDown()
    {
        if(Vector3.Distance(this.transform.position, User.Instance.CurrentCharacterObject.transform.position) > 2f)
        {
            User.Instance.CurrentCharacterObject.StartNav(this.transform.position);
        }
        Interactive();
    }

    // high light when mouse over
    private void OnMouseOver()
    {
        HighLight(true);
    }

    // high light when mouse enter
    private void OnMouseEnter()
    {
        HighLight(true);
    }

    // high ligh when mouse exit
    private void OnMouseExit()
    {
        HighLight(false);
    }

    // method to operate the High Light
    // which change the material color
    void HighLight(bool hightLight)
    {
        // set material color as white when high light
        if(hightLight)
        {
            if(renderer.sharedMaterial.color != Color.white)
                renderer.sharedMaterial.color = Color.white;
        }

        // set material color as orign color when not high light
        else
        {
            if (renderer.sharedMaterial.color != orignColor)
                renderer.sharedMaterial.color = orignColor;
        }
    }
}

