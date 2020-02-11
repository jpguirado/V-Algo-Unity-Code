﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TMPro;

public class MergeSort : MonoBehaviour
{
    //Numero de elementos que tendrá nuestro array, configurable desde editor
    public int numElementos;

    //Array a ordenar
    int[] arrayOriginal;

    //Array a ordenar
    int[] arrayOrdenado;

    //Tiempo de parada entre cada paso de la ejecución.
    public float StopSeconds;

    //Game Object que hace referencia al prefab del elemento gráfico del array
    public GameObject ElementoGraficoArray;

    //Lista con los GameObject que hacen referencia a la representación gráfica de cada uno de los elementos del array
    public List<GameObject> ArrayListGraphic;

    //List with the original positions of the graphic elemets of the array. This list will remain immutable
    public List<GameObject> OriginalPositions;

    //Determina si la ejecución esta parada o no
    public bool pausado;

    //Lista de estados para controlar la ejecución paso por paso
    public List<MergeSortState> listaEstados;

    //Indica el paso en el que nos encontramos
    public int ContadorPaso;

    //Indica si se esta realizando movimiento entre elementos
    private bool parado;

    //Sprite play
    public Sprite SpritePlay;

    //Sprite pausa
    public Sprite SpritePausa;

    //Boton de stepOver
    public Button BotonStepOver;

    //Boton de StepOver
    public Button BotonStepBack;

    //Boton de StepOver
    public Button BotonInicio;

    //Imagen Codigo
    public Image ImagenCodigo;

    //Sprite imagen codigo original
    public Sprite SpriteImagenCodigoLimpia;

    //Sprite imagen codigo if
    public Sprite SpriteImagenCodigoIf;

    //Sprite imagen codigo swap
    public Sprite SpriteImagenCodigoSwap;

    //Indica si esta ejecutando un paso
    private bool ejecutandoPaso;

    //Elemento texto con el código
    public Text Code;

    //Indica en la pantalla el número de paso
    public TextMeshProUGUI NumeroPaso;

    //Color original de los elementos gráficos del array
    public Color ColorOriginalElementosGráficos;

    //Color de resalte de los elementos gráficos del array
    public Color ColorResalteElementosGraficos;

    public Image ImagenPlayPausa;

    //Si esta true, engordamos las barras, si esta false dejamos mas espacio a los lados
    public bool engordarBarras;

    //Distancia entre elementos
    public float distanceBetweenElements;

    //Speed Slider
    public Slider SliderVelocidad;

    public BubbleLanguageManager BubbleLanguageManager;

    // Start is called before the first frame update
    void Start()
    {
        ////Set the language
        //BubbleLanguageManager.SetLanguage();

        ////Establecemos variables
        parado = true;
        ContadorPaso = 0;
        listaEstados = new List<MergeSortState>();
        ejecutandoPaso = false;
        numElementos = MenuConfiguracion.NumElementos;

        //Si arrancamos directamente la escena, esto habra que eliminarlo en build final
        if (numElementos == 0)
        {
            numElementos = 10;
        }

        //arrayOriginal = CreateRandomArray(numElementos);
        arrayOriginal = new int[] { 8, 7, 4, 3, 6, 5, 9, 2, 10, 1};
        ExecuteAndCreateStates(arrayOriginal);
        InstantiateGraphicArray(arrayOriginal, Vector3.zero);
    }

    //Ejecuta el algoritmo y crea los estados necesarios para su reproducción por pasos
    private void ExecuteAndCreateStates(int[] array)
    {
        //arrayOrdenado = (int[])array.Clone();
        //int aux;
        //for (int i = 1; i < arrayOrdenado.Length; i++)
        //    for (int p = arrayOrdenado.Length - 1; p >= i; p--)
        //    {
        //        //Crear estado de if
        //        listaEstados.Add(new BubbleState()
        //        {
        //            Estado = 0,
        //            IndiceElementoIzq = p - 1,
        //            IndiceElementoDcha = p
        //        });
        //        if (arrayOrdenado[p - 1] > arrayOrdenado[p])
        //        {
        //            //Crear estado de swap
        //            listaEstados.Add(new BubbleState()
        //            {
        //                Estado = 1,
        //                IndiceElementoIzq = p - 1,
        //                IndiceElementoDcha = p
        //            });
        //            aux = arrayOrdenado[p - 1];
        //            arrayOrdenado[p - 1] = arrayOrdenado[p];
        //            arrayOrdenado[p] = aux;
        //        }
        //    }

        arrayOrdenado = (int[])array.Clone();

        arrayOrdenado = mergesort(arrayOrdenado, 0, arrayOrdenado.Length - 1);

    }

