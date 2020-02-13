using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuConfiguracion : MonoBehaviour
{
    public static int NumElementos;
    public string AlgoritmoAEjecutar;
    public int MinNumElementos;
    public int MaxNumElementos;

    public TextMeshProUGUI TextoNumElementos;
    void Start()
    {
        NumElementos = MinNumElementos;
    }

    // Update is called once per frame
    void Update()
    {
        TextoNumElementos.text = NumElementos.ToString();
    }

    //Sumar elementos
    public void SumarElementos()
    {
        if(NumElementos < MaxNumElementos)
        {
            NumElementos++;
            TextoNumElementos.text = NumElementos.ToString();
        }
    }

    //Restar elementos
    public void RestarElementos()
    {
        if (NumElementos > MinNumElementos)
        {
            NumElementos--;
            TextoNumElementos.text = NumElementos.ToString();
        }
    }

    public void Ejecutar()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(AlgoritmoAEjecutar);
    }
}
