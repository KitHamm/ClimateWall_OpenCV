using TMPro;
using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.VideoModule;

public class TextCollider : MonoBehaviour
{
    public ParticleSystem wordPartical;
    public responseManager responseManager;
    public GameObject selectedContainer;
    public GameObject selected;
    public GameObject lockObject;
    public GameObject holdObject;
    public GameObject Undo;
    public TextMeshProUGUI lockTimer;
    public TextMeshProUGUI holdTimer;
    undoHandler undoHandler;
    control control;
    GameObject topLock;
    Vector2 selfPosition;
    Vector2 movePosition;
    Vector2 lockPosition;
    Vector2 holdPosition;
    bool locked;
    public bool isEmptyUpdate;
    public bool isEmpty;
    public bool particalPlayed;
    bool initialSet;
    float timer;
    float delayTimer;
    float delayLimit;
    float emptyTimer;
    public bool rb1Asleep;
    public bool rb2Asleep;
    public float lockTime = 3.0f;
    String id;
    Transform tR;
    Rigidbody2D rb;
    BoxCollider2D bc;
    TextMeshProUGUI txt;
    TextMeshProUGUI duplicateTxt;
    wordStoreHandler wordStore;
    float vFloorPos;
    float vRightPos;
    float vLeftPos;
    public int frameRate;
    bool boxColliderActive;

