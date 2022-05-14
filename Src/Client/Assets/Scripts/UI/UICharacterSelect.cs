using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Services;
using SkillBridge.Message;
public class UICharacterSelect : MonoBehaviour 
{
    // open components
    public GameObject PanelCreation;
    public GameObject PanelSelection;

    public InputField charName;
    CharacterClass charClass;

    public Transform uiCharList;
    public GameObject uiCharInfo;

    public List<GameObject> uiChars = new List<GameObject>();

    public Image[] titles;

    public Text descs;


    public Text[] names;

    private int selectCharacterIdx = -1;

    public UICharacterView characterView;

    // Use this for initialization
    void Start()
    {
        // when Character Selection Scene is loaded,
        // this part will firstly run

        DataManager.Instance.Load();    // load data
        InitCharacterSelect(true);      // active character select panel
        UserService.Instance.OnCharacterCreate = OnCharacterCreate; // listener for character create
                                                                    // from logic layer ( UserService.cs )
    }

	// initialization for character create pannel
    public void InitCharacterCreate()
    {
        // active character creation panle,
        // and close character selection panle
        PanelCreation.SetActive(true);
        PanelSelection.SetActive(false);
        OnSelectClass(1);   // defaultly select job whose class is 1,
                            // which is Warrior
    }

// Update is called once per frame
	void Update () {
		
	}

    // click create button
    public void OnClickCreate()
    {
        // check the character name inputed whether is available

        // character name inputed is unavailable
        if (string.IsNullOrEmpty(this.charName.text))
        {
            MessageBox.Show("Please input user name !");
            return;
        }

        else
        {
            // character name inputed is available
            // send character creation info to logic layer
            UserService.Instance.SendCharacterCreate(this.charName.text, this.charClass);

            InitCharacterSelect(true);
        }
    }
	
    // initialization for character select pannel
    public void InitCharacterSelect(bool init)
    {
        PanelCreation.SetActive(false); // close character creation panel
        PanelSelection.SetActive(true); // active character selection panle

        // if is initialization state
        if (init)
        {
            // travel characters existed and destroy them
            foreach (var old in uiChars)
            {
                Destroy(old);
            }

            // clear selection list
            uiChars.Clear();
			
            // initialize the characters when character selection panel is actived
			 for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
            {
                // add UICharInfo component
                GameObject go = Instantiate(uiCharInfo, this.uiCharList);
                UICharInfo chrInfo = go.GetComponent<UICharInfo>();
                chrInfo.info = User.Instance.Info.Player.Characters[i];

                // button listener for hight light 
                Button button = go.GetComponent<Button>();
                int idx = i;
                button.onClick.AddListener(() => {
                    OnSelectCharacter(idx);
                });

                uiChars.Add(go);
                go.SetActive(true);
            }
        }
    }


    // select job class
    public void OnSelectClass(int charClass)
    {
        // change int type to CharacterClass
        this.charClass = (CharacterClass)charClass;

        // give character view ui the id of slected character now
        characterView.CurrentCharacter = charClass - 1;

        // travel 3 characters' models, loade model's inforation
        for (int i = 0; i < 3; i++)
        {
            // load title & name components
            titles[i].gameObject.SetActive(i == charClass - 1);
            names[i].text = DataManager.Instance.Characters[i + 1].Name;
        }

        // update character description contents from DB
        descs.text = DataManager.Instance.Characters[charClass].Description;

    }

    
    // operation for character create event from logic layer
    // ( UserService.cs )
    void OnCharacterCreate(Result result, string message)
    {
        // character create successed, initializa the character selction
        if (result == Result.Success)
        {
            InitCharacterSelect(true);

        }

        // character create failed, print error message
        else
            MessageBox.Show(message, "Error!", MessageBoxType.Error);
    }

    public void OnSelectCharacter(int idx)
    {
        this.selectCharacterIdx = idx;
        var cha = User.Instance.Info.Player.Characters[idx];
        Debug.LogFormat("Select Char:[{0}]{1}[{2}]", cha.Id, cha.Name, cha.Class);
        User.Instance.CurrentCharacter = cha;
        characterView.CurrentCharacter = ((int)cha.Class - 1);

        // active character option's high light, and cancel actived high light
        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            
            // get ui chars's hight light component
            UICharInfo ci = this.uiChars[i].GetComponent<UICharInfo>();
            
            // when click character option, high light will be active
            ci.Selected = idx == i;
        }
    }
    
    // click ok button to loade Main City Scene,
    // and then start game
    public void OnClickPlay()
    {
        if (selectCharacterIdx >= 0)
        {
            UserService.Instance.SendGameEnter(selectCharacterIdx);
        }
    }
}
