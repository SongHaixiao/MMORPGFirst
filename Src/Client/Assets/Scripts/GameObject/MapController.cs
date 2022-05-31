using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    /* Function : call he Mini Map Manger to start*/
    public Collider BoudingBoxMiniMap;

    // Start is called before the first frame update
    void Start()
    {
        MinimapManager.Instance.UpdateMinimap(BoudingBoxMiniMap);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
