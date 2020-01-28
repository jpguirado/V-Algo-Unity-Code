using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Bubble : MonoBehaviour
{

    //Array with the numbers to sort
    int[] array;

    public GameObject Debug;


    // Start is called before the first frame update
    void Start()
    {
        array = new int[] { 3, 5, 1, 4, 7, 2, 8, 9, 10, 6 };
        print("Empieza");
        StartCoroutine(debug());
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
        for (int i = 1; i < array.Length; i++)
            for (int p = array.Length - 1; p >= i; p--)
            {
                if (array[p - 1] > array[p])
                {
                    aux = array[p - 1];
                    print("aux = array[p - 1];");
                    code.text = code.text.Replace("aux = array[p - 1];", "<b>aux = array[p - 1];</b>");
                    yield return new WaitForSeconds(0.5f);
                    code.text = code.text.Replace("<b>aux = array[p - 1];</b>", "aux = array[p - 1];");

                    array[p - 1] = array[p];
                    print("array[p - 1] = array[p];");
                    code.text = code.text.Replace("array[p - 1] = array[p];", "<b>array[p - 1] = array[p];</b>");
                    yield return new WaitForSeconds(0.5f);
                    code.text = code.text.Replace("<b>array[p - 1] = array[p];</b>", "array[p - 1] = array[p];");

                    array[p] = aux;
                    print("array[p] = aux;");
                    code.text = code.text.Replace("array[p] = aux;", "<b>array[p] = aux;</b>");
                    yield return new WaitForSeconds(0.5f);
                    code.text = code.text.Replace("<b>array[p] = aux;</b>", "array[p] = aux;");
                }
            }
        print("acaba");
    }
}
