using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    /* Function : control the UI Element*/

    // element class
    class UIElement
    {
        public string Resources;
        public bool Cache;
        public GameObject Instance;
    }

    // use dictionary to store the ui resources
    private Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>();

    // add ui resources in constructor
    public UIManager()
    {
        // ad UI Element Prefab in Assets/Resources/UI
        // Cache = false : replacement the UI Element when reopen this UI Element for test easily

        this.UIResources.Add(typeof(UISetting), new UIElement() { Resources = "UI/UISetting", Cache = true });
        this.UIResources.Add(typeof(UIBag), new UIElement() { Resources = "UI/UIBag", Cache = false });
        this.UIResources.Add(typeof(UIShop), new UIElement() { Resources = "UI/UIShop", Cache = false });
        this.UIResources.Add(typeof(UICharEquip), new UIElement() { Resources = "UI/UICharEquip", Cache = false });
        this.UIResources.Add(typeof(UIQuestSystem), new UIElement() { Resources = "UI/UIQuestSystem", Cache = false });
        this.UIResources.Add(typeof(UIQuestDialog), new UIElement() { Resources = "UI/UIQuestDialog", Cache = false });
        //this.UIResources.Add(typeof(UIRide), new UIElement() { Resources = "UI/UIRide", Cache = false });
        this.UIResources.Add(typeof(UISystemConfig), new UIElement() { Resources = "UI/UISystemConfig", Cache = false });

    }

    // 
    ~UIManager()
    {

    }

    ///<summary>
    ///Show UI
    ///</summary>
    public T Show<T>()
    {
        // load sound ui
        SoundManager.Instance.PlaySound("SoundDefine.SFX_UI_Win_Open");

        // define a type
        Type type = typeof(T);

        // this type is in UIResources
        if (this.UIResources.ContainsKey(type))
        {
            // get the UIElement from UIResources
            UIElement info = this.UIResources[type];

            // info UI Element Instance is unavailable
            if (info.Instance != null)
            {
                // active it
                info.Instance.SetActive(true);
            }

            // info UI Element Instance is available
            else
            {
                //get info UI Element prefab
                UnityEngine.Object prefab = Resources.Load(info.Resources);

                // prefab UI Element prefab is unavailable
                if (prefab == null)
                {
                    // return default type
                    return default(T);
                }

                // prefab UI Element prefab is avaiable,
                // instantiate this prefab
                info.Instance = (GameObject)GameObject.Instantiate(prefab);
            }

            // type UI Elemnt is in UIResourcs,
            // returen type prefab instanced in info
            return info.Instance.GetComponent<T>();
        }

        // type UI Elemnt is not in UIResources,
        // return default type
        return default(T);
    }

    // close UI Elemnt
    public void Close(Type type)
    {
        // play closed sound
        // SoundManger.Instance.PlaySound("ui_open");

        // type UI Element is in UIResources
        if (this.UIResources.ContainsKey(type))
        {
            // get type UI Element in UIResources
            UIElement info = this.UIResources[type];

            // info UI Elemnt data is in Cache
            if (info.Cache)
            {
                // close info UI Element
                info.Instance.SetActive(false);
            }

            // info UI Element data is not in Cache
            else
            {
                // destroy info UI Element prefabs
                // and set it to null
                GameObject.Destroy(info.Instance);
                info.Instance = null;
            }
        }
    }

    public void Close<T>()
    {
        this.Close(typeof(T));
    }
}
