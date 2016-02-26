using UnityEngine;
using System.Collections;
using UnityOSC;
using System.Collections.Generic;

public class FTWPPlane : MonoBehaviour {


    public string targetHost = "127.0.0.1";
    public string targetPort = "9082";

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
        m.Append<float>(transform.position.x);
        m.Append<float>(transform.position.y);
        m.Append<float>(transform.position.z);
        client.SendTo(m, targetHost, tPort);
    }
}
