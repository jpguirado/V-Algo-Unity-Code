using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Bubble : MonoBehaviour
{
    //Numero de elementos que tendrá nuestro array, configurable desde editor
    public int numElementos;

    //Array a ordenar
    int[] array;

    //Tiempo de parada entre cada paso de la ejecución.
    public float segundosParada;

    //Game Object que hace referencia al prefab del elemento gráfico del array
    public GameObject ElementoGraficoArray;

    //Lista con los GameObject que hacen referencia a la representación gráfica de cada uno de los elementos del array
    public List<GameObject> ArrayListGraphic;

    //Determina si la ejecución esta parada o no
    public bool pausado;

    //Lista de estados para controlar la ejecución paso por paso
    public List<State> listaEstados;

    //Indica el paso en el que nos encontramos
    public int ContadorPaso;

    //Indica si se esta realizando movimiento entre elementos
    private bool parado;

    //Indica si hay que realizar un paso hacia delante
    private bool StepOver;

    //Boton de stepOver
    public Button BotonStepOver;

    //Boton de StepOver
    public Button BotonStepBack;

    //Indica si esta ejecutando un paso
    private bool ejecutandoPaso;

    public Text Code;

    //Dice si esta en play o pausa
    public Text EstadoEjecucion;

    // Start is called before the first frame update
    void Start()
    {
        parado = true;
        ContadorPaso = 0;
        listaEstados = new List<State>();
        ejecutandoPaso = false;

        array = CreateRandomArray(numElementos);
        ExecuteAndCreateStates(array);
        InstantiateGraphicArray(array);
        //StartCoroutine(debug());
    }

    private void ExecuteAndCreateStates(int[] array)
    {
        int[] arrayAux = (int[])array.Clone();
        int aux;
        for (int i = 1; i < arrayAux.Length; i++)
            for (int p = arrayAux.Length - 1; p >= i; p--)
            {
                //Crear estado de if
                listaEstados.Add(new State()
                {
                    Estado = 0,
                    IndiceElementoIzq = p - 1,
                    IndiceElementoDcha = p
                });
                if (arrayAux[p - 1] > arrayAux[p])
                {
                    //Crear estado de swap
                    listaEstados.Add(new State()
                    {
                        Estado = 1,
                        IndiceElementoIzq = p - 1,
                        IndiceElementoDcha = p
                    });
                    aux = arrayAux[p - 1];
                    arrayAux[p - 1] = arrayAux[p];
                    arrayAux[p] = aux;
                }
            }
    }

    private void InstantiateGraphicArray(int[] array)
    {
        //Necesitamos obtener el ancho del canvas
        Vector2 CanvasResolution = this.GetComponent<CanvasScaler>().referenceResolution;

        //Distancia entre los elementos del array en funcion del ancho del canvas y el número de elementos
        float distanceBetweenElements = (CanvasResolution.x - (CanvasResolution.x / 10)) / (array.Length - 1);

        for (int i = 0; i<array.Length; i++)
        {
            //Instanciamos, cambiamos nombre, añadimos a la lista de GameObject, ponemos como padre el canvas y establecemos su posición.
            GameObject instanciado = Instantiate(ElementoGraficoArray);
            instanciado.name = array[i].ToString();
            ArrayListGraphic.Add(instanciado);
            instanciado.transform.SetParent(this.transform);
            Vector3 posicion = instanciado.GetComponent<RectTransform>().localPosition = new Vector3(distanceBetweenElements*i - (CanvasResolution.x/2) + (CanvasResolution.x/2/10), 0.0f, 0.0f);

            //Establecer el número correspondiente
            instanciado.GetComponentInChildren<Text>().text = array[i].ToString();

            //Cambiar la altura de la barra a un tamaño proporcional a su número
            RectTransform tamañoBarra = instanciado.transform.Find("Barra").GetComponent<RectTransform>();
            tamañoBarra.sizeDelta = new Vector2(40, array[i] * (90 / (array.Length - 1)) + 10);

            //Ajustar la altura de la barra debido a que el cambio de tamaño lo hace hacia arriba y abajo
            tamañoBarra.localPosition = new Vector3(tamañoBarra.localPosition.x, tamañoBarra.localPosition.y - ((100 - tamañoBarra.sizeDelta.y)/2), tamañoBarra.localPosition.z);
        }
    }

    //Función para crear un array aleatorio a partir del número de elementos dado
    private int[] CreateRandomArray(int numElementos)
    {
        //Creamos lista ordenada
        List<int> listaOrdenada = new List<int>();
        for (int i = 0; i < numElementos; i++)
            listaOrdenada.Add(i);

        //Desordenamos
        for(int i = numElementos - 1; i>0; i--)
        {
            int indiceAleatorio = Random.Range(0, i + 1);
            int aux = listaOrdenada[i];
            listaOrdenada[i] = listaOrdenada[indiceAleatorio];
            listaOrdenada[indiceAleatorio] = aux;
        }

        //Devolvemos como array
        return listaOrdenada.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject auxGameObject;
        

        if (!pausado && parado && ContadorPaso <= (listaEstados.Count - 1))
        {
            limpiarResalteCodigo();

            //Si el estado es un swap de elementos
            if (listaEstados[ContadorPaso].Estado == 1)
            {
                //Movemos los elementos
                StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq], ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha], segundosParada * 0.5f));
                StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha], ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq], segundosParada * 0.5f));

                //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
                StartCoroutine(WaitTimeSwap(segundosParada));

                //Realizamos los cambios en nuestra lista de elementos graficos que representa al array
                auxGameObject = ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq];
                ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq] = ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha];
                ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha] = auxGameObject;
            }

            //Si el estado es un if de elementos
            if (listaEstados[ContadorPaso].Estado == 0)
            {
                //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
                StartCoroutine(WaitTimeIf(segundosParada));
            }

            ContadorPaso += 1;
        }

        //Paso a paso
        if (pausado && parado && ContadorPaso <= (listaEstados.Count - 1))
        {
            //Paso adelante
            BotonStepOver.onClick.AddListener(delegate
            {
                if (!ejecutandoPaso)
                {
                    ejecutandoPaso = true;

                    print("Vamonos atomos");

                    limpiarResalteCodigo();

                    //Reproducir los estados

                    //Si el estado es un swap de elementos
                    if (listaEstados[ContadorPaso].Estado == 1)
                    {
                        //Ponemos en negrita la parte de codigo que estamos ejecutando
                        Code.text = Code.text.Replace("aux = array[p-1]", "<b>aux = array[p-1]</b>");
                        Code.text = Code.text.Replace("array[p-1] = array[p]", "<b>array[p-1] = array[p]</b>");
                        Code.text = Code.text.Replace("array[p] = aux", "<b>array[p] = aux</b>");

                        //Movemos los elementos
                        StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq], ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha], segundosParada * 0.5f));
                        StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha], ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq], segundosParada * 0.5f));

                        //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
                        StartCoroutine(WaitTimeSwapStep(0));

                        //Realizamos los cambios en nuestra lista de elementos graficos que representa al array
                        auxGameObject = ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq];
                        ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq] = ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha];
                        ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha] = auxGameObject;
                    }
                    //Si el estado es un if de elementos
                    if (listaEstados[ContadorPaso].Estado == 0)
                    {
                        //Ponemos en negrita la parte de codigo que estamos ejecutando
                        Code.text = Code.text.Replace("if array[p-1] > array[p]:", "<b>if array[p-1] > array[p]:</b>");
                        //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
                        StartCoroutine(WaitTimeIfStep(0));
                    }
                    ContadorPaso += 1;
                }
            });

            //Paso atras
            BotonStepBack.onClick.AddListener(delegate
            {
                if (!ejecutandoPaso && ContadorPaso > 0)
                {

                    ejecutandoPaso = true;

                    ContadorPaso -= 1;

                    print("Vamonos atomos");

                    limpiarResalteCodigo();

                    //Reproducir los estados

                    //Si el estado es un swap de elementos
                    if (listaEstados[ContadorPaso].Estado == 1)
                    {
                        //Ponemos en negrita la parte de codigo que estamos ejecutando
                        Code.text = Code.text.Replace("aux = array[p-1]", "<b>aux = array[p-1]</b>");
                        Code.text = Code.text.Replace("array[p-1] = array[p]", "<b>array[p-1] = array[p]</b>");
                        Code.text = Code.text.Replace("array[p] = aux", "<b>array[p] = aux</b>");

                        //Movemos los elementos
                        StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq], ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha], segundosParada * 0.5f));
                        StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha], ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq], segundosParada * 0.5f));

                        //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
                        StartCoroutine(WaitTimeSwapStep(0));

                        //Realizamos los cambios en nuestra lista de elementos graficos que representa al array
                        auxGameObject = ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq];
                        ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq] = ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha];
                        ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha] = auxGameObject;
                    }
                    //Si el estado es un if de elementos
                    if (listaEstados[ContadorPaso].Estado == 0)
                    {
                        //Ponemos en negrita la parte de codigo que estamos ejecutando
                        Code.text = Code.text.Replace("if array[p-1] > array[p]:", "<b>if array[p-1] > array[p]:</b>");
                        //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
                        StartCoroutine(WaitTimeIfStep(0));
                    }
                }
            });
        }
    }

    private void limpiarResalteCodigo()
    {
        //Swap
        Code.text = Code.text.Replace("<b>aux = array[p-1]</b>", "aux = array[p-1]");
        Code.text = Code.text.Replace("<b>array[p-1] = array[p]</b>", "array[p-1] = array[p]");
        Code.text = Code.text.Replace("<b>array[p] = aux</b>", "array[p] = aux");

        //If
        Code.text = Code.text.Replace("<b>if array[p-1] > array[p]:</b>", "if array[p-1] > array[p]:");
    }

    //private IEnumerator debug()
    //{
    //    int aux;
    //    float auxPosition;
    //    GameObject auxGameObject;
    //    for (int i = 1; i < array.Length; i++)
    //        for (int p = array.Length - 1; p >= i; p--)
    //        {
    //            //Esperamos si se ha presionado el boton de pausa
    //            yield return new WaitWhile(() => pausado==true);

    //            if (array[p - 1] > array[p])
    //            {
    //                //Guardamos en auxiliar
    //                aux = array[p - 1];
    //                auxPosition = ArrayListGraphic[p - 1].transform.position.x;
    //                auxGameObject = ArrayListGraphic[p - 1];
    //                code.text = code.text.Replace("aux = array[p-1]", "<b>aux = array[p-1]</b>");
    //                yield return new WaitForSeconds(segundosParada);
    //                code.text = code.text.Replace("<b>aux = array[p-1]</b>", "aux = array[p-1]");


    //                //El tiempo de movimiento de los elementos debe ser menor al de la parada para que todo funcione ok
    //                //Realizamos el movimiento entre ambos elementos antes de hacer las asignaciones lógicas
    //                StartCoroutine(MoveToPosition(ArrayListGraphic[p - 1], ArrayListGraphic[p], segundosParada * 0.9f));
    //                StartCoroutine(MoveToPosition(ArrayListGraphic[p], ArrayListGraphic[p - 1], segundosParada * 0.9f));


    //                //Cambiamos anterior por el siguiente
    //                code.text = code.text.Replace("array[p-1] = array[p]", "<b>array[p-1] = array[p]</b>");
    //                yield return new WaitForSeconds(segundosParada);
    //                code.text = code.text.Replace("<b>array[p-1] = array[p]</b>", "array[p-1] = array[p]");
    //                array[p - 1] = array[p];
    //                ArrayListGraphic[p - 1] = ArrayListGraphic[p];

                    
    //                //Cambiamos el siguiente por el anterior
    //                array[p] = aux;
    //                ArrayListGraphic[p] = auxGameObject;
    //                code.text = code.text.Replace("array[p] = aux", "<b>array[p] = aux</b>");
    //                yield return new WaitForSeconds(segundosParada);
    //                code.text = code.text.Replace("<b>array[p] = aux</b>", "array[p] = aux");
    //            }
    //        }
    //}

    //Función que permite animar las permutaciones del array
    private IEnumerator MoveToPosition(GameObject objetoInicial, GameObject objetoFinal, float timeToMove)
    {
        float time = 0f;
        Vector3 posicionInicial = objetoInicial.transform.position;
        Vector3 posicionFinal = objetoFinal.transform.position;

        while(time < 1)
        {
            time += Time.deltaTime / timeToMove;
            objetoInicial.transform.position = Vector3.Lerp(posicionInicial, posicionFinal, time);
            yield return null;
        }
    }

    //Funcion que espera tiempo determinado y resalta la parte del swap de elementos
    private IEnumerator WaitTimeSwap(float tiempoEspera)
    {
        parado = false;
        //Ponemos en negrita la parte de codigo que estamos ejecutando
        Code.text = Code.text.Replace("aux = array[p-1]", "<b>aux = array[p-1]</b>");
        Code.text = Code.text.Replace("array[p-1] = array[p]", "<b>array[p-1] = array[p]</b>");
        Code.text = Code.text.Replace("array[p] = aux", "<b>array[p] = aux</b>");

        yield return new WaitForSeconds(tiempoEspera);

        //Quitamos la negrita a la parte que estamos ejecutando
        Code.text = Code.text.Replace("<b>aux = array[p-1]</b>", "aux = array[p-1]");
        Code.text = Code.text.Replace("<b>array[p-1] = array[p]</b>", "array[p-1] = array[p]");
        Code.text = Code.text.Replace("<b>array[p] = aux</b>", "array[p] = aux");

        parado = true;
        ejecutandoPaso = false;
    }

    //Funcion que espera tiempo determinado y resalta la parte del if
    private IEnumerator WaitTimeIf(float tiempoEspera)
    {
        parado = false;

        //Ponemos en negrita la parte de codigo que estamos ejecutando
        Code.text = Code.text.Replace("if array[p-1] > array[p]:", "<b>if array[p-1] > array[p]:</b>");

        yield return new WaitForSeconds(tiempoEspera);

        //Quitamos la negrita a la parte que estamos ejecutando
        Code.text = Code.text.Replace("<b>if array[p-1] > array[p]:</b>", "if array[p-1] > array[p]:");

        parado = true;
        ejecutandoPaso = false;
    }

    //Funcion que espera tiempo determinado y resalta la parte del swap de elementos
    private IEnumerator WaitTimeSwapStep(float tiempoEspera)
    {
        parado = false;
        yield return new WaitForSeconds(tiempoEspera);
        parado = true;
        ejecutandoPaso = false;
    }

    //Funcion que espera tiempo determinado y resalta la parte del if
    private IEnumerator WaitTimeIfStep(float tiempoEspera)
    {
        parado = false;
        yield return new WaitForSeconds(tiempoEspera);
        parado = true;
        ejecutandoPaso = false;
    }

    //Para la ejecución
    public void pauseExecution()
    {
        pausado = true;
        EstadoEjecucion.text = "Pausa";
    }

    //Reanuda la ejecución
    public void playExecution()
    {
        pausado = false;
        EstadoEjecucion.text = "Play";
    }
}
