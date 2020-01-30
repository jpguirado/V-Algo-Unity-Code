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

    //Game Object que hace referencia al objecto que contiene el componente del texto del código
    public GameObject Debug;

    //Game Object que hace referencia al prefab del elemento gráfico del array
    public GameObject ElementoGraficoArray;

    //Lista con los GameObject que hacen referencia a la representación gráfica de cada uno de los elementos del array
    public List<GameObject> ArrayListGraphic;

    //Determina si la ejecución esta parada o no
    public bool pausado;

    // Start is called before the first frame update
    void Start()
    {
        array = CreateRandomArray(numElementos);
        InstantiateGraphicArray(array);
        StartCoroutine(debug());
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
        
    }

    private IEnumerator debug()
    {
        Text code = Debug.GetComponent<Text>();
        int aux;
        float auxPosition;
        GameObject auxGameObject;
        for (int i = 1; i < array.Length; i++)
            for (int p = array.Length - 1; p >= i; p--)
            {
                //Esperamos si se ha presionado el boton de pausa
                yield return new WaitWhile(() => pausado==true);

                if (array[p - 1] > array[p])
                {
                    //Guardamos en auxiliar
                    aux = array[p - 1];
                    auxPosition = ArrayListGraphic[p - 1].transform.position.x;
                    auxGameObject = ArrayListGraphic[p - 1];
                    code.text = code.text.Replace("aux = array[p - 1];", "<b>aux = array[p - 1];</b>");
                    yield return new WaitForSeconds(segundosParada);
                    code.text = code.text.Replace("<b>aux = array[p - 1];</b>", "aux = array[p - 1];");


                    //El tiempo de movimiento de los elementos debe ser menor al de la parada para que todo funcione ok
                    //Realizamos el movimiento entre ambos elementos antes de hacer las asignaciones lógicas
                    StartCoroutine(MoveToPosition(ArrayListGraphic[p - 1], ArrayListGraphic[p], segundosParada * 0.9f));
                    StartCoroutine(MoveToPosition(ArrayListGraphic[p], ArrayListGraphic[p - 1], segundosParada * 0.9f));


                    //Cambiamos anterior por el siguiente
                    code.text = code.text.Replace("array[p - 1] = array[p];", "<b>array[p - 1] = array[p];</b>");
                    yield return new WaitForSeconds(segundosParada);
                    code.text = code.text.Replace("<b>array[p - 1] = array[p];</b>", "array[p - 1] = array[p];");
                    array[p - 1] = array[p];
                    ArrayListGraphic[p - 1] = ArrayListGraphic[p];

                    
                    //Cambiamos el siguiente por el anterior
                    array[p] = aux;
                    ArrayListGraphic[p] = auxGameObject;
                    code.text = code.text.Replace("array[p] = aux;", "<b>array[p] = aux;</b>");
                    yield return new WaitForSeconds(segundosParada);
                    code.text = code.text.Replace("<b>array[p] = aux;</b>", "array[p] = aux;");
                }
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

    //Para la ejecución
    public void pauseExecution()
    {
        pausado = true;
    }

    //Reanuda la ejecución
    public void playExecution()
    {
        pausado = false;
    }
}
