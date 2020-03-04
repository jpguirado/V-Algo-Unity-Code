using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectionSortLanguageManager : MonoBehaviour
{
    public TextMeshProUGUI Speed, Sorted, Selection;

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
            Sorted.text = "Ordenado";
            Selection.text = "Selección";
        }

        if (language == "ENGLISH")
        {
            Speed.text = "Speed";
            Sorted.text = "Sorted";
            Selection.text = "Selection";
        }
    }
}
