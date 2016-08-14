using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using UnityOSC;

[RequireComponent(typeof(BodySourceManager))]
public class K2M : MonoBehaviour
{
    public GameObject bodyPrefab;

    
    public bool sendOSC = true;
    [Range(1,60)]
    public int sendRate;
    float lastSendTime = 0;

    public string targetHost = "127.0.0.1";
    public int targetPort = 9090;

    public bool sendExtendedSkeleton;
    int[] extendedSkeletonIndices = { 6, 10, 12, 14, 16, 18, 20, 21, 22, 23, 24 };

    private Dictionary<ulong, K2MBody> _Bodies = new Dictionary<ulong, K2MBody>();
    private BodySourceManager _BodyManager;

    OSCClient client;

    void Start()
    {
        _BodyManager = GetComponent<BodySourceManager>();

        targetHost = PlayerPrefs_AM.GetString("OSCTargetIP", "127.0.0.1");
        targetPort = PlayerPrefs_AM.GetInt("OSCTargetPort", 9090);

        client = new OSCClient(System.Net.IPAddress.Parse(targetHost),targetPort, false);

        PlayerPrefUpdateBroadcast.Instance.OnPlayerPrefsUpdated += OnPlayerPrefsUpdated;
    }

    void Update()
    {
        if (_BodyManager == null)
        {
            return;
        }

        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }

        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }

        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

        // First delete untracked bodies
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                Debug.Log("Destroy Body");
                GameObject.Destroy(_Bodies[trackingId].gameObject);
                _Bodies.Remove(trackingId);

                OSCMessage m;
                m = new OSCMessage("/k2m/body/left");
                m.Append<int>((int)trackingId);
                client.SendTo(m, targetHost, targetPort);
            }
        }

        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                if (!_Bodies.ContainsKey(body.TrackingId))
                {
                    
                    _Bodies[body.TrackingId] = GameObject.Instantiate(bodyPrefab).GetComponent<K2MBody>();
                    _Bodies[body.TrackingId].trackingId = (int)body.TrackingId;
                    _Bodies[body.TrackingId].transform.parent = transform;
                    
                    OSCMessage m;
                    m = new OSCMessage("/k2m/body/entered");
                    m.Append<int>((int)body.TrackingId);
                    client.SendTo(m, targetHost, targetPort);                    
                }

                _Bodies[body.TrackingId].updateBody(body);

            }
        }

        if (sendOSC)
        {
            float sendTime = 1f / sendRate;
            if (Time.time - lastSendTime > sendTime)
            {
                sendBodiesData();
                lastSendTime = Time.time;
            }
        }
        
    }

    void sendBodiesData()
    {
        foreach(KeyValuePair<ulong,K2MBody> body in _Bodies)
        {
            sendBodyData(body.Value);
        }
    }

    void sendBodyData(K2MBody body)
    {
        OSCMessage m;
        m = new OSCMessage("/k2m/body/update");
        m.Append<int>(body.trackingId);
        m.Append<int>(body.leftHandState);
        m.Append<int>(body.rightHandState);
        m.Append<int>(sendExtendedSkeleton?1:0);
        client.SendTo(m, targetHost, targetPort);

        for(int i=0;i<body.numJoints;i++)
        {

            if(!sendExtendedSkeleton)
            {
                bool isExtended = false;
                for(int e=0;e<extendedSkeletonIndices.Length;e++)
                {
                    if(extendedSkeletonIndices[e] == i)
                    {
                        isExtended = true;
                    }
                }
                if (!isExtended) continue;
            }

            Transform jt = body.joints[i];

            m = new OSCMessage("/k2m/joint");
            m.Append<int>(body.trackingId);
            m.Append<int>(i);
            m.Append<float>(jt.position.x);
            m.Append<float>(jt.position.y);
            m.Append<float>(jt.position.z);
            client.SendTo(m, targetHost, targetPort);
        }
    }

    void OnPlayerPrefsUpdated(string playerPrefKey)
    {
        targetHost = PlayerPrefs_AM.GetString(playerPrefKey + "TargetIP", "127.0.0.1");
        targetPort = PlayerPrefs_AM.GetInt(playerPrefKey + "TargetPort", 9090);

        client = new OSCClient(System.Net.IPAddress.Parse(targetHost), targetPort, false);
    }

    public void SendPing()
    {
        OSCMessage m;
        m = new OSCMessage("/k2m/ping");
        client.SendTo(m, targetHost, targetPort);
    }
}
