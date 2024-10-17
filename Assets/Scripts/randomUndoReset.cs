using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomUndoReset : MonoBehaviour
{
    public GameObject undo;
    public GameObject reset;
    float timer;
    int chance;
    // Start is called before the first frame update
    void Start()
    {
        chance = 0;
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 480) {
            chance = Random.Range(0, 10);
            if (chance < 8)
            {
                undo.GetComponent<undoHandler>().Undo();
            }
            else { 
                reset.GetComponent<resetHandler>().ResetList();
            }
            timer = 0;
            chance = 0;
        }
    }
}