    public int[] mergesort (int[]array, int p, int r)
    {
        if (p < r)
        {
            int q = (p + r) / 2;
            mergesort(array, p, q);
            mergesort(array, q + 1, r);
            array = merge(array, p, q, r);
        }
        return array;
    }

    private int[] merge(int[] array, int p, int q, int r)
    {
        //Save the state to remark
        listaEstados.Add(new MergeSortState()
        {
            StartIndex = p,
            EndIndex = r,
            State = 0
        });

        int n1 = q - p + 1;
        int n2 = r - q;
        int[] L = new int[n1 + 1];
        int[] R = new int[n2 + 1];

        int i, j = 0;

        for (i = 0; i < n1; i++)
            L[i] = array[p + i];
        for (j = 0; j < n2; j++)
            R[j] = array[q + 1 + j];
        
        //500 for us is like inf
        L[n1] = 500;
        R[n2] = 500;

        i = 0;
        j = 0;

        for (int k = p; k<(r+1); k++)
        {
            //Here makes the swap
            if (L[i] <= R[j])
            {
                //If this 2 numbers are equal, don't swap
                array[k] = L[i];
                //Save state
                listaEstados.Add(new MergeSortState()
                {
                    NumberToMove = L[i],
                    PositionIndexToMove = k,
                    State = 1,
                   
                });
                i++;
            }
            else
            {
                //If this 2 numbers are equal, don't swap
                array[k] = R[j];
                //Save state
                listaEstados.Add(new MergeSortState()
                {
                    NumberToMove = R[j],
                    PositionIndexToMove = k,
                    State = 1,
                });
                j++;
            }
        }


        for(int x = p; x <= r; x++)
        {
            listaEstados.Add(new MergeSortState()
            {
                PositionIndexToMove = x,
                State = 2,
                array = (int[])array.Clone()
            });
        }



        //print(array[0] + " " + array[1] + " " + array[2] + " " + array[3] + " " + array[4] + " " + array[5] + " " + array[6] + " " + array[7] + " " + array[8] + " " + array[9] + " ");
        return array;
    }


