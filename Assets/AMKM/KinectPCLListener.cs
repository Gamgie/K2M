using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

public class KinectPCLListener : MonoBehaviour
{

    private UdpClient _udpClient;
    public int localPort;
    private Thread _receiverThread;

    private List<KPCL> pclList;

    bool nextIsNew = false;

    bool filling = false;

    void Start()
    {
        pclList = new List<KPCL>();
       // Close();
        Connect();
    }

    void Update()
    {
        if (filling) return;

        //Debug.Log("PCL List length " + pclList.Count);
        for(int i=0;i<pclList.Count;i++)
        {

            KPCL pcl = pclList[i];
            int numPoints = pcl.points.Length;
            for (int j = 0; j < numPoints; j++)
            {
                Vector3 tPoint = transform.TransformPoint(new Vector3(pcl.points[j].x, pcl.points[j].y, pcl.points[j].z));
                Debug.DrawLine(tPoint, tPoint + Vector3.forward * .01f,Color.yellow);
            }
        }
    }

    void OnDestroy()
    {
        Close();
    }


    public void Connect()
    {
        Debug.Log("Connect");
        if (this._udpClient != null) Close();

        try
        {
            _udpClient = new UdpClient(localPort);
            _receiverThread = new Thread(new ThreadStart(this.ReceivePool));
            _receiverThread.Start();
        }
        catch (Exception e)
        {
            throw e;
        }
    }
    

    public void Close()
    {
        Debug.Log("Close");
        try
        {
            if (_receiverThread != null) _receiverThread.Abort();
            _receiverThread = null;
        
            _udpClient.Close();
            _udpClient = null;
        }catch(Exception e)
        {
            Debug.LogWarning("Error");
        }
    }


    
    private void Receive()
    {
        IPEndPoint ip = null;

        try
        {
            byte[] bytes = _udpClient.Receive(ref ip);

            Debug.Log("Received " + bytes.Length + " bytes");
            ReceiveInfo(bytes);
        }
        catch
        {
            throw new Exception(String.Format("Can't create server at port {0}", localPort));
        }
    }


    void ReceiveInfo(byte[] dataStream)
    {


        filling = true;
        MemoryStream stream = new MemoryStream(dataStream);
        stream.Position = 0;
        BinaryFormatter bf = new BinaryFormatter();
        KPCL pcl = (KPCL)bf.Deserialize(stream);
        if(pcl.isFirst)
        {
            pclList.Clear();
        }
        pclList.Add(pcl);
        filling = false;
        // now you can use the recieved data however you need to!
        
    }

    /// <summary>
    /// Thread pool that receives upcoming messages.
    /// </summary>
    private void ReceivePool()
    {
        while (true)
        {
            Receive();
            //                Thread.Sleep(1);
        }
    }
}

