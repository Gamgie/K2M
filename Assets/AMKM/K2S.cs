using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

[RequireComponent(typeof(BodySourceManager))]
public class K2S : MonoBehaviour
{
    public GameObject bodyPrefab;

    private Dictionary<ulong, K2SBody> _Bodies = new Dictionary<ulong, K2SBody>();
    private BodySourceManager _BodyManager;

    

    void Start()
    {
        _BodyManager = GetComponent<BodySourceManager>();
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
                    //_Bodies[body.TrackingId].transform.localPosition = Vector3.zero;
                    //_Bodies[body.TrackingId].transform.localRotation = Quaternion.identity;
                }

                _Bodies[body.TrackingId].updateBody(body);
            }
        }
    }
}