    //Instancia los elementos gráficos que representan al array y los mete en una lista de GameObject
    private void InstantiateGraphicArray(int[] array, Vector3 escalaActual)
    {
        //Solución que hace las barras más anchas cuantos menos elementos haya, para ocupar siempre toda la pantalla
        if (engordarBarras)
        {
            //Necesitamos obtener el ancho del canvas
            Vector2 CanvasResolution = this.GetComponent<CanvasScaler>().referenceResolution;

            //Distancia entre los elementos del array en funcion del ancho del canvas y el número de elementos
            distanceBetweenElements = (CanvasResolution.x - (CanvasResolution.x / 10)) / (array.Length - 1);

            for (int i = 0; i < array.Length; i++)
            {
                //Instanciamos, cambiamos nombre, añadimos a la lista de GameObject, ponemos como padre el canvas y establecemos su posición.
                GameObject instanciado = Instantiate(ElementoGraficoArray);
                instanciado.name = array[i].ToString();
                ArrayListGraphic.Add(instanciado);
                instanciado.transform.SetParent(this.transform);
                Vector3 posicion = instanciado.GetComponent<RectTransform>().localPosition = new Vector3(distanceBetweenElements * i - (CanvasResolution.x / 2) + (CanvasResolution.x / 2 / 10), 0.0f, 0.0f);

                //Restauramos la escala original si existia
                if (escalaActual != Vector3.zero)
                    instanciado.transform.localScale = escalaActual;

                //Establecer el número correspondiente
                instanciado.GetComponentInChildren<TextMeshProUGUI>().text = array[i].ToString();

                //Array que dice el ancho de las barras en funcion del número de elementos seleccionados
                int[] arrayWidth = new int[] { 80, 76, 72, 68, 64, 60, 56, 52, 48, 44, 40 };

                //Cambiar la altura de la barra a un tamaño proporcional a su número y establecer su color
                RectTransform tamañoBarra = instanciado.transform.Find("Barra").GetComponent<RectTransform>();
                instanciado.transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;
                tamañoBarra.sizeDelta = new Vector2(arrayWidth[numElementos - 10], array[i] * (90 / (array.Length - 1)) + 10);

                //Save the position
                GameObject originalPosition = new GameObject();
                originalPosition.transform.position = ArrayListGraphic[i].transform.position;
                OriginalPositions.Add(originalPosition);

                //Ajustar la altura de la barra debido a que el cambio de tamaño lo hace hacia arriba y abajo
                //tamañoBarra.localPosition = new Vector3(tamañoBarra.localPosition.x, tamañoBarra.localPosition.y - ((100 - tamañoBarra.sizeDelta.y) / 2), tamañoBarra.localPosition.z);
            }
        }
        else
        {
            //Array con distancia entre elementos en funcion del número que seleccione el usuario para aprovechar más la pantalla
            int[] arrayDistanciaElementos = new int[] { 40, 37, 34, 31, 28, 25, 22, 19, 16, 13, 10 };
            distanceBetweenElements = arrayDistanciaElementos[numElementos-10];

            for (int i = 0; i < array.Length; i++)
            {
                //Instanciamos, cambiamos nombre, añadimos a la lista de GameObject, ponemos como padre el canvas y establecemos su posición.
                GameObject instanciado = Instantiate(ElementoGraficoArray);
                instanciado.name = array[i].ToString();
                ArrayListGraphic.Add(instanciado);
                instanciado.transform.SetParent(this.transform);
                float posicionX = (numElementos * 80 + (numElementos - 1) * distanceBetweenElements) / 2;
                Vector3 posicion = instanciado.GetComponent<RectTransform>().localPosition = new Vector3(-posicionX + i * (40 * 2 + distanceBetweenElements) + 40, 100.0f, 0.0f);

                //Restauramos la escala original si existia
                if (escalaActual != Vector3.zero)
                    instanciado.transform.localScale = escalaActual;

                //Establecer el número correspondiente
                instanciado.GetComponentInChildren<TextMeshProUGUI>().text = array[i].ToString();

                //Array que dice el ancho de las barras en funcion del número de elementos seleccionados
                int[] arrayWidth = new int[] { 80, 76, 72, 68, 64, 60, 56, 52, 48, 44, 40 };

                //Cambiar la altura de la barra a un tamaño proporcional a su número y establecer su color
                RectTransform tamañoBarra = instanciado.transform.Find("Barra").GetComponent<RectTransform>();

                //Random colors because this algorithm at the start creates partitions of size 1
                Color RandomColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                instanciado.transform.Find("Barra").GetComponent<Image>().color = RandomColor;

                tamañoBarra.sizeDelta = new Vector2(40, array[i] * (90 / (array.Length - 1)) + 10);

                //Save the original position to help with the movements
                GameObject originalPosition = new GameObject();
                originalPosition = Instantiate(originalPosition);
                originalPosition.name = "Original Position " + i;
                OriginalPositions.Add(originalPosition);
                originalPosition.transform.SetParent(this.transform);
                Vector3 position = ArrayListGraphic[i].transform.position;
                originalPosition.transform.position = position;
                

                //Ajustar la altura de la barra debido a que el cambio de tamaño lo hace hacia arriba y abajo
                //tamañoBarra.localPosition = new Vector3(tamañoBarra.localPosition.x, tamañoBarra.localPosition.y - ((100 - tamañoBarra.sizeDelta.y) / 2), tamañoBarra.localPosition.z);
            }
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
        for (int i = numElementos - 1; i > 0; i--)
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
        //Text of the step
        if (PlayerPrefs.GetString("language") == "ESPAÑOL")
            NumeroPaso.text = "Paso: " + ContadorPaso + "/" + listaEstados.Count;
        else
            NumeroPaso.text = "Step: " + ContadorPaso + "/" + listaEstados.Count;

        //Get the speed
        StopSeconds = SliderVelocidad.value;

        GameObject auxGameObject;

        //Modo automatico
        if (!pausado && parado && ContadorPaso <= (listaEstados.Count - 1))
        {
            //limpiarResalteCodigo();

            ////Si el estado es un swap de elementos
            //if (listaEstados[ContadorPaso].Estado == 1)
            //{
            //    //Movemos los elementos
            //    StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq], ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha], segundosParada * 0.5f));
            //    StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha], ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq], segundosParada * 0.5f));

            //    //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
            //    StartCoroutine(WaitTimeSwap(segundosParada));

            //    //Realizamos los cambios en nuestra lista de elementos graficos que representa al array
            //    auxGameObject = ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq];
            //    ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq] = ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha];
            //    ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha] = auxGameObject;
            //}

            ////Si el estado es un if de elementos
            //if (listaEstados[ContadorPaso].Estado == 0)
            //{
            //    //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
            //    StartCoroutine(WaitTimeIf(segundosParada));
            //}

            //Remark Elements
            if (listaEstados[ContadorPaso].State == 0)
            {
                //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
                StartCoroutine(WaitTimeRemark(StopSeconds));
            }

            //Move elements down
            if (listaEstados[ContadorPaso].State == 1)
            {
                int index = 0;

                //Find the index of the element to move
                for(int i = 0; i < ArrayListGraphic.Count; i++)
                {
                    if (ArrayListGraphic[i].name == listaEstados[ContadorPaso].NumberToMove.ToString())
                    {
                        index = i;
                        break;
                    }
                }

                //Target position - 100.0f to move down
                Vector3 PositionToMove = OriginalPositions[listaEstados[ContadorPaso].PositionIndexToMove].transform.position;
                PositionToMove.y -= 100.0f;

                //Move the element
                StartCoroutine(MoveToPosition(ArrayListGraphic[index], PositionToMove, StopSeconds * 0.5f));
                //StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha], ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq], segundosParada * 0.5f));

                //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
                StartCoroutine(WaitTimeMove(StopSeconds));

                //Realizamos los cambios en nuestra lista de elementos graficos que representa al array
                //auxGameObject = ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq];
                //ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq] = ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha];
                //ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha] = auxGameObject;

                ////We have to clone the state of the array to our ArrayListGraphic
                //List<GameObject> listaAux = new List<GameObject>();
                
                ////Insert the elements in the right order in a aux list, then change our list by this.
                //for(int i = 0; i < listaEstados[ContadorPaso].array.Length; i++)
                //{
                //    for(int j = 0; j < ArrayListGraphic.Count; j++)
                //    {
                //        if (listaEstados[ContadorPaso].array[i].ToString() == ArrayListGraphic[j].name)
                //            listaAux.Add(ArrayListGraphic[j]);
                //    }
                //}

                //ArrayListGraphic = listaAux;
            }

            //Move elements up
            if (listaEstados[ContadorPaso].State == 2)
            {
                //We have to do the changes to our ArrayListGraphic

                //We have to clone the state of the array to our ArrayListGraphic
                List<GameObject> listaAux = new List<GameObject>();

                //Insert the elements in the right order in a aux list, then change our list by this.
                for (int i = 0; i < listaEstados[ContadorPaso].array.Length; i++)
                {
                    for (int j = 0; j < ArrayListGraphic.Count; j++)
                    {
                        if (listaEstados[ContadorPaso].array[i].ToString() == ArrayListGraphic[j].name)
                            listaAux.Add(ArrayListGraphic[j]);
                    }
                }

                ArrayListGraphic = listaAux;

                //Move the element up
                StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].PositionIndexToMove], OriginalPositions[listaEstados[ContadorPaso].PositionIndexToMove].transform.position, StopSeconds * 0.5f));
                
