﻿using UnityEngine;
using System.Collections;
using Windows.Kinect;
using SaveIt;
using DG.Tweening;


[System.Serializable]
[RequireComponent(typeof(DepthSourceManager))]
public class KinectCalib : MonoBehaviour {

    // Event Handler
    public delegate void OnPointCloudUpdateEvent(CameraSpacePoint[] pointCloud, int depthMapWidth, int depthMapHeight, int downsample);
    public event OnPointCloudUpdateEvent OnPointCloudUpdate;

    public static KinectCalib instance;

    private KinectSensor kinect;
    private DepthSourceManager depthManager;
    private CoordinateMapper mapper;

    private int depthWidth, depthHeight, depthMapSize;

    [HideInInspector]
    public KPCL[] pcl;

    //gui
    public bool mirror;
    [Range(1,20)]
    public int downSample = 1;

    public bool loadConfigOnStart;
    public bool saveConfigOnExit;
    public bool useTCLStreamer;

    [Header("Point cloud")]
    public bool drawPointCloud;
    public PointCloudDrawer pointCloudDrawer;

    void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start () {

        PlayerPrefUpdateBroadcast.Instance.OnPlayerPrefsUpdated += OnPlayerPrefsUpdated;
        if (loadConfigOnStart) loadConfig();

        kinect = KinectSensor.GetDefault();
        if (kinect != null)
        {
            mapper = kinect.CoordinateMapper;
            var frameDesc = kinect.DepthFrameSource.FrameDescription;

            // Downsample to lower resolution
            // CreateMesh(frameDesc.Width / _DownsampleSize, frameDesc.Height / _DownsampleSize);

            depthWidth = frameDesc.Width;
            depthHeight = frameDesc.Height;
            depthMapSize = depthHeight * depthWidth / downSample;

            Debug.Log("Kinect Depth Width :" + depthWidth + " / Height : " + depthHeight);

            if (!kinect.IsOpen)
            {
                kinect.Open();
            }
        }

        depthManager = GetComponent<DepthSourceManager>();

        if(useTCLStreamer)
        {
            int numLines = depthHeight / (downSample - 1);
            pcl = new KPCL[numLines];
            for (int i = 0; i < numLines; i++)
            {
                pcl[i] = new KPCL();
                pcl[i].points = new KPCL.Vector_3[(depthWidth / (downSample - 1))];

            }

            pcl[0].isFirst = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(drawPointCloud)
        {
            UpdatePointCloud();
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
        transform.DOMove(PlayerPrefs_AM.GetVector3("K2M_KinectPosition"), 0.3f);
        transform.DORotate(PlayerPrefs_AM.GetVector3("K2M_KinectRotation"), 0.3f);
        downSample = (int)PlayerPrefs_AM.GetFloat("PointCloudDownSample", 5);
        drawPointCloud = PlayerPrefs_AM.GetBool("ShowPointCloud");

        pointCloudDrawer.SetDrawPointCloud(drawPointCloud);
    }

    void UpdatePointCloud()
    {
        ushort[] depthMap = depthManager.GetData();

        CameraSpacePoint[] realWorldPoints = new CameraSpacePoint[depthMap.Length];
        mapper.MapDepthFrameToCameraSpace(depthMap, realWorldPoints);

        // Update Real world to kinect position.
        for (int ty = 0; ty < depthHeight; ty += downSample)
        {
            for (int tx = 0; tx < depthWidth; tx += downSample)
            {
                int index = ty * depthWidth + tx;
                CameraSpacePoint point = realWorldPoints[index];

                Vector3 pointV3 = new Vector3(mirror ? -point.X : point.X, point.Y, point.Z);
                pointV3 = transform.TransformPoint(pointV3);

                realWorldPoints[index].X = pointV3.x;
                realWorldPoints[index].Y = pointV3.y;
                realWorldPoints[index].Z = pointV3.z;
            }
        }

        OnPointCloudUpdate(realWorldPoints, depthWidth, depthHeight, downSample);
    }

    void OnPlayerPrefsUpdated(string playerPrefKey)
    {
        if(playerPrefKey.Contains("K2M_KinectPosition"))
        {
            transform.DOMove(PlayerPrefs_AM.GetVector3(playerPrefKey), 0.3f);
        }
        else if (playerPrefKey.Contains("K2M_KinectRotation"))
        {
            transform.DORotate(PlayerPrefs_AM.GetVector3(playerPrefKey), 0.3f);
        }
        else if (playerPrefKey.Contains("PointCloudDownSample"))
        {
            downSample = (int)PlayerPrefs_AM.GetFloat(playerPrefKey, 5);
        }
        else if (playerPrefKey.Contains("ShowPointCloud"))
        {
            drawPointCloud = PlayerPrefs_AM.GetBool(playerPrefKey);
            pointCloudDrawer.SetDrawPointCloud(drawPointCloud);
        }
    }

}
