using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using UnityOSC;

[RequireComponent(typeof(BodySourceManager))]
public class K2S : MonoBehaviour
{
    public GameObject bodyPrefab;

    bool sendOSC;
    public string targetHost = "127.0.0.1";
    public string targetPort = "9090";

    private Dictionary<ulong, K2SBody> _Bodies = new Dictionary<ulong, K2SBody>();
    private BodySourceManager _BodyManager;

    OSCClient client;

    void Start()
    {
        _BodyManager = GetComponent<BodySourceManager>();

        client = new OSCClient(System.Net.IPAddress.Parse(targetHost),int.Parse(targetPort), false);
    }

    void Update()
    {

        int tPort = int.Parse(targetPort);

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
                m = new OSCMessage("/k2s/body/left");
                List<object> args = new List<object>();
                args.Add(trackingId);
                client.SendTo(m, targetHost, tPort);
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
                    
                    _Bodies[body.TrackingId] = GameObject.Instantiate(bodyPrefab).GetComponent<K2SBody>();
                    _Bodies[body.TrackingId].transform.parent = transform;
                    
                    OSCMessage m;
                    m = new OSCMessage("/k2s/body/entered");
                    List<object> args = new List<object>();
                    args.Add(body.TrackingId);
                    client.SendTo(m, targetHost, tPort);
                }

                updateBody(body);
            }
        }
        
    }

    void updateBody(Kinect.Body body)
    {
        int tPort = int.Parse(targetPort);

        K2SBody b = _Bodies[body.TrackingId];
        b.updateBody(body);

        OSCMessage m;
        m = new OSCMessage("/k2s/body/update");
        List<object> args = new List<object>();
        args.Add(body.TrackingId);
        args.Add((int)body.HandLeftState);
        args.Add((int)body.HandRightState);
        client.SendTo(m, targetHost, tPort);

        for(int i=0;i<b.numJoints;i++)
        {
            Transform jt = b.joints[i];

            m = new OSCMessage("/k2s/joint");
            args = new List<object>();
            args.Add(body.TrackingId);
            args.Add(jt.localPosition.x);
            args.Add(jt.localPosition.y);
            args.Add(jt.localPosition.z);
            client.SendTo(m, targetHost, tPort);
        }
    }
}
