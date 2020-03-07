using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InsertionSortLanguageManager : MonoBehaviour
{
    public TextMeshProUGUI Speed, Insertion;

    // Start is called before the first frame update
    void Start()
    {
        SetLanguage();
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

        if (language == "ESPAÑOL")
        {
            Speed.text = "Velocidad";
            Insertion.text = "Inserción";
        }

        if (language == "ENGLISH")
        {
            Speed.text = "Speed";
            Insertion.text = "Insertion";
        }
    }
}
