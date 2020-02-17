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

    //List with the color of each position of the array
    private List<Color> ColorList;

    //List with the color of each position of the array
    private List<Color> OriginalColorList;

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

    //Step Over button
    public Button StepBackButton;

    //Restart execution button
    public Button RestartButton;

    //Image of the code
    public Image CodeImage;

    //Sprite original code
    public Sprite SpriteCodeImage;

    //Sprite image remark code
    public Sprite SpriteCodeRemarkImage;

    //Sprite image move down code
    public Sprite SpriteCodeMoveDownImage;

    //Sprite image move up code
    public Sprite SpriteCodeMoveUpImage;

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
    public MergeSortLanguageManager MergeSortLanguageManager;

    //Distance that will the elements move in the state 1
    public float MoveDownDistance;

    //Reference of Y position to move down the array graphic elements
    public RectTransform PosYReferenceMoveDown;

    //Distance of displacement of the bars with respect to the center
    public float LateralShift;

    // Start is called before the first frame update
    void Start()
    {
        ////Set the language
        //MergeSortLanguageManager.SetLanguage();

        ////Set variables
        Stopped = true;
        StepCounter = 0;
        StatesList = new List<QuickSortState>();
        ExecutingStep = false;
        //NElements = MenuConfiguracion.NumElementos;
        //OriginalColorList = new List<Color>();
        //ColorList = new List<Color>();

        //If we start directly from the execution scene, this has to be removed in the final build
        if (NElements == 0)
        {
            NElements = 15;
        }

        //ArrayOriginal = CreateRandomArray(NElements);
        ArrayOriginal = new int[] { 8, 7, 6, 5, 4, 3, 2, 1, 10, 9 };
        InstantiateGraphicArray(ArrayOriginal, GetScale(), true);
        ExecuteAndCreateStates(ArrayOriginal);
    }

    //Execute the algorith and creates the states to step reproduction
    private void ExecuteAndCreateStates(int[] array)
    {
        ArraySorted = (int[])array.Clone();
        ArraySorted = quicksort(ArraySorted, 0, ArraySorted.Length - 1);
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
        int ValorPivote = array[ini];
        int i_crec = ini + 1;
        int i_decrec = fin;
        bool done = false;
        while (!done)
        {
            while (i_crec <= i_decrec && array[i_crec] <= ValorPivote)
                i_crec = i_crec + 1;
            while (array[i_decrec] >= ValorPivote && i_decrec >= i_crec)
                i_decrec = i_decrec - 1;
            if (i_decrec < i_crec)
                done = true;
            else
                intercambiar(array, i_crec, i_decrec);
        }
        intercambiar(array, ini, i_decrec);
        return new PivoteResult()
        {
            Array = array,
            i_decrec = i_decrec
        };
    }

    private void intercambiar(int[] array, int a, int b)
    {
        //Swap elements
        StatesList.Add(new QuickSortState()
        {
            State = 3,
            LeftElementIndex = a,
            RightElementIndex = b,
        });


        int aux = array[a];
        array[a] = array[b];
        array[b] = aux;
    }

    //Instatiate the graphic array elements and put them in a list
    private void InstantiateGraphicArray(int[] array, Vector3 currentScale, bool CreateColorList)
    {
        //Array with the distance between elements depending on the number of elements selected by the user
        int[] arrayDistanciaElementos = new int[] { 40, 35, 30, 25, 20, 15 };
        DistanceBetweenElements = arrayDistanciaElementos[NElements - 10];

        for (int i = 0; i < array.Length; i++)
        {
            GameObject instantiated = Instantiate(ArrayGraphicElement);
            instantiated.name = array[i].ToString();
            ArrayListGraphic.Add(instantiated);
            instantiated.transform.SetParent(this.transform);
            float posicionX = (NElements * 80 + (NElements - 1) * DistanceBetweenElements) / 2;
            instantiated.GetComponent<RectTransform>().localPosition = new Vector3(-posicionX + i * (40 * 2 + DistanceBetweenElements) + 40 + LateralShift, 100.0f, 0.0f);

            //Restore the scale
            if (currentScale != Vector3.zero)
                instantiated.transform.localScale = currentScale;


            instantiated.GetComponentInChildren<TextMeshProUGUI>().text = array[i].ToString();
            RectTransform BarHeight = instantiated.transform.Find("Bar").GetComponent<RectTransform>();
            BarHeight.sizeDelta = new Vector2(40, array[i] * (90 / (array.Length - 1)) + 10);

            //if (CreateColorList)
            //{
            //    //Random colors because this algorithm at the start creates partitions of size 1
            //    Color RandomColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            //    instantiated.transform.Find("Bar").GetComponent<Image>().color = RandomColor;
            //    ColorList.Add(RandomColor);
            //    OriginalColorList.Add(RandomColor);
            //}
            //else
            //{
            //    instantiated.transform.Find("Bar").GetComponent<Image>().color = OriginalColorList[i];
            //}

            instantiated.transform.Find("Bar").GetComponent<Image>().color = MainProjectColor;

            ////Save the original position to help with the movements
            //GameObject originalPosition = Instantiate(OriginalPositionPrefab);
            //originalPosition.name = "Original Position " + i;
            //OriginalPositions.Add(originalPosition);
            //originalPosition.transform.SetParent(this.transform);
            //Vector3 position = ArrayListGraphic[i].transform.position;
            //originalPosition.transform.position = position;
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

        //Automatic mode
        if (!Paused && Stopped && StepCounter <= (StatesList.Count - 1))
        {
            ////Remark Elements
            //if (StatesList[StepCounter].State == 0)
            //{
            //    //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
            //    StartCoroutine(WaitTimeRemark(StopSeconds));
            //}

            ////Move elements down
            //if (StatesList[StepCounter].State == 1)
            //{
            //    int index = 0;

            //    //Find the index of the element to move
            //    for (int i = 0; i < ArrayListGraphic.Count; i++)
            //    {
            //        if (ArrayListGraphic[i].name == StatesList[StepCounter].NumberToMove.ToString())
            //        {
            //            index = i;
            //            break;
            //        }
            //    }

            //    Vector3 PositionToMove = OriginalPositions[StatesList[StepCounter].PositionIndexToMove].transform.position;
            //    PositionToMove.y -= MoveDownDistance * ArrayListGraphic[0].transform.localScale.x;
            //    PositionToMove.y = PosYReferenceMoveDown.position.y;

            //    //Move the element
            //    StartCoroutine(MoveToPosition(ArrayListGraphic[index], PositionToMove, StopSeconds * 0.5f));

            //    //Routine that waits for time and highlights the text of the running code
            //    StartCoroutine(WaitTimeMove(StopSeconds, 1));
            //}

            ////Move elements up
            //if (StatesList[StepCounter].State == 2)
            //{
            //    //We have to do the changes to our ArrayListGraphic
            //    //We have to clone the state of the array to our ArrayListGraphic
            //    List<GameObject> AuxList = new List<GameObject>();

            //    //Insert the elements in the right order in a aux list, then change our list by this.
            //    for (int i = 0; i < StatesList[StepCounter].array.Length; i++)
            //    {
            //        for (int j = 0; j < ArrayListGraphic.Count; j++)
            //        {
            //            if (StatesList[StepCounter].array[i].ToString() == ArrayListGraphic[j].name)
            //                AuxList.Add(ArrayListGraphic[j]);
            //        }
            //    }

            //    ArrayListGraphic = AuxList;

            //    //Move the element up
            //    StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].PositionIndexToMove], OriginalPositions[StatesList[StepCounter].PositionIndexToMove].transform.position, StopSeconds * 0.5f));

            //    //Routine that waits for time and highlights the text of the running code
            //    StartCoroutine(WaitTimeMove(StopSeconds, 2));
            //}


            //If state is elements swap
            if (StatesList[StepCounter].State == 3)
            {
                //Move Elements
                StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].LeftElementIndex], ArrayListGraphic[StatesList[StepCounter].RightElementIndex], StopSeconds * 0.5f));
                StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].RightElementIndex], ArrayListGraphic[StatesList[StepCounter].LeftElementIndex], StopSeconds * 0.5f));

                //Routine that waits for time and highlights the text of the running code
                StartCoroutine(WaitTimeSwap(StopSeconds));

                //Make changes to our ArrayListGraphic
                auxGameObject = ArrayListGraphic[StatesList[StepCounter].LeftElementIndex];
                ArrayListGraphic[StatesList[StepCounter].LeftElementIndex] = ArrayListGraphic[StatesList[StepCounter].RightElementIndex];
                ArrayListGraphic[StatesList[StepCounter].RightElementIndex] = auxGameObject;
            }


            StepCounter += 1;
        }

        ////Step Mode
        //if (Paused && Stopped && StepCounter <= (StatesList.Count))
        //{
        //    //Step Over
        //    StepOverButton.onClick.AddListener(delegate
        //    {
        //        if (Paused)
        //        {
        //            if (!ExecutingStep)
        //            {
        //                ExecutingStep = true;

        //                CleanHighLightedCode();

        //                //Remark Elements
        //                if (StatesList[StepCounter].State == 0)
        //                {
        //                    CodeImage.sprite = SpriteCodeRemarkImage;

        //                    //Remark graphic elements of the array
        //                    for (int i = StatesList[StepCounter].StartIndex; i <= StatesList[StepCounter].EndIndex; i++)
        //                    {
        //                        Color HighlightedColor = ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color;
        //                        ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color = new Color(HighlightedColor.r, HighlightedColor.g, HighlightedColor.b, 0.5f);
        //                    }
        //                    StartCoroutine(WaitTimeRemarkStep(0));
        //                }

        //                //Move elements down
        //                if (StatesList[StepCounter].State == 1)
        //                {
        //                    CodeImage.sprite = SpriteCodeMoveDownImage;

        //                    for (int j = StatesList[StepCounter - 1].StartIndex; j <= StatesList[StepCounter - 1].EndIndex; j++)
        //                    {
        //                        //Restore the alfa color
        //                        Color HighlightedColor = ArrayListGraphic[j].transform.Find("Bar").GetComponent<Image>().color;
        //                        ArrayListGraphic[j].transform.Find("Bar").GetComponent<Image>().color = new Color(HighlightedColor.r, HighlightedColor.g, HighlightedColor.b, 1f);

        //                        ArrayListGraphic[j].transform.Find("Bar").GetComponent<Image>().color = ArrayListGraphic[StatesList[StepCounter - 1].StartIndex].transform.Find("Bar").GetComponent<Image>().color;

        //                        if (StatesList[StepCounter - 1].StartIndex == 0 && StatesList[StepCounter - 1].EndIndex == (ArrayListGraphic.Count - 1))//Final step, we put main color of the proyect
        //                            ArrayListGraphic[j].transform.Find("Bar").GetComponent<Image>().color = MainProjectColor;
        //                    }

        //                    int index = 0;

        //                    //Find the index of the element to move
        //                    for (int i = 0; i < ArrayListGraphic.Count; i++)
        //                    {
        //                        if (ArrayListGraphic[i].name == StatesList[StepCounter].NumberToMove.ToString())
        //                        {
        //                            index = i;
        //                            break;
        //                        }
        //                    }

        //                    Vector3 PositionToMove = OriginalPositions[StatesList[StepCounter].PositionIndexToMove].transform.position;
        //                    PositionToMove.y = PosYReferenceMoveDown.position.y;

        //                    //Move the element
        //                    StartCoroutine(MoveToPosition(ArrayListGraphic[index], PositionToMove, StopSeconds * 0.5f));

        //                    //Routine that waits for time and highlights the text of the running code
        //                    StartCoroutine(WaitTimeMoveStep(StopSeconds));
        //                }

        //                //Move elements up
        //                if (StatesList[StepCounter].State == 2)
        //                {
        //                    CodeImage.sprite = SpriteCodeMoveUpImage;

        //                    //We have to do the changes to our ArrayListGraphic
        //                    //We have to clone the state of the array to our ArrayListGraphic
        //                    List<GameObject> listaAux = new List<GameObject>();

        //                    //Insert the elements in the right order in a aux list, then change our list by this.
        //                    for (int i = 0; i < StatesList[StepCounter].array.Length; i++)
        //                    {
        //                        for (int j = 0; j < ArrayListGraphic.Count; j++)
        //                        {
        //                            if (StatesList[StepCounter].array[i].ToString() == ArrayListGraphic[j].name)
        //                                listaAux.Add(ArrayListGraphic[j]);
        //                        }
        //                    }

        //                    ArrayListGraphic = listaAux;

        //                    //Move the element up
        //                    StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].PositionIndexToMove], OriginalPositions[StatesList[StepCounter].PositionIndexToMove].transform.position, StopSeconds * 0.5f));

        //                    //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
        //                    StartCoroutine(WaitTimeMoveStep(StopSeconds));
        //                }
        //                StepCounter += 1;
        //            }
        //        }
        //    });

        //    //Step back
        //    StepBackButton.onClick.AddListener(delegate
        //    {
        //        if (Paused)
        //        {
        //            if (!ExecutingStep && StepCounter > 0)
        //            {
        //                ExecutingStep = true;
        //                StepCounter -= 1;

        //                CleanHighLightedCode();

        //                //Remark Elements
        //                if (StatesList[StepCounter].State == 0)
        //                {
        //                    CodeImage.sprite = SpriteCodeImage;

        //                    RemarkCodeStepBack(StepCounter);

        //                    for (int i = StatesList[StepCounter].StartIndex; i <= StatesList[StepCounter].EndIndex; i++)
        //                    {
        //                        Color HighlightedColor = ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color;
        //                        ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color = new Color(HighlightedColor.r, HighlightedColor.g, HighlightedColor.b, 1f);
        //                    }
        //                    StartCoroutine(WaitTimeRemarkStep(0));
        //                }

        //                //Move elements down
        //                if (StatesList[StepCounter].State == 1)
        //                {
        //                    RemarkCodeStepBack(StepCounter);

        //                    int index = 0;

        //                    //Find the index of the element to move
        //                    for (int i = 0; i < ArrayListGraphic.Count; i++)
        //                    {
        //                        if (ArrayListGraphic[i].name == StatesList[StepCounter].NumberToMove.ToString())
        //                        {
        //                            index = i;
        //                            break;
        //                        }
        //                    }

        //                    //Restore the color
        //                    ArrayListGraphic[index].transform.Find("Bar").GetComponent<Image>().color = StatesList[StepCounter].Color;

        //                    //Target position - 100.0f to move down
        //                    Vector3 PositionToMove = OriginalPositions[index].transform.position;

        //                    //Move the element
        //                    StartCoroutine(MoveToPosition(ArrayListGraphic[index], PositionToMove, StopSeconds * 0.5f));
        //                    //StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha], ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq], segundosParada * 0.5f));

        //                    //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
        //                    StartCoroutine(WaitTimeMoveStep(StopSeconds));
        //                }

        //                //Move elements up
        //                if (StatesList[StepCounter].State == 2)
        //                {
        //                    RemarkCodeStepBack(StepCounter);

        //                    //Target position - 100.0f to move down
        //                    Vector3 PositionToMove = OriginalPositions[StatesList[StepCounter].PositionIndexToMove].transform.position;
        //                    PositionToMove.y -= MoveDownDistance * ArrayListGraphic[0].transform.localScale.x;

        //                    //Move the element up
        //                    StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].PositionIndexToMove], PositionToMove, StopSeconds * 0.5f));

        //                    //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
        //                    StartCoroutine(WaitTimeMoveStep(StopSeconds));

        //                    //We only revert the state of the array if we go to state 1
        //                    if (StatesList[StepCounter - 1].State == 1)
        //                    {
        //                        //We have to do the changes to our ArrayListGraphic
        //                        //We have to clone the state of the array to our ArrayListGraphic
        //                        List<GameObject> listaAux = new List<GameObject>();

        //                        //Insert the elements in the right order in a aux list, then change our list by this.
        //                        for (int i = 0; i < StatesList[StepCounter].arrayBeforeChanges.Length; i++)
        //                        {
        //                            for (int j = 0; j < ArrayListGraphic.Count; j++)
        //                            {
        //                                if (StatesList[StepCounter].arrayBeforeChanges[i].ToString() == ArrayListGraphic[j].name)
        //                                    listaAux.Add(ArrayListGraphic[j]);
        //                            }
        //                        }
        //                        ArrayListGraphic = listaAux;
        //                    }
        //                }
        //            }
        //        }
        //    });
        //}
    }

    ////Highlight the code depending on wich state we go
    //private void RemarkCodeStepBack(int contadorPaso)
    //{
    //    if (StepCounter > 0)
    //    {
    //        contadorPaso = StepCounter - 1;

    //        if (StatesList[contadorPaso].State == 0)
    //        {
    //            CodeImage.sprite = SpriteCodeRemarkImage;
    //            for (int i = StatesList[contadorPaso].StartIndex; i <= StatesList[contadorPaso].EndIndex; i++)
    //            {
    //                Color HighlightedColor = ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color;
    //                ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color = new Color(HighlightedColor.r, HighlightedColor.g, HighlightedColor.b, 0.5f);
    //            }
    //        }

    //        else if (StatesList[contadorPaso].State == 1)
    //        {
    //            CodeImage.sprite = SpriteCodeMoveDownImage;
    //        }
    //        else if (StatesList[contadorPaso].State == 2)
    //            CodeImage.sprite = SpriteCodeMoveUpImage;
    //    }
    //}

    //Clean the highlighted code
    private void CleanHighLightedCode()
    {
        CodeImage.sprite = SpriteCodeImage;
    }

    //Function that allows animate the permutations of the array
    private IEnumerator MoveToPosition(GameObject initialObject, GameObject finalObject, float timeToMove)
    {
        float time = 0f;
        Vector3 startPosition = initialObject.transform.position;
        Vector3 endPosition = finalObject.transform.position;

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

        //CodeImage.sprite = SpriteCodeSwapImage;

        ////Resaltar elementos graficos array
        //ArrayListGraphic[StatesList[step].LeftElementIndex].transform.Find("Bar").GetComponent<Image>().color = HighlightedColor;
        //ArrayListGraphic[StatesList[step].RightElementIndex].transform.Find("Bar").GetComponent<Image>().color = HighlightedColor;

        yield return new WaitForSeconds(waitTime);

        //CodeImage.sprite = SpriteCodeImage;

        ////Quitar resalte elementos graficos array
        //ArrayListGraphic[StatesList[step].LeftElementIndex].transform.Find("Bar").GetComponent<Image>().color = MainProjectColor;
        //ArrayListGraphic[StatesList[step].RightElementIndex].transform.Find("Bar").GetComponent<Image>().color = MainProjectColor;

        Stopped = true;
        ExecutingStep = false;
    }

    ////Wait time and highlighted the elements involved in the step
    //private IEnumerator WaitTimeRemark(float waitTime)
    //{
    //    Stopped = false;

    //    int step = StepCounter;

    //    //Remark graphic elements of the array
    //    for (int i = StatesList[step].StartIndex; i <= StatesList[step].EndIndex; i++)
    //    {
    //        Color HighlightedColor = ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color;
    //        ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color = new Color(HighlightedColor.r, HighlightedColor.g, HighlightedColor.b, 0.5f);
    //    }

    //    CodeImage.sprite = SpriteCodeRemarkImage;

    //    yield return new WaitForSeconds(waitTime);

    //    //Restore color of graphic elements of the array
    //    for (int i = StatesList[step].StartIndex; i <= StatesList[step].EndIndex; i++)
    //    {
    //        Color HighlightedColor = ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color;
    //        ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color = new Color(HighlightedColor.r, HighlightedColor.g, HighlightedColor.b, 1f);
    //    }

    //    CodeImage.sprite = SpriteCodeImage;

    //    for (int j = StatesList[step].StartIndex; j <= StatesList[step].EndIndex; j++)
    //    {
    //        ArrayListGraphic[j].transform.Find("Bar").GetComponent<Image>().color = ArrayListGraphic[StatesList[step].StartIndex].transform.Find("Bar").GetComponent<Image>().color;

    //        if (StatesList[step].StartIndex == 0 && StatesList[step].EndIndex == (ArrayListGraphic.Count - 1))//Final step, we put main color of the proyect
    //            ArrayListGraphic[j].transform.Find("Bar").GetComponent<Image>().color = MainProjectColor;
    //    }

    //    Stopped = true;
    //    ExecutingStep = false;
    //}

    //Wait time and highlighted the part of move elements in the step mode
    //private IEnumerator WaitTimeMoveStep(float waitTime)
    //{
    //    Stopped = false;
    //    yield return new WaitForSeconds(waitTime);
    //    Stopped = true;
    //    ExecutingStep = false;
    //}

    ////Wait time and highlighted the elements involved in the step in step mode
    //private IEnumerator WaitTimeRemarkStep(float waitTime)
    //{
    //    Stopped = false;
    //    yield return new WaitForSeconds(waitTime);
    //    Stopped = true;
    //    ExecutingStep = false;
    //}

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
    }

    //Termina la ejecución
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

        for (int i = 0; i < OriginalPositions.Count; i++)
        {
            Destroy(OriginalPositions[i]);
        }

        OriginalPositions.Clear();

        InstantiateGraphicArray(ArraySorted, escalaActual, false);

        //we color all the array elements of the final color
        for (int i = 0; i < ArrayListGraphic.Count; i++)
        {
            ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color = MainProjectColor;
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

public class PivoteResult
{
    public int[] Array;
    public int i_decrec;
}

public class QuickSortState
{
    public int State;

    //Swap Elements

    public int LeftElementIndex { get; set; }

    public int RightElementIndex { get; set; }
}
