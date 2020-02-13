using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MergeSortLanguageManager : MonoBehaviour
{
    public TextMeshProUGUI Speed;

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

        if (language == "ESPAÑOL")
        {
            Speed.text = "Velocidad";
        }

        if (language == "ENGLISH")
        {
            Speed.text = "Speed";
        }
    }
}
