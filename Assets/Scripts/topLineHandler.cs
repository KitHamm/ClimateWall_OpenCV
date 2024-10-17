using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class topLineHandler : MonoBehaviour
{
    TextMeshProUGUI text;
    List<GameObject> listGOs = new List<GameObject>();
    GameObject topLock;
    public finishHandler finishHandler;
    public resetHandler resetHandler;
    public responseManager responseManager;
    public int maxCharCount;
    public bool secondLine;
    topLockHandler topLockHandler;
    GameObject destroy;
    // Start is called before the first frame update
    void Start()
    {
        topLock = GameObject.FindGameObjectWithTag("topLock");
        topLockHandler = topLock.GetComponent<topLockHandler>();
        text = transform.GetComponent<TextMeshProUGUI>();
        destroy = GameObject.FindGameObjectWithTag("destroy");
        secondLine = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount > 0)
        {
            string myString = "";
            foreach (Transform t in transform)
            {
                myString += t.name + " ";
            }
            text.text = myString;
        }
        else
        {
            text.text = "";
        }
        if (text.text != "" && text.color.a < 1)
        {
            Color c = text.color;
            c.a += 0.05f;
            text.color = c;
        }
        if (!topLockHandler.pause)
        {
            if (topLockHandler.charCount > (maxCharCount / 2) && !secondLine)
            {
                //EditorApplication.isPaused = true;
                topLockHandler.pause = true;
                moveLine();
                secondLine = true;
            }
            if (topLockHandler.charCount > maxCharCount)
            {
                if (topLockHandler.inPlace.Count == GameObject.FindGameObjectsWithTag("fixedInPlace").Length)
                {
                    topLockHandler.pause = true;
                    finishHandler.onFinishEvent("max char limit reached");
                    secondLine = false;
                }
            }
        }
    }
    public void moveLine()
    {
        listGOs.Clear();
        if (topLock.transform.childCount > 0)
        {
            foreach (Transform t in topLock.transform)
            {
                listGOs.Add(t.gameObject);
            }
        }
        listGOs.RemoveAt(listGOs.Count - 1);
        if (listGOs.Count > 0)
        {
            for (int i = 0; i < listGOs.Count; i++)
            {
                listGOs[i].SetActive(false);
                listGOs[i].transform.SetParent(transform);
                listGOs[i].tag = "TextBox";
            }
            topLockHandler.inPlace.Clear();
            topLockHandler.inPlaceX.Clear();
            topLockHandler.inPlaceX.Add(60.0f);
        }
        topLockHandler.transform.GetChild(0).tag = "TextBox";
        topLockHandler.pause = false;
    }

}
