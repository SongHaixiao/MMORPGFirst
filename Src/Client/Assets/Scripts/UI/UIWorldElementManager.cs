using Entities;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager> 
{
    // open components
    public GameObject nameBarPrefab;
    public GameObject npcStatusPrefab;

    // dicionary to manager world element ui
    private Dictionary<Transform, GameObject> elementNames = new Dictionary<Transform, GameObject>();
    private Dictionary<Transform, GameObject> elementStatus = new Dictionary<Transform, GameObject>();

    protected override void OnStart()
    {
        nameBarPrefab.SetActive(false);
        npcStatusPrefab.SetActive(false);
    }

    // add UINameBar for character object when it is created
    public void AddCharacterNameBar(Transform owner, Character character) 
    {
        GameObject goNameBar = Instantiate(nameBarPrefab, this.transform); // instance UINameBar to this.transform positon with goNameBar
        goNameBar.name = "NameBar" + character.entityId;               // gieve goNameBar name's value
        goNameBar.GetComponent<UIWorldElement>().owner = owner;        // set goNameBar's UIWorldElement component  owner
        goNameBar.GetComponent<UINameBar>().character = character;     // set goNameBar's UINameBar component character object
        goNameBar.SetActive(true);                                     // active goNameBar
        this.elementNames[owner] = goNameBar;                       // add instanced goNameBar into elements
    }

    // remove UINameBar from character object when it is dead
    public void RemoveCharacterNameBar(Transform owner)
    {

        // if UINameBar of owner exieted in elemnts
        if (this.elementNames.ContainsKey(owner))
        {
            Destroy(this.elementNames[owner]);  // destroy UINameBar from owner
            this.elementNames.Remove(owner);    // remove UINameBar from owner
        }
    }

    // add task status to task npc
    public void AddNpcQuestStatus(Transform owner, NpcQuestStatus status)
    {
        // check task npc whether has status, 
        // have add status ui to npc, 
        // don't have, create status ui to npc
        if (this.elementStatus.ContainsKey(owner))
        {
            elementStatus[owner].GetComponent<UIQuestStatus>().SetQuestStatus(status);
        }
        else
        {
            GameObject go = Instantiate(npcStatusPrefab, this.transform);
            go.name = "NpcQuestStatus" + owner.name;
            go.GetComponent<UIWorldElement>().owner = owner;
            go.GetComponent<UIQuestStatus>().SetQuestStatus(status);
            go.SetActive(true);
            this.elementStatus[owner] = go;
        }
    }

    // delete task npc task status
    public void RemoveNpcQuestStatus(Transform owner)
    {
        if (this.elementStatus.ContainsKey(owner))
        {
            Destroy(this.elementStatus[owner]);
            this.elementStatus.Remove(owner);
        }
    }
}
