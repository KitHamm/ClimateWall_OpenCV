using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonHandler : MonoBehaviour
{
    GameObject topLock;
    GameObject destroy;
    public GameObject lastTopGO;
    List<GameObject> listGOs = new List<GameObject>();
    topLockHandler topLockHandler;
    private void Start()
    {
        topLock = GameObject.FindGameObjectWithTag("topLock");
        destroy = GameObject.FindGameObjectWithTag("destroy");
        topLockHandler = topLock.GetComponent<topLockHandler>();
    }
    public void onUndoPress()
    {
        Debug.Log("Undo Pressed");
        if (lastTopGO != null)
        {
            Debug.Log(lastTopGO.name);
            Debug.Log("inPlace count " + topLockHandler.inPlace.Count);
            Debug.Log("inPlace x count " + topLockHandler.inPlaceX.Count);
            lastTopGO.transform.SetParent(destroy.transform);
            topLockHandler.inPlaceX.RemoveAt(topLockHandler.inPlace.IndexOf(lastTopGO) + 1);
            topLockHandler.inPlace.Remove(lastTopGO);
            Debug.Log("inPlace count " + topLockHandler.inPlace.Count);
            Debug.Log("inPlace x count " + topLockHandler.inPlaceX.Count);
            lastTopGO = null;
        } 
    }
    public void onResetPress()
    {
        listGOs.Clear();
        foreach (Transform t in topLock.transform)
        {
            listGOs.Add(t.gameObject);
        }
        for(int i = 0; i < listGOs.Count; i++)
        {
            listGOs[i].transform.SetParent(destroy.transform);
            topLockHandler.inPlaceX.RemoveAt(topLockHandler.inPlace.IndexOf(listGOs[i]) + 1);
            topLockHandler.inPlace.Remove(listGOs[i]);
        }
    }
    public void onSavePress()
    {
        for (int i = 0;i < topLockHandler.inPlace.Count; i++)
        {
            float myPosition = 0;
            for (int j = 0; j <= i; j++)
            {
                myPosition = myPosition + topLockHandler.inPlaceX[j];
            }
            Debug.Log(i + " Logged Width = " + topLockHandler.inPlaceX[i + 1]);
            Debug.Log(i + " Actual Width = " + topLockHandler.inPlace[i].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x);
            Debug.Log(i + " Desired Position = " + myPosition);
            Debug.Log(i + " Actual Position = " + topLockHandler.inPlace[i].transform.GetChild(0).GetComponent<Rigidbody2D>().position.x);
        }
    }
}
