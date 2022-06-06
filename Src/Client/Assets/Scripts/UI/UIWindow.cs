using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIWindow : MonoBehaviour
{
    /* Function : UI Parent Class, apply to all other UIs,
     * that means all other UIs are inherited from UIWindow CLass.*/

    public delegate void CloseHandler(UIWindow sender, WindowResult result);
    
    // close window event
    public event CloseHandler OnClose;

    // get type attribute
    public virtual System.Type Type { get { return this.GetType(); } }

    public GameObject Root;
    
    // result type define
    public enum WindowResult
    {
        None = 0,
        Yes,
        No,
    }

    // method to close UI
    public void Close(WindowResult result = WindowResult.None)
    {
        // send this UI type to UIManager
        UIManager.Instance.Close(this.Type);

        // OnClose event is availables.
        // call OnClose event to close the UIs
        if (this.OnClose != null)
            this.OnClose(this, result);

        // after close the ui, 
        // set OnCloce() event as null
        this.OnClose = null;
    }

    // click close button
    public virtual void OnCloseClick()
    {
        this.Close();
    }

    // click yes button
    public virtual void OnYesClick()
    {
        this.Close(WindowResult.Yes);
    }

    // check mouse whether is clicked
    void OnMouseDown()
    {
        Debug.LogFormat(this.name + " Clicked !");
    }
}
