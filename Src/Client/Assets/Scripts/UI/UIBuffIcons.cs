using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Battle;
using Entities;

public class UIBuffIcons : MonoBehaviour
{
	Creature Owner;
	public GameObject prefabBuff;
	Dictionary<int, GameObject> buffs = new Dictionary<int, GameObject>();

	// Use this for initialization
	void Awake()
	{
		prefabBuff.SetActive(false);
	}

	// Update is called once per frame
	void OnDestroy()
	{
		this.Clear();
	}

	public void SetOwner(Creature owner)
    {
		if(this.Owner != null && this.Owner != owner)
        {
			this.Clear();
        }

		this.Owner = owner;
		this.Owner.OnBuffAdd += OnBuffAdd;
		this.Owner.OnBuffRemove += OnBuffRemove;

		this.InitBuffs();
    }

	private void InitBuffs()
    {
		foreach(var buff in this.Owner.BuffMgr.Buffs)
        {
			this.OnBuffAdd(buff.Value);
        }
    }

	private void Clear()
    {
		if(this.Owner != null)
        {
			this.Owner.OnBuffAdd -= OnBuffAdd;
			this.Owner.OnBuffRemove -= OnBuffRemove;
        }

		foreach(var buff in this.buffs)
        {
			Destroy(buff.Value);
        }

		this.buffs.Clear();
    }

	private void OnBuffAdd(Buff buff)
    {
		GameObject go = Instantiate(prefabBuff, this.transform);
		go.name = buff.Define.ID.ToString();
		UIBuffItem bi = go.GetComponent<UIBuffItem>();
		bi.SetItem(buff);
		go.SetActive(true);
		this.buffs[buff.BuffId] = go;
    }

	private void OnBuffRemove(Buff buff)
    {
		GameObject go;

		if(this.buffs.TryGetValue(buff.BuffId, out go))
        {
			this.buffs.Remove(buff.BuffId);
			Destroy(go);
        }
    }
}

