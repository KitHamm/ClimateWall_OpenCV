using GraphQL4Unity;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class finishHandler : MonoBehaviour
{
    public TextMeshProUGUI finishDelay;
    public TextMeshProUGUI finishTimer;
    public TextMeshProUGUI topLine;
    public GameObject ResponseMutation;
    public GameObject topLockHandler;
    public GameObject Reset;
    public GameObject responseHandler;
    public GameObject questionText;
    public GameObject finishAnimation;
    float finishAnimTimer;
    public bool awaitReturn;
    public bool finishEvent;
    public string finishedResponse;
    public GraphQLQuery responseMutationPrefab;
    public GameObject GraphQLHttp;
    control control;
    topLockHandler tpHandler;
    resetHandler resetHandler;
    responseManager responseManager;
    // Start is called before the first frame update
    [Serializable]
    public class ResponseObject
    {
        public string response;
        public string question;
    }

    void Start()
    {
        finishAnimTimer = 0f;
        finishEvent = false;
        awaitReturn = false;
        control = GameObject.FindGameObjectWithTag("control").GetComponent<control>();
        //MutationQuery = ResponseMutation.GetComponent<GraphQLQuery>();
        tpHandler = topLockHandler.GetComponent<topLockHandler>();
        resetHandler = Reset.GetComponent<resetHandler>();
        responseManager = responseHandler.GetComponent<responseManager>();
    }
    private void FixedUpdate()
    {
    }
    // Update is called once per frame
    void Update()
    {
        // Await return from finish mutation query
        if (awaitReturn)
        {
            GameObject MuatationGO = GameObject.FindGameObjectWithTag("responseMutation");
            GraphQLQuery mutationQuery = MuatationGO.GetComponent<GraphQLQuery>();
            if (mutationQuery.Data.Contains("createQRepsonse"))
            {
                if (control.finishAnim)
                {
                    finishEvent = true;
                    Vector2 animationPos = new Vector2();
                    animationPos.x = 960f;
                    animationPos.y = 740f;
                    GameObject finishAnimationGO = Instantiate(finishAnimation);
                    finishAnimationGO.transform.SetParent(GameObject.FindGameObjectWithTag("MainCanvas").transform);
                    finishAnimationGO.transform.SetSiblingIndex(1);
                    finishAnimationGO.transform.position = animationPos;
                    finishAnimationGO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = finishedResponse;
                }
                Debug.Log("Saved: " + finishedResponse);
                mutationQuery.ExecuteQuery = false;
                resetHandler.ResetList();
                awaitReturn = false;
                mutationQuery.VariablesAsJson = "";
                finishedResponse = "";
                Destroy(MuatationGO);
            }
        }
        if (finishEvent)
        {
            finishAnimTimer += Time.deltaTime;
            if (finishAnimTimer > 6.0f)
            {
                finishEvent = false;
                finishAnimTimer = 0;
            }
        }
    }


    // Build string and send to server
    public void onFinishEvent(string message)
    {
        GraphQLQuery responseMutationQuery = Instantiate(responseMutationPrefab);
        responseMutationQuery.transform.SetParent(GraphQLHttp.transform);
        responseMutationQuery.ExecuteQuery = false;
        responseMutationQuery.tag = "responseMutation";
        String myString = topLine.text + " ";
        if (tpHandler.inPlace.Count > 0 || topLine.text != "")
        {
            foreach (GameObject child in tpHandler.inPlace)
            {
                myString += child.name.ToString() + " ";
            }
            finishedResponse = myString;
            ResponseObject response = new ResponseObject();
            response.response = myString;
            response.question = questionText.GetComponent<TextMeshProUGUI>().text;
            string variable = JsonConvert.SerializeObject(response);
            responseMutationQuery.VariablesAsJson = variable;
            responseMutationQuery.ExecuteQuery = true;
            awaitReturn = true;
            Debug.Log("Content Saved due to " + message);
        }
        else
        {
            Debug.Log("List empty");
        }
    }
}
