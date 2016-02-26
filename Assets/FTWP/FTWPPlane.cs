using UnityEngine;
using System.Collections;
using UnityOSC;
using System.Collections.Generic;

public class FTWPPlane : MonoBehaviour {


    string targetHost = "127.0.0.1";
    string targetPort = "9082";

    OSCClient client;

    [Range(0, 1)]
    public float speed = .8f;

	// Use this for initialization
	void Start () {
        transform.rotation = Quaternion.Euler(0, -90, 90);

        client = new OSCClient(System.Net.IPAddress.Parse(targetHost), int.Parse(targetPort),false);
	}
	
	// Update is called once per frame
	void Update () {
        transform.localPosition = Vector3.Lerp(transform.localPosition,FTWPClient.planePosition,speed);

        int tPort = int.Parse(targetPort);

        OSCMessage m;
        m = new OSCMessage("/ftwp/position");
        List<object> args = new List<object>();
        args.Add(transform.localPosition.x);
        args.Add(transform.localPosition.y);
        args.Add(transform.localPosition.z);
        client.SendTo(m, targetHost, tPort);
    }
}
