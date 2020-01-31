using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase para almacenar los estados necesarios para controlar la reproducción del código
public class State
{
    //Determina si se esta ejecutando el if o el swap
    //0 para el if, 1 para el swap
    public int Estado { get; set; }

    //Elemento izquierda
    public int IndiceElementoIzq { get; set; }

    //Elemento derecha
    public int IndiceElementoDcha { get; set; }

}
