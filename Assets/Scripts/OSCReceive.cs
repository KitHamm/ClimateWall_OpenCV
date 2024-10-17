using extOSC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSCReceive : MonoBehaviour
{
    public OSCReceiver receiver;
    public topLockHandler topLockHandler;
    public control control;
    public undoHandler undoHandler;
    public resetHandler resetHandler;
    public finishHandler finishHandler;
    public responseManager responseManager;
    bool wait;
    float timer;
    float oldTopTimerLimit;

    private void Awake()
    {
        receiver = GetComponent<OSCReceiver>();
        receiver.Bind("/play", OnPlayReceived);
        receiver.Bind("/playcontrol", OnPlayControlReceived);
        receiver.Bind("/pausecontrol", OnPauseControlReceived);
        receiver.Bind("/undo", OnUndoReceived);
        receiver.Bind("/reset", OnResetReceived);
        receiver.Bind("/finsh", OnFinishReceived);
        receiver.Bind("/nextquestion", OnNextQuestionReceived);
        receiver.Bind("/refresh", OnRefreshReceived);
        receiver.Bind("/restart", OnRestartReceived);
        receiver.Bind("/exit", OnExitReceived);
        receiver.Bind("/addword", OnAddWordReceived);

        receiver.Bind("/video", OnVideoReceived);
        receiver.Bind("/caremissions", OnNewQuestionReceived);
        receiver.Bind("/coal", OnNewQuestionReceived);
        receiver.Bind("/coastal", OnNewQuestionReceived);
        receiver.Bind("/cycling", OnNewQuestionReceived);
        receiver.Bind("/education", OnNewQuestionReceived);
        receiver.Bind("/electricvehicles", OnNewQuestionReceived);
        receiver.Bind("/protest", OnNewQuestionReceived);
        receiver.Bind("/renewables", OnNewQuestionReceived);
        receiver.Bind("/rivers", OnNewQuestionReceived);
        receiver.Bind("/trees", OnNewQuestionReceived);
        receiver.Bind("/underwater", OnNewQuestionReceived);
        receiver.Bind("/vegetables", OnNewQuestionReceived);
        receiver.Bind("/walnuts", OnNewQuestionReceived);

        if (PlayerPrefs.HasKey("oscInPort"))
        {
            receiver.LocalPort = PlayerPrefs.GetInt("oscInPort");
        }
        else
        {
            receiver.LocalPort = 6969;
            PlayerPrefs.SetInt("oscInPort", 6969);
        }
    }
    private void Start()
    {
        wait = false;
    }
    private void Update()
    {
        if (wait)
        {
            timer += Time.deltaTime;
            if (timer > 2.05f)
            {
                topLockHandler.timerLimit = oldTopTimerLimit;
                topLockHandler.topTimer = topLockHandler.timerLimit;
                timer = 0;
                wait = false;
            }
        }
    }

    void OnPlayReceived(OSCMessage message)
    {
        if (Time.timeScale == 0f)
        {
            Debug.Log("OSC Message In: /play | On port: " + receiver.LocalPort);
            control.Play(1);
        }
    }
    void OnVideoReceived(OSCMessage message)
    {
        Debug.Log(message);
    }
    void OnNewQuestionReceived(OSCMessage message)
    {
        Debug.Log(message);
    }
    void OnPlayControlReceived(OSCMessage message)
    {
        control.Play(1);
    }
    void OnPauseControlReceived(OSCMessage message)
    {
        control.Pause();
    }
    void OnUndoReceived(OSCMessage message)
    {
        undoHandler.Undo();
    }
    void OnResetReceived(OSCMessage message)
    {
        resetHandler.ResetList();
    }
    void OnFinishReceived(OSCMessage message)
    {
        oldTopTimerLimit = topLockHandler.timerLimit;
        Debug.Log("Force Finish from App");
        topLockHandler.topTimer = 2;
        topLockHandler.timerLimit = 2;
        wait = true;
    }
    public void OnFinishPress()
    {
        oldTopTimerLimit = topLockHandler.timerLimit;
        Debug.Log("Force Finish from App");
        topLockHandler.topTimer = 2;
        topLockHandler.timerLimit = 2;
        wait = true;
    }
    void OnNextQuestionReceived(OSCMessage message)
    {
        control.NextQuestion(0);
    }
    void OnRefreshReceived(OSCMessage message)
    {
        control.OnRefreshQuestions();
    }
    void OnRestartReceived(OSCMessage message)
    {
        control.ResetScene();
    }
    void OnExitReceived(OSCMessage message)
    {
        control.ExitGame();
    }
    void OnAddWordReceived(OSCMessage message)
    {
        control.AddCustomWordApp(message.Values[0].Value.ToString());
        Debug.Log(message.Values[0].Value + " - received from app");
    }
}
