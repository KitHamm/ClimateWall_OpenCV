using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class userImageControl : MonoBehaviour
{
    control control;
    // Start is called before the first frame update
    void Start()
    {
        control = GameObject.FindGameObjectWithTag("control").GetComponent<control>();
    }

    // Update is called once per frame
    void Update()
    {
        if (control.debug)
        {
            Color color = transform.GetComponent<Image>().color;
            color.a = 255;
            transform.GetComponent<Image>().color = color;
            transform.parent.GetChild(0).GetComponent<TextMeshProUGUI>().color = color;
            transform.parent.GetChild(2).GetComponent<TextMeshProUGUI>().color = color;
            transform.parent.GetChild(3).GetComponent<TextMeshProUGUI>().color = color;
        }
        else
        {
            Color color = transform.GetComponent<Image>().color;
            color.a = 0;
            transform.GetComponent<Image>().color = color;
            transform.parent.GetChild(0).GetComponent<TextMeshProUGUI>().color = color;
            transform.parent.GetChild(2).GetComponent<TextMeshProUGUI>().color = color;
            transform.parent.GetChild(3).GetComponent<TextMeshProUGUI>().color = color;
        }
    }
}
