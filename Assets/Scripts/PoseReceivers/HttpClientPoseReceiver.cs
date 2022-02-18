using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class HttpClientPoseReceiver : PoseReceiver
{
    private HttpClient httpClient;
    // Start is called before the first frame update
    private void Start()
    {
        httpClient = new HttpClient();
    }

    public override JSONpose GetPose()
    {
        if (!performConnect)
            return null;

        Task<HttpResponseMessage> responseTask = httpClient.GetAsync("http://localhost:8000");
        responseTask.Wait();
        HttpResponseMessage response = responseTask.Result;


        System.Diagnostics.Stopwatch stopwatch2 = new System.Diagnostics.Stopwatch();
        stopwatch2.Start();
        Task<string> responseStringTask = response.Content.ReadAsStringAsync();
        responseStringTask.Wait();
        string responseString = responseStringTask.Result;
        stopwatch2.Stop();

        long time2 = stopwatch2.ElapsedMilliseconds;

        //Debug.Log(responseString);
        System.Diagnostics.Stopwatch stopwatch3 = new System.Diagnostics.Stopwatch();
        stopwatch3.Start();
        JSONpose currPose = JsonUtility.FromJson<JSONpose>(responseString);
        stopwatch3.Stop();

        long time3 = stopwatch3.ElapsedMilliseconds;
        return currPose;
    }
}
