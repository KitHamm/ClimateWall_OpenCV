#if !(PLATFORM_LUMIN && !UNITY_EDITOR)

#if !UNITY_WSA_10_0

using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgcodecsModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.UnityUtils.Helper;
using OpenCVForUnityExample.DnnModel;
using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using Unity.VisualScripting;
using System.Linq;

namespace OpenCVForUnityExample
{
    /// <summary>
    /// Object Detection YOLOv7 Example
    /// An example of using OpenCV dnn module with YOLOv7 Object Detection (model without NMS processing).
    /// Referring to https://github.com/AlexeyAB/darknet
    /// 
    /// [Tested Models]
    /// yolov7-tiny https://github.com/AlexeyAB/darknet/releases/download/yolov4/yolov7-tiny.weights, https://raw.githubusercontent.com/AlexeyAB/darknet/0faed3e60e52f742bbef43b83f6be51dd30f373e/cfg/yolov7-tiny.cfg
    /// yolov7 https://github.com/AlexeyAB/darknet/releases/download/yolov4/yolov7.weights, https://raw.githubusercontent.com/AlexeyAB/darknet/0faed3e60e52f742bbef43b83f6be51dd30f373e/cfg/yolov7.cfg
    /// </summary>
    [RequireComponent(typeof(WebCamTextureToMatHelper))]
    public class ObjectDetectionYOLOv7Example : MonoBehaviour
    {

        public GameObject Canvas;
        public GameObject centerPoint;
        public GameObject testDropdown;
        public control control;
        float destroyTimer;
        Vector2 locatorPosition;


        [TooltipAttribute("Path to a binary file of model contains trained weights. It could be a file with extensions .caffemodel (Caffe), .pb (TensorFlow), .t7 or .net (Torch), .weights (Darknet).")]
        public string model = "yolov7-tiny.weights";

        [TooltipAttribute("Path to a text file of model contains network configuration. It could be a file with extensions .prototxt (Caffe), .pbtxt (TensorFlow), .cfg (Darknet).")]
        public string config = "yolov7-tiny.cfg";

        [TooltipAttribute("Optional path to a text file with names of classes to label detected objects.")]
        public string classes = "coco.names";

        [TooltipAttribute("Confidence threshold.")]
        public float confThreshold = 0.25f;

        [TooltipAttribute("Non-maximum suppression threshold.")]
        public float nmsThreshold = 0.45f;

        //[TooltipAttribute("Maximum detections per image.")]
        //public int topK = 1000;

        [TooltipAttribute("Preprocess input image by resizing to a specific width.")]
        public int inpWidth = 416;

        [TooltipAttribute("Preprocess input image by resizing to a specific height.")]
        public int inpHeight = 416;

        [Header("TEST")]

        [TooltipAttribute("Path to test input image.")]
        public string testInputImage;
        public bool test = false;
        bool updateTest = false;

        protected string classes_filepath;
        protected string config_filepath;
        protected string model_filepath;

        /// The texture.
        Texture2D texture;

        /// The webcam texture to mat helper.
        WebCamTextureToMatHelper webCamTextureToMatHelper;

        /// Video TextureToMatHeler
        VideoCaptureToMatHelper videoCaptureToMatHelper;

        /// The bgr mat.
        Mat bgrMat;

        /// The YOLOv7 ObjectDetector.
        YOLOv7ObjectDetector objectDetector;

        public class distanceDict
        {
            public GameObject GO;
            public Vector3 position;
            public float distance;
        }

        // Use this for initialization
        void Start()
        {
            destroyTimer = 0;
            webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper>();
            videoCaptureToMatHelper = gameObject.GetComponent<VideoCaptureToMatHelper>();

            if (!string.IsNullOrEmpty(classes))
            {
                classes_filepath = Utils.getFilePath("OpenCVForUnity/dnn/" + classes);
                if (string.IsNullOrEmpty(classes_filepath)) Debug.Log("The file:" + classes + " did not exist in the folder “Assets/StreamingAssets/OpenCVForUnity/dnn”.");
            }
            if (!string.IsNullOrEmpty(config))
            {
                config_filepath = Utils.getFilePath("OpenCVForUnity/dnn/" + config);
                if (string.IsNullOrEmpty(config_filepath)) Debug.Log("The file:" + config + " did not exist in the folder “Assets/StreamingAssets/OpenCVForUnity/dnn”.");
            }
            if (!string.IsNullOrEmpty(model))
            {
                model_filepath = Utils.getFilePath("OpenCVForUnity/dnn/" + model);
                if (string.IsNullOrEmpty(model_filepath)) Debug.Log("The file:" + model + " did not exist in the folder “Assets/StreamingAssets/OpenCVForUnity/dnn”.");
            }
            Run();
        }