                //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
                StartCoroutine(WaitTimeMove(StopSeconds));
            }

            ContadorPaso += 1;
        }

        //Paso a paso
        if (pausado && parado && ContadorPaso <= (listaEstados.Count - 1))
        {
            ////Paso adelante
            //BotonStepOver.onClick.AddListener(delegate
            //{
            //    if (pausado)
            //    {
            //        if (!ejecutandoPaso)
            //        {
            //            ejecutandoPaso = true;

            //            limpiarResalteCodigo();

            //            //Reproducir los estados

            //            //Si el estado es un swap de elementos
            //            if (listaEstados[ContadorPaso].Estado == 1)
            //            {
            //                //Ponemos en negrita la parte de codigo que estamos ejecutando
            //                Code.text = Code.text.Replace("aux = array[p-1]", "<b>aux = array[p-1]</b>");
            //                Code.text = Code.text.Replace("array[p-1] = array[p]", "<b>array[p-1] = array[p]</b>");
            //                Code.text = Code.text.Replace("array[p] = aux", "<b>array[p] = aux</b>");

            //                ImagenCodigo.sprite = SpriteImagenCodigoSwap;

            //                //Resaltar elementos graficos array
            //                ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;
            //                ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;

            //                //Movemos los elementos
            //                StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq], ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha], segundosParada * 0.5f));
            //                StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha], ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq], segundosParada * 0.5f));

            //                //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
            //                StartCoroutine(WaitTimeSwapStep(segundosParada));

            //                //Realizamos los cambios en nuestra lista de elementos graficos que representa al array
            //                auxGameObject = ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq];
            //                ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq] = ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha];
            //                ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha] = auxGameObject;
            //            }
            //            //Si el estado es un if de elementos
            //            if (listaEstados[ContadorPaso].Estado == 0)
            //            {
            //                //Ponemos en negrita la parte de codigo que estamos ejecutando
            //                Code.text = Code.text.Replace("if array[p-1] > array[p]:", "<b>if array[p-1] > array[p]:</b>");

            //                ImagenCodigo.sprite = SpriteImagenCodigoIf;

            //                //Resaltar elementos graficos array
            //                ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;
            //                ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;

            //                //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
            //                StartCoroutine(WaitTimeIfStep(0));
            //            }
            //            ContadorPaso += 1;
            //        }
            //    }
            //});

            ////Paso atras
            //BotonStepBack.onClick.AddListener(delegate
            //{
            //    if (pausado)
            //    {
            //        if (!ejecutandoPaso && ContadorPaso > 0)
            //        {

            //            ejecutandoPaso = true;

            //            ContadorPaso -= 1;

            //            limpiarResalteCodigo();

            //            //Reproducir los estados

            //            //Si el estado es un swap de elementos
            //            if (listaEstados[ContadorPaso].Estado == 1)
            //            {
            //                //Ponemos en negrita la parte de codigo que estamos ejecutando
            //                Code.text = Code.text.Replace("aux = array[p-1]", "<b>aux = array[p-1]</b>");
            //                Code.text = Code.text.Replace("array[p-1] = array[p]", "<b>array[p-1] = array[p]</b>");
            //                Code.text = Code.text.Replace("array[p] = aux", "<b>array[p] = aux</b>");

            //                ImagenCodigo.sprite = SpriteImagenCodigoSwap;

            //                //Resaltar elementos graficos array
            //                ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;
            //                ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;

            //                //Movemos los elementos
            //                StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq], ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha], segundosParada * 0.5f));
            //                StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha], ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq], segundosParada * 0.5f));

            //                //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
            //                StartCoroutine(WaitTimeSwapStep(segundosParada));

            //                //Realizamos los cambios en nuestra lista de elementos graficos que representa al array
            //                auxGameObject = ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq];
            //                ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq] = ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha];
            //                ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha] = auxGameObject;
            //            }
            //            //Si el estado es un if de elementos
            //            if (listaEstados[ContadorPaso].Estado == 0)
            //            {
            //                //Ponemos en negrita la parte de codigo que estamos ejecutando
            //                Code.text = Code.text.Replace("if array[p-1] > array[p]:", "<b>if array[p-1] > array[p]:</b>");

            //                ImagenCodigo.sprite = SpriteImagenCodigoIf;

            //                //Resaltar elementos graficos array
            //                ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;
            //                ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;

            //                //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
            //                StartCoroutine(WaitTimeIfStep(0));
            //            }
            //        }
            //    }
            //});
        }
    }

    //Funcion que limpia todos los resaltes, tanto de código como de elementos gráficos del array
    private void limpiarResalteCodigo()
    {
        //Swap
        Code.text = Code.text.Replace("<b>aux = array[p-1]</b>", "aux = array[p-1]");
        Code.text = Code.text.Replace("<b>array[p-1] = array[p]</b>", "array[p-1] = array[p]");
        Code.text = Code.text.Replace("<b>array[p] = aux</b>", "array[p] = aux");

        //If
        Code.text = Code.text.Replace("<b>if array[p-1] > array[p]:</b>", "if array[p-1] > array[p]:");

        //Codigo limpio sin resaltes
        ImagenCodigo.sprite = SpriteImagenCodigoLimpia;

        //Reestablecemos el color de las barras
        for (int i = 0; i < ArrayListGraphic.Count; i++)
        {
            ArrayListGraphic[i].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;
        }

    }

    //Function that allows animate the permutations of the array
    private IEnumerator MoveToPosition(GameObject objetoInicial, Vector3 posicionFinal, float timeToMove)
    {
        float time = 0f;
        Vector3 posicionInicial = objetoInicial.transform.position;

        while (time < 1)
        {
            time += Time.deltaTime / timeToMove;
            objetoInicial.transform.position = Vector3.Lerp(posicionInicial, posicionFinal, time);
            yield return null;
        }
    }

    //Funcion que espera tiempo determinado y resalta la parte del swap de elementos
    private IEnumerator WaitTimeMove(float tiempoEspera)
    {
        parado = false;

        int paso = ContadorPaso;

        ////Ponemos en negrita la parte de codigo que estamos ejecutando
        //Code.text = Code.text.Replace("aux = array[p-1]", "<b>aux = array[p-1]</b>");
        //Code.text = Code.text.Replace("array[p-1] = array[p]", "<b>array[p-1] = array[p]</b>");
        //Code.text = Code.text.Replace("array[p] = aux", "<b>array[p] = aux</b>");

        //ImagenCodigo.sprite = SpriteImagenCodigoSwap;

        ////Resaltar elementos graficos array
        //ArrayListGraphic[listaEstados[paso].IndiceElementoIzq].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;
        //ArrayListGraphic[listaEstados[paso].IndiceElementoDcha].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;

        yield return new WaitForSeconds(tiempoEspera);

        ////Quitamos la negrita a la parte que estamos ejecutando
        //Code.text = Code.text.Replace("<b>aux = array[p-1]</b>", "aux = array[p-1]");
        //Code.text = Code.text.Replace("<b>array[p-1] = array[p]</b>", "array[p-1] = array[p]");
        //Code.text = Code.text.Replace("<b>array[p] = aux</b>", "array[p] = aux");

        //ImagenCodigo.sprite = SpriteImagenCodigoLimpia;

        ////Quitar resalte elementos graficos array
        //ArrayListGraphic[listaEstados[paso].IndiceElementoIzq].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;
        //ArrayListGraphic[listaEstados[paso].IndiceElementoDcha].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;

        parado = true;
        ejecutandoPaso = false;
    }

    //Funcion que espera tiempo determinado y resalta la parte del if
    private IEnumerator WaitTimeRemark(float tiempoEspera)
    {
        parado = false;

        int paso = ContadorPaso;

        ////Ponemos en negrita la parte de codigo que estamos ejecutando
        //Code.text = Code.text.Replace("if array[p-1] > array[p]:", "<b>if array[p-1] > array[p]:</b>");

        ////Resalta imagen codigo
        //ImagenCodigo.sprite = SpriteImagenCodigoIf;

        ////Resaltar elementos graficos array
        //ArrayListGraphic[listaEstados[paso].IndiceElementoIzq].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;
        //ArrayListGraphic[listaEstados[paso].IndiceElementoDcha].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;

        //Remark graphic elements of the array
        for(int i = listaEstados[paso].StartIndex; i<= listaEstados[paso].EndIndex; i++)
        {
            Color HighlightedColor = ArrayListGraphic[i].transform.Find("Barra").GetComponent<Image>().color;
            ArrayListGraphic[i].transform.Find("Barra").GetComponent<Image>().color = new Color(HighlightedColor.r,HighlightedColor.g,HighlightedColor.b,HighlightedColor.a - 0.5f);
        }


        yield return new WaitForSeconds(tiempoEspera);

        ////Quitamos la negrita a la parte que estamos ejecutando
        //Code.text = Code.text.Replace("<b>if array[p-1] > array[p]:</b>", "if array[p-1] > array[p]:");

        //ImagenCodigo.sprite = SpriteImagenCodigoLimpia;

        //Restore color and group by colors
        for (int j = listaEstados[paso].StartIndex; j <= listaEstados[paso].EndIndex; j++)
        {
            if (j == listaEstados[paso].StartIndex)
            {
                //Number of elements are odd
                if (ArrayListGraphic.Count % 2 != 0)
                {
                    if (listaEstados[paso].StartIndex > ArrayListGraphic.Count / 2)
                        ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;
                    else
                        ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;
                }
                else //Even
                {
                    if (listaEstados[paso].StartIndex >= ArrayListGraphic.Count / 2)
                        ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;
                    else
                        ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;
                }
            }
            else
            {
                ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ArrayListGraphic[listaEstados[paso].StartIndex].transform.Find("Barra").GetComponent<Image>().color;
            }

            //ArrayListGraphic[i].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;
        }

        parado = true;
        ejecutandoPaso = false;
    }

    //Funcion que espera tiempo determinado y resalta la parte del swap de elementos cuando la ejecución va paso por paso
    private IEnumerator WaitTimeSwapStep(float tiempoEspera)
    {
        parado = false;
        yield return new WaitForSeconds(tiempoEspera);
        parado = true;
        ejecutandoPaso = false;
    }

    //Funcion que espera tiempo determinado y resalta la parte del if cuando la ejecución va paso por paso
    private IEnumerator WaitTimeIfStep(float tiempoEspera)
    {
        parado = false;
        yield return new WaitForSeconds(tiempoEspera);
        parado = true;
        ejecutandoPaso = false;
    }

    //Reanuda la ejecución
    public void playPauseExecution()
    {
        //Esta en play, hay que pausarlo
        if (ImagenPlayPausa.sprite.name == "Pausa")
        {
            pausado = true;
            ImagenPlayPausa.sprite = SpritePlay;
            return;
        }

        //Esta en pausarlo, hay que ponerlo en play
        if (ImagenPlayPausa.sprite.name == "Play")
        {
            pausado = false;
            ImagenPlayPausa.sprite = SpritePausa;
            return;
        }
    }

    //Reinicia la ejecución
    public void RestartExecution()
    {
        pausado = true;

        ImagenPlayPausa.sprite = SpritePlay;

        ContadorPaso = 0;

        Vector3 escalaActual = ArrayListGraphic[0].transform.localScale;

        //Eliminamos los objetos vacios
        for (int i = 0; i < ArrayListGraphic.Count; i++)
        {
            Destroy(ArrayListGraphic[i]);
        }

        //limpiamos la lista y volvemos a isntanciar todos los elementos gráficos y volvemos a crear la lista
        ArrayListGraphic.Clear();
        InstantiateGraphicArray(arrayOriginal, escalaActual);
    }

    //Termina la ejecución
    public void EndExecution()
    {
        pausado = true;

        ImagenPlayPausa.sprite = SpritePlay;

        ContadorPaso = listaEstados.Count;

        Vector3 escalaActual = ArrayListGraphic[0].transform.localScale;

        //Eliminamos los objetos vacios
        for (int i = 0; i < ArrayListGraphic.Count; i++)
        {
            Destroy(ArrayListGraphic[i]);
        }

        //limpiamos la lista y volvemos a isntanciar todos los elementos gráficos y volvemos a crear la lista
        ArrayListGraphic.Clear();
        InstantiateGraphicArray(arrayOrdenado, escalaActual);
    }

    //Return to the main menu
    public void ReturnToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}

public class MergeSortState
{
    //0 to remark elements, 1 to move down, 2 to move up
    public int State { get; set; }

    //State 0
    //Start and end index to know what elements are involved
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }


    //State 1
    //Number of the array to move down
    public int NumberToMove { get; set; }

    //Index of the position to move
    public int PositionIndexToMove { get; set; }

    //State of the array
    public int[] array;

}