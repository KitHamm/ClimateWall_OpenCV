using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class finishAnimation : MonoBehaviour
{
    float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 6)
        {
            Destroy(gameObject);
        }
    }
}
