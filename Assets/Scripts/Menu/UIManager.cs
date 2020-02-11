using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public GameObject MenuConfiguracionEjecucion, MainMenu, MenuSettings;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Click en el boton de algoritmo de la burbuja
    public void OnClickBurbuja()
    {
        MainMenu.SetActive(false);
        MenuConfiguracionEjecucion.SetActive(true);

        MenuConfiguracionEjecucion.GetComponent<MenuConfiguracion>().AlgoritmoAEjecutar = "Burbuja";
    }
    //Click en el boton de algoritmo de la burbuja
    public void OnClickMergeSort()
    {
        MainMenu.SetActive(false);
        MenuConfiguracionEjecucion.SetActive(true);

        MenuConfiguracionEjecucion.GetComponent<MenuConfiguracion>().AlgoritmoAEjecutar = "MergeSort";
    }

    //Click en el boton de algoritmo de la burbuja
    public void OnClickSettings()
    {
        MainMenu.SetActive(false);
        MenuSettings.SetActive(true);
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
            //Procedemos del menú de configuracion de ejecucion
            case 0:
                MenuConfiguracionEjecucion.SetActive(false);
                break;
            case 1:
                MenuSettings.SetActive(false);
                break;
        }
        MainMenu.SetActive(true);
    }
}
