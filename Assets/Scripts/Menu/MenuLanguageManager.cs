using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuLanguageManager : MonoBehaviour
{

    public TextMeshProUGUI Bubble, Settings, NumberElements, Execute, ReturnConfigExecution, Language, ReturnSettings;

    // Start is called before the first frame update
    void Start()
    {
        SetLanguage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLanguage()
    {
        string language = PlayerPrefs.GetString("language");

        //There is no language established
        if (language == "")
        {
            PlayerPrefs.SetString("language", "ENGLISH");
            language = "ENGLISH";
        }

        if(language == "ESPAÑOL")
        {
            Bubble.text = "BURBUJA";
            Settings.text = "AJUSTES";
            NumberElements.text = "Nº Elementos";
            Execute.text = "EJECUTAR";
            ReturnConfigExecution.text = ReturnSettings.text = "VOLVER";
            Language.text = "IDIOMA";
        }

        if (language == "ENGLISH")
        {
            Bubble.text = "BUBBLE";
            Settings.text = "SETTINGS";
            NumberElements.text = "Nº Elements";
            Execute.text = "EXECUTE";
            ReturnConfigExecution.text = ReturnSettings.text = "RETURN";
            Language.text = "LANGUAGE";
        }

    }
}
