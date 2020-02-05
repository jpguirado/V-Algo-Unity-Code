using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public GameObject MenuConfiguracionEjecucion, MainMenu;

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
        }
        MainMenu.SetActive(true);
    }
}
