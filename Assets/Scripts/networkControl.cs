using GraphQL4Unity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class networkControl : MonoBehaviour
{
    public GraphQLHttp WordQueryHttp;
    public GraphQLHttp QuestionQueryHttp;
    public GraphQLHttp ResponseQueryHttp;
    public GraphQLHttp CurrentQuestionMutationHttp;
    public GraphQLHttp CurrentQuestionQueryHttp;
    public GraphQLHttp ResponseMutationHttp;
    public TextMeshProUGUI WordQueryIndicator;
    public TextMeshProUGUI QuestionQueryIndicator;
    public TextMeshProUGUI ResponseQueryIndicator;
    public TextMeshProUGUI CurrentQuestionMutationIndicator;
    public TextMeshProUGUI CurrentQuestionQueryIndicator;
    public TextMeshProUGUI ResponseMutationIndicator;
    public Image WordQueryImage;
    public Image QuestionQueryImage;
    public Image ResponseQueryImage;
    public Image CurrentQuestionMutationImage;
    public Image CurrentQuestionQueryImage;
    public Image ResponseMutationImage;
    public Button WordQEx;
    public Button QuestionQEx;
    public Button ResponseQEx;
    public Button CurrentQuestionQEx;
    public Button CurrentQuestionMEx;
    public Button ResponseMEx;
    public topLockHandler topLockHandler;
    public TextMeshProUGUI topLineHandler;
    public finishHandler finishHandler;
    Color ExGreen = new Color(0, 220/255f, 160/255f);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (WordQueryHttp.LogMessages)
        {
            WordQueryIndicator.text = "On";
            WordQueryIndicator.color = Color.black;
            WordQueryImage.color = Color.white;
            WordQEx.interactable = true;
        }
        else
        {
            WordQueryIndicator.text = "Off";
            WordQueryIndicator.color = ExGreen;
            WordQueryImage.color = Color.black;
            WordQEx.interactable = false;
        }
        if (QuestionQueryHttp.LogMessages)
        {
            QuestionQueryIndicator.text = "On";
            QuestionQueryIndicator.color = Color.black;
            QuestionQueryImage.color = Color.white;
            QuestionQEx.interactable = true;
        }
        else
        {
            QuestionQueryIndicator.text = "Off";
            QuestionQueryIndicator.color = ExGreen;
            QuestionQueryImage.color = Color.black;
            QuestionQEx.interactable = false;
        }
        if (ResponseQueryHttp.LogMessages)
        {
            ResponseQueryIndicator.text = "On";
            ResponseQueryIndicator.color = Color.black;
            ResponseQueryImage.color = Color.white;
            ResponseQEx.interactable = true;
        }
        else
        {
            ResponseQueryIndicator.text = "Off";
            ResponseQueryIndicator.color = ExGreen;
            ResponseQueryImage.color = Color.black;
            ResponseQEx.interactable = false;
        }
        if (CurrentQuestionQueryHttp.LogMessages)
        {
            CurrentQuestionQueryIndicator.text = "On";
            CurrentQuestionQueryIndicator.color = Color.black;
            CurrentQuestionQueryImage.color = Color.white;
            CurrentQuestionQEx.interactable = true;
        }
        else
        {
            CurrentQuestionQueryIndicator.text = "Off";
            CurrentQuestionQueryIndicator.color = ExGreen;
            CurrentQuestionQueryImage.color = Color.black;
            CurrentQuestionQEx.interactable= false;
        }
        if (CurrentQuestionMutationHttp.LogMessages)
        {
            CurrentQuestionMutationIndicator.text = "On";
            CurrentQuestionMutationIndicator.color = Color.black;
            CurrentQuestionMutationImage.color = Color.white;
            CurrentQuestionMEx.interactable = true;
        }
        else
        {
            CurrentQuestionMutationIndicator.text = "Off";
            CurrentQuestionMutationIndicator.color = ExGreen;
            CurrentQuestionMutationImage.color = Color.black;
            CurrentQuestionMEx.interactable = false;

        }
        if (ResponseMutationHttp.LogMessages)
        {
            ResponseMutationIndicator.text = "On";
            ResponseMutationIndicator.color = Color.black;
            ResponseMutationImage.color = Color.white;
            if (topLockHandler.inPlace.Count > 0 || topLineHandler.text != "") 
            {
                ResponseMEx.interactable = true;
            }
            else
            {
                ResponseMEx.interactable = false;
            }
        }
        else
        {
            ResponseMutationIndicator.text = "Off";
            ResponseMutationIndicator.color = ExGreen;
            ResponseMutationImage.color = Color.black;
            ResponseMEx.interactable = false;
        }
    }
    public void OnClosePress()
    {
        //WordQueryHttp.LogMessages = false;
        //QuestionQueryHttp.LogMessages = false;
        //CurrentQuestionQueryHttp.LogMessages = false;
        //ResponseQueryHttp.LogMessages = false;
        //CurrentQuestionMutationHttp.LogMessages = false;
        //ResponseMutationHttp.LogMessages = false;
        transform.gameObject.SetActive(false);
    }
    public void OnWordQPress()
    {
        WordQueryHttp.LogMessages = !WordQueryHttp.LogMessages;
    }
    public void OnQuestionQPress()
    {
        QuestionQueryHttp.LogMessages = !QuestionQueryHttp.LogMessages;
    }
    public void OnCurrentQuestionQPress()
    {
        CurrentQuestionQueryHttp.LogMessages = !CurrentQuestionQueryHttp.LogMessages;
    }
    public void OnResponseQPress()
    {
        ResponseQueryHttp.LogMessages = !ResponseQueryHttp.LogMessages;
    }
    public void OnCurrentQuestionMPress()
    {
        CurrentQuestionMutationHttp.LogMessages = !CurrentQuestionMutationHttp.LogMessages;
    }
    public void OnResponseMPress()
    {
        ResponseMutationHttp.LogMessages = !ResponseMutationHttp.LogMessages;
    }
    public void Execute(string target)
    {
        switch (target)
        {
            case "wordQ":
                WordQueryHttp.transform.GetChild(0).GetComponent<GraphQLQuery>().ExecuteQuery = true;
                break;
            case "QuestionQ":
                QuestionQueryHttp.transform.GetChild(0).GetComponent<GraphQLQuery>().ExecuteQuery = true;
                break;
            case "CurrentQuestionQ":
                CurrentQuestionQueryHttp.transform.GetChild(0).GetComponent<GraphQLQuery>().ExecuteQuery = true;
                break;
            case "ResponseQ":
                ResponseQueryHttp.transform.GetChild(0).GetComponent<GraphQLQuery>().ExecuteQuery = true;
                break;
            case "CurrentQuestionM":
                CurrentQuestionMutationHttp.transform.GetChild(0).GetComponent<GraphQLQuery>().ExecuteQuery = true;
                break;
            case "ResponseM":
                finishHandler.onFinishEvent("Execute Test");
                //ResponseMutationHttp.transform.GetChild(0).GetComponent<GraphQLQuery>().ExecuteQuery = true;
                break;
            default:
                Debug.Log("No target selected");
                break;
        }
    }
}
