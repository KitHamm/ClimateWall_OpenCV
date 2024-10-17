using GraphQL4Unity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class responseManager : MonoBehaviour
{
    public OSCSend OSCSend;
    public otherSettings otherSettings;
    public GameObject textPlaceHolder;
    public GameObject simulatedDelay;
    public GameObject wordStore;
    public GameObject question;
    public GameObject wordQuery;
    public GameObject responseQuery;
    public GameObject responseUpdateHttp;
    public GraphQLQuery responseUpdatePrefab;
    public undoHandler undoHandler;
    public topLineHandler topLineHandler;
    public control control;
    public finishHandler finishHandler;
    public int maxResponses = 100;
    public int numResponses;
    public int timeBetweenResponse = 90;
    bool coroutineRunning;
    public bool creatingWords;
    bool gotQuestionNumber;
    bool gotQuestionIds;
    bool sentFirstVideo;
    bool logRoutine;
    bool awaitReturn;
    String formattedDate;
    String updatedAt;
    public int selectedQuestion;
    int prevSelectedQuestion;
    public float timer;
    Vector2 newTextPosition;
    int count;
    public List<string> questions = new List<string>();
    public List<string> questionWords = new List<string>();
    public List<string> videoIds = new List<string>();
    public List<string> words = new List<string>();
    public List<string> responses = new List<string>();
    public List<int> iDs = new List<int>();
    public class Results
    {
        public string word;
    }
    public class UpdateResponseData
    {
        public int id;
    }
    public class ResponseData
    {
        public string updated;
        public string question;
    }
    private void Awake()
    {
        if (PlayerPrefs.HasKey("timeBetweenResponses"))
        {
            timeBetweenResponse = PlayerPrefs.GetInt("timeBetweenResponses");
        }
        else
        {
            timeBetweenResponse = 90;
            PlayerPrefs.SetInt("timeBetweenResponses", 90);
        }

    }
    private void Start()
    {
        gotQuestionIds = false;
        gotQuestionNumber = false;
        sentFirstVideo = false;
        creatingWords = false;
        numResponses = 0;
        awaitReturn = false;
        logRoutine = false;
        coroutineRunning = false;
        updatedAt = "";
        timer = 0;
        selectedQuestion = 0;
        prevSelectedQuestion = 0;
        formattedDate = DateTime.Today.ToString().Split(" ")[0].Split("/")[2] + "-" + DateTime.Today.ToString().Split(" ")[0].Split("/")[0] + "-" + DateTime.Today.ToString().Split(" ")[0].Split("/")[1] + "T00:00:00.000Z";
        Debug.Log(formattedDate);
    }
    private void Update()
    {
        if (gotQuestionNumber && gotQuestionIds && !sentFirstVideo)
        {
            OSCSend.OnSendNewQuestion(videoIds[selectedQuestion]);
            sentFirstVideo = true;
        }
        if (awaitReturn)
        {
            GameObject MutationGO = GameObject.FindGameObjectWithTag("responseUpdate");
            GraphQLQuery graphQLQuery = MutationGO.GetComponent<GraphQLQuery>();
            if (graphQLQuery.Data.Contains("updateResponse"))
            {
                awaitReturn = false;
                Destroy(MutationGO);
            }
        }
        if (creatingWords != logRoutine)
        {
            if (creatingWords)
            {
                Debug.Log("Printing: " + responses[0]);
            }
            else
            {
                Debug.Log("Done printing response.");
            }
            logRoutine = creatingWords;
        }
        if (questions.Count > 0 && selectedQuestion != prevSelectedQuestion)
        {
            question.GetComponent<TextMeshProUGUI>().text = questions[selectedQuestion];
            prevSelectedQuestion = selectedQuestion;
        }
        timer += Time.deltaTime;
        if (timer >= 5f && updatedAt != "" && !control.questionChanging)
        {
            //wordQuery.GetComponent<GraphQLQuery>().ExecuteQuery = true;
            ResponseData responseData = new ResponseData();
            responseData.updated = updatedAt;
            responseData.question = questions[selectedQuestion];
            //Debug.Log(responseData.question);
            string variable = JsonConvert.SerializeObject(responseData);
            responseQuery.GetComponent<GraphQLQuery>().VariablesAsJson = variable;
            responseQuery.GetComponent<GraphQLQuery>().ExecuteQuery = true;
            timer = 0;
        }
    }
    public void WordResultEvent(GraphQLResult result)
    {
        List<string> newWords = new List<string>();
        count = 0;
        foreach (var word in result.Data["words"]["data"])
        {
            if (!words.Contains(word["attributes"]["word"].ToString()))
            {
                newWords.Add(word["attributes"]["word"].ToString());
            }
        }
        foreach (var word in newWords)
        {
            words.Add(word);
        }
        StartCoroutine(CreateObjects(newWords));

    }

    public void responseResultEvent(GraphQLResult result)
    {
        List<string> newResults = new List<string>();
        foreach (var response in result.Data["responses"]["data"])
        {
            if (!responses.Contains(response["attributes"]["response"].ToString()))
            {
                responses.Add(response["attributes"]["response"].ToString());
                iDs.Add(int.Parse(response["id"].ToString()));
                //Debug.Log(int.Parse(response["id"].ToString()));
                //UpdateResponse(int.Parse(response["id"].ToString()));
            }
        }
        if (!coroutineRunning)
        {
            StartCoroutine(createWordsfromResponse());
        }
    }
    public void UpdateResponse(int id)
    {
        GraphQLQuery responseUpdate = Instantiate(responseUpdatePrefab);
        responseUpdate.transform.SetParent(responseUpdateHttp.transform);
        responseUpdate.ExecuteQuery = false;
        responseUpdate.tag = "responseUpdate";
        UpdateResponseData response = new UpdateResponseData();
        response.id = id;
        string variable = JsonConvert.SerializeObject(response);
        responseUpdate.VariablesAsJson = variable;
        responseUpdate.ExecuteQuery = true;
        awaitReturn = true;
    }
    public IEnumerator createWordsfromResponse()
    {
        coroutineRunning = true;
        if (responses.Count > 0 && numResponses < maxResponses && undoHandler.locks.Length == 0 && !control.questionChanging && !finishHandler.finishEvent)
        {
            int charCount = 0;
            creatingWords = true;
            numResponses++;
            bool firstLine = true;
            List<float> widths = new List<float>();
            int wordCount = 0;
            List<string> words = new List<string>();
            string s = responses[0].ToString();
            var sb = new StringBuilder();
            foreach (char c in s)
            {
                if (!char.IsPunctuation(c))
                {
                    sb.Append(c);
                }
            }
            s = sb.ToString().Replace("\r", " ").Replace("\n", " ");
            foreach (var word in s.Split(' '))
            {         
                if (word != " " && word != "")
                {
                    words.Add(word);
                }
            }
            int totalWords = words.Count;
            foreach (var item in words)
            {
                GameObject newText = Instantiate(textPlaceHolder);
                GameObject simulatedDelayGO = Instantiate(simulatedDelay);
                simulatedDelayGO.transform.SetParent(newText.transform);
                newText.name = item;
                newText.transform.SetParent(wordStore.transform);
                newText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item;
                float preferedWidth = Mathf.Ceil(newText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().GetPreferredValues()[0]);
                if (wordCount == 0)
                {
                    widths.Add(preferedWidth + 20);
                }
                else
                {
                    widths.Add(widths[wordCount - 1] + (preferedWidth + 20));
                }
                if (firstLine)
                {
                    newTextPosition.y = 0;
                }
                else
                {
                    newTextPosition.y = -50;
                }
                if (charCount >= topLineHandler.maxCharCount / 2)
                {
                    newTextPosition.y = -50;
                    if (firstLine)
                    {
                        widths.Clear();
                        widths.Add(preferedWidth + 20);
                        wordCount = 0;
                        firstLine = false;
                    }
                }
                newTextPosition.x = (-860 + widths[wordCount]);
                newText.transform.localPosition = newTextPosition;
                wordCount++;
                charCount = charCount + (item.Length + 1);
                yield return new WaitForSeconds(0.1f);

            }
            creatingWords = false;
            UpdateResponse(iDs[0]);
            iDs.RemoveAt(0);
            responses.RemoveAt(0);
            yield return new WaitForSeconds(timeBetweenResponse);
            //timer = 0;
        }
        coroutineRunning = false;
    }
    public void QuestionUpdated(GraphQLResult result)
    {
        string jRaw = JsonConvert.SerializeObject(result.Data["updateCurrentQuestion"]["data"]["attributes"]["updatedAt"]);
        updatedAt = jRaw.Split('"')[1];

    }
    public void QuestionNumberResultEvent(GraphQLResult result)
    {
        if (result.Data.ContainsKey("currentQuestion"))
        {
            gotQuestionNumber = true;
            selectedQuestion = int.Parse(result.Data["currentQuestion"]["data"]["attributes"]["number"].ToString());
            string jRaw = JsonConvert.SerializeObject(result.Data["currentQuestion"]["data"]["attributes"]["updatedAt"]);
            updatedAt = jRaw.Split('"')[1];
        }
    }
    public void QuestionResultEvent(GraphQLResult result)
    {
        videoIds.Clear();
        questions.Clear();
        questionWords.Clear();
        foreach (var question in result.Data["questions"]["data"])
        {
            questions.Add(question["attributes"]["question"].ToString());
            videoIds.Add(question["attributes"]["video_id"].ToString());
            questionWords.Add(question["attributes"]["words"].ToString());
        }
        question.GetComponent<TextMeshProUGUI>().text = result.Data["questions"]["data"][selectedQuestion]["attributes"]["question"].ToString();
        gotQuestionIds = true;
        AddQuestionWords(questionWords[selectedQuestion]);
    }
    public IEnumerator CreateObjects(List<string> result)
    {
        foreach (var word in result)
        {
            GameObject newText = Instantiate(textPlaceHolder);
            newText.name = word;
            newText.transform.SetParent(wordStore.transform);
            newText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = word;
            newTextPosition.y = 100;
            if (count % 2 == 0)
            {
                newTextPosition.x = UnityEngine.Random.Range(0, 750);
            }
            else
            {
                newTextPosition.x = UnityEngine.Random.Range(-750, 0);
            }
            newText.transform.localPosition = newTextPosition;
            count++;
            yield return new WaitForSeconds(0.2f);
        }
        StopCoroutine(CreateObjects(result));
    }
    public IEnumerator CreateQuestionWordObjects(List<string> result)
    {
        foreach (var word in result)
        {
            GameObject newText = Instantiate(textPlaceHolder);
            newText.name = word;
            newText.transform.SetParent(wordStore.transform);
            newText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = word;
            newTextPosition.y = 100;
            if (count % 2 == 0)
            {
                newTextPosition.x = UnityEngine.Random.Range(0, 750);
            }
            else
            {
                newTextPosition.x = UnityEngine.Random.Range(-750, 0);
            }
            newText.transform.localPosition = newTextPosition;
            count++;
            yield return new WaitForSeconds(0.05f);
        }
        StopCoroutine(CreateQuestionWordObjects(result));
    }
    public void OnMaxResponseReceive(GraphQLResult result)
    {
        maxResponses = int.Parse(result.Data["maxResponse"]["data"]["attributes"]["amount"].ToString());
        otherSettings.maxResponsesInput.text = maxResponses.ToString();
    }
    public void AddQuestionWords(string _words)
    {
        Debug.Log("Adding question words: " + _words);
        string noSpaces = _words.Replace(" ", "");
        List<string> words = new List<string>();
        foreach (string word in noSpaces.Split(","))
        {
            if (word != "")
            {
                words.Add(word);
            }
        }
        StartCoroutine(CreateQuestionWordObjects(words));
    }
}
