using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

    //Game Object empty that will represent the original position of the graphic array elements
    public GameObject OriginalPositionPrefab;

    //Lista con los GameObject que hacen referencia a la representación gráfica de cada uno de los elementos del array
    public List<GameObject> ArrayListGraphic;

    public List<Color> ColorList;

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
    public Image CodeImage;

    //Sprite original code
    public Sprite SpriteCodeImage;

    //Sprite image remark code
    public Sprite SpriteCodeRemarkImage;

    //Sprite image move down code
    public Sprite SpriteCodeMoveDownImage;

    //Sprite image move up code
    public Sprite SpriteCodeMoveUpImage;

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

    public MergeSortLanguageManager MergeSortLanguageManager;

    //Distance that will the elements move in the state 1
    public float MoveDownDistance;

    public RectTransform PosYReferenceMoveDown;

    //distance of displacement of the bars with respect to the center
    public float LateralShift; 

    // Start is called before the first frame update
    void Start()
    {
        ////Set the language
        MergeSortLanguageManager.SetLanguage();

        ////Establecemos variables
        parado = true;
        ContadorPaso = 0;
        listaEstados = new List<MergeSortState>();
        ejecutandoPaso = false;
        numElementos = MenuConfiguracion.NumElementos;

        //Si arrancamos directamente la escena, esto habra que eliminarlo en build final
        if (numElementos == 0)
        {
            numElementos = 15;
        }


        arrayOriginal = CreateRandomArray(numElementos);
        //arrayOriginal = new int[] { 8, 7, 4, 3, 6, 5, 9, 2, 10, 1};
        InstantiateGraphicArray(arrayOriginal, GetScale(),true);
        ExecuteAndCreateStates(arrayOriginal);
    }

    private Vector3 GetScale()
    {
        Vector3 scale = this.transform.localScale;

        scale.x += 1;
        scale.y += 1;
        scale.z += 1;

        return scale;
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
        if (p == 5)
            print("Debug");


        //Save the state to remark
        listaEstados.Add(new MergeSortState()
        {
            StartIndex = p,
            EndIndex = r,
            State = 0,
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

        int[] arrayBeforeChanges = (int[])array.Clone();

        for (int k = p; k<(r+1); k++)
        {
            //Here makes the swap
            if (L[i] <= R[j])
            {
                int index = 0;

                for (int m = 0; m < arrayBeforeChanges.Length; m++)
                {
                    if (L[i] == arrayBeforeChanges[m])
                    {
                        index = m;
                        break;
                    }
                }
                //If this 2 numbers are equal, don't swap
                array[k] = L[i];
                //Save state
                listaEstados.Add(new MergeSortState()
                {
                    NumberToMove = L[i],
                    PositionIndexToMove = k,
                    Color = ColorList[index],
                    State = 1,
                });
                i++;
            }
            else
            {
                int index = 0;
                for (int m = 0; m < arrayBeforeChanges.Length; m++)
                {
                    if (R[j] == arrayBeforeChanges[m]) 
                    {
                        index = m;
                        break;
                    }
                }
                //If this 2 numbers are equal, don't swap
                array[k] = R[j];
                //Save state
                listaEstados.Add(new MergeSortState()
                {
                    NumberToMove = R[j],
                    PositionIndexToMove = k,
                    Color = ColorList[index],
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
                array = (int[])array.Clone(),
                arrayBeforeChanges = (int[])arrayBeforeChanges.Clone()
            });
        }

        //Make changes in the color list
        for (int y = p; y <= r; y++)
        {
            ColorList[y] = ColorList[p];
        }

        return array;
    }


    //Instancia los elementos gráficos que representan al array y los mete en una lista de GameObject
    private void InstantiateGraphicArray(int[] array, Vector3 escalaActual, bool CreateColorList)
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
            int[] arrayDistanciaElementos = new int[] { 40, 35, 30, 25, 20, 15};
            distanceBetweenElements = arrayDistanciaElementos[numElementos-10];

            for (int i = 0; i < array.Length; i++)
            {
                //Instanciamos, cambiamos nombre, añadimos a la lista de GameObject, ponemos como padre el canvas y establecemos su posición.
                GameObject instanciado = Instantiate(ElementoGraficoArray);
                instanciado.name = array[i].ToString();
                ArrayListGraphic.Add(instanciado);
                instanciado.transform.SetParent(this.transform);
                float posicionX = (numElementos * 80 + (numElementos - 1) * distanceBetweenElements) / 2;
                Vector3 posicion = instanciado.GetComponent<RectTransform>().localPosition = new Vector3(-posicionX + i * (40 * 2 + distanceBetweenElements) + 40 + LateralShift, 100.0f, 0.0f);

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

                if(CreateColorList)
                    ColorList.Add(RandomColor);

                tamañoBarra.sizeDelta = new Vector2(40, array[i] * (90 / (array.Length - 1)) + 10);

                //Save the original position to help with the movements
                //GameObject originalPosition = new GameObject();
                GameObject originalPosition = Instantiate(OriginalPositionPrefab);
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
                PositionToMove.y -= MoveDownDistance * ArrayListGraphic[0].transform.localScale.x;
                PositionToMove.y = PosYReferenceMoveDown.position.y;

                //Move the element
                StartCoroutine(MoveToPosition(ArrayListGraphic[index], PositionToMove, StopSeconds * 0.5f));
                //StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha], ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq], segundosParada * 0.5f));

                //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
                StartCoroutine(WaitTimeMove(StopSeconds,1));

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
                StartCoroutine(WaitTimeMove(StopSeconds,2));
            }

            ContadorPaso += 1;
        }

        //Paso a paso
        if (pausado && parado && ContadorPaso <= (listaEstados.Count))
        {
            //Paso adelante
            BotonStepOver.onClick.AddListener(delegate
            {
                if (pausado)
                {
                    if (!ejecutandoPaso)
                    {
                        ejecutandoPaso = true;

                        limpiarResalteCodigo();

                        //Remark Elements
                        if (listaEstados[ContadorPaso].State == 0)
                        {
                            CodeImage.sprite = SpriteCodeRemarkImage;
                            //Remark graphic elements of the array
                            for (int i = listaEstados[ContadorPaso].StartIndex; i <= listaEstados[ContadorPaso].EndIndex; i++)
                            {
                                Color HighlightedColor = ArrayListGraphic[i].transform.Find("Barra").GetComponent<Image>().color;
                                ArrayListGraphic[i].transform.Find("Barra").GetComponent<Image>().color = new Color(HighlightedColor.r, HighlightedColor.g, HighlightedColor.b, 0.5f);
                            }
                            StartCoroutine(WaitTimeRemarkStep(0));
                        }

                        //Move elements down
                        if (listaEstados[ContadorPaso].State == 1)
                        {
                            ////Restore color and group by colors
                            //for (int j = listaEstados[ContadorPaso-1].StartIndex; j <= listaEstados[ContadorPaso-1].EndIndex; j++)
                            //{
                            //    if (j == listaEstados[ContadorPaso-1].StartIndex)
                            //    {
                            //        //Number of elements are odd
                            //        if (ArrayListGraphic.Count % 2 != 0)
                            //        {
                            //            if (listaEstados[ContadorPaso-1].StartIndex > ArrayListGraphic.Count / 2)
                            //                ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;
                            //            else
                            //                ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;
                            //        }
                            //        else //Even
                            //        {
                            //            if (listaEstados[ContadorPaso-1].StartIndex >= ArrayListGraphic.Count / 2)
                            //                ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;
                            //            else
                            //                ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ArrayListGraphic[listaEstados[ContadorPaso-1].StartIndex].transform.Find("Barra").GetComponent<Image>().color;
                            //    }

                            //    //ArrayListGraphic[i].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;
                            //}

                            CodeImage.sprite = SpriteCodeMoveDownImage;

                            for (int j = listaEstados[ContadorPaso-1].StartIndex; j <= listaEstados[ContadorPaso-1].EndIndex; j++)
                            {
                                //Restore the alfa color
                                Color HighlightedColor = ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color;
                                ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = new Color(HighlightedColor.r, HighlightedColor.g, HighlightedColor.b, 1f);

                                ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ArrayListGraphic[listaEstados[ContadorPaso-1].StartIndex].transform.Find("Barra").GetComponent<Image>().color;

                                if (listaEstados[ContadorPaso-1].StartIndex == 0 && listaEstados[ContadorPaso-1].EndIndex == (ArrayListGraphic.Count - 1))//Final step, we put main color of the proyect
                                    ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;
                            }


                            int index = 0;

                            //Find the index of the element to move
                            for (int i = 0; i < ArrayListGraphic.Count; i++)
                            {
                                if (ArrayListGraphic[i].name == listaEstados[ContadorPaso].NumberToMove.ToString())
                                {
                                    index = i;
                                    break;
                                }
                            }

                            //Target position - 100.0f to move down
                            Vector3 PositionToMove = OriginalPositions[listaEstados[ContadorPaso].PositionIndexToMove].transform.position;
                            PositionToMove.y -= MoveDownDistance * ArrayListGraphic[0].transform.localScale.x;
                            PositionToMove.y = PosYReferenceMoveDown.position.y;

                            //Move the element
                            StartCoroutine(MoveToPosition(ArrayListGraphic[index], PositionToMove, StopSeconds * 0.5f));
                            //StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha], ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq], segundosParada * 0.5f));

                            //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
                            StartCoroutine(WaitTimeMoveStep(StopSeconds));

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
                            CodeImage.sprite = SpriteCodeMoveUpImage;

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
                            StartCoroutine(WaitTimeMoveStep(StopSeconds));
                        }
                        ContadorPaso += 1;
                    }
                }
            });

            //Paso atras
            BotonStepBack.onClick.AddListener(delegate
            {
                if (pausado)
                {
                    if (!ejecutandoPaso && ContadorPaso > 0)
                    {

                        ejecutandoPaso = true;

                        ContadorPaso -= 1;

                        limpiarResalteCodigo();

                        //Reproducir los estados
                        //Remark Elements
                        if (listaEstados[ContadorPaso].State == 0)
                        {
                            //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
                            //Remark graphic elements of the array
                            CodeImage.sprite = SpriteCodeImage;

                            RemarkCodeStepBack(ContadorPaso);

                            for (int i = listaEstados[ContadorPaso].StartIndex; i <= listaEstados[ContadorPaso].EndIndex; i++)
                            {
                                Color HighlightedColor = ArrayListGraphic[i].transform.Find("Barra").GetComponent<Image>().color;
                                ArrayListGraphic[i].transform.Find("Barra").GetComponent<Image>().color = new Color(HighlightedColor.r, HighlightedColor.g, HighlightedColor.b, 1f);
                            }
                            StartCoroutine(WaitTimeRemarkStep(0));
                        }

                        //Move elements down
                        if (listaEstados[ContadorPaso].State == 1)
                        {
                            ////Restore color and group by colors
                            //for (int j = listaEstados[ContadorPaso - 1].StartIndex; j <= listaEstados[ContadorPaso - 1].EndIndex; j++)
                            //{
                            //    if (j == listaEstados[ContadorPaso - 1].StartIndex)
                            //    {
                            //        //Number of elements are odd
                            //        if (ArrayListGraphic.Count % 2 != 0)
                            //        {
                            //            if (listaEstados[ContadorPaso - 1].StartIndex > ArrayListGraphic.Count / 2)
                            //                ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;
                            //            else
                            //                ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;
                            //        }
                            //        else //Even
                            //        {
                            //            if (listaEstados[ContadorPaso - 1].StartIndex >= ArrayListGraphic.Count / 2)
                            //                ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;
                            //            else
                            //                ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ArrayListGraphic[listaEstados[ContadorPaso - 1].StartIndex].transform.Find("Barra").GetComponent<Image>().color;
                            //    }

                            //    //ArrayListGraphic[i].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;
                            //}
                            RemarkCodeStepBack(ContadorPaso);

                            int index = 0;

                            //Find the index of the element to move
                            for (int i = 0; i < ArrayListGraphic.Count; i++)
                            {
                                if (ArrayListGraphic[i].name == listaEstados[ContadorPaso].NumberToMove.ToString())
                                {
                                    index = i;
                                    break;
                                }
                            }

                            //Restore the color
                            //ArrayListGraphic[index].transform.Find("Barra").GetComponent<Image>().color = ArrayListGraphic[index].GetComponent<ArrayElement>().OriginalColor;
                            ArrayListGraphic[index].transform.Find("Barra").GetComponent<Image>().color = listaEstados[ContadorPaso].Color;

                            //Target position - 100.0f to move down
                            Vector3 PositionToMove = OriginalPositions[index].transform.position;

                            //Move the element
                            StartCoroutine(MoveToPosition(ArrayListGraphic[index], PositionToMove, StopSeconds * 0.5f));
                            //StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha], ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq], segundosParada * 0.5f));

                            //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
                            StartCoroutine(WaitTimeMoveStep(StopSeconds));

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
                            RemarkCodeStepBack(ContadorPaso);

                            //Target position - 100.0f to move down
                            Vector3 PositionToMove = OriginalPositions[listaEstados[ContadorPaso].PositionIndexToMove].transform.position;
                            PositionToMove.y -= MoveDownDistance * ArrayListGraphic[0].transform.localScale.x;

                            //Move the element up
                            StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].PositionIndexToMove], PositionToMove, StopSeconds * 0.5f));

                            //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
                            StartCoroutine(WaitTimeMoveStep(StopSeconds));

                            //We only revert the state of the array if we go to state 1
                            if (listaEstados[ContadorPaso - 1].State == 1)
                            {
                                //We have to do the changes to our ArrayListGraphic
                                //We have to clone the state of the array to our ArrayListGraphic
                                List<GameObject> listaAux = new List<GameObject>();

                                //Insert the elements in the right order in a aux list, then change our list by this.
                                for (int i = 0; i < listaEstados[ContadorPaso].arrayBeforeChanges.Length; i++)
                                {
                                    for (int j = 0; j < ArrayListGraphic.Count; j++)
                                    {
                                        if (listaEstados[ContadorPaso].arrayBeforeChanges[i].ToString() == ArrayListGraphic[j].name)
                                            listaAux.Add(ArrayListGraphic[j]);
                                    }
                                }
                                ArrayListGraphic = listaAux;
                            }
                        }
                    }
                }
            });
        }
    }

    private void RemarkCodeStepBack(int contadorPaso)
    {
        if (ContadorPaso > 0)
        {
            contadorPaso = ContadorPaso - 1;

            if (listaEstados[contadorPaso].State == 0)
             {
                CodeImage.sprite = SpriteCodeRemarkImage;
                for (int i = listaEstados[contadorPaso].StartIndex; i <= listaEstados[contadorPaso].EndIndex; i++)
                {
                    Color HighlightedColor = ArrayListGraphic[i].transform.Find("Barra").GetComponent<Image>().color;
                    ArrayListGraphic[i].transform.Find("Barra").GetComponent<Image>().color = new Color(HighlightedColor.r, HighlightedColor.g, HighlightedColor.b, 0.5f);
                }
            }

            else if (listaEstados[contadorPaso].State == 1)
            {
                CodeImage.sprite = SpriteCodeMoveDownImage;
            }
            else if (listaEstados[contadorPaso].State == 2)
                CodeImage.sprite = SpriteCodeMoveUpImage;
        }
    }

    //Funcion que limpia todos los resaltes, tanto de código como de elementos gráficos del array
    private void limpiarResalteCodigo()
    {
        //Codigo limpio sin resaltes
        CodeImage.sprite = SpriteCodeImage;
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
    private IEnumerator WaitTimeMove(float tiempoEspera, int state)
    {
        parado = false;

        int paso = ContadorPaso;

        ////Ponemos en negrita la parte de codigo que estamos ejecutando
        //Code.text = Code.text.Replace("aux = array[p-1]", "<b>aux = array[p-1]</b>");
        //Code.text = Code.text.Replace("array[p-1] = array[p]", "<b>array[p-1] = array[p]</b>");
        //Code.text = Code.text.Replace("array[p] = aux", "<b>array[p] = aux</b>");
        if (state == 1)
            CodeImage.sprite = SpriteCodeMoveDownImage;
        else
            CodeImage.sprite = SpriteCodeMoveUpImage;
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
        CodeImage.sprite = SpriteCodeImage;

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
            ArrayListGraphic[i].transform.Find("Barra").GetComponent<Image>().color = new Color(HighlightedColor.r,HighlightedColor.g,HighlightedColor.b,0.5f);
        }


        CodeImage.sprite = SpriteCodeRemarkImage;

        yield return new WaitForSeconds(tiempoEspera);


        //Restore color of graphic elements of the array
        for (int i = listaEstados[paso].StartIndex; i <= listaEstados[paso].EndIndex; i++)
        {
            Color HighlightedColor = ArrayListGraphic[i].transform.Find("Barra").GetComponent<Image>().color;
            ArrayListGraphic[i].transform.Find("Barra").GetComponent<Image>().color = new Color(HighlightedColor.r, HighlightedColor.g, HighlightedColor.b, 1f);
        }


        CodeImage.sprite = SpriteCodeImage;

        ////Quitamos la negrita a la parte que estamos ejecutando
        //Code.text = Code.text.Replace("<b>if array[p-1] > array[p]:</b>", "if array[p-1] > array[p]:");

        //ImagenCodigo.sprite = SpriteImagenCodigoLimpia;

        ////Restore color and group by colors
        //for (int j = listaEstados[paso].StartIndex; j <= listaEstados[paso].EndIndex; j++)
        //{
        //    if (j == listaEstados[paso].StartIndex)
        //    {
        //        //Number of elements are odd
        //        if (ArrayListGraphic.Count % 2 != 0)
        //        {
        //            if (listaEstados[paso].StartIndex > ArrayListGraphic.Count / 2)
        //                ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;
        //            else
        //                ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;
        //        }
        //        else //Even
        //        {
        //            if (listaEstados[paso].StartIndex >= ArrayListGraphic.Count / 2)
        //                ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;
        //            else
        //                ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;
        //        }
        //    }
        //    else
        //    {
        //        ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ArrayListGraphic[listaEstados[paso].StartIndex].transform.Find("Barra").GetComponent<Image>().color;
        //    }

        //    //ArrayListGraphic[i].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;
        //}

        for (int j = listaEstados[paso].StartIndex; j <= listaEstados[paso].EndIndex; j++)
        {
            ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ArrayListGraphic[listaEstados[paso].StartIndex].transform.Find("Barra").GetComponent<Image>().color;

            if (listaEstados[paso].StartIndex == 0 && listaEstados[paso].EndIndex == (ArrayListGraphic.Count - 1))//Final step, we put main color of the proyect
                ArrayListGraphic[j].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;
        }

        parado = true;
        ejecutandoPaso = false;
    }

    //Funcion que espera tiempo determinado y resalta la parte del swap de elementos cuando la ejecución va paso por paso
    private IEnumerator WaitTimeMoveStep(float tiempoEspera)
    {
        parado = false;
        yield return new WaitForSeconds(tiempoEspera);
        parado = true;
        ejecutandoPaso = false;
    }

    //Funcion que espera tiempo determinado y resalta la parte del if cuando la ejecución va paso por paso
    private IEnumerator WaitTimeRemarkStep(float tiempoEspera)
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

        //Eliminamos los objetos vacios
        for (int i = 0; i < OriginalPositions.Count; i++)
        {
            Destroy(OriginalPositions[i]);
        }

        //limpiamos la lista y volvemos a isntanciar todos los elementos gráficos y volvemos a crear la lista
        OriginalPositions.Clear();

        InstantiateGraphicArray(arrayOriginal, escalaActual,false);
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

        //Eliminamos los objetos vacios
        for (int i = 0; i < OriginalPositions.Count; i++)
        {
            Destroy(OriginalPositions[i]);
        }

        //limpiamos la lista y volvemos a isntanciar todos los elementos gráficos y volvemos a crear la lista
        OriginalPositions.Clear();


        InstantiateGraphicArray(arrayOrdenado, escalaActual,false);

        //we color all the array elements of the final color
        for (int i = 0; i < ArrayListGraphic.Count; i++)
        {
           ArrayListGraphic[i].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;
        }
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

    public Color Color;


    //State 1
    //Number of the array to move down
    public int NumberToMove { get; set; }

    //Index of the position to move
    public int PositionIndexToMove { get; set; }

    //State of the array
    public int[] array;

    //State of the array before changes
    public int[] arrayBeforeChanges;

}
