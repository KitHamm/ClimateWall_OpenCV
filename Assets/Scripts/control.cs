using GraphQL4Unity;
using Newtonsoft.Json;
using OpenCVForUnity.UnityUtils.Helper;
using OpenCVForUnityExample;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class control : MonoBehaviour
{
    public OSCSend OSCSend;
    public GraphQLQuery questions;
    public GameObject quad;
    public GameObject reset;
    public GameObject finish;
    public GameObject responseHandler;
    public GameObject testButton;
    public GameObject switchCameraButton;
    public GameObject topLock;
    public GameObject topLine;
    public GameObject pause;
    public GameObject play;
    public GameObject wordStore;
    public GameObject setHoldButton;
    public GameObject setLockButton;
    public GameObject setWordLimitButton;
    public GameObject setCharLimitButton;
    public GameObject randomEventHandler;
    public GameObject randomHandlerButton;
    public GameObject userIndicatorButton;
    public GameObject block;
    public GameObject blockButton;
    public GameObject logComponent;
    public GameObject partAnimButton;
    public GameObject finishAnimButton;
    public GameObject addWordHint;
    public GameObject networkSettings;
    public GameObject openCVConfig;
    public GameObject triggerLine;
    public GameObject centerLine;
    public GameObject customOSCContainer;
    public GameObject infoPanel;
    public Animator questionAnimation;
    public GameObject QuestionNumberMutation;
    public Button IndicatorProjectionButton;
    public Image background;
    public TMP_InputField timeoutInput;
    public TMP_InputField wordTimeoutInput;
    public TMP_InputField holdLimit;
    public TMP_InputField lockLimit;
    public TMP_InputField wordLimit;
    public TMP_InputField charLimit;
    public TMP_InputField addWord;
    public TMP_InputField red;
    public TMP_InputField blue;
    public TMP_InputField green;
    public TMP_Dropdown testDropdown;
    public TMP_Text testDropdownlabel;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI questionTimerText;
    public GraphQLHttp http;
    public Camera mainCamera;
    public Camera secondaryCamera;
    public int questionsPlayed;
    public int maxQuestions = 2;
    public bool debug;
    public bool questionChanging;
    public bool particalAnim;
    public bool finishAnim;
    public string output = "";
    public string stack = "";
    public string logs = "";
    public float holdTimer;
    public float lockTimer;
    public float deleteDelayTime;
    public float responseDelayTime = 5f;
    public int questionTimeout = 15;
    public float questionTimeoutTimer;
    string timerString = "0:00:00";
    int mainCameraTarget;
    int secondaryCameraTarget;
    int count;
    float gameTimer = 0f;
    bool awaitQuestionReturn;
    public int framerateTarget = 50;
    // Interaction Settings
    public float holdSens = 100;
    public float distanceSens = 50;
    public int force = 10;
    public float boxSize = 150;
    public int lockPoint = 380;
    public int holdPoint = 500;
    public int tempHold = 190;
    public int shiftScale = 200;
    public float offsideShiftFactor = 1.5f;
    EventSystem system;
    topLineHandler topLineHandler;
    topLockHandler topLockHandler;
    responseManager responseManager;
    finishHandler finishHandler;
    resetHandler resetHandler;
    WebCamTextureToMatHelper webCamTextureToMatHelper;
    VideoCaptureToMatHelper videoCaptureToMatHelper;
    ObjectDetectionYOLOv7Example objectDetectionYOLOv7Example;
    GraphQLQuery NumberMutation;
    wordStoreHandler wordStoreHandler;

    WebCamDevice[] devices;

    public class UpdateQuestionObject
    {
        public int number;
    }

    private void Awake()
    {
        wordStoreHandler = GameObject.FindGameObjectWithTag("wordStore").GetComponent<wordStoreHandler>();
        NumberMutation = QuestionNumberMutation.GetComponent<GraphQLQuery>();
        webCamTextureToMatHelper = quad.GetComponent<WebCamTextureToMatHelper>();
        videoCaptureToMatHelper = quad.GetComponent<VideoCaptureToMatHelper>();
        objectDetectionYOLOv7Example = quad.GetComponent<ObjectDetectionYOLOv7Example>();
        resetHandler = reset.GetComponent<resetHandler>();
        finishHandler = finish.GetComponent<finishHandler>();
        responseManager = responseHandler.GetComponent<responseManager>();
        topLockHandler = topLock.GetComponent<topLockHandler>();
        topLineHandler = topLine.GetComponent<topLineHandler>();
        if (PlayerPrefs.HasKey("flipCamera"))
        {
            if (PlayerPrefs.GetInt("flipCamera") == 0)
            {
                webCamTextureToMatHelper.flipHorizontal = false;
            }
            else
            {
                webCamTextureToMatHelper.flipHorizontal = true;
            }
        }
        else
        {
            webCamTextureToMatHelper.flipHorizontal = false;
            PlayerPrefs.SetInt("flipCamera", 0);
        }
        if (PlayerPrefs.HasKey("responseDelayTime"))
        {
            responseDelayTime = PlayerPrefs.GetFloat("responseDelayTime");
        }
        else
        {
            responseDelayTime = 5f;
            PlayerPrefs.SetFloat("responseDelayTime", 5f);
        }
        if (PlayerPrefs.HasKey("maxQuestions"))
        {
            maxQuestions = PlayerPrefs.GetInt("maxQuestions");
        }
        else
        {
            maxQuestions = 2;
            PlayerPrefs.SetInt("maxQuestions", 2);
        }
        if (PlayerPrefs.HasKey("questionTimeout"))
        {
            questionTimeout = PlayerPrefs.GetInt("questionTimeout");
        }
        else
        {
            questionTimeout = 15;
            PlayerPrefs.SetInt("questionTimeout", 15);
        }
        if (PlayerPrefs.HasKey("shiftScale"))
        {
            shiftScale = PlayerPrefs.GetInt("shiftScale");
        }
        else
        {
            shiftScale = 200;
            PlayerPrefs.SetInt("shiftScale", 200);
        }
        if (PlayerPrefs.HasKey("shiftFactor"))
        {
            offsideShiftFactor = PlayerPrefs.GetFloat("shiftFactor");
        }
        else
        {
            offsideShiftFactor = 1.5f;
            PlayerPrefs.SetFloat("shiftFactor", 1.5f);
        }
        if (PlayerPrefs.HasKey("deleteDelayTime"))
        {
            deleteDelayTime = PlayerPrefs.GetFloat("deleteDelayTime");
        }
        else
        {
            deleteDelayTime = 0.5f;
            PlayerPrefs.SetFloat("deleteDelayTime", deleteDelayTime);
        }
        if (PlayerPrefs.HasKey("lockPoint"))
        {
            lockPoint = PlayerPrefs.GetInt("lockPoint");
        }
        else
        {
            lockPoint = 380;
            PlayerPrefs.SetInt("lockPoint", 380);
        }
        if (PlayerPrefs.HasKey("holdPoint"))
        {
            holdPoint = PlayerPrefs.GetInt("holdPoint");
        }
        else
        {
            holdPoint = 500;
            PlayerPrefs.SetInt("holdPoint", 500);
        }
        if (PlayerPrefs.HasKey("tempHold"))
        {
            tempHold = PlayerPrefs.GetInt("tempHold");
        }
        else
        {
            tempHold = 190;
            PlayerPrefs.SetInt("tempHold", 190);
        }
        if (PlayerPrefs.HasKey("holdSens"))
        {
            holdSens = PlayerPrefs.GetFloat("holdSens");
        }
        else
        {
            holdSens = 100;
            PlayerPrefs.SetFloat("holdSens", 100);
        }
        if (PlayerPrefs.HasKey("distSens"))
        {
            distanceSens = PlayerPrefs.GetFloat("distSens");
        }
        else
        {
            distanceSens = 50;
            PlayerPrefs.SetFloat("distSens", 50);
        }
        if (PlayerPrefs.HasKey("forceSens"))
        {
            force = PlayerPrefs.GetInt("forceSens");
        }
        else
        {
            force = 10;
            PlayerPrefs.SetInt("forceSens", 10);
        }
        if (PlayerPrefs.HasKey("boxSize"))
        {
            boxSize = PlayerPrefs.GetFloat("boxSize");
        }
        else
        {
            boxSize = 150;
            PlayerPrefs.SetFloat("boxSize", 150);
        }
        if (PlayerPrefs.HasKey("bgRed"))
        {
            float r = PlayerPrefs.GetFloat("bgRed");
            float g = PlayerPrefs.GetFloat("bgGreen");
            float b = PlayerPrefs.GetFloat("bgBlue");
            background.color = new Color(r, g, b);
            red.text = (r * 255f).ToString();
            green.text = (g * 255f).ToString();
            blue.text = (b * 255f).ToString();
        }
        else
        {
            PlayerPrefs.SetFloat("bgRed", 0);
            PlayerPrefs.SetFloat("bgGreen", 0);
            PlayerPrefs.SetFloat("bgBlue", 0);
            background.color = new Color(0, 0, 0);
            red.text = "0";
            green.text = "0";
            blue.text = "0";
        }
        // Setting and getting PlayerPrefs
        if (PlayerPrefs.HasKey("partAnim"))
        {
            if (PlayerPrefs.GetInt("partAnim") == 1)
            {
                particalAnim = true;
            }
            else { particalAnim = false; }
        }
        else
        {
            particalAnim = true;
            PlayerPrefs.SetInt("partAnim", 1);
        }
        if (PlayerPrefs.HasKey("finishAnim"))
        {
            if (PlayerPrefs.GetInt("finishAnim") == 1)
            {
                finishAnim = true;
            }
            else { finishAnim = false; }
        }
        else
        {
            finishAnim = true;
            PlayerPrefs.SetInt("finishAnim", 1);
        }
        if (PlayerPrefs.HasKey("holdLimit"))
        {
            holdTimer = PlayerPrefs.GetFloat("holdLimit");
        }
        else
        {
            holdTimer = 3.0f;
            PlayerPrefs.SetFloat("holdLimit", holdTimer);
        }
        holdLimit.text = holdTimer.ToString("F1");
        if (PlayerPrefs.HasKey("lockLimit"))
        {
            lockTimer = PlayerPrefs.GetFloat("lockLimit");
        }
        else
        {
            lockTimer = 3.0f;
            PlayerPrefs.SetFloat("lockLimit", lockTimer);
        }
        lockLimit.text = lockTimer.ToString("F1");
        if (PlayerPrefs.HasKey("selectedCamera"))
        {
            webCamTextureToMatHelper.requestedDeviceName = PlayerPrefs.GetString("selectedCamera");
        }
        else
        {
            webCamTextureToMatHelper.requestedDeviceName = "OBS Virtual Camera";
            PlayerPrefs.SetString("selectedCamera", webCamTextureToMatHelper.requestedDeviceName);
        }
        if (!PlayerPrefs.HasKey("timeoutDuration"))
        {
            PlayerPrefs.SetFloat("timeoutDuration", topLockHandler.timerLimit);
        }
        else
        {
            topLockHandler.timerLimit = PlayerPrefs.GetFloat("timeoutDuration");
        }
        timeoutInput.text = topLockHandler.timerLimit.ToString();
        if (!PlayerPrefs.HasKey("wordTimeout"))
        {
            PlayerPrefs.SetFloat("wordTimeout", wordStore.GetComponent<wordStoreHandler>().timeOut);
        }
        else
        {
            wordStore.GetComponent<wordStoreHandler>().timeOut = PlayerPrefs.GetFloat("wordTimeout");
        }
        wordTimeoutInput.text = wordStore.GetComponent<wordStoreHandler>().timeOut.ToString();
        if (!PlayerPrefs.HasKey("wordLimit"))
        {
            PlayerPrefs.SetInt("wordLimit", wordStore.GetComponent<wordStoreHandler>().wordLimit);
        }
        else
        {
            wordStore.GetComponent<wordStoreHandler>().wordLimit = PlayerPrefs.GetInt("wordLimit");
        }
        wordLimit.text = wordStore.GetComponent<wordStoreHandler>().wordLimit.ToString();
        if (!PlayerPrefs.HasKey("charLimit"))
        {
            PlayerPrefs.SetInt("charLimit", topLineHandler.maxCharCount);
        }
        else
        {
            topLineHandler.maxCharCount = PlayerPrefs.GetInt("charLimit");
        }
        charLimit.text = topLineHandler.maxCharCount.ToString();
        if (!PlayerPrefs.HasKey("mainTarget") && !PlayerPrefs.HasKey("secondaryTarget"))
        {
            PlayerPrefs.SetInt("mainTarget", mainCamera.targetDisplay);
            PlayerPrefs.SetInt("secondaryTarget", secondaryCamera.targetDisplay);
            mainCameraTarget = mainCamera.targetDisplay;
            secondaryCameraTarget = secondaryCamera.targetDisplay;
        }
        else
        {
            mainCameraTarget = PlayerPrefs.GetInt("mainTarget");
            secondaryCameraTarget = PlayerPrefs.GetInt("secondaryTarget");
            mainCamera.targetDisplay = mainCameraTarget;
            secondaryCamera.targetDisplay = secondaryCameraTarget;
        }

    }
    // Start is called before the first frame update
    private void Start()
    {
        Screen.fullScreen = false;
        //awaitQuestionReturn = false;
        questionsPlayed = 1;
        questionChanging = false;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = framerateTarget;
        // Get script components
        // Set / Clear variables
        devices = WebCamTexture.devices;
        debug = false;
        count = 0;
        testDropdown.ClearOptions();
        // Set dropdown options
        var info = new DirectoryInfo(Application.streamingAssetsPath);
        FileInfo[] fileInfos = info.GetFiles("*.mp4");
        foreach (FileInfo fileInfo in fileInfos)
        {
            TMP_Dropdown.OptionData d_New = new TMP_Dropdown.OptionData();
            d_New.text = fileInfo.Name;
            testDropdown.options.Add(d_New);
        }
        videoCaptureToMatHelper.requestedVideoFilePath = fileInfos[0].Name;
        testDropdownlabel.text = fileInfos[0].Name;
        // Check webcam devices
        if (devices.Length == 0)
        {
            objectDetectionYOLOv7Example.test = true;
        }
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }
        if (objectDetectionYOLOv7Example.test)
        {
            testButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Test (on)";
        }
        else
        {
            testButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Test (off)";
        }
        system = EventSystem.current;
    }

    private void Update()
    {
        questionTimerText.text = ((questionTimeout * 60) - questionTimeoutTimer).ToString("F0");
        if (awaitQuestionReturn)
        {
            if (questions.Data.Contains("questions"))
            {
                Debug.Log("Questions order refreshed.");
                awaitQuestionReturn = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {

            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null)
            {
                InputField inputfield = next.GetComponent<InputField>();
                if (inputfield != null) inputfield.OnPointerClick(new PointerEventData(system));  //if it's an input field, also set the text caret

                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }
        }
        questionTimeoutTimer += Time.deltaTime;
        if (questionTimeoutTimer > questionTimeout * 60 && topLockHandler.inPlace.Count == 0 && !topLineHandler.secondLine)
        {
            if (questionsPlayed == maxQuestions)
            {
                if (!finishHandler.finishEvent)
                {
                    Debug.Log("Question timeout.");
                    NextQuestion(0);
                }
            }
            else
            {
                Debug.Log("Question timeout.");
                NextQuestion(0);
            }
        }
        if (Application.targetFrameRate != framerateTarget)
        {
            Application.targetFrameRate = framerateTarget;
        }
        if (debug)
        {
            IndicatorProjectionButton.gameObject.SetActive(true);
        }
        else
        {
            IndicatorProjectionButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Proj.Ind. (off)";
            mainCamera.cullingMask = 32;
            IndicatorProjectionButton.gameObject.SetActive(false);

        }
        // Button state controller
        if (wordStoreHandler.playDemo)
        {
            partAnimButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Demo (on)";
        }
        else
        {
            partAnimButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Demo (off)";
        }

        if (holdLimit.text != "")
        {
            if (float.Parse(holdLimit.text) != holdTimer)
            {
                setHoldButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                setHoldButton.GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            setHoldButton.GetComponent<Button>().interactable = false;
        }
        if (lockLimit.text != "")
        {
            if (float.Parse(lockLimit.text) != lockTimer)
            {
                setLockButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                setLockButton.GetComponent<Button>().interactable = false; ;
            }
        }
        else
        {
            setLockButton.GetComponent<Button>().interactable = false; ;
        }
        if (wordLimit.text != "")
        {
            if (int.Parse(wordLimit.text) != wordStore.GetComponent<wordStoreHandler>().wordLimit)
            {
                setWordLimitButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                setWordLimitButton.GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            setWordLimitButton.GetComponent<Button>().interactable = false;
        }
        if (charLimit.text != "")
        {
            if (int.Parse(charLimit.text) != topLineHandler.maxCharCount)
            {
                setCharLimitButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                setCharLimitButton.GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            setCharLimitButton.GetComponent<Button>().interactable = false;
        }
        // Live Timer
        gameTimer += Time.unscaledDeltaTime;
        int seconds = (int)(gameTimer % 60);
        int min = (int)(gameTimer / 60) % 60;
        int hour = (int)(gameTimer / 3600) % 24;
        timerString = string.Format("{0:0}:{1:00}:{2:00}", hour, min, seconds);
        GameObject.FindGameObjectWithTag("debug2").GetComponent<TextMeshProUGUI>().text = timerString;
    }
    // Log reader 
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        statusText.text = logString;
        output = logString;
        stack = stackTrace;
        logs = timerString + ": " + logString + "\n" + logs;
        logComponent.GetComponent<TextMeshProUGUI>().text = logs;
    }
    //Buton Functions
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SwitchDisplay()
    {
        mainCamera.targetDisplay = secondaryCameraTarget;
        secondaryCamera.targetDisplay = mainCameraTarget;
        mainCameraTarget = mainCamera.targetDisplay;
        secondaryCameraTarget = secondaryCamera.targetDisplay;
        PlayerPrefs.SetInt("mainTarget", mainCamera.targetDisplay);
        PlayerPrefs.SetInt("secondaryTarget", secondaryCamera.targetDisplay);
        Debug.Log("Main Display: " + mainCamera.targetDisplay);
        Debug.Log("Secondary Display: " + secondaryCamera.targetDisplay);
    }
    public void updateQuestion(int number)
    {
        UpdateQuestionObject response = new UpdateQuestionObject();
        response.number = number;
        string variable = JsonConvert.SerializeObject(response);
        NumberMutation.VariablesAsJson = variable;
        NumberMutation.ExecuteQuery = true;
    }
    public void NextQuestion(int osc)
    {
        //Debug.Log(osc);
        //questions.ExecuteQuery = true;
        if (questionsPlayed == maxQuestions)
        {
            questionsPlayed = 0;
            questionTimeoutTimer = 0;
            statusText.text = "Paused";
            OSCSend.OnSendPlayVideo();
            play.SetActive(true);
            pause.SetActive(false);
            Time.timeScale = 0.0f;
        }
        else
        {
            if (osc == 0)
            {
                questionChanging = true;
                StartCoroutine(NextQuestionRun());
                Debug.Log("Next Question");
                resetHandler.ResetList();
                questionTimeoutTimer = 0;
                responseManager.numResponses = 0;
                responseManager.responses.Clear();
                responseManager.iDs.Clear();
                responseManager.timer = 0;
                StopCoroutine(responseManager.createWordsfromResponse());
            }
            else
            {
                StopCoroutine(NextQuestionRun());
                Debug.Log("Next Question");
                resetHandler.ResetList();
                questionTimeoutTimer = 0;
                responseManager.numResponses = 0;
                responseManager.responses.Clear();
                responseManager.iDs.Clear();
                responseManager.timer = 0;
                if (responseManager.selectedQuestion < responseManager.questions.Count - 1)
                {
                    responseManager.selectedQuestion++;
                }
                else if (responseManager.selectedQuestion == responseManager.questions.Count - 1)
                {
                    responseManager.selectedQuestion = 0;
                }
                updateQuestion(responseManager.selectedQuestion);
                OSCSend.OnSendNewQuestion(responseManager.videoIds[responseManager.selectedQuestion]);
                responseManager.AddQuestionWords(responseManager.questionWords[responseManager.selectedQuestion]);
                //awaitQuestionReturn = true;
            }
            questionsPlayed++;
        }
    }
    public IEnumerator NextQuestionRun()
    {
        yield return new WaitForSeconds(4);
        questionAnimation.Play("Base Layer.QuestionAnimation", -1, 0);
        questionAnimation.enabled = true;
        yield return new WaitForSeconds(2);
        if (responseManager.selectedQuestion < responseManager.questions.Count - 1)
        {
            responseManager.selectedQuestion++;
        }
        else if (responseManager.selectedQuestion == responseManager.questions.Count - 1)
        {
            responseManager.selectedQuestion = 0;
        }
        updateQuestion(responseManager.selectedQuestion);
        OSCSend.OnSendNewQuestion(responseManager.videoIds[responseManager.selectedQuestion]);
        responseManager.AddQuestionWords(responseManager.questionWords[responseManager.selectedQuestion]);
        yield return new WaitForSeconds(2);
        questionAnimation.enabled = false;

        questionChanging = false;
    }
    public void PrevQuestion()
    {
        questionTimeoutTimer = 0;
        responseManager.numResponses = 0;
        responseManager.responses.Clear();
        responseManager.iDs.Clear();
        responseManager.timer = 0;
        StopCoroutine(responseManager.createWordsfromResponse());
        Debug.Log("Previous Question");
        resetHandler.ResetList();
        if (responseManager.selectedQuestion > 0)
        {
            responseManager.selectedQuestion--;
        }
        else if (responseManager.selectedQuestion == 0)
        {
            responseManager.selectedQuestion = responseManager.questions.Count - 1;
        }
        updateQuestion(responseManager.selectedQuestion);
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    public void SwitchCamera()
    {
        if (count < devices.Length - 1)
        {
            count++;
            webCamTextureToMatHelper.requestedDeviceName = devices[count].name;
            PlayerPrefs.SetString("selectedCamera", webCamTextureToMatHelper.requestedDeviceName);
        }
        else
        {
            count = 0;
            webCamTextureToMatHelper.requestedDeviceName = devices[count].name;
            PlayerPrefs.SetString("selectedCamera", webCamTextureToMatHelper.requestedDeviceName);
        }
    }

    public void TestButtonPressed()
    {
        objectDetectionYOLOv7Example.test = !objectDetectionYOLOv7Example.test;
        if (objectDetectionYOLOv7Example.test == true)
        {
            testButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Test (on)";
            switchCameraButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            testButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Test (off)";
            switchCameraButton.GetComponent<Button>().interactable = true;
        }

    }

    public void FlipCamera()
    {
        if (webCamTextureToMatHelper.flipHorizontal == true)
        {
            webCamTextureToMatHelper.flipHorizontal = false;
            PlayerPrefs.SetInt("flipCamera", 0);
        }
        else
        {
            webCamTextureToMatHelper.flipHorizontal = true;
            PlayerPrefs.SetInt("flipCamera", 1);
        }

    }

    public void onDropdownChange(TMP_Dropdown change)
    {
        videoCaptureToMatHelper.requestedVideoFilePath = change.captionText.text;
    }

    public void OnSetTimeout()
    {
        topLockHandler.timerLimit = float.Parse(timeoutInput.text);
        topLockHandler.topTimer = float.Parse(timeoutInput.text);
        PlayerPrefs.SetFloat("timeoutDuration", float.Parse(timeoutInput.text));
    }
    public void OnSetWordTimeout()
    {
        wordStore.GetComponent<wordStoreHandler>().timeOut = float.Parse(wordTimeoutInput.text);
        wordStore.GetComponent<wordStoreHandler>().timer = float.Parse(wordTimeoutInput.text);
        PlayerPrefs.SetFloat("wordTimeout", float.Parse(wordTimeoutInput.text));
    }
    public void OnSetWordLimit()
    {
        wordStore.GetComponent<wordStoreHandler>().wordLimit = int.Parse(wordLimit.text);
        PlayerPrefs.SetInt("wordLimit", int.Parse(wordLimit.text));
    }
    public void OnSetCharLimit()
    {
        topLineHandler.maxCharCount = int.Parse(charLimit.text);
        PlayerPrefs.SetInt("charLimit", int.Parse(charLimit.text));
    }
    public void OnStartPress()
    {
        Time.timeScale = 1.0f;
        pause.SetActive(true);
    }
    public void Pause()
    {
        Time.timeScale = 0;
        pause.SetActive(false);
        play.SetActive(true);
    }
    public void Play(int osc)
    {
        statusText.text = "Playing.";
        NextQuestion(osc);
        questionTimeoutTimer = 0;
        Time.timeScale = 1.0f;
        play.SetActive(false);
        pause.SetActive(true);
    }
    public void SetHoldLimit()
    {
        holdTimer = float.Parse(holdLimit.text);
        PlayerPrefs.SetFloat("holdLimit", holdTimer);
    }
    public void SetLockLimit()
    {
        lockTimer = float.Parse(lockLimit.text);
        PlayerPrefs.SetFloat("lockLimit", lockTimer);
    }
    public void OnDebugOpsClick()
    {
        if (randomHandlerButton.activeSelf)
        {
            debug = false;
            randomEventHandler.SetActive(false);
            randomHandlerButton.SetActive(false);
            userIndicatorButton.SetActive(false);
            userIndicatorButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Indicators (off)";
            randomHandlerButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Random (off)";

        }
        else
        {
            randomHandlerButton.SetActive(true);
            userIndicatorButton.SetActive(true);
        }
    }
    public void OnRandomPress()
    {
        if (randomEventHandler.activeSelf)
        {
            randomEventHandler.SetActive(false);
            randomHandlerButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Random (off)";
        }
        else
        {
            randomEventHandler.SetActive(true);
            randomHandlerButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Random (on)";
        }
    }
    public void OnPlayerIndicatorPress()
    {
        if (debug)
        {
            triggerLine.SetActive(false);
            debug = false;
            userIndicatorButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Indicators (off)";
        }
        else
        {
            triggerLine.SetActive(true);
            debug = true;
            userIndicatorButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Indicators (on)";
        }
    }
    public void OnShowSetupPress()
    {
        if (block.activeSelf)
        {
            block.SetActive(false);
        }
        else
        {
            block.SetActive(true);
        }
    }
    public void AddCustomWordApp(string message)
    {
        if (message != "")
        {
            string noSpaces = message.Replace(" ", "");
            List<string> words = new List<string>();
            foreach (string word in noSpaces.Split(","))
            {
                words.Add(word);
            }
            StartCoroutine(responseHandler.GetComponent<responseManager>().CreateObjects(words));
        }
        else
        {
            Debug.Log("Now word found.");
        }
    }
    
    public void AddCustomWord()
    {
        if (addWord.text != "")
        {
            string noSpaces = addWord.text.Replace(" ", "");
            List<string> words = new List<string>();
            foreach (string word in noSpaces.Split(","))
            {
                words.Add(word);
            }
            StartCoroutine(responseHandler.GetComponent<responseManager>().CreateObjects(words));
            addWord.text = "";
        }
        else
        {
            Debug.Log("Now word found.");
        }
    }
    public void OnParticalPress()
    {
        particalAnim = !particalAnim;
    }

    public void OnDemoPress()
    {
        if (wordStoreHandler.playDemo)
        {
            wordStoreHandler.playDemo = false;
            PlayerPrefs.SetInt("demo", 0);
        }
        else
        {
            wordStoreHandler.playDemo = true;
            PlayerPrefs.SetInt("demo", 1);
        }

        wordStoreHandler.allEmptyTimer = 0;

    }
    public void OnFinishAnimPress()
    {
        finishAnim = !finishAnim;
    }
    public void OnAddWordFocus()
    {
        addWordHint.gameObject.SetActive(true);
    }
    public void OnAddWordDeselect()
    {
        addWordHint.gameObject.SetActive(false);
    }
    public void SetBGColor()
    {
        if (red.text != "" && blue.text != "" && green.text != "")
        {
            float r = int.Parse(red.text);
            float g = int.Parse(green.text);
            float b = int.Parse(blue.text);
            if (r >= 0 && r <= 255 && b >= 0 && b <= 255 && g >= 0 && g <= 255)
            {
                background.color = new Color(r / 255f, g / 255f, b / 255f, 1);
                Debug.Log("Background color changed to: " + background.color);
                PlayerPrefs.SetFloat("bgRed", r / 255f);
                PlayerPrefs.SetFloat("bgGreen", g / 255f);
                PlayerPrefs.SetFloat("bgBlue", b / 255f);
            }
            else
            {
                Debug.Log("Int out of range. Please choose between 0 and 255.");
            }
        }
    }
    public void OnOpenNetworkSettings()
    {
        networkSettings.SetActive(true);
        networkSettings.transform.SetAsLastSibling();
    }

    public void OnOpenCVConf()
    {
        openCVConfig.SetActive(!openCVConfig.activeSelf);
        openCVConfig.transform.SetAsLastSibling();
    }
    public void ToggleIndicatorOnProjection()
    {
        if (mainCamera.cullingMask == 32)
        {
            mainCamera.cullingMask = 33;
            IndicatorProjectionButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Proj.Ind. (on)";
        }
        else
        {
            mainCamera.cullingMask = 32;
            IndicatorProjectionButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Proj.Ind. (off)";

        }
    }
    public void OnCenterLinePress()
    {
        centerLine.SetActive(!centerLine.activeSelf);
    }
    public void OnToggleCustomOSC()
    {
        customOSCContainer.SetActive(!customOSCContainer.activeSelf);
    }
    public void OnToggleInfo()
    {
        infoPanel.SetActive(!infoPanel.activeSelf);
    }
    public void OnRefreshQuestions()
    {
        questions.ExecuteQuery = true;
        awaitQuestionReturn = true;
    }
    public void ToggleFullscreen()
    {
        System.Diagnostics.Process.Start("C:/Program Files/obs-studio/bin/64bit/obs64.exe");
    }
}
