using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class resetHandler : MonoBehaviour
{
    public TextMeshProUGUI resetDelay;
    public TextMeshProUGUI resetTimer;
    public GameObject topLine;
    float timer;
    GameObject topLock;
    GameObject destroy;
    bool isEmptyUpdate;
    bool isEmpty;
    float delayTimer;
    float delayLimit;
    List<GameObject> listGOs = new List<GameObject>();
    topLockHandler topLockHandler;
    // Start is called before the first frame update
    void Start()
    {
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
        //resetDelay.text = delayTimer.ToString("F2");
        //resetTimer.text = timer.ToString("F2");
        //isEmptyUpdate = true;
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
        if (!isEmpty)
        {
            timer += Time.deltaTime;
            if (timer > 2.0f)
            {
                ResetList();
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
    public void ResetList()
    {
        topLockHandler.pause = true;
        listGOs.Clear();
        if (topLine.transform.childCount > 0)
        {
            foreach (Transform t in topLine.transform)
            {
                listGOs.Add(t.gameObject);
            }
            topLine.GetComponent<TextMeshProUGUI>().text = "";
        }
        if (topLock.transform.childCount > 0)
        {
            foreach (Transform t in topLock.transform)
            {
                listGOs.Add(t.gameObject);
            }
        }
        if (listGOs.Count > 0)
        {
            for (int i = 0; i < listGOs.Count; i++)
            {
                listGOs[i].transform.SetParent(destroy.transform);
            }
            topLockHandler.inPlace.Clear();
            topLockHandler.inPlaceX.Clear();
            topLockHandler.inPlaceX.Add(60.0f);
        }
        timer = 0.0f;
        topLockHandler.history.Clear();
        topLockHandler.charCount = 0;
        topLockHandler.pause = false;
        topLine.GetComponent<topLineHandler>().secondLine = false;
    }
}
