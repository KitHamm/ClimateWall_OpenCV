using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class destroyHandler : MonoBehaviour
{
    public GameObject textBox;
    GameObject wordStore;
    Vector2 newGOPosition;
    int count;
    // Start is called before the first frame update
    void Start()
    { 
        count = 0;
        //newGOPosition.y = 750;
        wordStore = GameObject.FindGameObjectWithTag("wordStore");
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount > 0)
        {
            string word = transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            Vector2 position = transform.GetChild(0).transform.GetChild(0).transform.position;
            float preferedWidth = transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Handler>().preferedWidth;
            Destroy(transform.GetChild(0).gameObject);
            GameObject newGO = Instantiate(textBox);
            newGO.name = word;
            newGO.transform.SetParent(wordStore.transform);
            newGO.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = word;
            //if (count%2 == 0)
            //{
                newGOPosition.x = position.x + (preferedWidth / 2);
            newGOPosition.y = position.y;
            /*}
            else
            {
                newGOPosition.x = Random.Range(-500, 0);
            }*/
            newGO.transform.position = newGOPosition;
            count++;
        }
    }
}
