using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    public TextMeshProUGUI FpsText;

    private float pollingTime = 1f;
    private float time;
    private float frameCount;

    // Update is called once per frame
    void Update()
    {
        time += Time.unscaledDeltaTime;
        frameCount++;
        if (time >= pollingTime) 
        {
            int frameRate = Mathf.RoundToInt(frameCount / time);
            FpsText.text = frameRate.ToString() + " FPS"; 
        }
    }
}