        // Use this for initialization
        void Run()
        {
            //if true, The error log of the Native side OpenCV will be displayed on the Unity Editor Console.
            Utils.setDebugMode(true);


            if (string.IsNullOrEmpty(model_filepath) || string.IsNullOrEmpty(classes_filepath))
            {
                Debug.LogError("model: " + model + " or " + "config: " + config + " or " + "classes: " + classes + " is not loaded.");
            }
            else
            {
                objectDetector = new YOLOv7ObjectDetector(model_filepath, config_filepath, classes_filepath, new Size(inpWidth, inpHeight), confThreshold, nmsThreshold/*, topK*/);
            }
            // IMAGE TEST CHECK
            if (!test)
            {
                webCamTextureToMatHelper.Initialize();
            }
            else
            {
                Debug.Log("Video");
                videoCaptureToMatHelper.Initialize();
            }
        }

        /// <summary>
        /// Raises the webcam texture to mat helper initialized event.
        /// </summary>
        public void OnWebCamTextureToMatHelperInitialized()
        {
            Debug.Log("OnWebCamTextureToMatHelperInitialized");

            Mat webCamTextureMat = webCamTextureToMatHelper.GetMat();

            texture = new Texture2D(webCamTextureMat.cols(), webCamTextureMat.rows(), TextureFormat.RGBA32, false);
            Utils.matToTexture2D(webCamTextureMat, texture);

            gameObject.GetComponent<Renderer>().material.mainTexture = texture;

            gameObject.transform.localScale = new Vector3(webCamTextureMat.cols(), webCamTextureMat.rows(), 1);
            Debug.Log("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " + Screen.orientation);

            float width = webCamTextureMat.width();
            float height = webCamTextureMat.height();

            float widthScale = (float)Screen.width / width;
            float heightScale = (float)Screen.height / height;
            if (widthScale < heightScale)
            {
                Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
            }
            else
            {
                Camera.main.orthographicSize = height / 2;
            }

            bgrMat = new Mat(webCamTextureMat.rows(), webCamTextureMat.cols(), CvType.CV_8UC3);
        }
        public void OnVideoTextureToMatHelperInitialized()
        {
            Debug.Log("OnVideoTextureToMatHelperInitialized");

            Mat webCamTextureMat = videoCaptureToMatHelper.GetMat();

            texture = new Texture2D(webCamTextureMat.cols(), webCamTextureMat.rows(), TextureFormat.RGBA32, false);
            Utils.matToTexture2D(webCamTextureMat, texture);

            gameObject.GetComponent<Renderer>().material.mainTexture = texture;

            gameObject.transform.localScale = new Vector3(webCamTextureMat.cols(), webCamTextureMat.rows(), 1);
            Debug.Log("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " + Screen.orientation);

            float width = webCamTextureMat.width();
            float height = webCamTextureMat.height();

            float widthScale = (float)Screen.width / width;
            float heightScale = (float)Screen.height / height;
            if (widthScale < heightScale)
            {
                Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
            }
            else
            {
                Camera.main.orthographicSize = height / 2;
            }

            bgrMat = new Mat(webCamTextureMat.rows(), webCamTextureMat.cols(), CvType.CV_8UC3);
        }

        /// <summary>
        /// Raises the webcam texture to mat helper disposed event.
        /// </summary>
        public void OnWebCamTextureToMatHelperDisposed()
        {
            Debug.Log("OnWebCamTextureToMatHelperDisposed");

            if (bgrMat != null)
                bgrMat.Dispose();

            if (texture != null)
            {
                Texture2D.Destroy(texture);
                texture = null;
            }
        }

        /// <summary>
        /// Raises the webcam texture to mat helper error occurred event.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        public void OnWebCamTextureToMatHelperErrorOccurred(WebCamTextureToMatHelper.ErrorCode errorCode)
        {
            Debug.Log("OnWebCamTextureToMatHelperErrorOccurred " + errorCode);
        }

        public void VideoTextureToMatHelperErrorOccurred(VideoCaptureToMatHelper.ErrorCode errorCode)
        {
            Debug.Log("VideoTextureToMatHelperErrorOccurred " + errorCode);
        }

