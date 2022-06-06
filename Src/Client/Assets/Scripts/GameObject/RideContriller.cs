using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using Managers;

public class EntityController : MonoBehaviour, IEntityNotify
{
    public Transform mountPoint;
    public EntityController rider;
    public Vector3 offset;
    private Animator anim;

    // Use this for initialization
    void Start()
    {
        this.anim = this.GetComponent<Animator>();
    }

    void Update()
    {
        if(this.mountPint = null || this.rider == null) return;
        this.rider.SetRidePosition(this.mountPoint.position + this.mountPoint.TransformDirection(this.offset));
    }

    public void SetRider(EntityController rider)
    {
        this.rideController = rider;
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
        }
    }
}
