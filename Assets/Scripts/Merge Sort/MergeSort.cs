using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MergeSort : MonoBehaviour
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
    public List<MergeSortState> StatesList;

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

    //Image of the code merge
    public Image CodeImageMerge;

    //Sprite original code merge
    public Sprite SpriteCodeImageMerge;

    //Sprite image remark code merge
    public Sprite SpriteCodeRemarkMerge;

    //Sprite image move down code merge
    public Sprite SpriteCodeMoveDownMerge;

    //Sprite image move up code merge
    public Sprite SpriteCodeMoveUpImageMerge;

    //Image of the code mergesort
    public Image CodeImageMergeSort;

    //Image of the code mergesort 1º mergesort
    public Sprite SpriteCodeImageMergeSort;

    //Image of the code mergesort 1º mergesort
    public Sprite SpriteCodeImageMergeSort1mergesort;

    //Image of the code mergesort 2º mergesort
    public Sprite SpriteCodeImageMergeSort2mergesort;

    //Image of the code mergesort merge
    public Sprite SpriteCodeImageMergeSortmerge;

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
        //Set the language
        MergeSortLanguageManager.SetLanguage();

        //Set variables
        Stopped = true;
        StepCounter = 0;
        StatesList = new List<MergeSortState>();
        ExecutingStep = false;
        NElements = MenuConfiguracion.NumElementos;
        OriginalColorList = new List<Color>();
        ColorList = new List<Color>();

        //If we start directly from the execution scene, this has to be removed in the final build
        if (NElements == 0)
        {
            NElements = 15;
        }

        ArrayOriginal = CreateRandomArray(NElements);
        InstantiateGraphicArray(ArrayOriginal, GetScale(),true);
        ExecuteAndCreateStates(ArrayOriginal);
    }

    //Execute the algorith and creates the states to step reproduction
    private void ExecuteAndCreateStates(int[] array)
    {
        ArraySorted = (int[])array.Clone();
        ArraySorted = mergesort(ArraySorted, 0, ArraySorted.Length - 1);
    }

    public int[] mergesort (int[]array, int p, int r)
    {
        if (p < r)
        {
            int q = (p + r) / 2;

            //Save the state to highlight the 1 function
            StatesList.Add(new MergeSortState()
            {
                State = 3,
                FunctionToHighlight = 1
            });
            mergesort(array, p, q);

            //Save the state to highlight the 1 function
            StatesList.Add(new MergeSortState()
            {
                State = 3,
                FunctionToHighlight = 2
            });
            mergesort(array, q + 1, r);

            //Save the state to highlight the 1 function
            StatesList.Add(new MergeSortState()
            {
                State = 3,
                FunctionToHighlight = 3
            });
            array = merge(array, p, q, r);
        }
        return array;
    }

    private int[] merge(int[] array, int p, int q, int r)
    {
        //Save the state to remark
        StatesList.Add(new MergeSortState()
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
                StatesList.Add(new MergeSortState()
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
                StatesList.Add(new MergeSortState()
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
            StatesList.Add(new MergeSortState()
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

    //Instatiate the graphic array elements and put them in a list
    private void InstantiateGraphicArray(int[] array, Vector3 currentScale, bool CreateColorList)
    {
        //Array with the distance between elements depending on the number of elements selected by the user
        int[] arrayDistanciaElementos = new int[] { 40, 35, 30, 25, 20, 15};
        DistanceBetweenElements = arrayDistanciaElementos[NElements-10];

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

            if (CreateColorList)
            {
                //Random colors because this algorithm at the start creates partitions of size 1
                Color RandomColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                instantiated.transform.Find("Bar").GetComponent<Image>().color = RandomColor;
                ColorList.Add(RandomColor);
                OriginalColorList.Add(RandomColor);
            }
            else
            {
                instantiated.transform.Find("Bar").GetComponent<Image>().color = OriginalColorList[i];
            }

            //Save the original position to help with the movements
            GameObject originalPosition = Instantiate(OriginalPositionPrefab);
            originalPosition.name = "Original Position " + i;
            OriginalPositions.Add(originalPosition);
            originalPosition.transform.SetParent(this.transform);
            Vector3 position = ArrayListGraphic[i].transform.position;
            originalPosition.transform.position = position;
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

        //If last step, pause
        if (StepCounter == StatesList.Count)
        {
            Paused = true;
            PlayPauseImage.sprite = SpritePlay;
        }

        //Automatic mode
        if (!Paused && Stopped && StepCounter <= (StatesList.Count - 1))
        {
            //Remark Elements
            if (StatesList[StepCounter].State == 0)
            {
                //Routine that waits for time and highlights the text of the running code
                StartCoroutine(WaitTimeRemark(StopSeconds));
            }

            //Move elements down
            if (StatesList[StepCounter].State == 1)
            {
                int index = 0;

                //Find the index of the element to move
                for(int i = 0; i < ArrayListGraphic.Count; i++)
                {
                    if (ArrayListGraphic[i].name == StatesList[StepCounter].NumberToMove.ToString())
                    {
                        index = i;
                        break;
                    }
                }

                Vector3 PositionToMove = OriginalPositions[StatesList[StepCounter].PositionIndexToMove].transform.position;
                PositionToMove.y -= MoveDownDistance * ArrayListGraphic[0].transform.localScale.x;
                PositionToMove.y = PosYReferenceMoveDown.position.y;

                //Move the element
                StartCoroutine(MoveToPosition(ArrayListGraphic[index], PositionToMove, StopSeconds * 0.5f));

                //Routine that waits for time and highlights the text of the running code
                StartCoroutine(WaitTimeMove(StopSeconds,1));
            }

            //Move elements up
            if (StatesList[StepCounter].State == 2)
            {
                //We have to do the changes to our ArrayListGraphic
                //We have to clone the state of the array to our ArrayListGraphic
                List<GameObject> AuxList = new List<GameObject>();

                //Insert the elements in the right order in a aux list, then change our list by this.
                for (int i = 0; i < StatesList[StepCounter].array.Length; i++)
                {
                    for (int j = 0; j < ArrayListGraphic.Count; j++)
                    {
                        if (StatesList[StepCounter].array[i].ToString() == ArrayListGraphic[j].name)
                            AuxList.Add(ArrayListGraphic[j]);
                    }
                }

                ArrayListGraphic = AuxList;

                //Move the element up
                StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].PositionIndexToMove], OriginalPositions[StatesList[StepCounter].PositionIndexToMove].transform.position, StopSeconds * 0.5f));

                //Routine that waits for time and highlights the text of the running code
                StartCoroutine(WaitTimeMove(StopSeconds,2));
            }

            //Highlight calls in the mergesort function
            if (StatesList[StepCounter].State == 3)
            {
                //Routine that waits for time and highlights the text of the running code
                StartCoroutine(WaitTimeHighlightFunction(StopSeconds, false));
            }

            StepCounter += 1;
        }

        //Step Mode
        if (Stopped )
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

                        //Remark Elements
                        if (StatesList[StepCounter].State == 0)
                        {
                            CodeImageMerge.sprite = SpriteCodeRemarkMerge;

                            //Remark graphic elements of the array
                            for (int i = StatesList[StepCounter].StartIndex; i <= StatesList[StepCounter].EndIndex; i++)
                            {
                                Color HighlightedColor = ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color;
                                ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color = new Color(HighlightedColor.r, HighlightedColor.g, HighlightedColor.b, 0.5f);
                            }
                            StartCoroutine(WaitTimeRemarkStep(0));
                        }

                        //Move elements down
                        if (StatesList[StepCounter].State == 1)
                        {
                            CodeImageMerge.sprite = SpriteCodeMoveDownMerge;

                            for (int j = StatesList[StepCounter-1].StartIndex; j <= StatesList[StepCounter-1].EndIndex; j++)
                            {
                                //Restore the alfa color
                                Color HighlightedColor = ArrayListGraphic[j].transform.Find("Bar").GetComponent<Image>().color;
                                ArrayListGraphic[j].transform.Find("Bar").GetComponent<Image>().color = new Color(HighlightedColor.r, HighlightedColor.g, HighlightedColor.b, 1f);

                                ArrayListGraphic[j].transform.Find("Bar").GetComponent<Image>().color = ArrayListGraphic[StatesList[StepCounter-1].StartIndex].transform.Find("Bar").GetComponent<Image>().color;

                                if (StatesList[StepCounter-1].StartIndex == 0 && StatesList[StepCounter-1].EndIndex == (ArrayListGraphic.Count - 1))//Final step, we put main color of the proyect
                                    ArrayListGraphic[j].transform.Find("Bar").GetComponent<Image>().color = MainProjectColor;
                            }

                            int index = 0;

                            //Find the index of the element to move
                            for (int i = 0; i < ArrayListGraphic.Count; i++)
                            {
                                if (ArrayListGraphic[i].name == StatesList[StepCounter].NumberToMove.ToString())
                                {
                                    index = i;
                                    break;
                                }
                            }

                            Vector3 PositionToMove = OriginalPositions[StatesList[StepCounter].PositionIndexToMove].transform.position;
                            PositionToMove.y = PosYReferenceMoveDown.position.y;

                            //Move the element
                            StartCoroutine(MoveToPosition(ArrayListGraphic[index], PositionToMove, StopSeconds * 0.5f));

                            //Routine that waits for time and highlights the text of the running code
                            StartCoroutine(WaitTimeMoveStep(StopSeconds));
                        }

                        //Move elements up
                        if (StatesList[StepCounter].State == 2)
                        {
                            CodeImageMerge.sprite = SpriteCodeMoveUpImageMerge;

                            //We have to do the changes to our ArrayListGraphic
                            //We have to clone the state of the array to our ArrayListGraphic
                            List<GameObject> listaAux = new List<GameObject>();

                            //Insert the elements in the right order in a aux list, then change our list by this.
                            for (int i = 0; i < StatesList[StepCounter].array.Length; i++)
                            {
                                for (int j = 0; j < ArrayListGraphic.Count; j++)
                                {
                                    if (StatesList[StepCounter].array[i].ToString() == ArrayListGraphic[j].name)
                                        listaAux.Add(ArrayListGraphic[j]);
                                }
                            }

                            ArrayListGraphic = listaAux;

                            //Move the element up
                            StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].PositionIndexToMove], OriginalPositions[StatesList[StepCounter].PositionIndexToMove].transform.position, StopSeconds * 0.5f));

                            //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
                            StartCoroutine(WaitTimeMoveStep(StopSeconds));
                        }

                        //Highlight calls in the mergesort function
                        if (StatesList[StepCounter].State == 3)
                        {
                            //Routine that waits for time and highlights the text of the running code
                            StartCoroutine(WaitTimeHighlightFunction(0, true));
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

                        //Remark Elements
                        if (StatesList[StepCounter].State == 0)
                        {
                            CodeImageMerge.sprite = SpriteCodeImageMerge;

                            RemarkCodeStepBack(StepCounter);

                            for (int i = StatesList[StepCounter].StartIndex; i <= StatesList[StepCounter].EndIndex; i++)
                            {
                                Color HighlightedColor = ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color;
                                ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color = new Color(HighlightedColor.r, HighlightedColor.g, HighlightedColor.b, 1f);
                            }
                            StartCoroutine(WaitTimeRemarkStep(0));
                        }

                        //Move elements down
                        if (StatesList[StepCounter].State == 1)
                        {
                            RemarkCodeStepBack(StepCounter);

                            int index = 0;

                            //Find the index of the element to move
                            for (int i = 0; i < ArrayListGraphic.Count; i++)
                            {
                                if (ArrayListGraphic[i].name == StatesList[StepCounter].NumberToMove.ToString())
                                {
                                    index = i;
                                    break;
                                }
                            }

                            //Restore the color only if the previous step is 0
                            if (StatesList[StepCounter-1].State != 0)
                                ArrayListGraphic[index].transform.Find("Bar").GetComponent<Image>().color = StatesList[StepCounter].Color;

                            //Target position - 100.0f to move down
                            Vector3 PositionToMove = OriginalPositions[index].transform.position;

                            //Move the element
                            StartCoroutine(MoveToPosition(ArrayListGraphic[index], PositionToMove, StopSeconds * 0.5f));
                            //StartCoroutine(MoveToPosition(ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoDcha], ArrayListGraphic[listaEstados[ContadorPaso].IndiceElementoIzq], segundosParada * 0.5f));

                            //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
                            StartCoroutine(WaitTimeMoveStep(StopSeconds));
                        }

                        //Move elements up
                        if (StatesList[StepCounter].State == 2)
                        {
                            RemarkCodeStepBack(StepCounter);

                            //Target position - 100.0f to move down
                            Vector3 PositionToMove = OriginalPositions[StatesList[StepCounter].PositionIndexToMove].transform.position;
                            PositionToMove.y -= MoveDownDistance * ArrayListGraphic[0].transform.localScale.x;

                            //Move the element up
                            StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].PositionIndexToMove], PositionToMove, StopSeconds * 0.5f));

                            //Rutina que espera el tiempo y resalta el texto del codigo en ejecución
                            StartCoroutine(WaitTimeMoveStep(StopSeconds));

                            //We only revert the state of the array if we go to state 1
                            if (StatesList[StepCounter - 1].State == 1)
                            {
                                //We have to do the changes to our ArrayListGraphic
                                //We have to clone the state of the array to our ArrayListGraphic
                                List<GameObject> listaAux = new List<GameObject>();

                                //Insert the elements in the right order in a aux list, then change our list by this.
                                for (int i = 0; i < StatesList[StepCounter].arrayBeforeChanges.Length; i++)
                                {
                                    for (int j = 0; j < ArrayListGraphic.Count; j++)
                                    {
                                        if (StatesList[StepCounter].arrayBeforeChanges[i].ToString() == ArrayListGraphic[j].name)
                                            listaAux.Add(ArrayListGraphic[j]);
                                    }
                                }
                                ArrayListGraphic = listaAux;
                            }
                        }

                        //Highlight calls in the mergesort function
                        if (StatesList[StepCounter].State == 3)
                        {
                            //Routine that waits for time and highlights the text of the running code
                            StartCoroutine(WaitTimeHighlightFunction(0, true));
                        }
                    }
                }
            });
        }
    }

    //Wait time and highlighted the function
    private IEnumerator WaitTimeHighlightFunction(float waitTime, bool stepMode)
    {
        Stopped = false;

        switch (StatesList[StepCounter].FunctionToHighlight)
        {
            case 1:
                CodeImageMergeSort.sprite = SpriteCodeImageMergeSort1mergesort;
                break;
            case 2:
                CodeImageMergeSort.sprite = SpriteCodeImageMergeSort2mergesort;
                break;
            case 3:
                CodeImageMergeSort.sprite = SpriteCodeImageMergeSortmerge;
                break;
        }

        yield return new WaitForSeconds(waitTime);

        if (!stepMode)
            CodeImageMergeSort.sprite = SpriteCodeImageMergeSort;

        Stopped = true;
        ExecutingStep = false;
    }

    //Highlight the code depending on wich state we go
    private void RemarkCodeStepBack(int contadorPaso)
    {
        if (StepCounter > 0)
        {
            contadorPaso = StepCounter - 1;

            if (StatesList[contadorPaso].State == 0)
             {
                CodeImageMerge.sprite = SpriteCodeRemarkMerge;
                for (int i = StatesList[contadorPaso].StartIndex; i <= StatesList[contadorPaso].EndIndex; i++)
                {
                    Color HighlightedColor = ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color;
                    ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color = new Color(HighlightedColor.r, HighlightedColor.g, HighlightedColor.b, 0.5f);
                }
            }

            else if (StatesList[contadorPaso].State == 1)
            {
                CodeImageMerge.sprite = SpriteCodeMoveDownMerge;
            }
            else if (StatesList[contadorPaso].State == 2)
                CodeImageMerge.sprite = SpriteCodeMoveUpImageMerge;
        }
    }

    //Clean the highlighted code
    private void CleanHighLightedCode()
    {
        CodeImageMerge.sprite = SpriteCodeImageMerge;
        CodeImageMergeSort.sprite = SpriteCodeImageMergeSort;
    }

    //Function that allows animate the permutations of the array
    private IEnumerator MoveToPosition(GameObject initialObject, Vector3 finalPosition, float timeToMove)
    {
        float time = 0f;
        Vector3 startPosition = initialObject.transform.position;

        while (time < 1)
        {
            time += Time.deltaTime / timeToMove;
            initialObject.transform.position = Vector3.Lerp(startPosition, finalPosition, time);
            yield return null;
        }
    }

    //Wait time and highlighted the part of move elements
    private IEnumerator WaitTimeMove(float waitTime, int state)
    {
        Stopped = false;

        if (state == 1)
            CodeImageMerge.sprite = SpriteCodeMoveDownMerge;
        else
            CodeImageMerge.sprite = SpriteCodeMoveUpImageMerge;
        yield return new WaitForSeconds(waitTime);

        CodeImageMerge.sprite = SpriteCodeImageMerge;

        Stopped = true;
        ExecutingStep = false;
    }

    //Wait time and highlighted the elements involved in the step
    private IEnumerator WaitTimeRemark(float waitTime)
    {
        Stopped = false;

        int step = StepCounter;

        //Remark graphic elements of the array
        for(int i = StatesList[step].StartIndex; i<= StatesList[step].EndIndex; i++)
        {
            Color HighlightedColor = ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color;
            ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color = new Color(HighlightedColor.r,HighlightedColor.g,HighlightedColor.b,0.5f);
        }

        CodeImageMerge.sprite = SpriteCodeRemarkMerge;

        yield return new WaitForSeconds(waitTime);

        //Restore color of graphic elements of the array
        for (int i = StatesList[step].StartIndex; i <= StatesList[step].EndIndex; i++)
        {
            Color HighlightedColor = ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color;
            ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color = new Color(HighlightedColor.r, HighlightedColor.g, HighlightedColor.b, 1f);
        }

        CodeImageMerge.sprite = SpriteCodeImageMerge;

        for (int j = StatesList[step].StartIndex; j <= StatesList[step].EndIndex; j++)
        {
            ArrayListGraphic[j].transform.Find("Bar").GetComponent<Image>().color = ArrayListGraphic[StatesList[step].StartIndex].transform.Find("Bar").GetComponent<Image>().color;

            if (StatesList[step].StartIndex == 0 && StatesList[step].EndIndex == (ArrayListGraphic.Count - 1))//Final step, we put main color of the proyect
                ArrayListGraphic[j].transform.Find("Bar").GetComponent<Image>().color = MainProjectColor;
        }

        Stopped = true;
        ExecutingStep = false;
    }

    //Wait time and highlighted the part of move elements in the step mode
    private IEnumerator WaitTimeMoveStep(float waitTime)
    {
        Stopped = false;
        yield return new WaitForSeconds(waitTime);
        Stopped = true;
        ExecutingStep = false;
    }

    //Wait time and highlighted the elements involved in the step in step mode
    private IEnumerator WaitTimeRemarkStep(float waitTime)
    {
        Stopped = false;
        yield return new WaitForSeconds(waitTime);
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

        InstantiateGraphicArray(ArrayOriginal, localScale,false);
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

        for (int i = 0; i < OriginalPositions.Count; i++)
        {
            Destroy(OriginalPositions[i]);
        }

        OriginalPositions.Clear();

        InstantiateGraphicArray(ArraySorted, escalaActual,false);

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

//Class that represent the states of the execution
public class MergeSortState
{
    /*
     * State 0 - Highlight elements in this iteration
     * State 1 - Move element down
     * State 2 - Move elements up
     * State 3 - Highlight calls in the mergesort function
     */

    public int State { get; set; }

    //Start and end index to know what elements are involved
    public int StartIndex { get; set; }

    public int EndIndex { get; set; }

    public Color Color;

    //Number of the array to move down
    public int NumberToMove { get; set; }

    //Index of the position to move
    public int PositionIndexToMove { get; set; }

    //State of the array
    public int[] array;

    //State of the array before changes
    public int[] arrayBeforeChanges;

    //Number of the function in mergesort to highlight
    public int FunctionToHighlight;

}
