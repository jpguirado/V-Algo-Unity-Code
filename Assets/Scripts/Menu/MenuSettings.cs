using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class MenuSettings : MonoBehaviour
{
    public TextMeshProUGUI TextoLenguaje;

    private string[] languages = new string[] {"ESPAÑOL", "ENGLISH"};

    public string language;

    public MenuLanguageManager LanguageManager;

    // Start is called before the first frame update
    void Start()
    {
        language = PlayerPrefs.GetString("language");

        //There is no language established
        if(language == "")
        {
            PlayerPrefs.SetString("language", "ENGLISH");
            language = "ENGLISH";
        }

        TextoLenguaje.text = language;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IdiomaIzq()
    {
        int index = Array.IndexOf(languages, language);
        
        if(index == 0)
        {
            TextoLenguaje.text = languages[languages.Length - 1];
            language = TextoLenguaje.text;
            PlayerPrefs.SetString("language", TextoLenguaje.text);
        }
        else
        {
            index--;
            TextoLenguaje.text = languages[index];
            language = TextoLenguaje.text;
            PlayerPrefs.SetString("language", TextoLenguaje.text);
        }

        LanguageManager.SetLanguage();
    }

    public void IdiomaDcha()
    {
        int index = Array.IndexOf(languages, language);

        if (index == languages.Length - 1)
        {
            TextoLenguaje.text = languages[0];
            language = TextoLenguaje.text;
            PlayerPrefs.SetString("language", TextoLenguaje.text);
        }
        else
        {
            index++;
            TextoLenguaje.text = languages[index];
            language = TextoLenguaje.text;
            PlayerPrefs.SetString("language", TextoLenguaje.text);
        }

        LanguageManager.SetLanguage();
    }
}
