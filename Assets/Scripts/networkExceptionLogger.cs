using GraphQL4Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class networkExceptionLogger : MonoBehaviour
{
    public GraphQLHttp WordQueryHttp;
    public GraphQLHttp QuestionQueryHttp;
    public GraphQLHttp ResponseQueryHttp;
    public GraphQLHttp CurrentQuestionMutationHttp;
    public GraphQLHttp CurrentQuestionQueryHttp;
    public GraphQLHttp ResponseMutationHttp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (WordQueryHttp.Exception != "")
        {
            Debug.Log("Word Query: " + WordQueryHttp.Exception);
            WordQueryHttp.Exception = "";
        }
        if (QuestionQueryHttp.Exception != "")
        {
            Debug.Log("Question Query: " + QuestionQueryHttp.Exception);
            QuestionQueryHttp.Exception = "";
        }
        if (CurrentQuestionQueryHttp.Exception != "")
        {
            Debug.Log("Current Question Query: " + CurrentQuestionQueryHttp.Exception);
            CurrentQuestionQueryHttp.Exception = "";
        }
        if (ResponseQueryHttp.Exception != "")
        {
            Debug.Log("Response Query: " + ResponseQueryHttp.Exception);
            ResponseQueryHttp.Exception = "";
        }
        if (ResponseMutationHttp.Exception != "")
        {
            Debug.Log("Response Mutation: " + ResponseMutationHttp.Exception);
            ResponseMutationHttp.Exception = "";
        }
        if (CurrentQuestionMutationHttp.Exception != "")
        {
            Debug.Log("Current Question Mutation: " + CurrentQuestionMutationHttp.Exception);
            CurrentQuestionMutationHttp.Exception = "";
        }
    }
}
