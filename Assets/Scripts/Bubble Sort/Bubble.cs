using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TMPro;

public class Bubble : MonoBehaviour
{
    //Number of elements of the array
    public int NElements;

    //Messy array
    int[] ArrayOriginal;

    //Sorted Array
    int[] ArraySorted;

    //Time to wait between each step
    public float StopSeconds;

    //Array graphic element GameObject
    public GameObject ArrayGraphicElement;

    //Lista of the array graphic elements
    public List<GameObject> ArrayListGraphic;

    //Determine if the execution is paused
    public bool Paused;

    //List of states to control the execution
    public List<BubbleState> StatesList;

    //Current step number
    public int StepCounter;

    //Determine if there is movement between elements
    private bool Stopped;

    //Sprite play
    public Sprite SpritePlay;

    //Sprite pause
    public Sprite SpritePause;

    //stepOver button
    public Button StepOverButton;

    //Step Back Button
    public Button StepBackButton;

    //Restart Button
    public Button RestartButton;

    //Code Image
    public Image CodeImage;

    //Sprite original code
    public Sprite SpriteCodeImage;

    //Sprite image code if
    public Sprite SpriteCodeIfImage;

    //Sprite image code swap
    public Sprite SpriteCodeSwapImage;

    //Determines if one step is being executed
    private bool ExecutingStep;

    //Show in the screen the number of current step
    public TextMeshProUGUI StepCounterText;

    //Original color of graphic array elements
    public Color MainProjectColor;

    //Highlighted Color
    public Color HighlightedColor;

    public Image PlayPauseImage;

    //Distance between elements
    public float distanceBetweenElements;

    //Speed Slider
    public Slider SpeedSlider;

    public BubbleLanguageManager BubbleLanguageManager;

    // Start is called before the first frame update
    void Start()
    {
        //Set the language
        BubbleLanguageManager.SetLanguage();

        //Set variables
        Stopped = true;
        StepCounter = 0;
        StatesList = new List<BubbleState>();
        ExecutingStep = false;
        NElements = MenuConfiguracion.NumElementos;

        //If we start directly from the execution scene, this has to be removed in the final build
        if (NElements == 0)
        {
            NElements = 10;
        }

        ArrayOriginal = CreateRandomArray(NElements);
        ExecuteAndCreateStates(ArrayOriginal);
        InstantiateGraphicArray(ArrayOriginal, GetScale());
    }

    //Execute the algorith and creates the states to step reproduction
    private void ExecuteAndCreateStates(int[] array)
    {
        ArraySorted = (int[])array.Clone();
        int aux;
        for (int i = 1; i < ArraySorted.Length; i++)
            for (int p = ArraySorted.Length - 1; p >= i; p--)
            {
                //Create If state
                StatesList.Add(new BubbleState()
                {
                    State = 0,
                    LeftElementIndex = p - 1,
                    RightElementIndex = p
                });
                if (ArraySorted[p - 1] > ArraySorted[p])
                {
                    //Create Swap State
                    StatesList.Add(new BubbleState()
                    {
                        State = 1,
                        LeftElementIndex = p - 1,
                        RightElementIndex = p
                    });
                    aux = ArraySorted[p - 1];
                    ArraySorted[p - 1] = ArraySorted[p];
                    ArraySorted[p] = aux;
                }
            }
    }

    //Instatiate the graphic array elements and put them in a list
    private void InstantiateGraphicArray(int[] array, Vector3 escalaActual)
    {
        //Array with the distance between elements depending on the number of elements selected by the user
        int[] arrayDistanciaElementos = new int[] { 40, 37, 34, 31, 28, 25, 22, 19, 16, 13, 10 };
        distanceBetweenElements = arrayDistanciaElementos[NElements-10];

        for (int i = 0; i < array.Length; i++)
        {
            GameObject instantiated = Instantiate(ArrayGraphicElement);
            instantiated.name = array[i].ToString();
            ArrayListGraphic.Add(instantiated);
            instantiated.transform.SetParent(this.transform);
            float posicionX = (NElements * 80 + (NElements - 1) * distanceBetweenElements) / 2;
            instantiated.GetComponent<RectTransform>().localPosition = new Vector3(-posicionX + i * (40 * 2 + distanceBetweenElements) + 40, 0.0f, 0.0f);

            //Restore scale
            if (escalaActual != Vector3.zero)
                instantiated.transform.localScale = escalaActual;

            instantiated.GetComponentInChildren<TextMeshProUGUI>().text = array[i].ToString();

            RectTransform tamañoBarra = instantiated.transform.Find("Bar").GetComponent<RectTransform>();
            instantiated.transform.Find("Bar").GetComponent<Image>().color = MainProjectColor;
            tamañoBarra.sizeDelta = new Vector2(40, array[i] * (90 / (array.Length - 1)) + 10);
        }
    }