    private void Start()
    {
        selectedContainer = GameObject.FindGameObjectWithTag("selectedContainer");
        wordStore = GameObject.FindGameObjectWithTag("wordStore").GetComponent<wordStoreHandler>();
        responseManager = GameObject.FindGameObjectWithTag("responseManager").GetComponent<responseManager>();
        vFloorPos = GameObject.FindGameObjectWithTag("virtualFloor").transform.position.y;
        vRightPos = GameObject.FindGameObjectWithTag("virtualRight").transform.position.x;
        vLeftPos = GameObject.FindGameObjectWithTag("virtualLeft").transform.position.x;
        boxColliderActive = false;
        rb1Asleep = false;
        rb2Asleep = false;
        initialSet = false;
        particalPlayed = false;
        control = GameObject.FindGameObjectWithTag("control").GetComponent<control>();
        lockTimer = GameObject.FindGameObjectWithTag("lockTimer").GetComponent<TextMeshProUGUI>();
        holdTimer = GameObject.FindGameObjectWithTag("holdTimer").GetComponent<TextMeshProUGUI>();
        undoHandler = GameObject.FindGameObjectWithTag("undoHandler").GetComponent<undoHandler>();
        delayLimit = 0.5f;
        delayTimer = 0.0f;
        isEmpty = true;
        isEmptyUpdate = true;
        topLock = GameObject.FindGameObjectWithTag("topLock");
        selfPosition.y = 330f;
        timer = 0.0f;
        locked = false;
        holdPosition.x = 0.0f;
        emptyTimer = 0.0f;
        tR = transform.parent.GetChild(0);
        rb = transform.parent.GetChild(0).GetComponent<Rigidbody2D>();
        bc = transform.parent.GetChild(1).GetComponent<BoxCollider2D>();
        txt = transform.parent.GetChild(0).GetComponent<TextMeshProUGUI>();
        duplicateTxt = transform.parent.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    private void FixedUpdate()
    {
        isEmptyUpdate = true;
        
    }
    private void Update()
    {
        if (duplicateTxt.text == "")
        {
            duplicateTxt.text = transform.parent.name;
        }
        if (transform.parent.GetChild(0).childCount > 0)
        {
            duplicateTxt.transform.position = txt.transform.position;
        }
        if(tR.position.y < vFloorPos)
        {
            tR.position = new Vector2(960.0f, 540.0f);
            if (rb.bodyType == RigidbodyType2D.Dynamic)
            {
                rb.velocity = Vector2.zero;
            }
            Debug.Log(transform.parent.name);
        }
        if (tR.position.x < vLeftPos)
        {
            tR.position = new Vector2(960.0f, 540.0f);
            if (rb.bodyType == RigidbodyType2D.Dynamic)
            {
                rb.velocity = Vector2.zero;
            }
            Debug.Log(transform.parent.name);
        }
        if (tR.position.x > vRightPos)
        {
            tR.position = new Vector2(960.0f, 540.0f);
            if (rb.bodyType == RigidbodyType2D.Dynamic)
            {
                rb.velocity = Vector2.zero;
            }
            Debug.Log(transform.parent.name);
        }
        if (rb.bodyType == RigidbodyType2D.Dynamic && rb.IsSleeping() && !rb1Asleep)
        {
            Debug.Log(transform.parent.name + " rb1 is asleep.");
            rb1Asleep = true;
        }
        if (rb.bodyType == RigidbodyType2D.Dynamic && rb.IsAwake() && rb1Asleep)
        {
            Debug.Log(transform.parent.name + " rb1 is awake.");
            rb1Asleep = false;
        }
        if (txt.color.a < 1)
        {
            Color c = txt.color;
            c.a += 0.1f;
            txt.color = c;
        }
        selfPosition.y = 330f;
        selfPosition.x = tR.position.x;
        transform.position = selfPosition;
        if (tR.position.y < control.lockPoint)
        {
            timer = 0.0f;
            if (!boxColliderActive)
            {
                
            }
        }
        if (tR.position.y < control.lockPoint && !boxColliderActive)
        {
            transform.GetComponent<BoxCollider2D>().enabled = true;
            boxColliderActive = true;
        }
        if (isEmptyUpdate)
        {
            if (delayTimer < delayLimit)
            {
                delayTimer += Time.deltaTime;
                if (delayTimer > delayLimit)
                {
                    isEmpty = true;
                    delayTimer = 0.0f;
                    initialSet = false;
                }
            }
        }
        else
        {
            if (transform.parent.childCount > 2 && transform.parent.GetChild(2).name.Split("-")[0] == "lock")
            {
                locked = true;
                if (txt.fontSize < 50)
                {
                    txt.fontSize = txt.fontSize + 1;
                    duplicateTxt.fontSize = duplicateTxt.fontSize + 1;
                }
            }
            delayTimer = 0.0f;
            isEmpty = false;
        }
        if (isEmpty)
        {
            // check if is selected word
            if (selectedContainer.transform.childCount > 0)
            {
                if (selectedContainer.transform.GetChild(0).name == transform.parent.name)
                {
                    Destroy(selectedContainer.transform.GetChild(0).gameObject);
                }
            }
            rb.bodyType = RigidbodyType2D.Dynamic;
            // Reset Dupolicate Text alpha
            if (duplicateTxt.color.a > 0)
            {
                Color c = duplicateTxt.color;
                c.a -= 0.1f;
                duplicateTxt.color = c;
            }
            // Reset Text Size
            if (txt.fontSize > 40)
            {
                txt.fontSize = txt.fontSize - 1;
                duplicateTxt.fontSize = duplicateTxt.fontSize - 1;
                txt.color = Color.white;
            }
            // Reset Collision box size
            if (!locked)
            {
                bc.size = new Vector2(50, 660);
            }
            // Destroy Lock
            if (undoHandler.locks.Length > 0)
            {
                if (transform.parent.childCount > 2 && transform.parent.GetChild(2).name.Split("-")[0] == "lock")
                {
                    emptyTimer += Time.deltaTime;
                    if (emptyTimer > 1.0f)
                    {
                        Destroy(transform.parent.GetChild(2).gameObject);
                        emptyTimer = 0.0f;
                        locked = false;
                        particalPlayed = false;
                        timer = 0.0f;
                    }
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "User") 
        { 
        
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.tag == "User")
        {
            // Initial states for interaction
            isEmptyUpdate = false;
            wordStore.allEmpty = false;
            emptyTimer = 0.0f;
            rb.bodyType = RigidbodyType2D.Static;

            // Below the lock line
            if (!locked)
            { 
                if (tR.position.y > control.lockPoint - 80)
                {
                    if (selectedContainer.transform.childCount == 0)
                    {
                        GameObject SelectedWord = Instantiate(selected);
                        SelectedWord.name = transform.parent.name;
                        SelectedWord.transform.SetParent(selectedContainer.transform);
                    }
                }
                // if no other objects are locked
                if (undoHandler.locks.Length == 0 || undoHandler.locks == null && !responseManager.creatingWords)
                {
                    foreach (Transform child in transform.parent.parent)
                    {
                        if (child != transform.parent)
                        {
                            float Distance = Vector2.Distance(bc.transform.position, child.GetChild(1).transform.position);
                            if (Distance < control.distanceSens)
                            {
                                child.GetChild(0).GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                                if (child.GetChild(1).transform.position.x < bc.transform.position.x)
                                {
                                    child.GetChild(0).GetComponent<Rigidbody2D>().AddForce(Vector2.left * control.force, ForceMode2D.Impulse);
                                }
                                else
                                {
                                    child.GetChild(0).GetComponent<Rigidbody2D>().AddForce(Vector2.right * control.force, ForceMode2D.Impulse);
                                }
                            }
                        }
                    }
                    // not sure, check what this does
                    bc.size = new Vector2(control.boxSize / 2, 710);
                    // Set the ID
                    id = collision.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
                    // Rotate to face the correct way
                    if (rb.rotation > 2)
                    {
                        rb.rotation = rb.rotation - 2;
                    }
                    else if (rb.rotation < -2)
                    {
                        rb.rotation = rb.rotation + 2;
                    }
                    // Change to duplicate text (color change)
                    if (duplicateTxt.color.a < 1)
                    {
                        Color c = duplicateTxt.color;
                        c.a += scale(50, control.lockPoint,0,1, tR.position.y);
                        duplicateTxt.color = c;
                    }
                    // enlarge font
                    if (txt.fontSize < 50)
                    {
                        txt.fontSize = txt.fontSize + 1;
                        duplicateTxt.fontSize = duplicateTxt.fontSize + 1;
                    }
                    // Move Updward to lock point
                    // If is sleceted word
                    if (selectedContainer.transform.childCount == 0 || selectedContainer.transform.GetChild(0).name == transform.parent.name)
                    {
                        if (tR.position.y < control.lockPoint + 20)
                        {
                            movePosition.x = tR.position.x;
                            movePosition.y = tR.position.y + 4;
                            tR.position = movePosition;
                        }
                    }
                    else // is not selected word
                    {
                        if (tR.position.y < control.lockPoint - 80)
                        {
                            movePosition.x = tR.position.x;
                            movePosition.y = tR.position.y + 4;
                            tR.position = movePosition;
                        }
                    }
                    if (tR.position.y > control.lockPoint && !responseManager.creatingWords)
                    {
                        timer += Time.deltaTime;
                        holdTimer.text = timer.ToString("F1");
                        if (timer > control.holdTimer)
                        {
                            locked = true;
                            bc.size = new Vector2(control.boxSize, 710);
                        }
                    }
                }
                // if another object is locked
                else
                {
                    // Rotate to face the correct way
                    if (rb.rotation > 2)
                    {
                        rb.rotation = rb.rotation - 2;
                    }
                    else if (rb.rotation < -2)
                    {
                        rb.rotation = rb.rotation + 2;
                    }
                    // Change to normal text (color change)
                    if (duplicateTxt.color.a > 0)
                    {
                        Color c = duplicateTxt.color;
                        c.a -= 0.1f;
                        duplicateTxt.color = c;
                    }
                    // change to noremal font size
                    if (txt.fontSize > 40)
                    {
                        txt.fontSize = txt.fontSize - 1;
                        duplicateTxt.fontSize = duplicateTxt.fontSize - 1;
                    }
                    // Move Updward to lock point
                    if (tR.position.y < control.tempHold - 10)
                    {
                        movePosition.x = tR.position.x;
                        movePosition.y = tR.position.y + 4;
                        tR.position = movePosition;
                    }
                    else if (tR.position.y > control.tempHold + 10)
                    {
                        movePosition.x = tR.position.x;
                        movePosition.y = tR.position.y - 4;
                        tR.position = movePosition;
                    }
                }
            }
            // Above the lock line
            else
            {
                // incase selected word got removed
                if (tR.position.y > control.lockPoint - 80)
                {
                    if (selectedContainer.transform.childCount == 0)
                    {
                        GameObject SelectedWord = Instantiate(selected);
                        SelectedWord.name = transform.parent.name;
                        SelectedWord.transform.SetParent(selectedContainer.transform);
                    }
                }
                // Change to duplicate text (color change)
                if (duplicateTxt.color.a < 1)
                {
                    Color c = duplicateTxt.color;
                    c.a += scale(0, control.lockPoint, 0, 1, tR.position.y);
                    duplicateTxt.color = c;
                }
                // Larger box collider size to keep with tracking
                // add lock object
                bc.size = new Vector2(control.boxSize, 710);
                if (transform.parent.childCount < 3 && undoHandler.locks.Length == 0)
                {
                    GameObject lockGO = Instantiate(lockObject);
                    lockGO.transform.SetParent(transform.parent);
                    lockGO.name = ("lock-" + id);
                }
                // if has lock object
                if (transform.parent.childCount > 2)
                {
                    if (!particalPlayed && control.particalAnim)
                    {
                        PlayPartical(tR.position.x, tR.position.y);
                        particalPlayed = true;
                    }
                    if (transform.parent.GetChild(2).name != ("lock-" + id))
                    {
                        transform.parent.GetChild(2).name = ("lock-" + id);
                    }
                    // move with tracking object between left and right mac points
                    if (collision.transform.position.x > (transform.parent.GetChild(0).GetComponent<TMP_Handler>().preferedWidth / 2) && collision.transform.position.x < (1920 - (transform.parent.GetChild(0).GetComponent<TMP_Handler>().preferedWidth / 2)))
                    {
                        if (!initialSet)
                        {
                            lockPosition.x = collision.transform.position.x;
                            initialSet = true;
                        }
                        else
                        {
                            lockPosition.x = collision.transform.position.x;
                        }
                    }
                    else
                    {
                        lockPosition.x = tR.position.x;
                    }
                    // raise to top line
                    if (tR.position.y < control.holdPoint)
                    {
                        timer = 0.0f;
                        lockPosition.y = tR.position.y + 3;
                        tR.position = lockPosition;
                        if (rb.rotation > 1f)
                        {
                            rb.rotation -= 1f;
                        }
                        else if (rb.rotation < -1f)
                        {
                            rb.rotation += 1f;
                        }
                        else
                        {
                            rb.rotation = 0f;
                        }
                    }
                    else // above lock point
                    {
                        if (transform.parent.childCount > 2)
                        {
                            if (tR.position.y >= control.holdPoint - 10)
                            {
                                rb.rotation = 0;
                                lockPosition.y = tR.position.y;
                                timer += Time.deltaTime;
                                lockTimer.text = timer.ToString("F1");
                                if (timer >= 1f)
                                {
                                    if (holdPosition.x < tR.position.x - control.holdSens || holdPosition.x > tR.position.x + control.holdSens)
                                    {
                                        holdPosition.x = tR.position.x;
                                        timer = 0.0f;
                                    }
                                }
                                // add to topLock
                                if (timer > control.lockTimer)
                                {
                                    bc.size = new Vector2(control.boxSize, 710);
                                    if (selectedContainer.transform.childCount > 0)
                                    {
                                        if (selectedContainer.transform.GetChild(0).name == transform.parent.name)
                                        {
                                            Destroy(selectedContainer.transform.GetChild(0).gameObject);
                                        }
                                    }
                                    Destroy(transform.parent.GetChild(0).GetComponent<PolygonCollider2D>());
                                    Destroy(transform.parent.GetChild(0).GetChild(0).gameObject);
                                    rb.bodyType = RigidbodyType2D.Static;
                                    rb.gravityScale = 0f;
                                    if (transform.parent.childCount == 3)
                                    {
                                        if (transform.parent.GetChild(2).tag == "textLock")
                                        {
                                            transform.parent.GetChild(2).tag = "textHold";
                                            transform.parent.GetChild(2).name = ("inPlace-" + (undoHandler.holds.Length));
                                        }
                                    }
                                    transform.parent.transform.SetParent(topLock.transform);
                                }
                                else
                                {
                                    tR.position = lockPosition;
                                }
                            }
                        }
                    }

                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "User")
        {

        }
    }
    private void PlayPartical(float x, float y)
    {
        ParticleSystem wordExplosion = Instantiate(wordPartical);
        Vector2 position;
        position.x = x;
        position.y = y;
        wordExplosion.transform.position = position;
        wordExplosion.transform.SetParent(GameObject.FindGameObjectWithTag("MainCanvas").transform);
        wordExplosion.Play();
    }
    public float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {

        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }
}

