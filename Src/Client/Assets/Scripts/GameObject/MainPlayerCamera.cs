using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    /* Functions : 使摄像机跟随 玩家角色 */

    public Camera camera;

    public Transform viewPoint;

    public GameObject player;

    public float followSpeed = 5f;

    public float rotateSpeed = 5f;

    Quaternion yaw = Quaternion.identity;

    private void Update()
    {

    }

    private void LateUpdate()
    {
        if(player == null && User.Instance.CurrentCharacterObject != null)
        {
            player = User.Instance.CurrentCharacterObject.gameObject;
        }

        // 如果玩家角色不为空 null，则将玩家位置赋值给相机位置，即使相机跟随玩家
        if (player == null)
           return;

        this.transform.position = Vector3.Lerp(this.transform.position, player.transform.position, Time.deltaTime * followSpeed);

        if(Input.GetMouseButton(1))
        {
            Vector3 angleBase = this.transform.localRotation.eulerAngles;
            this.transform.localRotation = Quaternion.Euler(angleBase.x - Input.GetAxis("Mouse Y") * rotateSpeed, angleBase.y + Input.GetAxis("Mouse X") * rotateSpeed, 0);
            Vector3 angle = this.transform.rotation.eulerAngles - player.transform.rotation.eulerAngles;
            angle.z = 0;
            yaw = Quaternion.Euler(angle);
        }
        else
        {
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, player.transform.rotation * yaw, Time.deltaTime * followSpeed);
        }

        if(Input.GetAxis("Vertical") > 0.01)
        {
            yaw = Quaternion.Lerp(yaw, Quaternion.identity, Time.deltaTime * followSpeed);
        }
    }
}
