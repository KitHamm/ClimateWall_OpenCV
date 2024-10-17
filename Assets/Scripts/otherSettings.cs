using GraphQL4Unity;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class otherSettings : MonoBehaviour
{
    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

    public OSCReceive OSCReceive;
    public OSCSend OSCSend;
    public responseManager responseManager;
    public Camera secondCam;
    private bool awaitReturn;
    private bool set;
    public GameObject otherSettingsScreen;
    public control control;
    public TMP_InputField holdSens;
    public TMP_InputField distSens;
    public TMP_InputField force;
    public TMP_InputField boxSize;
    public TMP_InputField lockPoint;
    public TMP_InputField holdPoint;
    public TMP_InputField tempHold;
    public TMP_InputField deleteDelay;
    public TMP_InputField shiftAmount;
    public TMP_InputField shiftFactor;
    public TMP_InputField questionTimeout;
    public TMP_InputField maxQuestionsInput;
    public TMP_InputField maxResponsesInput;
    public TMP_InputField timeBetweenResponseInput;
    public TMP_InputField OSC_IP_Input;
    public TMP_InputField OSC_InPortInput;
    public TMP_InputField OSC_OutPortInput;
    public TMP_InputField OSC_QLabPortInput;
    public TMP_InputField responseDelayInput;
    public TMP_InputField DemoTimerInput;
    public Button setHoldSens;
    public Button setDistSens;
    public Button setForce;
    public Button setBoxSize;
    public Button setLockPoint;
    public Button setHoldPoint;
    public Button setTempHold;
    public Button setDeleteDelay;
    public Button setShiftAmount;
    public Button setShiftFactor;
    public Button setQuestionTimeout;
    public Button setMaxQuestions;
    public Button setMaxResponses;
    public Button setTimeBetweenResponse;
    public Button setOSC_IP;
    public Button setOSC_InPort;
    public Button setOSC_OutPort;
    public Button setOSC_QLabPort;
    public Button setResponseDelay;
    public Button setDemoTimer;
    public TextMeshProUGUI questionsPlayed;
    public GameObject maxResponseHTTP;
    public GameObject deleteWord;
    public GraphQLQuery updateMaxResponse;
    public class updateMaxResponseData
    {
        public int amount;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("demoTimer"))
        {
            DemoTimerInput.text = PlayerPrefs.GetFloat("demoTimer").ToString("F1");

        }
        else
        {
            DemoTimerInput.text = GameObject.FindGameObjectWithTag("wordStore").GetComponent<wordStoreHandler>().allEmptyTimerLimit.ToString("F1");
        }
        awaitReturn = false;
        holdSens.text = control.holdSens.ToString();
        distSens.text = control.distanceSens.ToString();
        force.text = control.force.ToString();
        boxSize.text = control.boxSize.ToString();
        lockPoint.text = control.lockPoint.ToString();
        holdPoint.text = control.holdPoint.ToString();
        tempHold.text = control.tempHold.ToString();
        deleteDelay.text = control.deleteDelayTime.ToString("F1");
        shiftAmount.text = control.shiftScale.ToString();
        questionTimeout.text = control.questionTimeout.ToString();
        timeBetweenResponseInput.text = responseManager.timeBetweenResponse.ToString();
        OSC_IP_Input.text = OSCSend.transmitter.RemoteHost.ToString();
        OSC_OutPortInput.text = OSCSend.transmitter.RemotePort.ToString();
        OSC_QLabPortInput.text = OSCSend.qLabPort.ToString(); 
        OSC_InPortInput.text = OSCReceive.receiver.LocalPort.ToString();
        maxQuestionsInput.text = control.maxQuestions.ToString();
        shiftFactor.text = control.offsideShiftFactor.ToString();
        responseDelayInput.text = control.responseDelayTime.ToString("F1");
    }

    // Update is called once per frame
    void Update()
    {
        questionsPlayed.text = control.questionsPlayed.ToString();

        if (awaitReturn) 
        {
            GameObject maxResponseMutaionGO = GameObject.FindGameObjectWithTag("maxResponseUpdate");
            GraphQLQuery graphQLQuery = maxResponseMutaionGO.GetComponent<GraphQLQuery>();
            if (graphQLQuery.Data.Contains("updateMaxResponse"))
            {
                awaitReturn = false;
                Destroy(maxResponseMutaionGO);
            }
        }
        if (DemoTimerInput.text != GameObject.FindGameObjectWithTag("wordStore").GetComponent<wordStoreHandler>().allEmptyTimerLimit.ToString("F1"))
        {
            setDemoTimer.interactable = true;
        }
        else
        {
            setDemoTimer.interactable = false;
        }
        if (responseDelayInput.text != control.responseDelayTime.ToString("F1"))
        {
            setResponseDelay.interactable = true;
        }
        else
        {
            setResponseDelay.interactable = false;
        }
        if (maxQuestionsInput.text != control.maxQuestions.ToString())
        {
            setMaxQuestions.interactable = true;
        }
        else
        {
            setMaxQuestions.interactable = false;
        }
        if (OSC_IP_Input.text != OSCSend.transmitter.RemoteHost.ToString())
        {
            setOSC_IP.interactable = true;
        }
        else
        {
            setOSC_IP.interactable = false;
        }
        if (OSC_OutPortInput.text != OSCSend.transmitter.RemotePort.ToString())
        {
            setOSC_OutPort.interactable = true;
        }
        else
        {
            setOSC_OutPort.interactable = false;
        }
        if (OSC_QLabPortInput.text != OSCSend.qLabPort.ToString())
        {
            setOSC_QLabPort.interactable = true;
        }
        else
        {
            setOSC_QLabPort.interactable = false;
        }

        if (OSC_InPortInput.text != OSCReceive.receiver.LocalPort.ToString())
        {
            setOSC_InPort.interactable = true;
        }
        else
        {
            setOSC_InPort.interactable = false;
        }
        if (timeBetweenResponseInput.text != responseManager.timeBetweenResponse.ToString())
        {
            setTimeBetweenResponse.interactable = true;
        }
        else
        {
            setTimeBetweenResponse.interactable = false;
        }
        if (maxResponsesInput.text != responseManager.maxResponses.ToString())
        {
            setMaxResponses.interactable = true;
        }
        else
        {
            setMaxResponses.interactable = false;
        }
        if (questionTimeout.text != control.questionTimeout.ToString())
        {
            setQuestionTimeout.interactable = true;
        }
        else
        {
            setQuestionTimeout.interactable = false;
        }
        if (shiftAmount.text != control.shiftScale.ToString())
        {
            setShiftAmount.interactable = true;
        }
        else
        {
            setShiftAmount.interactable = false;
        }
        if (shiftFactor.text != control.offsideShiftFactor.ToString())
        {
            setShiftFactor.interactable = true;
        }
        else
        {
            setShiftFactor.interactable = false;
        }
        if (deleteDelay.text != control.deleteDelayTime.ToString("F1"))
        {
            setDeleteDelay.interactable = true;
        }
        else
        {
            setDeleteDelay.interactable = false;
        }
        if (holdSens.text != control.holdSens.ToString())
        {
            setHoldSens.interactable = true;
        }
        else
        {
            setHoldSens.interactable = false;
        }
        if (distSens.text != control.distanceSens.ToString())
        {
            setDistSens.interactable = true;
        }
        else
        {
            setDistSens.interactable = false;
        }
        if (force.text != control.force.ToString())
        {
            setForce.interactable = true;
        }
        else
        {
            setForce.interactable = false;
        }
        if (boxSize.text != control.boxSize.ToString())
        {
            setBoxSize.interactable = true;
        }
        else
        {
            setBoxSize.interactable = false;
        }
        if (lockPoint.text != control.lockPoint.ToString())
        {
            setLockPoint.interactable = true;
        }
        else
        {
            setLockPoint.interactable = false;
        }
        if (holdPoint.text != control.holdPoint.ToString())
        {
            setHoldPoint.interactable = true;
        }
        else
        {
            setHoldPoint.interactable = false;
        }
        if (tempHold.text != control.tempHold.ToString())
        {
            setTempHold.interactable = true;
        }
        else
        {
            setTempHold.interactable = false;
        }
    }
    public void OnClose()
    {
        otherSettingsScreen.SetActive(false);
    }
    public void OnSetHoldSens()
    {
        control.holdSens = float.Parse(holdSens.text);
        PlayerPrefs.SetFloat("holdSens", float.Parse(holdSens.text));
    }
    public void OnSetDistSens()
    {
        control.distanceSens = float.Parse(distSens.text);
        PlayerPrefs.SetFloat("distSens", float.Parse(distSens.text));
    }
    public void OnSetForceSens()
    {
        control.force = int.Parse(force.text);
        PlayerPrefs.SetInt("forceSens", int.Parse(force.text));
    }
    public void OnSetBoxSize()
    {
        control.boxSize = float.Parse(boxSize.text);
        PlayerPrefs.SetFloat("boxSize", float.Parse(boxSize.text));
    }
    public void OnSetLockPoint()
    {
        control.lockPoint = int.Parse(lockPoint.text);
        PlayerPrefs.SetInt("lockPoint", int.Parse(lockPoint.text));
    }
    public void OnSetHoldPoint()
    {
        control.holdPoint = int.Parse(holdPoint.text);
        PlayerPrefs.SetInt("holdPoint", int.Parse(holdPoint.text));
    }
    public void OnSetTempHold()
    {
        control.tempHold = int.Parse(tempHold.text);
        PlayerPrefs.SetInt("tempHold", int.Parse(tempHold.text));
    }
    public void OnSetDeleteDelay()
    {
        control.deleteDelayTime = float.Parse(deleteDelay.text);
        PlayerPrefs.SetFloat("deleteDelayTime", float.Parse(deleteDelay.text));
    }
    public void OnSetResponseDelay()
    {
        control.responseDelayTime = float.Parse(responseDelayInput.text);
        PlayerPrefs.SetFloat("responseDelayTime", float.Parse(responseDelayInput.text));
    }
    public void OnSetShiftAmount()
    {
        control.shiftScale = int.Parse(shiftAmount.text);
        PlayerPrefs.SetInt("shiftScale", int.Parse(shiftAmount.text));
    }
    public void OnSetShiftFactor()
    {
        control.offsideShiftFactor = float.Parse(shiftFactor.text);
        PlayerPrefs.SetFloat("shiftFactor", float.Parse(shiftFactor.text));
    }
    public void OnSetQuestionTimeout()
    {
        control.questionTimeout = int.Parse(questionTimeout.text);
        PlayerPrefs.SetInt("questionTimeout", int.Parse(questionTimeout.text));
    }
    public void OnSetMaxResponses()
    {
        responseManager.maxResponses = int.Parse(maxResponsesInput.text);
        // Mutation to set max responses on server
        GraphQLQuery maxResponseUpdate = Instantiate(updateMaxResponse);
        maxResponseUpdate.transform.SetParent(maxResponseHTTP.transform);
        maxResponseUpdate.ExecuteQuery = false;
        maxResponseUpdate.tag = "maxResponseUpdate";
        updateMaxResponseData data = new updateMaxResponseData();
        data.amount = int.Parse(maxResponsesInput.text);
        string variable = JsonConvert.SerializeObject(data);
        maxResponseUpdate.VariablesAsJson = variable;
        maxResponseUpdate.ExecuteQuery = true;
        awaitReturn = true;
    }
    public void OnSetTimeBetweenResponse()
    {
        responseManager.timeBetweenResponse = int.Parse(timeBetweenResponseInput.text);
        PlayerPrefs.SetInt("timeBetweenResponses", int.Parse(timeBetweenResponseInput.text));
    }
    
    public void OnSetIP()
    {
        OSCSend.transmitter.RemoteHost = OSC_IP_Input.text;
        PlayerPrefs.SetString("oscIP", OSC_IP_Input.text);
    }
    public void OnSetOutPort()
    {
        OSCSend.outPort = int.Parse(OSC_OutPortInput.text);
        OSCSend.transmitter.RemotePort = int.Parse(OSC_OutPortInput.text);
        PlayerPrefs.SetInt("oscOutPort", int.Parse(OSC_OutPortInput.text));
    }
    public void OnSetQLabPort()
    {
        OSCSend.qLabPort = int.Parse(OSC_QLabPortInput.text);
        PlayerPrefs.SetInt("oscQLabPort", int.Parse(OSC_QLabPortInput.text));
    }
    public void OnSetInPort()
    {
        OSCReceive.receiver.LocalPort = int.Parse(OSC_InPortInput.text);
        PlayerPrefs.SetInt("oscInPort", int.Parse(OSC_InPortInput.text));
    }
    public void OnSetMaxQuestions()
    {
        if (int.Parse(maxQuestionsInput.text) < control.questionsPlayed)
        {
            Debug.Log("Cannot set max questions to less than questions played.");
        }
        else
        {
            control.maxQuestions = int.Parse(maxQuestionsInput.text);
            PlayerPrefs.SetInt("maxQuestions", int.Parse(maxQuestionsInput.text));
        }
    }
    public void OpenDeleteWord()
    {
        deleteWord.SetActive(true);
        deleteWord.transform.parent.transform.SetAsLastSibling();
    }
    public void OnSetDemoTimer()
    {
        GameObject.FindGameObjectWithTag("wordStore").GetComponent<wordStoreHandler>().allEmptyTimerLimit = float.Parse(DemoTimerInput.text);
        PlayerPrefs.SetFloat("demoTimer", float.Parse(DemoTimerInput.text));
        DemoTimerInput.text = GameObject.FindGameObjectWithTag("wordStore").GetComponent<wordStoreHandler>().allEmptyTimerLimit.ToString("F1");
    }
}
