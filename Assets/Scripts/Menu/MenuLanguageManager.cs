using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuLanguageManager : MonoBehaviour
{

    public TextMeshProUGUI Bubble, NumberElements, Execute, ReturnConfigExecution, Language, ReturnSettings, Selection, Insertion, ReturnAbout;

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
            NumberElements.text = "Nº Elementos";
            Execute.text = "EJECUTAR";
            ReturnConfigExecution.text = ReturnSettings.text = ReturnAbout.text = "VOLVER";
            Language.text = "IDIOMA";
            Selection.text = "SELECCIÓN";
            Insertion.text = "INSERCIÓN";
        }

        if (language == "ENGLISH")
        {
            Bubble.text = "BUBBLE";
            NumberElements.text = "Nº Elements";
            Execute.text = "EXECUTE";
            ReturnConfigExecution.text = ReturnSettings.text = ReturnAbout.text = "RETURN";
            Language.text = "LANGUAGE";
            Selection.text = "SELECTION";
            Insertion.text = "INSERTION";
        }
    }
}
