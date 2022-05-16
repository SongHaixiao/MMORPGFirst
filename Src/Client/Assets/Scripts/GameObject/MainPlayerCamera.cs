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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void LateUpdate()
    {
        if(player == null)
        {
            player = User.Instance.CurrentCharacterObject;
        }

        // 如果玩家角色不为空 null，则将玩家位置赋值给相机位置，即使相机跟随玩家
        if (player == null)
           return;
           
        this.transform.position = player.transform.position;
        this.transform.rotation = player.transform.rotation;
    }
}
