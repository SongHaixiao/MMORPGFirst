using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    class MinimapManager : Singleton<MinimapManager>
    {
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
    }
}
