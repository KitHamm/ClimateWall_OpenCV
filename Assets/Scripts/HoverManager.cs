using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HoverManager : MonoBehaviour
{
    public TextMeshProUGUI tipText;
    public RectTransform tipWindow;
    public static Action<string, Vector2> OnMouseHover;
    public static Action OnMouseLoseFocus;

    private void OnEnable()
    {
        OnMouseHover += ShowTip;
        OnMouseLoseFocus += HideTip;
    }

    private void OnDisable()
    {
        OnMouseHover -= ShowTip;
        OnMouseLoseFocus -= HideTip;
    }

    void Start()
    {
        HideTip();
    }

    private void ShowTip(string tip, Vector2 mousePosition)
    {
        tipWindow.transform.SetAsLastSibling();
        tipText.text = tip;
        tipWindow.sizeDelta = new Vector2(tipText.preferredWidth > 250 ? 250 : tipText.preferredWidth, tipText.preferredHeight);
        tipWindow.gameObject.SetActive(true);
        tipWindow.transform.position = new Vector3(scale(0, Screen.width, 0, 1920, mousePosition.x) + 20, (1080 + 90) + scale(0, Screen.height, 0, 1080, mousePosition.y), -1.2f);
    }

    private void HideTip()
    {
        tipText.text = default;
        tipWindow.gameObject.SetActive(false);
    }
    public float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {

        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }
}
