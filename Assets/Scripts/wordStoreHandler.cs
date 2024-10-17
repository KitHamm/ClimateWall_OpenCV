using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class wordStoreHandler : MonoBehaviour
{
    public bool playDemo = true;
    public Image backgroundTwo;
    int childCount;
    public float allEmptyTimer;
    public float allEmptyTimerLimit = 120f;
    public float timer;
    public float timeOut = 120.0f;
    public int wordLimit = 20;
    public TextMeshProUGUI timerText;
    public bool allEmpty;
    bool fadingOut;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("demo"))
        {
            if (PlayerPrefs.GetInt("demo") == 0)
            {
                playDemo = false;
            }
            else
            {
                playDemo = true;
            }
        }
        else
        {
            PlayerPrefs.SetInt("demo", 0);
            playDemo = false;
        }
        if (PlayerPrefs.HasKey("demoTimer"))
        {
            allEmptyTimerLimit = PlayerPrefs.GetFloat("demoTimer");
        }
        else
        {
            allEmptyTimerLimit = 120f;
            PlayerPrefs.SetFloat("demoTimer", 120f);
        }
        fadingOut = false;
        allEmptyTimer = 0;
        timer = timeOut;
    }
    private void FixedUpdate()
    {
        allEmpty = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (playDemo)
        {
            if (allEmpty)
            {
                allEmptyTimer += Time.deltaTime;
                if (allEmptyTimer > allEmptyTimerLimit)
                {
                    if (backgroundTwo.color.a > 0)
                    {
                        Color c = backgroundTwo.color;
                        c.a -= 0.1f;
                        backgroundTwo.color = c;
                    }
                }
            }
            else
            {
                allEmptyTimer = 0;
                fadingOut = true;
            }
            if (fadingOut)
            {
                if (backgroundTwo.color.a < 1)
                {
                    Color c = backgroundTwo.color;
                    c.a += 0.1f;
                    backgroundTwo.color = c;
                }
                else
                {
                    fadingOut = false;
                }
            }
        }
        else
        {
            if (backgroundTwo.color.a < 1)
            {
                Color c = backgroundTwo.color;
                c.a += 0.1f;
                backgroundTwo.color = c;
            }
        }
        if (transform.childCount != childCount)
        {
            childCount = transform.childCount;
        }
        if (childCount > wordLimit)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = timeOut;
        }

        if (timer < 0 || childCount > wordLimit + 10)
        {
            TextCollider tC = transform.GetChild(0).transform.GetChild(1).GetComponent<TextCollider>();
            if (tC.isEmpty)
            {
                Destroy(transform.GetChild(0).gameObject);
                timer = timeOut;
            }
            else
            {
                transform.GetChild(0).SetAsLastSibling();
            }
        }
        timerText.text = timer.ToString("F0");
    }
}
