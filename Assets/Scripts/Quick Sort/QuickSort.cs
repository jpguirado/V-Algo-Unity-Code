using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class QuickSort : MonoBehaviour
{
    //Number of elements of the array
    public int NElements;

    //Messy array
    int[] ArrayOriginal;

    //Sorted array
    int[] ArraySorted;

    //Time to wait between each step
    public float StopSeconds;

    //Array graphic element GameObject
    public GameObject ArrayGraphicElement;

    //Game Object empty that will represent the original position of the graphic array elements
    public GameObject OriginalPositionPrefab;

    //Lista of the array graphic elements
    public List<GameObject> ArrayListGraphic;

    //List with the original positions of the graphic elemets of the array. This list will remain immutable
    public List<GameObject> OriginalPositions;

    //Determine if the execution is paused
    public bool Paused;

    //List of states to control the execution
    public List<QuickSortState> StatesList;

    //Current step number
    public int StepCounter;

    //Determine if there is movement between elements
    private bool Stopped;

    //Sprite play
    public Sprite SpritePlay;

    //Sprite pause
    public Sprite SpritePause;

    //Step Over Button
    public Button StepOverButton;

    //Step Back Button
    public Button StepBackButton;

    //Image of the code
    public Image CodeImage;

    //Sprite original code
    public Sprite SpriteCodeImage;

    //Sprite image pivot 0
    public Sprite SpriteCodePivot0;

    //Sprite image left half 1
    public Sprite SpriteCodeLeftHalf1;

    //Sprite image right half 2
    public Sprite SpriteCodeRightHalf2;

    //Sprite image swap 1
    public Sprite SpriteCodeSwap1;

    //Sprite image swap 2
    public Sprite SpriteCodeSwap2;

    //Determines if one step is being executed
    private bool ExecutingStep;

    //Show in the screen the number of current step
    public TextMeshProUGUI StepCounterText;

    //Main Color of the project
    public Color MainProjectColor;

    //Image of the play/pause button
    public Image PlayPauseImage;

    //Distance between elements
    public float DistanceBetweenElements;

    //Speed Slider
    public Slider SpeedSlider;

    //Language Manager
    public QuickSortLanguageManager QuickSortLanguageManager;

    //Distance that will the elements move in the state 1
    public float MoveDownDistance;

    //Reference of Y position to move down the array graphic elements
    public RectTransform PosYReferenceMoveDown;

    //Distance of displacement of the bars with respect to the center
    public float LateralShift;

    //Color to higlight pivot
    public Color PivotColor;

    //Left Half Color
    public Color LeftHalfColor;

    //Right Half Color
    public Color RightHalfColor;

    //The element is in his sorted position
    public Color FinalPositionColor;

    //Dtermines if the element is in his final position
    public List<bool> ElementsPlaced;

    // Start is called before the first frame update
    void Start()
    {
        ////Set the language
        QuickSortLanguageManager.SetLanguage();

        ////Set variables
        Stopped = true;
        StepCounter = 0;
        StatesList = new List<QuickSortState>();
        ExecutingStep = false;
        NElements = MenuConfiguracion.NumElementos;

        //If we start directly from the execution scene, this has to be removed in the final build
        if (NElements == 0)
        {
            NElements = 20;
        }

        ArrayOriginal = CreateRandomArray(NElements);
        InstantiateGraphicArray(ArrayOriginal, GetScale(), true);
        ExecuteAndCreateStates(ArrayOriginal);
        RestoreElementsPlaced();
    }

    //Restore the list of bools with the index oh the array elements placed
    private void RestoreElementsPlaced()
    {
       for(int i = 0; i<ElementsPlaced.Count; i++)
        {
            ElementsPlaced[i] = false;
        }
    }

    //Execute the algorith and creates the states to step reproduction
    private void ExecuteAndCreateStates(int[] array)
    {
        ArraySorted = (int[])array.Clone();
        ArraySorted = quicksort(ArraySorted, 0, ArraySorted.Length - 1);

        for (int x = 0; x < ElementsPlaced.Count; x++)
        {
            if (ElementsPlaced[x] == false)
            {
                StatesList.Add(new QuickSortState()
                {
                    State = 4,
                    indexToHighlight = x,
                    NextElements = true
                });
            }
        }
    }

    public int[] quicksort(int[] array, int i, int j)
    {
        PivoteResult result = new PivoteResult();
        if (i < j)
        {
            result = pivote(array, i, j);
            array = quicksort(array, i, result.i_decrec - 1);
            array = quicksort(array, result.i_decrec + 1, j);
        }
        return array;
    }

    public PivoteResult pivote(int[] array, int ini, int fin)
    {
        if(ini > 0)
        {
            for(int x = ini - 1; x >= 0; x--)
            {
                if (ElementsPlaced[x] == false)
                {
                    StatesList.Add(new QuickSortState()
                    {
                        State = 4,
                        indexToHighlight = x,
                        NextElements = true
                    });
                    ElementsPlaced[x] = true;
                }
            }
        }

        StatesList.Add(new QuickSortState()
        {
            State = 0,
            IndexPivot = ini
        });

        int ValorPivote = array[ini];

        int i_crec = ini + 1;
        int i_decrec = fin;
        bool done = false;
        while (!done)
        {
            while (i_crec <= i_decrec && array[i_crec] <= ValorPivote)
            {
                StatesList.Add(new QuickSortState()
                {
                    State = 1,
                    indexToHighlight = i_crec
                });
                i_crec = i_crec + 1;
            }
            while (array[i_decrec] >= ValorPivote && i_decrec >= i_crec)
            {
                StatesList.Add(new QuickSortState()
                {
                    State = 2,
                    indexToHighlight = i_decrec
                });
                i_decrec = i_decrec - 1;
            }
            if (i_decrec < i_crec)
                done = true;
            else
                intercambiar(array, i_crec, i_decrec,1);
        }

        intercambiar(array, ini, i_decrec,2);

        return new PivoteResult()
        {
            Array = array,
            i_decrec = i_decrec
        };
    }

    private void intercambiar(int[] array, int a, int b, int type)
    {
        if (a != b)
        {
            //Swap elements
            StatesList.Add(new QuickSortState()
            {
                State = 3,
                LeftElementIndex = a,
                RightElementIndex = b,
                SwapType = type
            });
        }

        if (type == 2)
        {
            if (b - a == 1)//Both in correct positions
            {
                //Highlitgh the elemts in the correct position
                StatesList.Add(new QuickSortState()
                {
                    State = 4,
                    indexToHighlight = b,
                    SwapType = type
                });
                ElementsPlaced[b] = true;
                //Highlitgh the elemts in the correct position
                StatesList.Add(new QuickSortState()
                {
                    State = 4,
                    indexToHighlight = a,
                    NextElements = true,
                    SwapType = type
                });
                ElementsPlaced[a] = true;
            }
            else
            {
                //Highlitgh the elemts in the correct position
                StatesList.Add(new QuickSortState()
                {
                    State = 4,
                    indexToHighlight = b,
                    SwapType = type
                });
                ElementsPlaced[b] = true;
            }
        }

        int aux = array[a];
        array[a] = array[b];
        array[b] = aux;
    }

    //Instatiate the graphic array elements and put them in a list
    private void InstantiateGraphicArray(int[] array, Vector3 currentScale, bool firstTime)
    {
        //Array with the distance between elements depending on the number of elements selected by the user
        int[] arrayDistanciaElementos = new int[] { 40, 37, 34, 31, 28, 25, 22, 19, 16, 13, 10 };
        DistanceBetweenElements = arrayDistanciaElementos[NElements - 10];

        for (int i = 0; i < array.Length; i++)
        {
            GameObject instantiated = Instantiate(ArrayGraphicElement);
            instantiated.name = array[i].ToString();
            ArrayListGraphic.Add(instantiated);
            if(firstTime)
                ElementsPlaced.Add(false);
            instantiated.transform.SetParent(this.transform);
            float posicionX = (NElements * 80 + (NElements - 1) * DistanceBetweenElements) / 2;
            instantiated.GetComponent<RectTransform>().localPosition = new Vector3(-posicionX + i * (40 * 2 + DistanceBetweenElements) + 40 + LateralShift, 100.0f, 0.0f);

            //Restore the scale
            if (currentScale != Vector3.zero)
                instantiated.transform.localScale = currentScale;

            instantiated.GetComponentInChildren<TextMeshProUGUI>().text = array[i].ToString();
            RectTransform BarHeight = instantiated.transform.Find("Bar").GetComponent<RectTransform>();
            BarHeight.sizeDelta = new Vector2(40, array[i] * (90 / (array.Length - 1)) + 10);

            instantiated.transform.Find("Bar").GetComponent<Image>().color = MainProjectColor;
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
        for (int i = numElements - 1; i > 0; i--)
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
        if (PlayerPrefs.GetString("language") == "ESPAÑOL")
            StepCounterText.text = "Paso: " + StepCounter + "/" + StatesList.Count;
        else
            StepCounterText.text = "Step: " + StepCounter + "/" + StatesList.Count;

        //Get the speed
        StopSeconds = SpeedSlider.value;

        GameObject auxGameObject;

        //If last step, pause
        if (StepCounter == StatesList.Count)
        {
            Paused = true;
            PlayPauseImage.sprite = SpritePlay;
        }

        //Automatic mode
        if (!Paused && Stopped && StepCounter <= (StatesList.Count - 1))
        {
            //If state is highlight pivot
            if (StatesList[StepCounter].State == 0)
            {
                //Routine that waits for time and highlights the text of the running code
                StartCoroutine(WaitTimeHighlightPivot(StopSeconds,false,false));
            }

            //If state is highlight Left Half Element
            if (StatesList[StepCounter].State == 1)
            {
                //Routine that waits for time and highlights the text of the running code
                StartCoroutine(WaitTimeHighlight(StopSeconds,LeftHalfColor,false));
            }

            //If state is highlight Right Half Element
            if (StatesList[StepCounter].State == 2)
            {
                //Routine that waits for time and highlights the text of the running code
                StartCoroutine(WaitTimeHighlight(StopSeconds,RightHalfColor,false));
            }

            //If state is elements swap
            if (StatesList[StepCounter].State == 3)
            {
                Vector3 PosRight = ArrayListGraphic[StatesList[StepCounter].RightElementIndex].transform.position;
                Vector3 PosLeft = ArrayListGraphic[StatesList[StepCounter].LeftElementIndex].transform.position;

                //Move Elements
                StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].LeftElementIndex], PosRight, StopSeconds * 0.5f));
                StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].RightElementIndex], PosLeft, StopSeconds * 0.5f));

                //Routine that waits for time and highlights the text of the running code
                StartCoroutine(WaitTimeSwap(StopSeconds,false));

                //Make changes to our ArrayListGraphic
                auxGameObject = ArrayListGraphic[StatesList[StepCounter].LeftElementIndex];
                ArrayListGraphic[StatesList[StepCounter].LeftElementIndex] = ArrayListGraphic[StatesList[StepCounter].RightElementIndex];
                ArrayListGraphic[StatesList[StepCounter].RightElementIndex] = auxGameObject;
            }

            //If state is highlight the element in correct position
            if (StatesList[StepCounter].State == 4)
            {
                //Routine that waits for time and highlights the text of the running code
                StartCoroutine(WaitTimeHighlightElementCorrectPosition(StopSeconds,false));
            }

            StepCounter += 1;
        }

        //Step Mode
        if (Stopped)
        {
            //Step Over
            StepOverButton.onClick.AddListener(delegate
            {
                if (Paused && StepCounter < (StatesList.Count))
                {
                    if (!ExecutingStep)
                    {
                        ExecutingStep = true;

                        CleanHighLightedCode();

                        //If state is highlight pivot
                        if (StatesList[StepCounter].State == 0)
                        {
                            CodeImage.sprite = SpriteCodePivot0;
                            //Routine that waits for time and highlights the text of the running code
                            StartCoroutine(WaitTimeHighlightPivot(0,false,true));
                        }

                        //If state is highlight Left Half Element
                        if (StatesList[StepCounter].State == 1)
                        {
                            CodeImage.sprite = SpriteCodeLeftHalf1;
                            //Routine that waits for time and highlights the text of the running code
                            StartCoroutine(WaitTimeHighlight(0,LeftHalfColor,true));
                        }

                        //If state is highlight Right Half Element
                        if (StatesList[StepCounter].State == 2)
                        {
                            CodeImage.sprite = SpriteCodeRightHalf2;
                            //Routine that waits for time and highlights the text of the running code
                            StartCoroutine(WaitTimeHighlight(0,RightHalfColor,true));
                        }

                        //If state is elements swap
                        if (StatesList[StepCounter].State == 3)
                        {
                            if (StatesList[StepCounter].SwapType == 1)
                                CodeImage.sprite = SpriteCodeSwap1;
                            else
                                CodeImage.sprite = SpriteCodeSwap2;

                            Vector3 PosRight = ArrayListGraphic[StatesList[StepCounter].RightElementIndex].transform.position;
                            Vector3 PosLeft = ArrayListGraphic[StatesList[StepCounter].LeftElementIndex].transform.position;

                            //Move Elements
                            StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].LeftElementIndex], PosRight, StopSeconds * 0.5f));
                            StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].RightElementIndex], PosLeft, StopSeconds * 0.5f));

                            //Routine that waits for time and highlights the text of the running code
                            StartCoroutine(WaitTimeSwap(StopSeconds,true));

                            //Make changes to our ArrayListGraphic
                            auxGameObject = ArrayListGraphic[StatesList[StepCounter].LeftElementIndex];
                            ArrayListGraphic[StatesList[StepCounter].LeftElementIndex] = ArrayListGraphic[StatesList[StepCounter].RightElementIndex];
                            ArrayListGraphic[StatesList[StepCounter].RightElementIndex] = auxGameObject;
                        }

                        //If state is highlight the element in correct position
                        if (StatesList[StepCounter].State == 4)
                        {
                            if (StatesList[StepCounter].SwapType == 1)
                                CodeImage.sprite = SpriteCodeSwap1;
                            else
                                CodeImage.sprite = SpriteCodeSwap2;
                            //Routine that waits for time and highlights the text of the running code
                            StartCoroutine(WaitTimeHighlightElementCorrectPosition(0,true));
                        }

                        StepCounter += 1;
                    }
                }
            });

            //Step back
            StepBackButton.onClick.AddListener(delegate
            {
                if (Paused)
                {
                    if (!ExecutingStep && StepCounter > 0)
                    {
                        ExecutingStep = true;
                        StepCounter -= 1;

                        CleanHighLightedCode();

                        //If state is highlight pivot
                        if (StatesList[StepCounter].State == 0)
                        {
                            CodeImage.sprite = SpriteCodePivot0;
                            //Routine that waits for time and highlights the text of the running code
                            StartCoroutine(WaitTimeHighlightPivot(0,true,true));
                        }

                        //If state is highlight Left Half Element
                        if (StatesList[StepCounter].State == 1)
                        {
                            CodeImage.sprite = SpriteCodeLeftHalf1;
                            //Routine that waits for time and highlights the text of the running code
                            StartCoroutine(WaitTimeHighlight(0,MainProjectColor,true));
                        }

                        //If state is highlight Right Half Element
                        if (StatesList[StepCounter].State == 2)
                        {
                            CodeImage.sprite = SpriteCodeRightHalf2;
                            //Routine that waits for time and highlights the text of the running code
                            StartCoroutine(WaitTimeHighlight(0,MainProjectColor,true));
                        }

                        //If state is elements swap
                        if (StatesList[StepCounter].State == 3)
                        {
                            if (StatesList[StepCounter].SwapType == 1)
                                CodeImage.sprite = SpriteCodeSwap1;
                            else
                                CodeImage.sprite = SpriteCodeSwap2;

                            Vector3 PosRight = ArrayListGraphic[StatesList[StepCounter].RightElementIndex].transform.position;
                            Vector3 PosLeft = ArrayListGraphic[StatesList[StepCounter].LeftElementIndex].transform.position;

                            //Move Elements
                            StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].LeftElementIndex], PosRight, StopSeconds * 0.5f));
                            StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].RightElementIndex], PosLeft, StopSeconds * 0.5f));

                            //Routine that waits for time and highlights the text of the running code
                            StartCoroutine(WaitTimeSwap(StopSeconds,true));

                            //Make changes to our ArrayListGraphic
                            auxGameObject = ArrayListGraphic[StatesList[StepCounter].LeftElementIndex];
                            ArrayListGraphic[StatesList[StepCounter].LeftElementIndex] = ArrayListGraphic[StatesList[StepCounter].RightElementIndex];
                            ArrayListGraphic[StatesList[StepCounter].RightElementIndex] = auxGameObject;
                        }

                        //If state is highlight the element in correct position
                        if (StatesList[StepCounter].State == 4)
                        {
                            if (StatesList[StepCounter].SwapType == 1)
                                CodeImage.sprite = SpriteCodeSwap1;
                            else
                                CodeImage.sprite = SpriteCodeSwap2;
                            //Routine that waits for time and highlights the text of the running code
                            StartCoroutine(WaitTimeHighlightElementCorrectPositionStepBack(0));
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
    private IEnumerator WaitTimeSwap(float waitTime, bool stepMode)
    {
        Stopped = false;

        int step = StepCounter;

        if (StatesList[step].SwapType == 1)
            CodeImage.sprite = SpriteCodeSwap1;
        else
            CodeImage.sprite = SpriteCodeSwap2;

        yield return new WaitForSeconds(waitTime);

        if(!stepMode)
            CodeImage.sprite = SpriteCodeImage;

        Stopped = true;
        ExecutingStep = false;
    }

    //Wait time and highlighted the pivot
    private IEnumerator WaitTimeHighlightPivot(float waitTime, bool stepBack, bool stepMode)
    {
        Stopped = false;

        int step = StepCounter;

        CodeImage.sprite = SpriteCodePivot0;

        //Highlight Pivot
        if (!stepBack)
            ArrayListGraphic[StatesList[step].IndexPivot].transform.Find("Bar").GetComponent<Image>().color = PivotColor;
        else
            ArrayListGraphic[StatesList[step].IndexPivot].transform.Find("Bar").GetComponent<Image>().color = MainProjectColor;

        yield return new WaitForSeconds(waitTime);

        if(!stepMode)
            CodeImage.sprite = SpriteCodeImage;

        Stopped = true;
        ExecutingStep = false;
    }

    //Wait time and step back Highlight Element Correct Position
    private IEnumerator WaitTimeHighlightElementCorrectPositionStepBack(float waitTime)
    {
        Stopped = false;

        int step = StepCounter;

        //Highlight Element in correct position
        if(StatesList[step].NextElements)
            ArrayListGraphic[StatesList[step].indexToHighlight].transform.Find("Bar").GetComponent<Image>().color = MainProjectColor;
        else
            ArrayListGraphic[StatesList[step].indexToHighlight].transform.Find("Bar").GetComponent<Image>().color = PivotColor;
        
        ElementsPlaced[StatesList[step].indexToHighlight] = false;

        //Coloring elements on the left until we found one in correct position
        for (int i = StatesList[StepCounter].indexToHighlight - 1; i >= 0; i--)
        {
            if (ElementsPlaced[i])
                break;
            else
                ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color = LeftHalfColor;
        }

        //Coloring elements on the right until we found one in correct position
        for (int i = StatesList[StepCounter].indexToHighlight + 1; i < ArrayListGraphic.Count; i++)
        {
            if (ElementsPlaced[i])
                break;
            else
                ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color = RightHalfColor;
        }

        yield return new WaitForSeconds(waitTime);

        Stopped = true;
        ExecutingStep = false;
    }

    //Wait time and highlighted the pivot
    private IEnumerator WaitTimeHighlightElementCorrectPosition(float waitTime, bool stepMode)
    {
        Stopped = false;

        int step = StepCounter;

        if (StatesList[step].SwapType == 1)
            CodeImage.sprite = SpriteCodeSwap1;
        else
            CodeImage.sprite = SpriteCodeSwap2;

        //Restore colors
        for (int i = 0; i < ArrayListGraphic.Count; i++)
        {
            if (!ElementsPlaced[i])
                ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color = MainProjectColor;
        }

        //Highlight Element in correct position
        ArrayListGraphic[StatesList[step].indexToHighlight].transform.Find("Bar").GetComponent<Image>().color = FinalPositionColor;
        ElementsPlaced[StatesList[step].indexToHighlight] = true;


        yield return new WaitForSeconds(waitTime);

        if(!stepMode)
            CodeImage.sprite = SpriteCodeImage;

        Stopped = true;
        ExecutingStep = false;
    }

    //Wait time and highlighted the left half
    private IEnumerator WaitTimeHighlight(float waitTime, Color color, bool stepMode)
    {
        Stopped = false;

        int step = StepCounter;

        //Highlight Pivot
        if(!ElementsPlaced[StatesList[step].indexToHighlight])
            ArrayListGraphic[StatesList[step].indexToHighlight].transform.Find("Bar").GetComponent<Image>().color = color;
        else
        {
            print("Debug");
        }

        //Highlight code
        if(color == LeftHalfColor)
            CodeImage.sprite = SpriteCodeLeftHalf1;
        else
            CodeImage.sprite = SpriteCodeRightHalf2;

        yield return new WaitForSeconds(waitTime);

        if(!stepMode)
            CodeImage.sprite = SpriteCodeImage;

        Stopped = true;
        ExecutingStep = false;
    }

    //Resume execution
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

    //Restart execution
    public void RestartExecution()
    {
        Paused = true;

        PlayPauseImage.sprite = SpritePlay;

        StepCounter = 0;

        Vector3 localScale = ArrayListGraphic[0].transform.localScale;

        //Delete the array graphic elements
        for (int i = 0; i < ArrayListGraphic.Count; i++)
        {
            Destroy(ArrayListGraphic[i]);
        }

        ArrayListGraphic.Clear();

        for (int i = 0; i < OriginalPositions.Count; i++)
        {
            Destroy(OriginalPositions[i]);
        }

        OriginalPositions.Clear();

        InstantiateGraphicArray(ArrayOriginal, localScale, false);

        //Reset Element Placed
        for (int i = 0; i < ArrayListGraphic.Count; i++)
        {
            ElementsPlaced[i] = false;
        }
    }

    //End execution
    public void EndExecution()
    {
        Paused = true;

        PlayPauseImage.sprite = SpritePlay;

        StepCounter = StatesList.Count;

        Vector3 escalaActual = ArrayListGraphic[0].transform.localScale;


        for (int i = 0; i < ArrayListGraphic.Count; i++)
        {
            Destroy(ArrayListGraphic[i]);
        }

        ArrayListGraphic.Clear();

        InstantiateGraphicArray(ArraySorted, escalaActual, false);

        //We color all the array elements of the final color and all the elements are sorted
        for (int i = 0; i < ArrayListGraphic.Count; i++)
        {
            ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color = FinalPositionColor;
            ElementsPlaced[i] = true;
        }
    }

    //Return to the main menu
    public void ReturnToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    //Gets the actual scale
    private Vector3 GetScale()
    {
        Vector3 scale = this.transform.localScale;

        scale.x += 1;
        scale.y += 1;
        scale.z += 1;

        return scale;
    }
}

//Class to return the result of the pivote function
public class PivoteResult
{
    public int[] Array;
    public int i_decrec;
}

//Class that represent the states of the execution
public class QuickSortState
{
    /*
     * State 0 - Highlight pivot
     * State 1 - Highlight left half
     * State 2 - Highlight right half
     * State 3 - Swap element
     * State 4 - Highlight sorted number
     */

    public int State;

    //Highlighted pivot - 0 && 4
    public int IndexPivot;

    //Left and Right Half- 1 && 2
    public int indexToHighlight;

    //Swap Elements - 3
    public int LeftElementIndex { get; set; }

    public int RightElementIndex { get; set; }

    public int SwapType;

    public bool NextElements;
}
