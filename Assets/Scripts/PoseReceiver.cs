using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public abstract class PoseReceiver : MonoBehaviour
{

    public bool performConnect = true;

    public abstract JSONpose GetPose();
}
