using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class topLockHandler : MonoBehaviour
{
    public Image timerCircle;
    public ParticleSystem wordPartical;
    public TextMeshProUGUI charCountText;
    public List<GameObject> inPlace = new List<GameObject>();
    public List<GameObject> history = new List<GameObject>();
    public List<float> inPlaceX = new List<float>();
    public GameObject topLine;
    public GameObject fixedPlace;
    public GameObject undo;
    public TextMeshProUGUI topTimerText;
    public TextMeshProUGUI statusText;
    public int charCount;
    public float timerLimit = 560;
    public float topTimer;
    public bool pause;
    int keepCount;
    int topLock = 600;
    public float checkTimer;
    float checkTimerExecute;
    bool checkTags;
    public GameObject finishButton;
    finishHandler finishHandler;

    void Start()
    {
        checkTags = false;
        checkTimer = 0;
        checkTimerExecute = 30;
        pause = false;
        charCount = 0;
        finishHandler = finishButton.GetComponent<finishHandler>();
        inPlaceX.Add(60.0f);
        topTimer = timerLimit;
        keepCount = 0;
    }
    private void Update()
    {
        if (inPlace.Count > 0 || topLine.GetComponent<TextMeshProUGUI>().text != "")
        {
            if (timerCircle.color.a < 1)
            {
                Color c = timerCircle.color;
                c.a += 0.1f;
                timerCircle.color = c;
            }
        }
        else
        {
            if (timerCircle.color.a > 0)
            {
                Color c = timerCircle.color;
                c.a -= 0.1f;
                timerCircle.color = c;
            }
        }
        timerCircle.fillAmount = scale(timerLimit, 0, 1, 0, topTimer);
        checkTimer += Time.deltaTime;
        if (checkTimerExecute - checkTimer < 10)
        {
            statusText.text = "Checking words in " + (checkTimerExecute - checkTimer).ToString("F0");
        }
        if (checkTimer > checkTimerExecute)
        {
            statusText.text = "Waiting to check words...";
            if (undo.GetComponent<undoHandler>().locks.Length == 0) {
                statusText.text = "Checking words are in place...";
                if (!checkTags)
                {
                    foreach (Transform inPlace in transform)
                    {
                        inPlace.tag = "TextBox";
                    }
                    checkTags = true;
                }
                if (GameObject.FindGameObjectsWithTag("fixedInPlace").Length == transform.childCount)
                {
                    checkTimer = 0;
                    statusText.text = "Check done. Words in place.";
                    checkTags = false;
                }
            }
        }
        charCount = topLine.GetComponent<TextMeshProUGUI>().text.Length;
        for (int i = 0; i < inPlace.Count; i++)
        {
            charCount = charCount + (inPlace[i].name.Length + 1);
        }
        charCountText.text = charCount.ToString() + " / " + topLine.GetComponent<topLineHandler>().maxCharCount.ToString();
        if (transform.childCount > 0)
        {
            if (keepCount != transform.childCount)
            {
                //Debug.Log("Added to sentence");
                keepCount = transform.childCount;
                topTimer = timerLimit;
            }
            else
            {
                // Change Timer Circle
                topTimer -= Time.deltaTime;
            }
        }
        else
        {
            topTimer = timerLimit;
        }
        if (topTimer <= 0)
        {
            pause = true;
            finishHandler.onFinishEvent("timout");
            topLine.GetComponent<topLineHandler>().secondLine = false;
            topTimer = timerLimit;
        }
        topTimerText.text = topTimer.ToString("F0");
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!pause)
        {
            if (transform.childCount > inPlace.Count)
            {
                List<Transform> GOs = new List<Transform>();
                for(int i = 0; i < transform.childCount; i++)
                {
                    GOs.Add(transform.GetChild(i));
                }
                addGOtoList(GOs);
            }
            if (inPlace.Count > 0)
            {
                float lastPos = 0;
                for (int i = 0; i < inPlace.Count; i++)
                {
                    if (topLine.GetComponent<topLineHandler>().secondLine == false)
                    {
                        lastPos = lastPos + inPlaceX[i];
                        if (i == inPlace.Count - 1)
                        {
                            undo.GetComponent<undoHandler>().lastPosition = lastPos + (inPlace[inPlace.Count - 1].transform.GetChild(0).GetComponent<TMP_Handler>().preferedWidth + (inPlace[inPlace.Count - 1].transform.GetChild(0).GetComponent<TMP_Handler>().preferedWidth / 2));
                        }
                    }
                    if (inPlace[i].tag != "fixedInPlace")
                    {
                        //Rigidbody2D rb = inPlace[i].transform.GetChild(0).GetComponent<Rigidbody2D>();
                        Transform tR = inPlace[i].transform.GetChild(0);
                        float myPosition = 0;
                        for (int j = 0; j <= i; j++)
                        {
                            myPosition = myPosition + inPlaceX[j];
                        }
                        myPosition = myPosition + (inPlaceX[i + 1] / 2);
                        Vector2 holdPosition = new Vector2();
                        if (tR.position.y < topLock)
                        {
                            holdPosition.y = (tR.position.y + 5);
                            holdPosition.x = tR.position.x;
                        }
                        else
                        {
                            /*if (rb.rotation != 0)
                            {
                                rb.rotation = 0;
                            }*/
                            holdPosition.y = topLock;

                            if (tR.position.x > (myPosition + 50))
                            {
                                holdPosition.x = (tR.position.x - 5);
                            }
                            else if (tR.position.x < (myPosition - 50))
                            {
                                holdPosition.x = (tR.position.x + 5);
                            }
                            else if (tR.position.x >= (myPosition - 50) && tR.position.x <= (myPosition + 50))
                            {
                                holdPosition.x = myPosition;
                                inPlace[i].tag = "fixedInPlace";
                            }
                        }
                        tR.position = holdPosition;
                        //Debug.Log(rb.position.x + " | " + myPosition);
                    }
                }
            }
        }

    }

    void addGOtoList(List<Transform> GOs)
    {
        bool placedInside = false;
        for (int i = 0; i < GOs.Count; i++)
        {
            if (!inPlace.Contains(GOs[i].gameObject))
            {
                Destroy(GOs[i].transform.GetChild(0).GetComponent<Rigidbody2D>());
                //Rigidbody2D myRb = GOs[i].transform.GetChild(0).GetComponent<Rigidbody2D>();
                Transform myTr = GOs[i].transform.GetChild(0);
                //GOs[i].transform.GetChild(1).gameObject.SetActive(false);
                Destroy(GOs[i].transform.GetChild(1).gameObject);
                GOs[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 40;
                GOs[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
                float myWidth = GOs[i].transform.GetChild(0).GetComponent<TMP_Handler>().preferedWidth;
                float myPosition = myTr.position.x;
                if (inPlace.Count < 1)
                {
                    if (!history.Contains(GOs[i].gameObject))
                    {
                        history.Add(GOs[i].gameObject);
                    }
                    inPlace.Add(GOs[i].gameObject);
                    inPlaceX.Add(myWidth);
                }
                else if (inPlace.Count >= 1)
                {
                    for (int j = 0; j < inPlace.Count; j++)
                    {
                        if (!placedInside)
                        {
                            float rtLeft;
                            float rtRight;
                            if (j == 0)
                            {
                                //Rigidbody2D rb = inPlace[j].transform.GetChild(0).GetComponent<Rigidbody2D>();
                                Transform tr = inPlace[j].transform.GetChild(0);
                                RectTransform rt = inPlace[j].transform.GetChild(0).GetComponent<RectTransform>();
                                float rtWidth = rt.sizeDelta.x;
                                float rbPosition = tr.position.x;
                                rtLeft = rbPosition - (rtWidth / 2);
                                rtRight = rbPosition + (rtWidth / 2);
                            }
                            else
                            {
                                //Rigidbody2D rb = inPlace[j].transform.GetChild(0).GetComponent<Rigidbody2D>();
                                Transform tr = inPlace[j].transform.GetChild(0);
                                //Rigidbody2D prevRB = inPlace[j - 1].transform.GetChild(0).GetComponent<Rigidbody2D>();
                                Transform prevTr = inPlace[j - 1].transform.GetChild(0);
                                rtLeft = prevTr.position.x;
                                rtRight = tr.position.x;
                            }
                            if (myPosition > rtLeft && myPosition < rtRight)
                            {
                                if (!history.Contains(GOs[i].gameObject))
                                {
                                    history.Add(GOs[i].gameObject);
                                }
                                inPlace.Insert(j, GOs[i].gameObject);
                                inPlaceX.Insert(j + 1, myWidth);
                                GOs[i].gameObject.transform.SetSiblingIndex(j);
                                placedInside = true;
                                for (int k = j; k < inPlace.Count; k++)
                                {
                                    if (inPlace[k].tag == "fixedInPlace")
                                    {
                                        inPlace[k].tag = "TextBox";
                                    }
                                }
                                return;
                            }
                        }
                        if (j == inPlace.Count - 1 && !placedInside)
                        {
                            if (!history.Contains(GOs[i].gameObject))
                            {
                                history.Add(GOs[i].gameObject);
                            }
                            inPlace.Add(GOs[i].gameObject);
                            inPlaceX.Add(myWidth);
                            return;
                        }
                    }
                }
            }
        }
    }
    private void PlayPartical(float x, float y)
    {
        ParticleSystem wordExplosion = Instantiate(wordPartical);
        Vector2 position;
        position.x = x;
        position.y = y;
        wordExplosion.transform.position = position;
        wordExplosion.transform.SetParent(GameObject.FindGameObjectWithTag("MainCanvas").transform);
        wordExplosion.Play();
    }
    public float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;
        return (NewValue);
    }
}
