using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterView : MonoBehaviour
{

    public GameObject[] characters;

    // current character attribution :
    // character id from unity
    private int currentCharacter = 0;
    public int CurrentCharacter
    {
        get
        {
            return currentCharacter;
        }

        set
        {
            currentCharacter = value;
            this.UpdateCharacter();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // active character models
    void UpdateCharacter()
    {
        // travel 3 models
        for(int i = 0; i < 3; i++)
        {
            // active selected model
            characters[i].SetActive(i == this.currentCharacter);
        }
    }
}
