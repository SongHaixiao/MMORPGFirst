﻿using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using Managers;
using Models;

public class EntityController : MonoBehaviour, IEntityNotify, IEntityController
{
    /* Function:
     *  1.  接收服务器数据，通过接收到的数据来进行逻辑控制除了玩家本身角色实体外的其他角色实体；
     *  2.  与角色输入控制器类似，但不考虑输入功能. */

    public Animator anim;
    public Rigidbody rb;
    private AnimatorStateInfo currentBaseState;

    public Entity entity;

    public UnityEngine.Vector3 position;
    public UnityEngine.Vector3 direction;
    Quaternion rotation;

    public UnityEngine.Vector3 lastPosition;
    Quaternion lastRotation;

    public float speed;
    public float animSpeed = 1.5f;
    public float jumpPower = 3.0f;
    public bool isPlayer = false;

    public RideController rideController;
    public int currentRide = 0;
    public Transform rideBone;

    // Use this for initialization
    void Start()
    {
        if (entity != null)
        {
            EntityManager.Instance.RegiserEntityChangeNotity(entity.entityId, this);
            this.UpdateTransform();
        }

        if (!this.isPlayer)
            rb.useGravity = false;
    }

    void UpdateTransform()
    {
        this.position = GameObjectTool.LogicToWorld(entity.position);
        this.direction = GameObjectTool.LogicToWorld(entity.direction);

        this.rb.MovePosition(this.position);
        this.transform.forward = this.direction;
        this.lastPosition = this.position;
        this.lastRotation = this.rotation;
    }

    public void UpdateDirection()
    {
        this.direction = GameObjectTool.LogicToWorld(entity.direction);
        this.GetTransform.forward = this.direction;
        this.lastRotation = this.rotation;
    }

    void OnDestroy()
    {
        if (entity != null)
            Debug.LogFormat("{0} OnDestroy :ID:{1} POS:{2} DIR:{3} SPD:{4} ", this.name, entity.entityId, entity.position, entity.direction, entity.speed);

        // 因为所有游戏对象实体的操作都在 EntityConroller.cs 中进行，
        // 所以在实体控制器中的 OnDestroy（）中将 UINameBar 一起随之角色实体删除
        if (UIWorldElementManager.Instance != null)
        {
            UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.entity == null)
            return;

        this.entity.OnUpdate(Time.fixedDeltaTime);

        if (!this.isPlayer)
        {
            this.UpdateTransform();
        }
    }

    public void onEntityRemoved()
    {
        if (UIWorldElementManager.Instance != null)
            UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
        Destroy(this.gameObject);
    }

    public void OnEntityChanged(Entity entity)
    {
        Debug.LogFormat("OnEntityChanged : ID : {0} POS:{1} DIR : {2} SPD ： {3}", entity.entityId, entity.position, entity.direction, entity.speed);
    }

    public void OnEntityEvent(EntityEvent entityEvent, int param)
    {
        switch (entityEvent)
        {
            case EntityEvent.Idle:
                anim.SetBool("Move", false);
                anim.SetTrigger("Idle");
                break;
            case EntityEvent.MoveFwd:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.MoveBack:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.Jump:
                anim.SetTrigger("Jump");
                break;
            case EntityEvent.Ride:
                {
                    this.Ride(param);
                }
                break;
        }
        if(this.rideController != null) this.rideController.OnEntityEvent(entityEvent, param);
    }

    public void Ride(int rideId)
    {
        if(currentRide == rideId) return;
        currentRide = rideId;
        if(rideId > 0)
        {
            this.rideController = GameObjectManager.Instance.LoadRide(rideId, this.transform);    
        }
        else
        {
            Destroy(this.rideController.gameObject);
            this.rideController = null;
        }

        if(this.rideController == null)
        {
            this.anim.transform.localPosition = Vector3.zero;
            this.anim.SetLayerWeight(1,0);
        }
        else
        {
            this.rideController.SetRider(this);
            this.anim.SetLayerWeight(1,1);
        }
    }

    public void SetRidePosition(Vector3 position)
    {
        this.anim.transform.position = position + (this.anim.transform.position - this.rideBone.position);
    }

    void OnMouseDown()
    {
        Creature target = this.entity as Creature;
        if (target.IsCurrentPlayer)
            return;

        BattleManager.Instance.CurrentTarget = this.entity as Creature;
    }

    public void PlayAnim(string name)
    {
        this.anim.SetTrigger(name);
    }

    public void SetStandby(bool standby)
    {
        this.anim.SetBool("Standby", standby);
    }

    public void PlayEffect(EffectType type, string name, EntityController target, float duration)
    {
        Transform tarnsform = target.Controller.GetTransform();
        this.EffectMgr.PlayEffect(type, name, transform, duration);
        if(type == EffectType.Position || type == EffectType.Hit)
        {
            FXManager.Instance.PlayEffect(type, name, transform, target.GetHitOffset(), duration);
        }
        else
        {
            this.EffectMgr.PlayEffect(type, name, GetTransform, target.GetHitOffset(), duration);
        }
    }

    public void PlayEffect(EffectType type, string name, NVector3 position, float duration)
    {
        if (type == EffectType.Position || type == EffectType.Hit)
            FXManager.Instance.PlayEffect(type, name, null, GameObjectTool.LogicToWorld(position), duration);
        else
            this.EffectMgr.PlayEffect(type, name, null, GameObjectToll.LogicToWorld(position), duration);
    }

    public Transform GetTransform()
    {
        return this.transform;
    }

}

