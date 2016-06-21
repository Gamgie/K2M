using UnityEngine;
using System.Collections;
using Windows.Kinect;
using SaveIt;


[System.Serializable]
[RequireComponent(typeof(DepthSourceManager))]
public class KinectCalib : MonoBehaviour {

    public static KinectCalib instance;

    private KinectSensor kinect;
    private DepthSourceManager depthManager;
    private CoordinateMapper mapper;

    private int depthWidth, depthHeight;

    [HideInInspector]
    public KPCL[] pcl;

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

            Debug.Log("Kinect Depth Width :" + depthWidth + " / Height : " + depthHeight);

            if (!kinect.IsOpen)
            {
                kinect.Open();
            }
        }

        depthManager = GetComponent<DepthSourceManager>();

        int numLines = depthHeight / (downSample-1);
        pcl = new KPCL[numLines];
        for (int i = 0; i < numLines; i++)
        {
            pcl[i] = new KPCL();
            pcl[i].points = new KPCL.Vector_3[(depthWidth / (downSample-1))];
            
        }

        pcl[0].isFirst = true;

        if (loadConfigOnStart) loadConfig();
    }

    // Update is called once per frame
    void Update()
    {

        ushort[] depthMap = depthManager.GetData();

        CameraSpacePoint[] realWorldPoints = new CameraSpacePoint[depthMap.Length];
        mapper.MapDepthFrameToCameraSpace(depthMap, realWorldPoints);

        int curLine = 0;
        int pIndex = 0;
        for(int ty = 0; ty < depthHeight; ty += downSample)
        {
            pIndex = 0;

            for (int tx=0; tx < depthWidth; tx += downSample)
            {
                int index = ty * depthWidth + tx;
                CameraSpacePoint point = realWorldPoints[index];

                Vector3 pointV3 = new Vector3(mirror?-point.X:point.X, point.Y, point.Z);
                Vector3 tPoint = transform.TransformPoint(pointV3);
                Debug.DrawLine(tPoint, tPoint+Vector3.forward*.01f);


                if (pcl[curLine].points.Length > pIndex)
                {
                    pcl[curLine].points[pIndex] = new KPCL.Vector_3(tPoint.x, tPoint.y, tPoint.z);
                }
                pIndex++;
            }

            curLine++;
            
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
