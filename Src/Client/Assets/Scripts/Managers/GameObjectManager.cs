﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entities;
using Services;
using SkillBridge.Message;
using Models;
using Managers;

public class GameObjectManager : MonoBehaviour
{

    Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>();
    // Use this for initialization
    void Start()
    {
        StartCoroutine(InitGameObjects());
        CharacterManager.Instance.OnCharacterEnter = OnCharacterEnter;
    }

    private void OnDestroy()
    {
        CharacterManager.Instance.OnCharacterEnter = null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCharacterEnter(Character cha)
    {
        CreateCharacterObject(cha);
    }

    IEnumerator InitGameObjects()
    {
        foreach (var cha in CharacterManager.Instance.Characters.Values)
        {
            CreateCharacterObject(cha);
            yield return null;
        }
    }

    // 作用：角色进入的初始化逻辑
    private void CreateCharacterObject(Character character)
    {
        // No.1: 当Character ID 不存在时，或者 Character Object 被销毁时，进入 if 语句来创建角色；
        if (!Characters.ContainsKey(character.Info.Id) || Characters[character.Info.Id] == null)
        {
            // No.2 : 通过配置表，将角色资源加载出来，Resource 代表的角色资源路径；
            Object obj = Resloader.Load<Object>(character.Define.Resource);

            // No.3 : 防止加载出来的角色资源为 空 null；
            if (obj == null)
            {
                Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.",character.Define.TID, character.Define.Resource);
                return;
            }

            // No.4 : 创建游戏对象： 实例化一个 GameObject go， 并填充 GameObject go 的信息；
            GameObject go = (GameObject)Instantiate(obj);
            go.name = "Character_" + character.Info.Id + "_" + character.Info.Name;

            // No.5 : 设置位置：将 chracter 实体的逻辑坐标转化为游戏对象 go 的世界坐标，然后将游戏对 go 存到对应 ID 的游戏实体中；
            go.transform.position = GameObjectTool.LogicToWorld(character.position);
            go.transform.forward = GameObjectTool.LogicToWorld(character.direction);
            Characters[character.Info.Id] = go;

            // No.6 : 启用 EntityController 控制器组件 ：将角色对象 分别赋值给 entity
            EntityController ec = go.GetComponent<EntityController>();
            if (ec != null)
            {
                ec.entity = character;
                ec.isPlayer = character.IsPlayer;
            }

            // No.7 : 启用 PlayerInputController 控制器组件：
            PlayerInputController pc = go.GetComponent<PlayerInputController>();
            if (pc != null)
            {
                // P1 : 如果是玩家使用的角色，则将 PlayerInputController 控制器启动，并将 MainPlayerCamera 实例化;
                if (character.Info.Id == Models.User.Instance.CurrentCharacter.Id)
                {
                    User.Instance.CurrentCharacterObject = go;
                    MainPlayerCamera.Instance.player = go;
                    pc.enabled = true;
                    pc.character = character;
                    pc.entityController = ec;
                }

                // P2 : 如果是其他玩家的角色，则将 PlayerInputController 控制器禁用 .
                else
                {
                    pc.enabled = false;
                }
            }

            // 因为角色信息框 UINameBar 需要合角色一起创建，所以将 UINameBar 的创建添加到角色对象管理器中
            UIWorldElementManager.Instance.AddCharacterNameBar(go.transform, character);
        }
    }
}

