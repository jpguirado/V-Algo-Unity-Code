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
    int[] arrayOriginal;

    //Array a ordenar
    int[] arrayOrdenado;

    //Tiempo de parada entre cada paso de la ejecución.
    public float segundosParada;

    //Game Object que hace referencia al prefab del elemento gráfico del array
    public GameObject ElementoGraficoArray;

    //Lista con los GameObject que hacen referencia a la representación gráfica de cada uno de los elementos del array
    public List<GameObject> ArrayListGraphic;

    //Determina si la ejecución esta parada o no
    public bool pausado;

    //Lista de estados para controlar la ejecución paso por paso
    public List<BubbleState> listaEstados;

    //Indica el paso en el que nos encontramos
    public int ContadorPaso;

    //Indica si se esta realizando movimiento entre elementos
    private bool parado;

    //Indica si hay que realizar un paso hacia delante
    private bool StepOver;

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
    public Text NumeroPaso;

    //Color original de los elementos gráficos del array
    public Color ColorOriginalElementosGráficos;

    //Color de resalte de los elementos gráficos del array
    public Color ColorResalteElementosGraficos;

    public Image ImagenPlayPausa;

    // Start is called before the first frame update
    void Start()
    {
        parado = true;
        ContadorPaso = 0;
        listaEstados = new List<BubbleState>();
        ejecutandoPaso = false;

        arrayOriginal = CreateRandomArray(numElementos);
        ExecuteAndCreateStates(arrayOriginal);
        InstantiateGraphicArray(arrayOriginal,Vector3.zero);
        //StartCoroutine(debug());
    }

    private void ExecuteAndCreateStates(int[] array)
    {
        arrayOrdenado = (int[])array.Clone();
        int aux;
        for (int i = 1; i < arrayOrdenado.Length; i++)
            for (int p = arrayOrdenado.Length - 1; p >= i; p--)
            {
                //Crear estado de if
                listaEstados.Add(new BubbleState()
                {
                    Estado = 0,
                    IndiceElementoIzq = p - 1,
                    IndiceElementoDcha = p
                });
                if (arrayOrdenado[p - 1] > arrayOrdenado[p])
                {
                    //Crear estado de swap
                    listaEstados.Add(new BubbleState()
                    {
                        Estado = 1,
                        IndiceElementoIzq = p - 1,
                        IndiceElementoDcha = p
                    });
                    aux = arrayOrdenado[p - 1];
                    arrayOrdenado[p - 1] = arrayOrdenado[p];
                    arrayOrdenado[p] = aux;
                }
            }
    }

    private void InstantiateGraphicArray(int[] array, Vector3 escalaActual)
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

            //Restauramos la escala original si existia
            if(escalaActual != Vector3.zero) 
                instanciado.transform.localScale = escalaActual;

            //Establecer el número correspondiente
            instanciado.GetComponentInChildren<Text>().text = array[i].ToString();

            //Cambiar la altura de la barra a un tamaño proporcional a su número y establecer su color
            RectTransform tamañoBarra = instanciado.transform.Find("Barra").GetComponent<RectTransform>();
            instanciado.transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;
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

        NumeroPaso.text = "Paso " + ContadorPaso + "/" + listaEstados.Count;

        
        //Modo automatico
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
                if (pausado)
                {
                    if (!ejecutandoPaso)
                    {
                        ejecutandoPaso = true;

                        limpiarResalteCodigo();

                        //Reproducir los estados

                        //Si el estado es un swap de elementos
                        if (listaEstados[ContadorPaso].Estado == 1)
                        {
                            //Ponemos en negrita la parte de codigo que estamos ejecutando
                            Code.text = Code.text.Replace("aux = array[p-1]", "<b>aux = array[p-1]</b>");
                            Code.text = Code.text.Replace("array[p-1] = array[p]", "<b>array[p-1] = array[p]</b>");
                            Code.text = Code.text.Replace("array[p] = aux", "<b>array[p] = aux</b>");

                            ImagenCodigo.sprite = SpriteImagenCodigoSwap;

                            //Resaltar elementos graficos array
                            ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;
                            ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;

                            //Movemos los elementos
                            StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq], ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha], segundosParada * 0.5f));
                            StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha], ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq], segundosParada * 0.5f));

                            //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
                            StartCoroutine(WaitTimeSwapStep(segundosParada));

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

                            ImagenCodigo.sprite = SpriteImagenCodigoIf;

                            //Resaltar elementos graficos array
                            ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;
                            ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;

                            //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
                            StartCoroutine(WaitTimeIfStep(0));
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

                        //Si el estado es un swap de elementos
                        if (listaEstados[ContadorPaso].Estado == 1)
                        {
                            //Ponemos en negrita la parte de codigo que estamos ejecutando
                            Code.text = Code.text.Replace("aux = array[p-1]", "<b>aux = array[p-1]</b>");
                            Code.text = Code.text.Replace("array[p-1] = array[p]", "<b>array[p-1] = array[p]</b>");
                            Code.text = Code.text.Replace("array[p] = aux", "<b>array[p] = aux</b>");

                            ImagenCodigo.sprite = SpriteImagenCodigoSwap;

                            //Resaltar elementos graficos array
                            ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;
                            ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;

                            //Movemos los elementos
                            StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq], ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha], segundosParada * 0.5f));
                            StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha], ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq], segundosParada * 0.5f));

                            //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
                            StartCoroutine(WaitTimeSwapStep(segundosParada));

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

                            ImagenCodigo.sprite = SpriteImagenCodigoIf;

                            //Resaltar elementos graficos array
                            ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;
                            ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;

                            //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
                            StartCoroutine(WaitTimeIfStep(0));
                        }
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

        ImagenCodigo.sprite = SpriteImagenCodigoLimpia;

        //Reestablecemos el color de las barras
        for (int i = 0; i < ArrayListGraphic.Count; i++)
        {
            ArrayListGraphic[i].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;
        }

    }

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

        int paso = ContadorPaso;

        //Ponemos en negrita la parte de codigo que estamos ejecutando
        Code.text = Code.text.Replace("aux = array[p-1]", "<b>aux = array[p-1]</b>");
        Code.text = Code.text.Replace("array[p-1] = array[p]", "<b>array[p-1] = array[p]</b>");
        Code.text = Code.text.Replace("array[p] = aux", "<b>array[p] = aux</b>");

        ImagenCodigo.sprite = SpriteImagenCodigoSwap;

        //Resaltar elementos graficos array
        ArrayListGraphic[listaEstados[paso].IndiceElementoIzq].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;
        ArrayListGraphic[listaEstados[paso].IndiceElementoDcha].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;

        yield return new WaitForSeconds(tiempoEspera);

        //Quitamos la negrita a la parte que estamos ejecutando
        Code.text = Code.text.Replace("<b>aux = array[p-1]</b>", "aux = array[p-1]");
        Code.text = Code.text.Replace("<b>array[p-1] = array[p]</b>", "array[p-1] = array[p]");
        Code.text = Code.text.Replace("<b>array[p] = aux</b>", "array[p] = aux");

        ImagenCodigo.sprite = SpriteImagenCodigoLimpia;

        //Quitar resalte elementos graficos array
        ArrayListGraphic[listaEstados[paso].IndiceElementoIzq].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;
        ArrayListGraphic[listaEstados[paso].IndiceElementoDcha].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;

        parado = true;
        ejecutandoPaso = false;
    }

    //Funcion que espera tiempo determinado y resalta la parte del if
    private IEnumerator WaitTimeIf(float tiempoEspera)
    {
        parado = false;

        int paso = ContadorPaso;

        //Ponemos en negrita la parte de codigo que estamos ejecutando
        Code.text = Code.text.Replace("if array[p-1] > array[p]:", "<b>if array[p-1] > array[p]:</b>");

        ImagenCodigo.sprite = SpriteImagenCodigoIf;

        //Resaltar elementos graficos array
        ArrayListGraphic[listaEstados[paso].IndiceElementoIzq].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;
        ArrayListGraphic[listaEstados[paso].IndiceElementoDcha].transform.Find("Barra").GetComponent<Image>().color = ColorResalteElementosGraficos;

        yield return new WaitForSeconds(tiempoEspera);

        //Quitamos la negrita a la parte que estamos ejecutando
        Code.text = Code.text.Replace("<b>if array[p-1] > array[p]:</b>", "if array[p-1] > array[p]:");

        ImagenCodigo.sprite = SpriteImagenCodigoLimpia;

        //Quitar resalte elementos graficos array
        ArrayListGraphic[listaEstados[paso].IndiceElementoIzq].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;
        ArrayListGraphic[listaEstados[paso].IndiceElementoDcha].transform.Find("Barra").GetComponent<Image>().color = ColorOriginalElementosGráficos;

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

    //End execution
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
        InstantiateGraphicArray(arrayOrdenado,escalaActual);
    }
}