    //Function that creates a random array from the number of elements
    private int[] CreateRandomArray(int numElements)
    {
        //Create sorted list
        List<int> sortedList = new List<int>();
        for (int i = 0; i < numElements; i++)
            sortedList.Add(i);

        //Mess it
        for (int i = numElements - 1; i>0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int aux = sortedList[i];
            sortedList[i] = sortedList[randomIndex];
            sortedList[randomIndex] = aux;
        }

        return sortedList.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        //Text of the step
        if(PlayerPrefs.GetString("language") == "ESPAÑOL")
            StepCounterText.text =  "Paso: " + StepCounter + "/" + StatesList.Count;
        else
            StepCounterText.text = "Step: " + StepCounter + "/" + StatesList.Count;

        //Get the speed
        StopSeconds = SpeedSlider.value;

        GameObject auxGameObject;
        
        //AutomaticMode
        if (!Paused && Stopped && StepCounter <= (StatesList.Count - 1))
        {
            CleanHighLightedCode();

            //If state is elements swap
            if (StatesList[StepCounter].State == 1)
            {
                Vector3 PosRight = ArrayListGraphic[StatesList[StepCounter].RightElementIndex].transform.position;
                Vector3 PosLeft = ArrayListGraphic[StatesList[StepCounter].LeftElementIndex].transform.position;

                //Move Elements
                StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].LeftElementIndex], PosRight, StopSeconds * 0.5f));
                StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].RightElementIndex], PosLeft, StopSeconds * 0.5f));

                //Routine that waits for time and highlights the text of the running code
                StartCoroutine(WaitTimeSwap(StopSeconds));

                //Make changes to our ArrayListGraphic
                auxGameObject = ArrayListGraphic[StatesList[StepCounter].LeftElementIndex];
                ArrayListGraphic[StatesList[StepCounter].LeftElementIndex] = ArrayListGraphic[StatesList[StepCounter].RightElementIndex];
                ArrayListGraphic[StatesList[StepCounter].RightElementIndex] = auxGameObject;
            }

            //If state is IF
            if (StatesList[StepCounter].State == 0)
            {
                //Routine that waits for time and highlights the text of the running code
                StartCoroutine(WaitTimeIf(StopSeconds));
            }

            StepCounter += 1;
        }

        //Step Mode
        if (Paused && Stopped && StepCounter <= (StatesList.Count - 1))
        {
            //Step Over
            StepOverButton.onClick.AddListener(delegate
            {
                if (Paused)
                {
                    if (!ExecutingStep)
                    {
                        ExecutingStep = true;

                        CleanHighLightedCode();

                        //If state is elements swap
                        if (StatesList[StepCounter].State == 1)
                        {
                            CodeImage.sprite = SpriteCodeSwapImage;

                            //Highlighted graphic array elements
                            ArrayListGraphic[StatesList[StepCounter].LeftElementIndex].transform.Find("Bar").GetComponent<Image>().color = HighlightedColor;
                            ArrayListGraphic[StatesList[StepCounter].RightElementIndex].transform.Find("Bar").GetComponent<Image>().color = HighlightedColor;

                            Vector3 PosRight = ArrayListGraphic[StatesList[StepCounter].RightElementIndex].transform.position;
                            Vector3 PosLeft = ArrayListGraphic[StatesList[StepCounter].LeftElementIndex].transform.position;

                            //Move Elements
                            StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].LeftElementIndex], PosRight, StopSeconds * 0.5f));
                            StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].RightElementIndex], PosLeft, StopSeconds * 0.5f));

                            //Routine that waits for time and highlights the text of the running code
                            StartCoroutine(WaitTimeSwapStep(StopSeconds));

                            //Make changes to our ArrayListGraphic
                            auxGameObject = ArrayListGraphic[StatesList[StepCounter].LeftElementIndex];
                            ArrayListGraphic[StatesList[StepCounter].LeftElementIndex] = ArrayListGraphic[StatesList[StepCounter].RightElementIndex];
                            ArrayListGraphic[StatesList[StepCounter].RightElementIndex] = auxGameObject;
                        }
                        //If state is IF
                        if (StatesList[StepCounter].State == 0)
                        {
                            CodeImage.sprite = SpriteCodeIfImage;

                            //Highlighted graphic array elements
                            ArrayListGraphic[StatesList[StepCounter].LeftElementIndex].transform.Find("Bar").GetComponent<Image>().color = HighlightedColor;
                            ArrayListGraphic[StatesList[StepCounter].RightElementIndex].transform.Find("Bar").GetComponent<Image>().color = HighlightedColor;

                            //Routine that waits for time and highlights the text of the running code
                            StartCoroutine(WaitTimeIfStep(0));
                        }
                        StepCounter += 1;
                    }
                }
            });

            //Step Back
            StepBackButton.onClick.AddListener(delegate
            {
                if (Paused)
                {
                    if (!ExecutingStep && StepCounter > 0)
                    {
                        ExecutingStep = true;
                        StepCounter -= 1;

                        CleanHighLightedCode();

                        //Reproducir los estados

                        //If state is elements swap
                        if (StatesList[StepCounter].State == 1)
                        {

                            CodeImage.sprite = SpriteCodeSwapImage;

                            //Highlighted graphic array elements
                            ArrayListGraphic[StatesList[StepCounter].LeftElementIndex].transform.Find("Bar").GetComponent<Image>().color = HighlightedColor;
                            ArrayListGraphic[StatesList[StepCounter].RightElementIndex].transform.Find("Bar").GetComponent<Image>().color = HighlightedColor;

                            Vector3 PosRight = ArrayListGraphic[StatesList[StepCounter].RightElementIndex].transform.position;
                            Vector3 PosLeft = ArrayListGraphic[StatesList[StepCounter].LeftElementIndex].transform.position;

                            //Move Elements
                            StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].LeftElementIndex], PosRight, StopSeconds * 0.5f));
                            StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].RightElementIndex], PosLeft, StopSeconds * 0.5f));

                            //Routine that waits for time and highlights the text of the running code
                            StartCoroutine(WaitTimeSwapStep(StopSeconds));

                            //Make changes to our ArrayListGraphic
                            auxGameObject = ArrayListGraphic[StatesList[StepCounter].LeftElementIndex];
                            ArrayListGraphic[StatesList[StepCounter].LeftElementIndex] = ArrayListGraphic[StatesList[StepCounter].RightElementIndex];
                            ArrayListGraphic[StatesList[StepCounter].RightElementIndex] = auxGameObject;
                        }
                        //If state is IF
                        if (StatesList[StepCounter].State == 0)
                        {
                            CodeImage.sprite = SpriteCodeIfImage;

                            //Highlighted graphic array elements
                            ArrayListGraphic[StatesList[StepCounter].LeftElementIndex].transform.Find("Bar").GetComponent<Image>().color = HighlightedColor;
                            ArrayListGraphic[StatesList[StepCounter].RightElementIndex].transform.Find("Bar").GetComponent<Image>().color = HighlightedColor;

                            //Routine that waits for time and highlights the text of the running cod
                            StartCoroutine(WaitTimeIfStep(0));
                        }
                    }
                }
            });
        }
    }

    //Clean the highlighted code
    private void CleanHighLightedCode()
    {
        CodeImage.sprite = SpriteCodeImage;

        //Restore color of the bars
        for (int i = 0; i < ArrayListGraphic.Count; i++)
        {
            ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color = MainProjectColor;
        }
    }

    //Function that allows animate the permutations of the array
    private IEnumerator MoveToPosition(GameObject initialObject, Vector3 endPosition, float timeToMove)
    {
        float time = 0f;
        Vector3 startPosition = initialObject.transform.position;

        while (time < 1)
        {
            time += Time.deltaTime / timeToMove;
            initialObject.transform.position = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
        }
    }

    //Wait time and highlighted the part of swap elements
    private IEnumerator WaitTimeSwap(float waitTime)
    {
        Stopped = false;

        int step = StepCounter;

        CodeImage.sprite = SpriteCodeSwapImage;

        //Resaltar elementos graficos array
        ArrayListGraphic[StatesList[step].LeftElementIndex].transform.Find("Bar").GetComponent<Image>().color = HighlightedColor;
        ArrayListGraphic[StatesList[step].RightElementIndex].transform.Find("Bar").GetComponent<Image>().color = HighlightedColor;

        yield return new WaitForSeconds(waitTime);

        CodeImage.sprite = SpriteCodeImage;

        //Quitar resalte elementos graficos array
        ArrayListGraphic[StatesList[step].LeftElementIndex].transform.Find("Bar").GetComponent<Image>().color = MainProjectColor;
        ArrayListGraphic[StatesList[step].RightElementIndex].transform.Find("Bar").GetComponent<Image>().color = MainProjectColor;

        Stopped = true;
        ExecutingStep = false;
    }

    //Wait time and highlighted the part of IF
    private IEnumerator WaitTimeIf(float waitTime)
    {
        Stopped = false;

        int step = StepCounter;

        CodeImage.sprite = SpriteCodeIfImage;

        ArrayListGraphic[StatesList[step].LeftElementIndex].transform.Find("Bar").GetComponent<Image>().color = HighlightedColor;
        ArrayListGraphic[StatesList[step].RightElementIndex].transform.Find("Bar").GetComponent<Image>().color = HighlightedColor;

        yield return new WaitForSeconds(waitTime);

        CodeImage.sprite = SpriteCodeImage;

        ArrayListGraphic[StatesList[step].LeftElementIndex].transform.Find("Bar").GetComponent<Image>().color = MainProjectColor;
        ArrayListGraphic[StatesList[step].RightElementIndex].transform.Find("Bar").GetComponent<Image>().color = MainProjectColor;

        Stopped = true;
        ExecutingStep = false;
    }

    //Wait time and highlighted the part of swap elements in step mode
    private IEnumerator WaitTimeSwapStep(float waitTime)
    {
        Stopped = false;
        yield return new WaitForSeconds(waitTime);
        Stopped = true;
        ExecutingStep = false;
    }

    //Wait time and highlighted the part of IF in step mode
    private IEnumerator WaitTimeIfStep(float waitTime)
    {
        Stopped = false;
        yield return new WaitForSeconds(waitTime);
        Stopped = true;
        ExecutingStep = false;
    }

    //Resume Execution
    public void playPauseExecution()
    {
        //Is in play, go to pause
        if (PlayPauseImage.sprite.name == "Pause")
        {
            Paused = true;
            PlayPauseImage.sprite = SpritePlay;
            return;
        }

        //Is in pause, go play
        if (PlayPauseImage.sprite.name == "Play")
        {
            Paused = false;
            PlayPauseImage.sprite = SpritePause;
            return;
        }
    }

    //Restart Execution
    public void RestartExecution()
    {
        Paused = true;

        PlayPauseImage.sprite = SpritePlay;

        StepCounter = 0;

        Vector3 actualScale = ArrayListGraphic[0].transform.localScale;

        for (int i = 0; i < ArrayListGraphic.Count; i++)
        {
            Destroy(ArrayListGraphic[i]);
        }

        ArrayListGraphic.Clear();
        InstantiateGraphicArray(ArrayOriginal, actualScale);
    }

    //End Execution
    public void EndExecution()
    {
        Paused = true;

        PlayPauseImage.sprite = SpritePlay;

        StepCounter = StatesList.Count;

        Vector3 actualScale = ArrayListGraphic[0].transform.localScale;

        for (int i = 0; i < ArrayListGraphic.Count; i++)
        {
            Destroy(ArrayListGraphic[i]);
        }

        ArrayListGraphic.Clear();
        InstantiateGraphicArray(ArraySorted,actualScale);
    }
    
    //Return to main menu
    public void ReturnToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    //Gets actual scale
    private Vector3 GetScale()
    {
        Vector3 scale = this.transform.localScale;

        scale.x += 1;
        scale.y += 1;
        scale.z += 1;

        return scale;
    }
}

//Class that represent the states of the execution
public class BubbleState
{
    //Determina si se esta ejecutando el if o el swap
    //0 para el if, 1 para el swap
    public int State { get; set; }

    //Elemento izquierda
    public int LeftElementIndex { get; set; }

    //Elemento derecha
    public int RightElementIndex { get; set; }

}
