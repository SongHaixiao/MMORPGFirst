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
    private Transform PlayerTransform;

    // Start is called before the first frame update
    void Start()
    {
        // tell MinimapManager this map's Mini Map
        MinimapManager.Instance.MiniMap = this;

        // initialize mini map
        this.UpdateMiniMap();
    }

    // method for initializing mini map
    public void UpdateMiniMap()
    {
        // loade current map's name to minimap
        this.NameMiniMap.text = User.Instance.CurrentMapData.Name;

        // load mini map resouce to override mini map
        this.ImageMiniMap.overrideSprite = MinimapManager.Instance.LoadCurrentMinimap();

        // set mini map as native size
        this.ImageMiniMap.SetNativeSize();

        // set local postion as origin potin
        this.ImageMiniMap.transform.localPosition = Vector3.zero;

        // get tthe bouding box of minimap from MiniMapManager
        this.BoudingBoxMiniMap = MinimapManager.Instance.BoudingBoxMiniMap;

        // clear PlayerTransform in order to execute the PlayerTransfrom 
        // in Update() method
        this.PlayerTransform = null;
    }

    // Update is called once per frame
    void Update()
    {
        // update character postion and arrow direction
        // in minimap per frame

        if(this.PlayerTransform == null)
        {
            // get transform position data of current character
            this.PlayerTransform = MinimapManager.Instance.PlyaerTransform;
        }

        if (this.BoudingBoxMiniMap == null || this.PlayerTransform == null) return;

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
