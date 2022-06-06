using Managers;
using Services;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour 
{
    /* Function :  Manage game's starting */

    // open components to unity editor
    //public GameObject UIGameTip;
    public GameObject UILoading;
    public GameObject UILogin;

    public Slider ProcessBar;
    public Text ProcessText;
    public Text ProcessNumber;

    // Use this for initialization
    IEnumerator Start()
    {  
        /* init log format */
        log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
        UnityLogger.Init();
        Common.Log.Init("Unity");
        Common.Log.Info("LoadingManager start");

        /* the order in loading scene 
         * UI Tips  -> UI Loading -> init services
         * -> Loading Process Number ->  UI Login ->*/

        // UI loading
        //UIGameTip.SetActive(true);     // active game tips ui
        UILoading.SetActive(false); // close laoding ui
        UILogin.SetActive(false);   // close login ui
        //yield return new WaitForSeconds(2f); // wait 2 s

        UILoading.SetActive(true);          //  active loading ui
        yield return new WaitForSeconds(1f); // wait 2s

        //UIGameTip.SetActive(false);            // close gale tips ui

        // yield return DataManager.Instance.LoadData();
        StartCoroutine(DataManager.Instance.LoadData());

        //Init basic services
        MapService.Instance.Init();
        UserService.Instance.Init();
        TestManager.Instance.Init();
        StatusService.Instance.Init();
        ShopManager.Instance.Init();
        SoundManager.Instance.PlayMusic(SoundDefine.Music_Login);

        // simulate the loading process number
        for (float i = 0; i < 100;)
        {
            i += Random.Range(0.1f, 1.5f);
            ProcessBar.value = i;
            ProcessNumber.text = i.ToString();
            yield return new WaitForEndOfFrame();
        }

        UILoading.SetActive(false); // close loading ui
        UILogin.SetActive(true);    // active login
        yield return null;
    }


    // Update is called once per frame
    void Update () {

    }
}
