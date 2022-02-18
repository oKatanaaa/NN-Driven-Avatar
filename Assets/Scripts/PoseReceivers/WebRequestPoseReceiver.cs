using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class WebRequestPoseReceiver : PoseReceiver
{

    public override JSONpose GetPose()
    {
        if (!performConnect)
            return null;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8000");
        request.Proxy = null;
        request.ServicePoint.Expect100Continue = false;
        request.ServicePoint.UseNagleAlgorithm = false;
        request.Method = "GET";
        WebResponse response = request.GetResponse();
        string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        JSONpose currPose = JsonUtility.FromJson<JSONpose>(responseString);
        return currPose;
    }
}
