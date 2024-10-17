using OpenCVForUnity.CoreModule;
using OpenCVForUnity.VideoModule;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class UserTrigger : MonoBehaviour
{
    /// The kalman filter.
    KalmanFilter KF;

    /// The measurement.
    Mat measurement;

    public Vector2 initialPos;
    public Vector2 prevPos;
    public Vector2 filteredPos;
    public int trackinIndex;
    public bool started;
    private float time;
    private float pollingTime = 1f;
    public float conf;
    private float avgConf;
    private int frameCount;
    bool rbAsleep;
    control control;
    private void Start()
    {
        frameCount = 0;
        control = GameObject.FindGameObjectWithTag("control").GetComponent<control>();
        rbAsleep = false;
        conf = 0.0f;
        initialPos = transform.position;
        prevPos = initialPos;
        // intialization of KF...
        KF = new KalmanFilter(4, 2, 0, CvType.CV_32FC1);
        Mat transitionMat = new Mat(4, 4, CvType.CV_32F);
        transitionMat.put(0, 0, new float[] { 1, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1 });
        KF.set_transitionMatrix(transitionMat);
        measurement = new Mat(2, 1, CvType.CV_32FC1);
        measurement.setTo(Scalar.all(0));

        // Set initial state estimate.
        Mat statePreMat = KF.get_statePre();
        statePreMat.put(0, 0, new float[] { (float)transform.position.x, (float)transform.position.y, 0, 0 });
        Mat statePostMat = KF.get_statePost();
        statePostMat.put(0, 0, new float[] { (float)transform.position.x, (float)transform.position.y, 0, 0 });

        Mat measurementMat = new Mat(2, 4, CvType.CV_32FC1);
        Core.setIdentity(measurementMat);
        KF.set_measurementMatrix(measurementMat);

        Mat processNoiseCovMat = new Mat(4, 4, CvType.CV_32FC1);
        Core.setIdentity(processNoiseCovMat, Scalar.all(1e-4));
        KF.set_processNoiseCov(processNoiseCovMat);

        Mat measurementNoiseCovMat = new Mat(2, 2, CvType.CV_32FC1);
        Core.setIdentity(measurementNoiseCovMat, Scalar.all(10));
        KF.set_measurementNoiseCov(measurementNoiseCovMat);

        Mat errorCovPostMat = new Mat(4, 4, CvType.CV_32FC1);
        Core.setIdentity(errorCovPostMat, Scalar.all(.1));
        KF.set_errorCovPost(errorCovPostMat);
    }

    private void Update()
    {
        if (initialPos != prevPos)
        {
            if (initialPos.x < 960)
            {
                //float scaledPos = scale(640, 0, 640, scale(640, 0, control.shiftScale, 0 - control.shiftScale, initialPos.x), initialPos.x);
                float scaledPos = scale(960, 0, 960, scale(960, 0, 0, 0 - control.shiftScale, initialPos.x), scale(960, 0, 960, 100, initialPos.x));
                transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = (scaledPos - initialPos.x).ToString("F0");
                if (scaledPos > 0)
                {
                    initialPos.x = scaledPos;
                }
                else
                {
                    initialPos.x = 0;
                }
            }
            else if (initialPos.x > 960)
            {
                //float scaledPos = scale(1280, 1920, 1280, 1920 - scale(1280, 1920, control.shiftScale, 0 - control.shiftScale, initialPos.x), initialPos.x);
                float scaledPos = scale(960, 1920, 960, scale(960, 1920, 1920, 1920 + (control.shiftScale * control.offsideShiftFactor), initialPos.x), scale(960, 1920, 960, 1820, initialPos.x));
                transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "+" + (scaledPos - initialPos.x).ToString("F0");
                if (scaledPos < 1920)
                {
                    initialPos.x = scaledPos;
                }
                else
                {
                    initialPos.x = 1920;
                }
            }
            else
            {
                transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "0";
            }
            prevPos = initialPos;
        }
        else
        {
            conf = 0;
        }
        avgConf = avgConf + conf;
        frameCount++;
        time += Time.deltaTime;
        if (time >= pollingTime)
        {
            float newAvg = avgConf / frameCount;
            transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = (newAvg * 100).ToString("F0") + "%";
            time = 0;
            avgConf = 0;
            frameCount = 0;
        }

        
        if (transform.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic && transform.GetComponent<Rigidbody2D>().IsSleeping() && !rbAsleep)
        {
            Debug.Log(transform.name + " is asleep.");
            rbAsleep = true;
        }
        if (transform.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic && transform.GetComponent<Rigidbody2D>().IsAwake() && rbAsleep)
        {
            Debug.Log(transform.name + " is awake.");
            rbAsleep = false;
        }
        if (transform.name.Contains("centerpoint"))
        {
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = transform.name.Split("-")[1];
        }


        Point predictedPt;
        Point estimatedPt;

        

        using (Mat prediction = KF.predict())
        {
            predictedPt = new Point(prediction.get(0, 0)[0], prediction.get(1, 0)[0]);
        }
        measurement.put(0, 0, new float[] { initialPos.x, initialPos.y });
        Point measurementPt = new Point(measurement.get(0, 0)[0], measurement.get(1, 0)[0]);
        using (Mat estimated = KF.correct(measurement))
        {
            estimatedPt = new Point(estimated.get(0, 0)[0], estimated.get(1, 0)[0]);
        }

        filteredPos.x = (float)estimatedPt.x;
        filteredPos.y = (float)estimatedPt.y;
        float distance = Vector2.Distance(transform.position, initialPos);
        if (distance > 400)
        {
            transform.position = initialPos;
            ResetKF();
        }
        else
        {
            transform.position = filteredPos;
        }
    }

    public float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {

        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }
    void ResetKF()
    {
        KF = null;
        measurement.release();
        // intialization of KF...
        KF = new KalmanFilter(4, 2, 0, CvType.CV_32FC1);
        Mat transitionMat = new Mat(4, 4, CvType.CV_32F);
        transitionMat.put(0, 0, new float[] { 1, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1 });
        KF.set_transitionMatrix(transitionMat);
        measurement = new Mat(2, 1, CvType.CV_32FC1);
        measurement.setTo(Scalar.all(0));

        // Set initial state estimate.
        Mat statePreMat = KF.get_statePre();
        statePreMat.put(0, 0, new float[] { (float)transform.position.x, (float)transform.position.y, 0, 0 });
        Mat statePostMat = KF.get_statePost();
        statePostMat.put(0, 0, new float[] { (float)transform.position.x, (float)transform.position.y, 0, 0 });

        Mat measurementMat = new Mat(2, 4, CvType.CV_32FC1);
        Core.setIdentity(measurementMat);
        KF.set_measurementMatrix(measurementMat);

        Mat processNoiseCovMat = new Mat(4, 4, CvType.CV_32FC1);
        Core.setIdentity(processNoiseCovMat, Scalar.all(1e-4));
        KF.set_processNoiseCov(processNoiseCovMat);

        Mat measurementNoiseCovMat = new Mat(2, 2, CvType.CV_32FC1);
        Core.setIdentity(measurementNoiseCovMat, Scalar.all(10));
        KF.set_measurementNoiseCov(measurementNoiseCovMat);

        Mat errorCovPostMat = new Mat(4, 4, CvType.CV_32FC1);
        Core.setIdentity(errorCovPostMat, Scalar.all(.1));
        KF.set_errorCovPost(errorCovPostMat);

    }
}
