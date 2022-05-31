using Common.Data;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterObject : MonoBehaviour
{
    /* Fucntion : control the teleporter objects*/


    public int ID;              // teleporter's id
    private Mesh mesh = null;  //  teleporter's Mesh component
    
    // Start is called before the first frame update
    void Start()
    {
        this.mesh = this.GetComponent<MeshFilter>().sharedMesh;
        
    }

    // Update is called once per frame
    void Update()
    {
    
    }

   

    // extension unity editor
#if UNITY_EDITOR

    // draw mesh in setting format
    private void OnDrawGizmos()
    {
        // set mesh as green color
        Gizmos.color = Color.green;
        if(this.mesh != null)
        {
            // draw mesh outline
            //Gizmos.DrawWireMesh(this.mesh, this.transform.position + Vector3.up * this.transform.localScale.y * .5f,
            //                        this.transform.rotation, this.transform.localScale);

            Gizmos.DrawWireMesh(this.mesh, this.transform.position,
                                   this.transform.rotation, this.transform.localScale);
        }

        // add a arrow in mesh to tell the direction of this object
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.ArrowHandleCap(0, this.transform.position, this.transform.rotation, 1f, EventType.Repaint);
    }
#endif


    // Trigger Event
    // when character touch teleporters, then this method will be triggered
    private void OnTriggerEnter(Collider other)
    {
        Debug.LogFormat("Teleporter is triggered!");

        // get the player input controller component
        PlayerInputController pc = other.GetComponent<PlayerInputController>();

        // pc is availabl
        if (pc != null && pc.isActiveAndEnabled)
        {
            // get teleporter data from db
            TeleporterDefine td = DataManager.Instance.Teleporters[this.ID];

            // td is unavailable
            if (td == null)
            {
                Debug.LogErrorFormat("TeleporterObject : Character [{0}] Enter Teleporter [{1}], But TeleporterDefine not existed !",
                                        pc.character.Info.Name, this.ID);
                return;
            }

            // td is avaiable
            Debug.LogFormat("TeleporterObject : Character [{0}] Enter Teleporter [{1}:{2}]", pc.character.Info.Name, td.ID, td.Name);

            // link to is available
            if (td.LinkTo > 0)
            {
                // LinkTo is existed in DB, send map telepoter id 
                if (DataManager.Instance.Teleporters.ContainsKey(td.LinkTo))
                    MapService.Instance.SendMapTeleport(this.ID);
                else
                    Debug.LogErrorFormat("Teleporter ID : {0} LinkID {1} Error !", td.ID, td.LinkTo);
            }
        }
    }
}
