using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    class MinimapManager : Singleton<MinimapManager>
    {
        /*Function : Upadte the bouding box of minimap afeter updating map.*/

        // open compoents
        public UIMinimap MiniMap;

        private Collider boudingBoxMiniMap;
        public Collider BoudingBoxMiniMap
        {
            get { return boudingBoxMiniMap; }

        }

        public Transform PlyaerTransform
        {
            get
            {
                if (User.Instance.CurrentCharacterObject == null)
                    return null;

                return User.Instance.CurrentCharacterObject.transform;
            }
        }

        // loading mini map resources
        public Sprite LoadCurrentMinimap()
        {
            return Resloader.Load<Sprite>("UI/Minimap/" + User.Instance.CurrentMapData.MiniMap);
        }

        // method to update Mini Map
        public void UpdateMinimap(Collider boudingBoxMiniMap)
        {
            this.boudingBoxMiniMap = boudingBoxMiniMap;
            if (this.MiniMap != null)
                this.MiniMap.UpdateMiniMap();
        }
    }
}