        // Update is called once per frame
        void Update()
        {
            GameObject.FindGameObjectWithTag("destroyTimer").GetComponent<TextMeshProUGUI>().text = destroyTimer.ToString("F2");
            if (webCamTextureToMatHelper.IsPlaying() && webCamTextureToMatHelper.DidUpdateThisFrame())
            {

                Mat rgbaMat = webCamTextureToMatHelper.GetMat();

                if (objectDetector == null)
                {
                    Imgproc.putText(rgbaMat, "model file is not loaded.", new Point(5, rgbaMat.rows() - 30), Imgproc.FONT_HERSHEY_SIMPLEX, 0.7, new Scalar(255, 255, 255, 255), 2, Imgproc.LINE_AA, false);
                    Imgproc.putText(rgbaMat, "Please read console message.", new Point(5, rgbaMat.rows() - 10), Imgproc.FONT_HERSHEY_SIMPLEX, 0.7, new Scalar(255, 255, 255, 255), 2, Imgproc.LINE_AA, false);
                }
                else
                {

                    Imgproc.cvtColor(rgbaMat, bgrMat, Imgproc.COLOR_RGBA2BGR);
                    Mat results = objectDetector.infer(bgrMat);
                    Imgproc.cvtColor(bgrMat, rgbaMat, Imgproc.COLOR_BGR2RGBA);
                    objectDetector.visualize(rgbaMat, results, false, true);
                    List<GameObject> trackedMovedObjects = new List<GameObject>();
                    trackedMovedObjects.Clear();
                    for (int i = 0; i < results.rows(); ++i)
                    {
                        float[] cls = new float[1];
                        results.get(i, 5, cls);
                        int classId = (int)cls[0];

                        float[] box = new float[4];
                        results.get(i, 0, box);
                        float[] conf = new float[1];
                        results.get(i, 4, conf);
                        float xMidPoint = ((box[2] - box[0]) / 2) + box[0];
                        float yMidPoint = 1080 - (((box[3] - box[1]) / 2) + box[1]);
                        locatorPosition.x = xMidPoint;
                        locatorPosition.y = yMidPoint;

                        if (GameObject.Find("centerpoint-" + i) == null)
                        {
                            GameObject newCenter = Instantiate(centerPoint);
                            newCenter.name = "centerpoint-" + i;
                            newCenter.transform.SetParent(Canvas.transform);
                            newCenter.transform.position = locatorPosition;
                            newCenter.GetComponent<UserTrigger>().conf = conf[0];

                        }
                        else
                        {
                            float lowestDistance = 30000;
                            GameObject userToMove = null;
                            foreach (GameObject User in GameObject.FindGameObjectsWithTag("User"))
                            {
                                if (!trackedMovedObjects.Contains(User))
                                {
                                    float distance = Vector2.Distance(locatorPosition, User.transform.position);
                                    if (distance < lowestDistance)
                                    {
                                        lowestDistance = distance;
                                        userToMove = User;
                                    }
                                }
                            }
                            if (userToMove != null)
                            {
                                trackedMovedObjects.Add(userToMove);
                                userToMove.GetComponent<UserTrigger>().initialPos = locatorPosition;
                                userToMove.GetComponent<UserTrigger>().conf = conf[0];
                            }
                        }
                    }
                    List<GameObject> centerObjects = new List<GameObject>();
                    foreach (Transform child in Canvas.transform)
                    {
                        if (child.tag == "User")
                        {
                            centerObjects.Add(child.gameObject);
                        }
                    }
                    GameObject.FindGameObjectWithTag("rowUserCount").GetComponent<TextMeshProUGUI>().text = centerObjects.Count + " | " + results.rows();
                    if (centerObjects.Count > results.rows() && centerObjects.Count > 0)
                    {
                        destroyTimer += Time.deltaTime;
                        if (destroyTimer > control.deleteDelayTime)
                        {
                            Destroy(centerObjects[centerObjects.Count - 1]);
                            destroyTimer = 0;
                        }
                    }
                    else
                    {
                        destroyTimer = 0;
                    }
                }
                Utils.matToTexture2D(rgbaMat, texture);
            }
            if (videoCaptureToMatHelper.IsPlaying() && videoCaptureToMatHelper.DidUpdateThisFrame())
            {
                Mat rgbaMat = videoCaptureToMatHelper.GetMat();

                if (objectDetector == null)
                {
                    Imgproc.putText(rgbaMat, "model file is not loaded.", new Point(5, rgbaMat.rows() - 30), Imgproc.FONT_HERSHEY_SIMPLEX, 0.7, new Scalar(255, 255, 255, 255), 2, Imgproc.LINE_AA, false);
                    Imgproc.putText(rgbaMat, "Please read console message.", new Point(5, rgbaMat.rows() - 10), Imgproc.FONT_HERSHEY_SIMPLEX, 0.7, new Scalar(255, 255, 255, 255), 2, Imgproc.LINE_AA, false);
                }
                else
                {

                    Imgproc.cvtColor(rgbaMat, bgrMat, Imgproc.COLOR_RGBA2BGR);
                    Mat results = objectDetector.infer(bgrMat);
                    Imgproc.cvtColor(bgrMat, rgbaMat, Imgproc.COLOR_BGR2RGBA);
                    objectDetector.visualize(rgbaMat, results, false, true);
                    List<GameObject> trackedMovedObjects = new List<GameObject>();
                    trackedMovedObjects.Clear();
                    for (int i = 0; i < results.rows(); ++i)
                    {
                        float[] cls = new float[1];
                        results.get(i, 5, cls);
                        int classId = (int)cls[0];

                        float[] box = new float[4];
                        results.get(i, 0, box);
                        float[] conf = new float[1];
                        results.get(i, 4, conf);
                        float xMidPoint = ((box[2] - box[0]) / 2) + box[0];
                        float yMidPoint = 1080 - (((box[3] - box[1]) / 2) + box[1]);
                        locatorPosition.x = xMidPoint;
                        locatorPosition.y = yMidPoint;

                        if (GameObject.Find("centerpoint-" + i) == null)
                        {
                            GameObject newCenter = Instantiate(centerPoint);
                            newCenter.name = "centerpoint-" + i;
                            newCenter.transform.SetParent(Canvas.transform);
                            newCenter.transform.position = locatorPosition;
                            newCenter.GetComponent<UserTrigger>().conf = conf[0];

                        }
                        else
                        {
                            float lowestDistance = 30000;
                            GameObject userToMove = null;
                            foreach (GameObject User in GameObject.FindGameObjectsWithTag("User"))
                            {
                                if (!trackedMovedObjects.Contains(User))
                                {
                                    float distance = Vector2.Distance(locatorPosition, User.transform.position);
                                    if (distance < lowestDistance)
                                    {
                                        lowestDistance = distance;
                                        userToMove = User;
                                    }
                                }
                            }
                            if (userToMove != null)
                            {
                                trackedMovedObjects.Add(userToMove);
                                userToMove.GetComponent<UserTrigger>().initialPos = locatorPosition;
                                userToMove.GetComponent<UserTrigger>().conf = conf[0];
                            }
                        }
                    }
                    List<GameObject> centerObjects = new List<GameObject>();
                    foreach (Transform child in Canvas.transform)
                    {
                        if (child.tag == "User")
                        {
                            centerObjects.Add(child.gameObject);
                        }
                    }
                    GameObject.FindGameObjectWithTag("rowUserCount").GetComponent<TextMeshProUGUI>().text = centerObjects.Count + " | " + results.rows();
                    if (centerObjects.Count > results.rows() && centerObjects.Count > 0)
                    {
                        destroyTimer += Time.deltaTime;
                        if (destroyTimer > control.deleteDelayTime)
                        {
                            Destroy(centerObjects[centerObjects.Count - 1]);
                            destroyTimer = 0;
                        }
                    }
                    else
                    {
                        destroyTimer = 0;
                    }
                }
                Utils.matToTexture2D(rgbaMat, texture);
            }
            if (test != updateTest)
            {
                if (test)
                {
                    webCamTextureToMatHelper.Dispose();
                    videoCaptureToMatHelper.Initialize();
                    testDropdown.gameObject.SetActive(true);
                }
                else
                {
                    videoCaptureToMatHelper.Dispose();
                    webCamTextureToMatHelper.Initialize();
                    testDropdown.gameObject.SetActive(false);
                }
                updateTest = test;
            }
        }




        /// Raises the destroy event.
        void OnDestroy()
        {
            Debug.Log("Destroy");
            webCamTextureToMatHelper.Dispose();

            if (objectDetector != null)
                objectDetector.dispose();

            Utils.setDebugMode(false);

        }
    }
}
#endif

#endif