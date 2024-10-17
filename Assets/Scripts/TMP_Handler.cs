using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TMP_Handler : MonoBehaviour
{
    TMPro.TextMeshProUGUI m_TextMeshPro;
    RectTransform rt;
    PolygonCollider2D m_PolygonCollider;
    TextCollider tc;
    public float preferedWidth;
    float halfWidth;
    float halfWidthBuffer;
    String word;
    Vector2 element0;
    Vector2 element1;
    Vector2 element2;
    Vector2 element3;
    Vector2 element4;
    Vector2 element5;

    // Start is called before the first frame update
    void Start()
    {
        tc = transform.parent.GetChild(1).GetComponent<TextCollider>();
        m_TextMeshPro = transform.GetComponent<TextMeshProUGUI>();
        rt = transform.GetComponent<RectTransform>();
        word = m_TextMeshPro.text;
        m_PolygonCollider = transform.GetComponent<PolygonCollider2D>();
        preferedWidth = Mathf.Ceil(m_TextMeshPro.GetPreferredValues()[0] + 20);
        halfWidth = (preferedWidth / 2);
        halfWidthBuffer = halfWidth + 10;
        rt.offsetMin = new Vector2 (0 - halfWidth, rt.offsetMin.y);
        rt.offsetMax = new Vector2 (halfWidth, rt.offsetMax.y);
        element0 = new Vector2(0, 20);
        element1 = new Vector2(0 - (halfWidthBuffer - 20), 15);
        element2 = new Vector2(0 - (halfWidthBuffer - 20), -20);
        element3 = new Vector2(0, -25);
        element4 = new Vector2((halfWidthBuffer - 20), -20);
        element5 = new Vector2((halfWidthBuffer - 20), 15);
        m_PolygonCollider.enabled = false;
        m_PolygonCollider.SetPath(0, new[] { element0, element1, element2, element3, element4, element5 });
        m_PolygonCollider.enabled = true;
        transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(preferedWidth, 1080);
        transform.parent.name = word;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
