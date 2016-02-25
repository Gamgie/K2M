using UnityEngine;
using System.Collections;
using Windows.Kinect;
using SaveIt;

[RequireComponent(typeof(DepthSourceManager))]
public class KinectCalib : MonoBehaviour {

    public static KinectCalib instance;

    private KinectSensor kinect;
    private DepthSourceManager depthManager;
    private CoordinateMapper mapper;

    private int depthWidth, depthHeight;
    private Vector3[] points;

    //gui
    public bool mirror;
    [Range(1,20)]
    public int downSample = 1;

    public bool loadConfigOnStart;
    public bool saveConfigOnExit;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {
        kinect = KinectSensor.GetDefault();
        if (kinect != null)
        {
            mapper = kinect.CoordinateMapper;
            var frameDesc = kinect.DepthFrameSource.FrameDescription;

            // Downsample to lower resolution
            // CreateMesh(frameDesc.Width / _DownsampleSize, frameDesc.Height / _DownsampleSize);

            depthWidth = frameDesc.Width;
            depthHeight = frameDesc.Height;

            Debug.Log("Depth Width/Height " + depthWidth + " / " + depthHeight);

            if (!kinect.IsOpen)
            {
                kinect.Open();
            }
        }

        depthManager = GetComponent<DepthSourceManager>();

        if (loadConfigOnStart) loadConfig();
    }

    // Update is called once per frame
    void Update()
    {

        ushort[] depthMap = depthManager.GetData();

        CameraSpacePoint[] realWorldPoints = new CameraSpacePoint[depthMap.Length];
        mapper.MapDepthFrameToCameraSpace(depthMap, realWorldPoints);

        for(int ty = 0; ty < depthHeight; ty += downSample)
        {
            for(int tx=0; tx < depthWidth; tx += downSample)
            {
                int index = ty * depthWidth + tx;
                CameraSpacePoint point = realWorldPoints[index];

                Vector3 pointV3 = new Vector3(mirror?-point.X:point.X, point.Y, point.Z);
                Vector3 tPoint = transform.TransformPoint(pointV3);
                Debug.DrawLine(tPoint, tPoint+Vector3.forward*.01f);
            }
        }
    }


    void OnDestroy()
    {
        if(saveConfigOnExit) saveConfig();
    }


    public void saveConfig()
    {
        SaveContext saveContext = SaveContext.ToFile("kinect");
        saveContext.Save<bool>(mirror, "mirror");
        saveContext.Save<int>(downSample, "downSample");
        saveContext.Save<Vector3>(transform.position, "position");
        saveContext.Save<Quaternion>(transform.rotation, "rotation");
        saveContext.Flush();
    }

    public void loadConfig()
    {
        LoadContext loadContext = LoadContext.FromFile("kinect");
        mirror = loadContext.Load<bool>("mirror");
        downSample = loadContext.Load<int>("downSample");
        transform.position = loadContext.Load<Vector3>("position");
        transform.rotation = loadContext.Load<Quaternion>("rotation");
    }
}
