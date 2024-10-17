using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simulatedDelay : MonoBehaviour
{
    control control;
    Rigidbody2D rb;
    BoxCollider2D box;
    PolygonCollider2D polygon;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        control = GameObject.FindGameObjectWithTag("control").GetComponent<control>();
        timer = 0;
        rb = transform.parent.GetChild(0).GetComponent<Rigidbody2D>();
        polygon = transform.parent.GetChild(0).GetComponent<PolygonCollider2D>();
        box = transform.parent.GetChild(1).GetComponent<BoxCollider2D>();
        rb.simulated = false;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer < control.responseDelayTime + 2f)
        {
            polygon.enabled = false;
            box.enabled = false;
        }
        if (timer > control.responseDelayTime) 
        {
            rb.simulated = true;
        }
        if (timer > control.responseDelayTime + 1f)
        {
            polygon.enabled = true;

        }
        if (timer > control.responseDelayTime + 2f)
        {
            box.enabled = true;
            Destroy(gameObject);
        }
    }
}
