using UnityEngine;
using System.Collections;
using UnityOSC;

public class FTWPClient : MonoBehaviour {


    OSCServer server;
    public static Vector3 planePosition;
    public float scale = 1;

	// Use this for initialization
	void Start () {
        server = new OSCServer(9080);
        server.PacketReceivedEvent += packetReceived;
	}
	
	// Update is called once per frame
	void Update () {
        server.Update();
	}

    void packetReceived(OSCPacket p)
    {
        OSCMessage m = (OSCMessage)p;
        
        if(m.Address == "/ftwp/position")
        {
            planePosition = new Vector3(-(float)m.Data[0], (float)m.Data[1], (float)m.Data[2]) * scale;
        }
    }

    void OnDestroy()
    {
        server.Close();
    }
}
