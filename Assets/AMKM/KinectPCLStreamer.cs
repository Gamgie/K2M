using UnityEngine;
using System.Collections;
//using UnityEngine.Networking;

//using WebSocketSharp;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;


using System.Net.Sockets;

public class KinectPCLStreamer : MonoBehaviour {


    public string targetHost;
    public string targetPort;

    UdpClient client;
    
    [Range(0,30)]
    public int sendRate;
    float lastSendTime = 0;
    public bool sendAlways;


    int curLineIndex;

    //WebSocket ws;

   // NetworkView view;
    //BinaryFormatter formatter;

    // Use this for initialization
    void Start() {
        /*
        ws = new WebSocket("ws://127.0.0.1/pcl"))
        
        ws.OnMessage += (sender, e) =>
            Debug.Log("Websocket data: " + e.Data);
        */

        // view.SendMessage()

        // formatter = new BinaryFormatter();

        client = new UdpClient();
        
        
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            sendPoints();
        }

        if (sendAlways)
        {
            if (Time.time - lastSendTime > 1f/sendRate)
            {
                sendPoints();
                lastSendTime = Time.time;
            }
        }
        

    }

    void OnDestroy()
    {
        //NetworkServer.Shutdown();
        if(client != null)
            client.Close();
    }

    void OnServerInitialized()
    {
        Debug.Log("Server Initializied");
    }


    void sendPoints()
    {
        curLineIndex = 0;
        loopSend();
        
    }

    void loopSend()
    {
        if (curLineIndex >= KinectCalib.instance.pcl.Length) return;

        byte[] data = SerializeObject<KPCL>(KinectCalib.instance.pcl[curLineIndex]);
        client.Send(data, data.Length, targetHost, int.Parse(targetPort));

        curLineIndex++;
        Debug.Log("send !");
        Invoke("loopSend", .01f);
    }

    byte[] SerializeObject<_T>(_T objectToSerialize)
    //same as above, but should technically work anyway
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream memStr = new MemoryStream();
        bf.Serialize(memStr, objectToSerialize);
        memStr.Position = 0;
        //return "";
        return memStr.ToArray();
    }


}
