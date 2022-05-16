using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMinimap : MonoBehaviour
{
    // open components
    public Image ImageMiniMap;
    public Image ArrowMiniMap;
    public Text NameMiniMap;
    public Collider BoudingBoxMiniMap;

    // define private varaibles
    public Transform PlayerTransform;

    // Start is called before the first frame update
    void Start()
    {
        // initialize mini map
        this.InitMiniMap();
    }

    // method for initializing mini map
    void InitMiniMap()
    {
        // loade current map's name to minimap
        this.NameMiniMap.text = User.Instance.CurrentMapData.Name;

        // if mini map is not override,
        // load mini map resouce to override mini map
        if (this.ImageMiniMap.overrideSprite == null)
            this.ImageMiniMap.overrideSprite = MinimapManager.Instance.LoadCurrentMinimap();

        // set mini map as native size
        this.ImageMiniMap.SetNativeSize();

        // set local postion as origin potin
        this.ImageMiniMap.transform.localPosition = Vector3.zero;

        // get transform position data of current character
        //this.ownerTransform = User.Instance.CurrentCharacterObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // update character postion and arrow direction
        // in minimap per frame

        // get real width & height
        float realWidth = this.BoudingBoxMiniMap.bounds.size.x;
        float realHeight = this.BoudingBoxMiniMap.bounds.size.z;

        // get real x & y
        float realX = this.PlayerTransform.position.x - this.BoudingBoxMiniMap.bounds.min.x;
        float realY = this.PlayerTransform.position.z - this.BoudingBoxMiniMap.bounds.min.z;

        // calculate center point : X & Y
        float pivotX = realX / realWidth;
        float pivotY = realY / realHeight;

        // set backgound image of mini map's center point as pivotX & Y
        this.ImageMiniMap.rectTransform.pivot = new Vector2(pivotX, pivotY);

        // set backgornd image of mini map's local position as (0,0)
        this.ImageMiniMap.rectTransform.localPosition = Vector2.zero;

        // set direction of arrow as play's towards direction
        this.ArrowMiniMap.transform.eulerAngles = new Vector3(0, 0, -PlayerTransform.eulerAngles.y);
    }
}
