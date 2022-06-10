using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entities;
using SkillBridge.Message;
using Services;

public class PlayerInputController : MonoBehaviour
{
    /* Function : Control the input of player*/

    // open componets
    public Rigidbody rb;
    SkillBridge.Message.CharacterState state;

    public Character character;

    public float rotateSpeed = 2.0f;

    public float turnAngle = 10;

    public int speed;

    public EntityController entityController;

    public bool onAir = false;

    // Use this for initialization
    void Start()
    {
        state = SkillBridge.Message.CharacterState.Idle;
        //if (this.character == null)
        //{
        //    DataManager.Instance.Load();
        //    NCharacterInfo cinfo = new NCharacterInfo();
        //    cinfo.Id = 1;
        //    cinfo.Name = "Test";
        //    cinfo.ConfigId = 1;
        //    cinfo.Entity = new NEntity();
        //    cinfo.Entity.Position = new NVector3();
        //    cinfo.Entity.Direction = new NVector3();
        //    cinfo.Entity.Direction.X = 0;
        //    cinfo.Entity.Direction.Y = 100;
        //    cinfo.Entity.Direction.Z = 0;
        //    cinfo.attrDynamic = new NAttributeDynamic();
        //    this.character = new Character(cinfo);

        //    if (entityController != null) entityController.entity = this.character;
        //}
    }


    // move update
    void FixedUpdate()
    {
        // if character object is unavailable， return   
        if (character == null)
            return;

        //if (InputManager.Instance != null && InputManager.Instance.IsInputMode) return;

        // get vertical varaible from untiy
        float v = Input.GetAxis("Vertical");

        // move forward
        if (v > 0.01)
        {
            // character object state setting
            if (state != SkillBridge.Message.CharacterState.Move)
            {
                // charactter object current state is not move

                state = SkillBridge.Message.CharacterState.Move; // set it as move state
                this.character.MoveForward();              //  character obejct move forward
                this.SendEntityEvent(EntityEvent.MoveFwd); //  update character state as move forward via unity evetn
            }

            // set speed to character object
            this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f;
        }

        // move backwards
        else if (v < -0.01)
        {
            if (state != SkillBridge.Message.CharacterState.Move)
            {
                state = SkillBridge.Message.CharacterState.Move;
                this.character.MoveBack();
                this.SendEntityEvent(EntityEvent.MoveBack);
            }
            this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f;
        }

        // stop moving
        else
        {
            if (state != SkillBridge.Message.CharacterState.Idle)
            {
                state = SkillBridge.Message.CharacterState.Idle;
                this.rb.velocity = Vector3.zero;
                this.character.Stop();
                this.SendEntityEvent(EntityEvent.Idle);
            }
        }

        // jump
        if (Input.GetButtonDown("Jump"))
        {
            this.SendEntityEvent(EntityEvent.Jump);
        }

        // move horizontal
        float h = Input.GetAxis("Horizontal");
        if (h < -0.1 || h > 0.1)
        {
            // rotate setting
            this.transform.Rotate(0, h * rotateSpeed, 0);

            // change character direction from logical coordinate to world coordinate
            Vector3 dir = GameObjectTool.LogicToWorld(character.direction);

            // Wheel Algorithm ( Importance )
            Quaternion rot = new Quaternion();
            rot.SetFromToRotation(dir, this.transform.forward);

            if (rot.eulerAngles.y > this.turnAngle && rot.eulerAngles.y < (360 - this.turnAngle))
            {
                character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
                rb.transform.forward = this.transform.forward;
                this.SendEntityEvent(EntityEvent.None);
            }

        }
        //Debug.LogFormat("velocity {0}", this.rb.velocity.magnitude);
    }

    // location synchronization after per frame
    Vector3 lastPos;
    float lastSync = 0;

    private void LateUpdate()
    {
        if (this.character == null) return;

        // calculate difference between current and last position
        Vector3 offset = this.rb.transform.position - lastPos;
        this.speed = (int)(offset.magnitude * 100f / Time.deltaTime);
        //Debug.LogFormat("LateUpdate velocity {0} : {1}", this.rb.velocity.magnitude, this.speed);
        this.lastPos = this.rb.transform.position;

        // sycnchronizate the object's position
        if ((GameObjectTool.WorldToLogic(this.rb.transform.position) - this.character.position).magnitude > 50)
        {
            this.character.SetPosition(GameObjectTool.WorldToLogic(this.rb.transform.position));
            this.SendEntityEvent(EntityEvent.None);
        }

        // change object positon to rige body position
        this.transform.position = this.rb.transform.position;
    }


    // update event for entity
    public void SendEntityEvent(EntityEvent entityEvent, int param = 0)
    {
        if (entityController != null)
            entityController.OnEntityEvent(entityEvent, param);

        MapService.Instance.SendMapEntitySync(entityEvent, this.character.EntityData, param);
    }
}
