using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class undoHandler : MonoBehaviour
{
    public TextMeshProUGUI undoDelay;
    public TextMeshProUGUI undoTimer;
    public GameObject topLine;
    public float lastPosition;
    float timer;
    public GameObject lastTopGO;
    GameObject topLock;
    GameObject destroy;

    topLockHandler topLockHandler;
    bool isEmptyUpdate;
    bool isEmpty;
    bool reset = false;
    float delayLimit;
    float delayTimer;
    public GameObject[] locks;
    public GameObject[] holds;
    // Start is called before the first frame update
    void Start()
    {
        lastPosition = 0;
        delayLimit = 1f;
        delayTimer = 0.0f;
        isEmptyUpdate = true;
        isEmpty = true;
        timer = 0.0f;
        topLock = GameObject.FindGameObjectWithTag("topLock");
        destroy = GameObject.FindGameObjectWithTag("destroy");
        topLockHandler = topLock.GetComponent<topLockHandler>();
    }
    private void FixedUpdate()
    {
        isEmptyUpdate = true;
    }
    // Update is called once per frame
    void Update()
    {
        locks = GameObject.FindGameObjectsWithTag("textLock");
        holds = GameObject.FindGameObjectsWithTag("textHold");
        // Not Empty Update
        if (!isEmptyUpdate)
        {
            delayTimer = 0.0f;
            isEmpty = false;

        }
        else
        {
            //isEmptyUpdate = false;
            transform.GetComponent<TextMeshProUGUI>().color = Color.white;
            if (transform.GetComponent<TextMeshProUGUI>().fontSize > 36)
            {
                transform.GetComponent<TextMeshProUGUI>().fontSize = transform.GetComponent<TextMeshProUGUI>().fontSize - 1;
            }
        }
        // Is Empty Update
        if (isEmptyUpdate && delayTimer < delayLimit)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer > delayLimit)
            {
                isEmpty = true;
                delayTimer = 0.0f;
            }
        }
        if (!isEmpty && locks.Length < 1)
        {
            timer += Time.deltaTime;
            if (timer > 2.0f)
            {
                Undo();
                timer = 0.0f;
            }
        }
        if (reset)
        {
            topLine.GetComponent<topLineHandler>().secondLine = false;
            for (int i = 0; i < topLine.transform.childCount; ++i)
            {
                GameObject go = topLine.transform.GetChild(0).gameObject;
                go.transform.SetParent(topLock.transform, false);
                //go.transform.SetSiblingIndex(i);
                go.SetActive(true);
                if (topLine.transform.childCount == 0)
                {
                    reset = false;
                    topLockHandler.charCount = 0;
                    topLockHandler.pause = false;
                }
            }
        }
    }



    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "User")
        {
            isEmptyUpdate = false;
            transform.GetComponent<TextMeshProUGUI>().color = new Color32(225, 248, 186, 225);
            if (transform.GetComponent<TextMeshProUGUI>().fontSize < 46)
            {
                transform.GetComponent<TextMeshProUGUI>().fontSize = transform.GetComponent<TextMeshProUGUI>().fontSize + 1;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "User")
        {
            if (timer > 0)
            {
                timer = 0.0f;
            }
        }
    }

    public void Undo()
    {
        if (topLock.transform.childCount > 0)
        {
            bool pass = true;
            if (topLockHandler.history[topLockHandler.history.Count - 1].transform.parent.name == "topLine")
            {
                pass = false;
            }
            Remove();
            if (topLock.transform.childCount == 1 && topLine.GetComponent<TextMeshProUGUI>().text != "" && !pass)
            {
                topLockHandler.pause = true;
                GameObject leftOverGO = topLock.transform.GetChild(0).gameObject;
                leftOverGO.transform.SetParent(topLine.transform);
                Vector2 newPosiition = new Vector2();
                newPosiition.y = leftOverGO.transform.position.y;
                newPosiition.x = leftOverGO.transform.position.x + (1840 - leftOverGO.transform.GetChild(0).GetComponent<TMP_Handler>().preferedWidth);
                leftOverGO.transform.position = newPosiition;
                Debug.Log("LeftOverGO: " + leftOverGO.gameObject.name + " Parent now: " + leftOverGO.transform.parent.name + " with child index of: " + leftOverGO.transform.GetSiblingIndex());
                if (topLockHandler.inPlace.Contains(leftOverGO))
                {
                    Debug.Log("Removed inPlaceX: " + topLockHandler.inPlace.IndexOf(leftOverGO) + 1);
                    topLockHandler.inPlaceX.RemoveAt(topLockHandler.inPlace.IndexOf(leftOverGO) + 1);
                    topLockHandler.inPlace.Remove(leftOverGO);
                }
                if (topLock.transform.childCount == 0 && topLine.transform.GetChild(topLine.transform.childCount - 1).name == leftOverGO.name)
                {
                    reset = true;
                }
            }
        }
        else
        {
            Debug.Log("Nothing to Undo");
        }
    }

    void Remove()
    {
        topLockHandler.pause = true;
        GameObject lastGO = topLockHandler.history[topLockHandler.history.Count - 1];
        int inPlaceIndex = topLockHandler.inPlace.IndexOf(lastGO);
        topLockHandler.charCount = topLockHandler.charCount - (lastGO.name.Length + 1);
        if (topLockHandler.inPlace.Contains(lastGO))
        {
            topLockHandler.inPlaceX.RemoveAt(topLockHandler.inPlace.IndexOf(lastGO) + 1);
            topLockHandler.inPlace.Remove(lastGO);
        }
        topLockHandler.history.Remove(lastGO);
        Debug.Log("Undo Position Parent: " + lastGO.transform.position);
        Debug.Log("Undo Position Wordt: " + lastGO.transform.GetChild(0).transform.position);
        lastGO.transform.SetParent(destroy.transform);
        for (int i = 0; i < (topLockHandler.inPlace.Count); i++)
        {
            if (topLockHandler.inPlace[i].tag == "fixedInPlace")
            {
                topLockHandler.inPlace[i].tag = "TextBox";
            }
        }
        topLockHandler.pause = false;
        Debug.Log("Undo");
    }
}
