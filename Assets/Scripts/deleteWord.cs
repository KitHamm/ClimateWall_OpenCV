using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class deleteWord : MonoBehaviour
{
    public GameObject wordStore;
    public TMP_InputField wordToDeleteInput;
    public Button deleteWordButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(wordToDeleteInput.text == "")
        {
            deleteWordButton.interactable = false;
        }
        else
        {
            deleteWordButton.interactable = true;
        }
    }

    public void DeleteWord()
    {
        var count = 0;
        foreach (Transform word in wordStore.transform)
        {
            if (word.name == wordToDeleteInput.text)
            {
                Debug.Log("Removed: " + word.name);
                Destroy(word.gameObject);
                wordToDeleteInput.text = "";
                break;
            }
            if (count == wordStore.transform.childCount - 1)
            {
                Debug.Log("Word could not be found. Please check spelling");
            }
            count++;
        }
    }
    public void OnClose()
    {
        wordToDeleteInput.text = "";
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
