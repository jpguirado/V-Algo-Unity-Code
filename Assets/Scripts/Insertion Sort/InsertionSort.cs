using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class InsertionSort : MonoBehaviour
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
    public List<InsertionSortState> StatesList;

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

    //Image of the code
    public Image CodeImage;

    //Sprite original code
    public Sprite SpriteCodeImage;

    //Sprite image min index 1
    public Sprite SpriteCodeFirstElementSorted0;

    //Sprite image min index 2
    public Sprite SpriteCodeKeyMoveDown;

    //Sprite image number compare
    public Sprite SpriteCodeSwapElements;

    //Sprite image swap
    public Sprite SpriteCodeMoveUpKey;

    //Determines if one step is being executed
    private bool ExecutingStep;

    //Show in the screen the number of current step
    public TextMeshProUGUI StepCounterText;

    //Image of the play/pause button
    public Image PlayPauseImage;

    //Distance between elements
    public float DistanceBetweenElements;

    //Speed Slider
    public Slider SpeedSlider;

    //Language Manager
    public InsertionSortLanguageManager InsertionSortLanguageManager;

    //Distance that will the elements move in the state 1
    public float MoveDownDistance;

    //Reference of Y position to move down the array graphic elements
    public RectTransform PosYReferenceMoveDown;

    //Distance of displacement of the bars with respect to the center
    public float LateralShift;

    //Main Color of the project
    public Color MainProjectColor;

    //Color to higlight min index
    public Color KeyColor;

    //Color to higlight number to compare
    public Color NumberToCompareColor;

    //Right Half Color
    public Color RightHalfColor;

    //The element is in his sorted position
    public Color SortedColor;

    //Dtermines if the element is in his final position
    public List<bool> ElementsPlaced;

    // Start is called before the first frame update
    void Start()
    {
        //Set the language
        InsertionSortLanguageManager.SetLanguage();

        ////Set variables
        Stopped = true;
        StepCounter = 0;
        StatesList = new List<InsertionSortState>();
        ExecutingStep = false;
        NElements = MenuConfiguracion.NumElementos;

        //If we start directly from the execution scene, this has to be removed in the final build
        if (NElements == 0)
        {
            NElements = 10;
        }

        ArrayOriginal = CreateRandomArray(NElements);
        InstantiateGraphicArray(ArrayOriginal, GetScale(), true);
        ExecuteAndCreateStates(ArrayOriginal);
    }

    //Execute the algorith and creates the states to step reproduction
    private void ExecuteAndCreateStates(int[] array)
    {
        ArraySorted = (int[])array.Clone();

        //Mark the first element sorted
        StatesList.Add(new InsertionSortState()
        {
            State = 0,
            IndexToHighlight = 0
        });

        for (int i = 1; i < ArraySorted.Length; i++)
        {
            int key = ArraySorted[i];

            //Highlight and move down the key
            StatesList.Add(new InsertionSortState()
            {
                State = 1,
                IndexToHighlight = i
            });

            int j = i - 1;

            while(j >= 0 && ArraySorted[j] > key)
            {
                //Swap elements
                StatesList.Add(new InsertionSortState()
                {
                    State = 2,
                    IndexToMoveKey = j,
                    Key = key,
                    RestoreElement = ArraySorted[j + 1]
                });
                ArraySorted[j + 1] = ArraySorted[j];
                j = j - 1;
            }

            //Move up key and highlight sorted
            StatesList.Add(new InsertionSortState()
            {
                State = 3,
                IndexToMoveKey = j + 1,
                Key = key,
                RestoreElement = ArraySorted[j+1]
            });
            ArraySorted[j + 1] = key;
        }
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
            if (firstTime)
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
            //If state is highlight first sorted element
            if (StatesList[StepCounter].State == 0)
            {
                //Routine that waits for time and highlights the text of the running code
                StartCoroutine(WaitTimeHighlightMinSortedElement(StopSeconds, false, false));
            }

            //If state is move down the key and highlight it
            if (StatesList[StepCounter].State == 1)
            {
                Vector3 PositionToMove = OriginalPositions[StatesList[StepCounter].IndexToHighlight].transform.position;
                PositionToMove.y = PosYReferenceMoveDown.position.y;

                //Move the element
                StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].IndexToHighlight], PositionToMove, StopSeconds * 0.5f));

                //Routine that waits for time and highlights the text of the running code
                StartCoroutine(WaitTimeHighlightKey(StopSeconds, false, false));
            }

            //If state is swap positions
            if (StatesList[StepCounter].State == 2)
            {
                GameObject key = GameObject.Find(StatesList[StepCounter].Key.ToString());

                //Move key to left
                Vector3 PositionToMoveKey = OriginalPositions[StatesList[StepCounter].IndexToMoveKey].transform.position;
                PositionToMoveKey.y = PosYReferenceMoveDown.position.y;
                StartCoroutine(MoveToPosition(key, PositionToMoveKey, StopSeconds * 0.5f));


                //Move other element to right
                Vector3 PositionToMoveOtherElement = OriginalPositions[StatesList[StepCounter].IndexToMoveKey + 1].transform.position;
                StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].IndexToMoveKey], PositionToMoveOtherElement, StopSeconds * 0.5f));

                ArrayListGraphic[StatesList[StepCounter].IndexToMoveKey + 1] = ArrayListGraphic[StatesList[StepCounter].IndexToMoveKey];

                //Routine that waits for time and highlights the text of the running code
                StartCoroutine(WaitTimeSwapPositions(StopSeconds, false, false));
            }

            //If state move up key
            if (StatesList[StepCounter].State == 3)
            {
                GameObject key = GameObject.Find(StatesList[StepCounter].Key.ToString());

                Vector3 PositionToMove = OriginalPositions[StatesList[StepCounter].IndexToMoveKey].transform.position;

                //Move the element
                StartCoroutine(MoveToPosition(key, PositionToMove, StopSeconds * 0.5f));

                //Make The changes
                ArrayListGraphic[StatesList[StepCounter].IndexToMoveKey] = key;

                StartCoroutine(WaitTimeHighlightMinSortedElement(StopSeconds, false,false));
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

                        //If state is highlight first sorted element
                        if (StatesList[StepCounter].State == 0)
                        {
                            //Routine that waits for time and highlights the text of the running code
                            StartCoroutine(WaitTimeHighlightMinSortedElement(0, false, true));
                        }

                        //If state is move down the key and highlight it
                        if (StatesList[StepCounter].State == 1)
                        {
                            Vector3 PositionToMove = OriginalPositions[StatesList[StepCounter].IndexToHighlight].transform.position;
                            PositionToMove.y = PosYReferenceMoveDown.position.y;

                            //Move the element
                            StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].IndexToHighlight], PositionToMove, StopSeconds * 0.5f));

                            //Routine that waits for time and highlights the text of the running code
                            StartCoroutine(WaitTimeHighlightKey(StopSeconds, true, false));
                        }

                        //If state is swap positions
                        if (StatesList[StepCounter].State == 2)
                        {
                            GameObject key = GameObject.Find(StatesList[StepCounter].Key.ToString());

                            //Move key to left
                            Vector3 PositionToMoveKey = OriginalPositions[StatesList[StepCounter].IndexToMoveKey].transform.position;
                            PositionToMoveKey.y = PosYReferenceMoveDown.position.y;
                            StartCoroutine(MoveToPosition(key, PositionToMoveKey, StopSeconds * 0.5f));


                            //Move other element to right
                            Vector3 PositionToMoveOtherElement = OriginalPositions[StatesList[StepCounter].IndexToMoveKey + 1].transform.position;
                            StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].IndexToMoveKey], PositionToMoveOtherElement, StopSeconds * 0.5f));

                            ArrayListGraphic[StatesList[StepCounter].IndexToMoveKey + 1] = ArrayListGraphic[StatesList[StepCounter].IndexToMoveKey];

                            //Routine that waits for time and highlights the text of the running code
                            StartCoroutine(WaitTimeSwapPositions(StopSeconds, false, true));
                        }

                        //If state move up key
                        if (StatesList[StepCounter].State == 3)
                        {
                            GameObject key = GameObject.Find(StatesList[StepCounter].Key.ToString());

                            Vector3 PositionToMove = OriginalPositions[StatesList[StepCounter].IndexToMoveKey].transform.position;

                            //Move the element
                            StartCoroutine(MoveToPosition(key, PositionToMove, StopSeconds * 0.5f));

                            //Make The changes
                            ArrayListGraphic[StatesList[StepCounter].IndexToMoveKey] = key;

                            StartCoroutine(WaitTimeHighlightMinSortedElement(StopSeconds, false, true));
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

                        //If state is highlight first sorted element
                        if (StatesList[StepCounter].State == 0)
                        {
                            //Routine that waits for time and highlights the text of the running code
                            StartCoroutine(WaitTimeHighlightMinSortedElement(0, true, true));
                        }

                        //If state is move down the key and highlight it
                        if (StatesList[StepCounter].State == 1)
                        {
                            Vector3 PositionToMove = OriginalPositions[StatesList[StepCounter].IndexToHighlight].transform.position;

                            //Move the element
                            StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].IndexToHighlight], PositionToMove, StopSeconds * 0.5f));

                            //Routine that waits for time and highlights the text of the running code
                            StartCoroutine(WaitTimeHighlightKey(StopSeconds, true, true));
                        }

                        //If state is swap positions
                        if (StatesList[StepCounter].State == 2)
                        {
                            GameObject key = GameObject.Find(StatesList[StepCounter].Key.ToString());

                            //Move key to right
                            Vector3 PositionToMoveKey = OriginalPositions[StatesList[StepCounter].IndexToMoveKey + 1].transform.position;
                            PositionToMoveKey.y = PosYReferenceMoveDown.position.y;
                            StartCoroutine(MoveToPosition(key, PositionToMoveKey, StopSeconds * 0.5f));


                            //Move other element to right
                            Vector3 PositionToMoveOtherElement = OriginalPositions[StatesList[StepCounter].IndexToMoveKey].transform.position;
                            StartCoroutine(MoveToPosition(ArrayListGraphic[StatesList[StepCounter].IndexToMoveKey], PositionToMoveOtherElement, StopSeconds * 0.5f));

                            ArrayListGraphic[StatesList[StepCounter].IndexToMoveKey + 1] = GameObject.Find(StatesList[StepCounter].RestoreElement.ToString());

                            //Routine that waits for time and highlights the text of the running code
                            StartCoroutine(WaitTimeSwapPositions(StopSeconds, true, true));


                        }

                        //If state move up key
                        if (StatesList[StepCounter].State == 3)
                        {
                            GameObject key = GameObject.Find(StatesList[StepCounter].Key.ToString());

                            Vector3 PositionToMove = OriginalPositions[StatesList[StepCounter].IndexToMoveKey].transform.position;
                            PositionToMove.y = PosYReferenceMoveDown.position.y;

                            //Move the element
                            StartCoroutine(MoveToPosition(key, PositionToMove, StopSeconds * 0.5f));

                            StartCoroutine(WaitTimeHighlightMinSortedElement(StopSeconds, true, true));

                            //Restore changes
                            ArrayListGraphic[StatesList[StepCounter].IndexToMoveKey] = GameObject.Find(StatesList[StepCounter].RestoreElement.ToString());
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

    //Wait time and highlighted the min sorted element
    private IEnumerator WaitTimeHighlightMinSortedElement(float waitTime, bool stepBack, bool stepMode)
    {
        Stopped = false;

        int step = StepCounter;
        if (StatesList[step].State == 0)
            CodeImage.sprite = SpriteCodeFirstElementSorted0;
        else
            CodeImage.sprite = SpriteCodeMoveUpKey;

        if (!stepBack)
        {
            if (StatesList[step].State == 0)
                ArrayListGraphic[StatesList[step].IndexToHighlight].transform.Find("Bar").GetComponent<Image>().color = SortedColor;
            else
                ArrayListGraphic[StatesList[step].IndexToMoveKey].transform.Find("Bar").GetComponent<Image>().color = SortedColor;
        }
        else
        {
            if (StatesList[step].State == 0)
                ArrayListGraphic[StatesList[step].IndexToHighlight].transform.Find("Bar").GetComponent<Image>().color = MainProjectColor;
            else
                ArrayListGraphic[StatesList[step].IndexToMoveKey].transform.Find("Bar").GetComponent<Image>().color = KeyColor;
        }

        yield return new WaitForSeconds(waitTime);

        if (!stepMode)
            CodeImage.sprite = SpriteCodeImage;

        Stopped = true;
        ExecutingStep = false;
    }

    private IEnumerator WaitTimeSwapPositions(float waitTime, bool stepBack, bool stepMode)
    {
        Stopped = false;
        CodeImage.sprite = SpriteCodeSwapElements;
        yield return new WaitForSeconds(waitTime);
        if (!stepMode)
            CodeImage.sprite = SpriteCodeImage;
        Stopped = true;
        ExecutingStep = false;
    }

    //Wait time and highlighted the key
    private IEnumerator WaitTimeHighlightKey(float waitTime, bool stepMode, bool stepBack)
    {
        Stopped = false;

        int step = StepCounter;

        CodeImage.sprite = SpriteCodeKeyMoveDown;

        if(!stepBack)
            ArrayListGraphic[StatesList[step].IndexToHighlight].transform.Find("Bar").GetComponent<Image>().color = KeyColor;
        else
            ArrayListGraphic[StatesList[step].IndexToHighlight].transform.Find("Bar").GetComponent<Image>().color = MainProjectColor;

        yield return new WaitForSeconds(waitTime);


        if (!stepMode)
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

        InstantiateGraphicArray(ArrayOriginal, localScale, false);

        //Reset Element Placed
        for (int i = 0; i < ArrayListGraphic.Count; i++)
        {
            ElementsPlaced[i] = false;
        }
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

        InstantiateGraphicArray(ArraySorted, escalaActual, false);

        //we color all the array elements of the final color and all the elements are sorted
        for (int i = 0; i < ArrayListGraphic.Count; i++)
        {
            ArrayListGraphic[i].transform.Find("Bar").GetComponent<Image>().color = SortedColor;
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

public class InsertionSortState
{
    /*
     * State 0 - Highlight first sorted element
     * State 1 - Move down the key and highlight it
     * State 2 - Swap positions
     * State 3 - Move Up Key and Highligh correct position
     */

    public int State;

    public int IndexToHighlight;

    public int RestoreElement;

    public int IndexToMoveKey;

    public int Key;
}
