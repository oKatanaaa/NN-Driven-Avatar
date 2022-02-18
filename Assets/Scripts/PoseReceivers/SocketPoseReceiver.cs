using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class SocketPoseReceiver : PoseReceiver
{
    private Socket socket;

    private Byte[] recvBuff = new Byte[2048];
    // Start is called before the first frame update
    private void Start()
    {
        IPHostEntry hostEntry = Dns.GetHostEntry("127.0.0.1");
        IPEndPoint ipe = new IPEndPoint(hostEntry.AddressList[0], 8000);
        socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(ipe);
    }

    public override JSONpose GetPose()
    {
        if (!performConnect)
            return null;

        int bytes = socket.Receive(recvBuff, recvBuff.Length, 0);

        string responseString = Encoding.ASCII.GetString(recvBuff, 0, bytes);
        //Debug.Log(responseString);
        System.Diagnostics.Stopwatch stopwatch3 = new System.Diagnostics.Stopwatch();
        stopwatch3.Start();
        JSONpose currPose = JsonUtility.FromJson<JSONpose>(responseString);
        stopwatch3.Stop();

        long time3 = stopwatch3.ElapsedMilliseconds;
        return currPose;
    }
}
