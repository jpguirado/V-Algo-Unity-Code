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
        //code.text = "<b>for (int i = 1; i < array.Length; i++)\r\n            for (int p = array.Length - 1; p >= i; p--)\r\n            {\r\n                if (array[p-1] > array[p])\r\n                {\r\n                    aux = array[p - 1];\r\n                    array[p - 1] = array[p];\r\n                    array[p] = aux;\r\n                }\r\n            </b>}";
        int aux;
        float auxPosition;
        GameObject auxGameObject;
        for (int i = 1; i < array.Length; i++)
            for (int p = array.Length - 1; p >= i; p--)
            {
                if (array[p - 1] > array[p])
                {
                    aux = array[p - 1];
                    auxPosition = ArrayListGraphic[p - 1].transform.position.x;
                    auxGameObject = ArrayListGraphic[p - 1];
                    code.text = code.text.Replace("aux = array[p - 1];", "<b>aux = array[p - 1];</b>");
                    yield return new WaitForSeconds(segundosParada);
                    code.text = code.text.Replace("<b>aux = array[p - 1];</b>", "aux = array[p - 1];");

                    array[p - 1] = array[p];
                    ArrayListGraphic[p - 1].transform.position = new Vector3(ArrayListGraphic[p].transform.position.x, ArrayListGraphic[p - 1].transform.position.y, ArrayListGraphic[p - 1].transform.position.z);
                    ArrayListGraphic[p - 1] = ArrayListGraphic[p];
                    code.text = code.text.Replace("array[p - 1] = array[p];", "<b>array[p - 1] = array[p];</b>");
                    yield return new WaitForSeconds(segundosParada);
                    code.text = code.text.Replace("<b>array[p - 1] = array[p];</b>", "array[p - 1] = array[p];");

                    array[p] = aux;
                    ArrayListGraphic[p].transform.position = new Vector3(auxPosition, ArrayListGraphic[p].transform.position.y, ArrayListGraphic[p].transform.position.z);
                    ArrayListGraphic[p] = auxGameObject;
                    code.text = code.text.Replace("array[p] = aux;", "<b>array[p] = aux;</b>");
                    yield return new WaitForSeconds(segundosParada);
                    code.text = code.text.Replace("<b>array[p] = aux;</b>", "array[p] = aux;");
                }
            }
        print("acaba");
    }
}
