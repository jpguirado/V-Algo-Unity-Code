using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuickSortLanguageManager : MonoBehaviour
{
    public TextMeshProUGUI Speed, Pivot, LeftHalf, RightHalf, Sorted;

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
            Pivot.text = "Pivote";
            LeftHalf.text = "Mitad izq";
            RightHalf.text = "Mitad dcha";
            Sorted.text = "Ordenado";
        }

        if (language == "ENGLISH")
        {
            Speed.text = "Speed";
            Pivot.text = "Pivot";
            LeftHalf.text = "Left Half";
            RightHalf.text = "Right Half";
            Sorted.text = "Sorted";
        }
    }
}
