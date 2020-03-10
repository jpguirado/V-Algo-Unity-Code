using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public GameObject MenuConfiguracionEjecucion, MainMenu, MenuSettings, MenuAbout;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //On click Bubble
    public void OnClickBurbuja()
    {
        MainMenu.SetActive(false);
        MenuConfiguracionEjecucion.SetActive(true);

        MenuConfiguracionEjecucion.GetComponent<MenuConfiguracion>().AlgoritmoAEjecutar = "Burbuja";
        MenuConfiguracion.NumElementos = 10;
        MenuConfiguracionEjecucion.GetComponent<MenuConfiguracion>().MaxNumElementos = 20;
        MenuConfiguracionEjecucion.GetComponent<MenuConfiguracion>().MinNumElementos= 10;
    }
    //On click merge sort
    public void OnClickMergeSort()
    {
        MainMenu.SetActive(false);
        MenuConfiguracionEjecucion.SetActive(true);

        MenuConfiguracionEjecucion.GetComponent<MenuConfiguracion>().AlgoritmoAEjecutar = "MergeSort";
        MenuConfiguracion.NumElementos = 10;
        MenuConfiguracionEjecucion.GetComponent<MenuConfiguracion>().MaxNumElementos = 15;
        MenuConfiguracionEjecucion.GetComponent<MenuConfiguracion>().MinNumElementos = 10;
    }

    //On Click Quick Sort
    public void OnClickQuickSort()
    {
        MainMenu.SetActive(false);
        MenuConfiguracionEjecucion.SetActive(true);

        MenuConfiguracionEjecucion.GetComponent<MenuConfiguracion>().AlgoritmoAEjecutar = "QuickSort";
        MenuConfiguracion.NumElementos = 10;
        MenuConfiguracionEjecucion.GetComponent<MenuConfiguracion>().MaxNumElementos = 20;
        MenuConfiguracionEjecucion.GetComponent<MenuConfiguracion>().MinNumElementos = 10;
    }

    //On Click Selection
    public void OnClickSelectionSort()
    {
        MainMenu.SetActive(false);
        MenuConfiguracionEjecucion.SetActive(true);

        MenuConfiguracionEjecucion.GetComponent<MenuConfiguracion>().AlgoritmoAEjecutar = "SelectionSort";
        MenuConfiguracion.NumElementos = 10;
        MenuConfiguracionEjecucion.GetComponent<MenuConfiguracion>().MaxNumElementos = 20;
        MenuConfiguracionEjecucion.GetComponent<MenuConfiguracion>().MinNumElementos = 10;
    }

    //On Click Insertion
    public void OnClickInsertionSort()
    {
        MainMenu.SetActive(false);
        MenuConfiguracionEjecucion.SetActive(true);

        MenuConfiguracionEjecucion.GetComponent<MenuConfiguracion>().AlgoritmoAEjecutar = "InsertionSort";
        MenuConfiguracion.NumElementos = 10;
        MenuConfiguracionEjecucion.GetComponent<MenuConfiguracion>().MaxNumElementos = 20;
        MenuConfiguracionEjecucion.GetComponent<MenuConfiguracion>().MinNumElementos = 10;
    }

    //On Click Button Setting
    public void OnClickSettings()
    {
        MainMenu.SetActive(false);
        MenuSettings.SetActive(true);
    }

    //On Click Button About
    public void OnClickAbout()
    {
        MainMenu.SetActive(false);
        MenuAbout.SetActive(true);
    }

    //Carga la escena del menu
    public void CargarMenu()
    {
        MenuConfiguracionEjecucion.GetComponent<MenuConfiguracion>().AlgoritmoAEjecutar = "Menu";
    }

    //Volver al menu
    public void VolverMenu(int menuProcedencia)
    {
        switch (menuProcedencia)
        {
            //Execution Config Menu
            case 0:
                MenuConfiguracionEjecucion.SetActive(false);
                break;
            //Setting Menu
            case 1:
                MenuSettings.SetActive(false);
                break;
            //About menu
            case 2:
                MenuAbout.SetActive(false);
                break;
        }
        MainMenu.SetActive(true);
    }
}
