using extOSC;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OSCSend : MonoBehaviour
{
    public OSCTransmitter transmitter;
    public TMP_InputField address;
    public TMP_InputField message;
    public TMP_InputField backgroundTag;
    public Button sendBackgroundButton;
    public Button sendMessage;
    public Dropdown type;
    string choice = "String";
    public int outPort;
    public int qLabPort;

    private void Awake()
    {
        transmitter = GetComponent<OSCTransmitter>();
        if (PlayerPrefs.HasKey("oscIP"))
        {
            transmitter.RemoteHost = PlayerPrefs.GetString("oscIP");
        }
        else
        {
            transmitter.RemoteHost = "192.168.0.0";
            PlayerPrefs.SetString("oscIP", "192.168.0.0");
        }
        if (PlayerPrefs.HasKey("oscOutPort"))
        {
            outPort = PlayerPrefs.GetInt("oscOutPort");
            transmitter.RemotePort = PlayerPrefs.GetInt("oscOutPort");
        }
        else
        {
            outPort = 8010;
            transmitter.RemotePort = 8010;
            PlayerPrefs.SetInt("oscOutPort", 8010);
        }
        if (PlayerPrefs.HasKey("oscQLabPort"))
        {
            qLabPort = PlayerPrefs.GetInt("oscQLabPort");
        }
        else
        {
            qLabPort = 8020;
            PlayerPrefs.SetInt("oscQLabPort", 8010);
        }
    }
    private void Update()
    {
        if (backgroundTag.text == "")
        {
            sendBackgroundButton.interactable = false;
        }
        else
        {
            sendBackgroundButton.interactable = true;
        }
        if (address.text == "" || message.text == "")
        {
            sendMessage.interactable = false;
        }
        else
        {
            sendMessage.interactable = true;
        }
    }

    public void OnSendNewQuestionTest()
    {
        var _message = new OSCMessage("/" + backgroundTag.text);
        _message.AddValue(OSCValue.Int(1));
        transmitter.RemotePort = outPort;
        transmitter.Send(_message);
    }
    public void OnSendNewQuestion(string backgroundTag)
    {
        var _message = new OSCMessage("/" + backgroundTag);
        _message.AddValue(OSCValue.Int(1));
        transmitter.RemotePort = outPort;
        transmitter.Send(_message);
        Debug.Log("OSC Message Out: /" + backgroundTag + " | On port: " + transmitter.RemotePort);
    }
    public void OnSendPlayVideo()
    {
        var _message = new OSCMessage("/video");
        _message.AddValue(OSCValue.Int(1));
        transmitter.Send(_message);
        Debug.Log("OSC Message Out: /video | On port: " + transmitter.RemotePort);
        transmitter.RemotePort = qLabPort;
        transmitter.Send(_message);
        Debug.Log("OSC Message Out: /video | On port: " + transmitter.RemotePort);
        transmitter.RemotePort = outPort;
    }
    public void OnSendCustomOSC()
    {
        var _message = new OSCMessage("/" + address.text);
        switch (choice)
        {
            case "String":
                _message.AddValue(OSCValue.String(message.text));
                break;
            case "Int":
                _message.AddValue(OSCValue.Int(int.Parse(message.text)));
                break;
            case "Float":
                _message.AddValue(OSCValue.Float(float.Parse(message.text)));
                break;
        }
        transmitter.Send(_message);
        message.text = "";
        address.text = "";
    }

    public void OnOptionChange(TMP_Dropdown change)
    {
        choice = change.captionText.text;
    }
}
