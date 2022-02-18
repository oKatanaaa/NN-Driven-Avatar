using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using OpenCvSharp;
using System;
using System.IO;    // For StreamReader
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text;

[Serializable]
public class JSONpose
{
    public List<HumanJSON> humans;
    public float time;
}

[Serializable]
public class HumanJSON
{
    public int human_id;
    public PointsJSON human_2d;
    public PointsJSON human_3d;
    public PoseInfo poseInfo;
}

[Serializable]
public class PoseInfo
{
    public string human_pose_name;
    public float human_pose_confidence;
}

[Serializable]
public class PointsJSON
{
    public float[] p0;
    public float[] p1;
    public float[] p2;
    public float[] p3;
    public float[] p4;
    public float[] p5;
    public float[] p6;
    public float[] p7;
    public float[] p8;
    public float[] p9;
    public float[] p10;
    public float[] p11;
    public float[] p12;
    public float[] p13;
    public float[] p14;
    public float[] p15;
    public float[] p16;
    public float[] p17;
    public float[] p18;
    public float[] p19;
    public float[] p20;
    public float[] p21;
    public float[] p22;
}

public struct dataJSON
{

    public Vector4 Center;

    public Vector4 Chest;

    public Vector4 Neck;

    public Vector4 Head;

    public Vector4 ShoulderL;

    public Vector4 ShoulderR;

    public Vector4 ElbL;

    public Vector4 ElbR;

    public Vector4 HandL;

    public Vector4 HandR;

    public Vector4 LegL;

    public Vector4 LegR;

    public Vector4 KneeL;

    public Vector4 KneeR;

    public Vector4 FootL;

    public Vector4 FootR;

    public Vector4 FootFingerL;

    public Vector4 FootFingerR;

    public Vector4 ThumbL;

    public Vector4 ThumbR;

    public Vector4 SmallL;

    public Vector4 SmallR;

    public Vector4 Waist;

}

public class NNTransformInstance : MonoBehaviour
{
    // 3D joint skeleton

    private List<dataJSON> humans = new List<dataJSON>();
    private Vector3 spine;
    private Vector3 spine1;
    private Vector3 spine2;
    private Vector3 head;
    private Vector3 shdL;
    private Vector3 shdR;
    private Vector3 elbL;
    private Vector3 elbR;
    private Vector3 handL;
    private Vector3 handR;
    private Vector3 hipL;
    private Vector3 hipR;
    private Vector3 legL;
    private Vector3 legR;
    private Vector3 footL;
    private Vector3 footR;
    private Vector3 N;
    private Quaternion temp;

    public PoseReceiver poseReceiver;
    public bool performSkeletonTransform = true;


    enum JointIndices
    {
        Invalid = -1,
        Hips = 0, // root 
        LeftUpLeg = 1, // parent: Hips [0]
        LeftLeg = 2, // parent: LeftUpLeg [1]
        LeftFoot = 3, // parent: LeftLeg [2]
        LeftToeBase = 4, // parent: LeftFoot [3]
        LeftToe_End = 5, // parent: LeftToeBase [4]
        RightUpLeg = 6, // parent: Hips [0]
        RightLeg = 7, // parent: RightUpLeg [6]
        RightFoot = 8, // parent: RightLeg [7]
        RightToeBase = 9, // parent: RightFoot [8]
        RightToe_End = 10, // parent: RightToeBase [9]
        Spine = 11, // parent: Hips [0]
        Spine1 = 12, // parent: Spine1 [11]
        Spine2 = 13, // parent: Spine2 [12]
        LeftShoulder = 14, // parent: Spine2 [13]
        LeftArm = 15, // parent: LeftShoulder [14]
        LeftForeArm = 16, // parent: LeftArm [15]
        LeftHand = 17, // parent: LeftForearm [16]
        LeftHandIndex1 = 18, // parent: LeftHand [17]
        LeftHandIndex2 = 19, // parent: LeftHandIndex1 [24]
        LeftHandIndex3 = 20, // parent: LeftHandIndex2 [25]
        LeftHandIndex4 = 21, // parent: LeftHandIndex3 [26]
        LeftHandMiddle1 = 22, // parent: LeftHand [17]
        LeftHandMiddle2 = 23, // parent: LeftHandMiddle1 [22]
        LeftHandMiddle3 = 24, // parent: LeftHandMiddle2 [23]
        LeftHandMiddle4 = 25, // parent: LeftHandMiddle3 [24]
        LeftHandPinky1 = 26, // parent: LeftHand [17]
        LeftHandPinky2 = 27, // parent: LeftHandPinky1 [26]
        LeftHandPinky3 = 28, // parent: LeftHandPinky2 [27]
        LeftHandPinky4 = 29, // parent: LeftHandPinky3 [28]
        LeftHandRing1 = 30, // parent: LeftHand [17]
        LeftHandRing2 = 31, // parent: LeftHandRing1 [30]
        LeftHandRing3 = 32, // parent: LeftHandRing2 [31]
        LeftHandRing4 = 33, // parent: LeftHandRing3 [32]
        LeftHandThumb1 = 34, // parent: LeftHand [17]
        LeftHandThumb2 = 35, // parent: LeftHandThumb1 [34]
        LeftHandThumb3 = 36, // parent: LeftHandThumb2 [35]
        LeftHandThumb4 = 37, // parent: LeftHandThumb3 [36]
        Neck = 38, // parent: Spine2 [13]
        Head = 39, // parent: Neck [38]
        HeadTop_End = 40, // parent: Head [39]
        RightShoulder = 41, // parent: Spine2 [13]
        RightArm = 42, // parent: RightShoulder [41]
        RightForeArm = 43, // parent: RightArm [42]
        RightHand = 44, // parent: RightForearm [43]
        RightHandIndex1 = 45, // parent: RightHand [44]
        RightHandIndex2 = 46, // parent: RightHandIndex1 [45]
        RightHandIndex3 = 47, // parent: RightHandIndex2 [46]
        RightHandIndex4 = 48, // parent: RightHandIndex3 [47]
        RightHandMiddle1 = 49, // parent: RightHand [44]
        RightHandMiddle2 = 50, // parent: RightHandMiddle1 [49]
        RightHandMiddle3 = 51, // parent: RightHandMiddle2 [50]
        RightHandMiddle4 = 52, // parent: RightHandMiddle3 [51]
        RightHandPinky1 = 53, // parent: RightHand [44]
        RightHandPinky2 = 54, // parent: RightHandPinky1 [53]
        RightHandPinky3 = 55, // parent: RightHandPinky2 [54]
        RightHandPinky4 = 56, // parent: RightHandPinky3 [55]
        RightHandRing1 = 57, // parent: RightHand [44]
        RightHandRing2 = 58, // parent: RightHandRing1 [57]
        RightHandRing3 = 59, // parent: RightHandRing2 [58]
        RightHandRing4 = 60, // parent: RightHandRing3 [59]
        RightHandThumb1 = 61, // parent: RightHand [44]
        RightHandThumb2 = 62, // parent: RightHandThumb1 [61]
        RightHandThumb3 = 63, // parent: RightHandThumb2 [62]
        RightHandThumb4 = 64, // parent: RightHandThumb3 [63]
    }
    const int k_NumSkeletonJoints = 65;

    [SerializeField]
    [Tooltip("The root bone of the skeleton.")]
    Transform m_SkeletonRoot;

    Transform[] m_BoneMapping = new Transform[k_NumSkeletonJoints];

    public void InitializeSkeletonJoints()
    {
        // Walk through all the child joints in the skeleton and
        // store the skeleton joints at the corresponding index in the m_BoneMapping array.
        // This assumes that the bones in the skeleton are named as per the
        // JointIndices enum above.
        Queue<Transform> nodes = new Queue<Transform>();
        nodes.Enqueue(m_SkeletonRoot);
        while (nodes.Count > 0)
        {
            Transform next = nodes.Dequeue();
            for (int i = 0; i < next.childCount; ++i)
            {
                nodes.Enqueue(next.GetChild(i));
            }
            ProcessJoint(next);
        }
    }

    private void rotateEyeVector(ref Vector3 eye, float angle)
    {
        eye.x = eye.x * Mathf.Cos(angle) + eye.z * Mathf.Sin(angle);
        eye.z = eye.z * Mathf.Cos(angle) + eye.x * Mathf.Sin(angle);
    }

    public void ApplyBodyPose(dataJSON human)
    {
        Quaternion temp2 = new Quaternion();
        Vector3 WaistP = (Vector3)human.Waist;
        Vector3 CenterP = (Vector3)human.Center;
        Vector3 LegLP = (Vector3)human.LegL;
        Vector3 LegRP = (Vector3)human.LegR;
        Vector3 KneeLP = (Vector3)human.KneeL;
        Vector3 KneeRP = (Vector3)human.KneeR;
        Vector3 FootLP = (Vector3)human.FootL;
        Vector3 FootRP = (Vector3)human.FootR;
        Vector3 HandLP = (Vector3)human.HandL;
        Vector3 HandRP = (Vector3)human.HandR;
        Vector3 ElbLP = (Vector3)human.ElbL;
        Vector3 ElbRP = (Vector3)human.ElbR;
        Vector3 ShldLP = (Vector3)human.ShoulderL;
        Vector3 ShldRP = (Vector3)human.ShoulderR;
        Vector3 NeckP = (Vector3)human.Neck;
        Vector3 HeadP = (Vector3)human.Head;
        Vector3 ChestP = (Vector3)human.Chest;
        Vector3 LegL = LegLP - KneeLP; LegL = LegL.normalized;
        Vector3 LegR = LegRP - KneeRP; LegR = LegR.normalized;
        Vector3 spine = CenterP - WaistP; spine = spine.normalized;
        Vector3 hipL = WaistP - LegLP; hipL = hipL.normalized;
        Vector3 KneeL = FootLP - KneeLP; KneeL = KneeL.normalized;
        Vector3 KneeR = FootRP - KneeRP; KneeR = KneeR.normalized;
        Vector3 HandL = ShldLP - ElbLP; HandL = HandL.normalized;
        Vector3 HandR = ShldRP - ElbRP; HandR = HandR.normalized;
        Vector3 ElbL = HandLP - ElbLP; ElbL = ElbL.normalized;
        Vector3 ElbR = HandRP - ElbRP; ElbR = ElbR.normalized;
        Vector3 Neck = NeckP - ChestP; Neck = Neck.normalized;
        Vector3 ShldL = ShldLP - ChestP; ShldL = ShldL.normalized;
        Vector3 ShldR = ShldRP - ChestP; ShldR = ShldR.normalized;
        Vector3 Head = HeadP - NeckP; Head = Head.normalized;
        Vector3 eyeX = new Vector3(1.0f, 0.0f, 0.0f);
        rotateEyeVector(ref eyeX, -16.0f);
        float[] angles = new float[11];
        angles[0] = (Vector3.Angle(LegL, spine) - 180) * Mathf.Deg2Rad / 2;
        angles[1] = (Vector3.Angle(LegR, spine) - 180) * Mathf.Deg2Rad / 2;
        angles[2] = (Vector3.Angle(LegL, KneeL) - 180) * Mathf.Deg2Rad / 2;
        angles[3] = (Vector3.Angle(LegR, KneeR) - 180) * Mathf.Deg2Rad / 2;
        /*angles[4] = (Vector3.Angle(spine, HandL) - 90) * Mathf.Deg2Rad;
        angles[5] = (Vector3.Angle(spine, HandR) - 90) * Mathf.Deg2Rad;*/
        angles[4] = (Vector3.Angle(ShldL, HandL) - 180) * Mathf.Deg2Rad;
        angles[5] = (Vector3.Angle(ShldR, HandR) - 180) * Mathf.Deg2Rad;
        angles[6] = (Vector3.Angle(ElbL, HandL) - 150) * Mathf.Deg2Rad / 2;
        angles[7] = (Vector3.Angle(ElbR, HandR) - 150) * Mathf.Deg2Rad / 2;
        angles[8] = (Vector3.Angle(spine, Neck) - 51) * Mathf.Deg2Rad / 2;
        angles[9] = Vector3.Angle(eyeX, hipL) * Mathf.Deg2Rad / 2;
        angles[10] = (Vector3.Angle(Head, Neck) - 47) * Mathf.Deg2Rad / 2;
        //-------------------------------------ноги----------------------------------------------------
        //с правой ногой все правильно
        Vector3 vectOfCurrBone;
        N = new Vector3(LegL.x, LegL.y, LegL.z);
        vectOfCurrBone = m_BoneMapping[2].transform.position - m_BoneMapping[1].transform.position; //нога куклы
        temp2 = makeRotation(vectOfCurrBone, N, 1); //legL
        temp2.Set(temp2.x, 0.0f, temp2.z, temp2.w);
        m_BoneMapping[1].transform.localRotation = temp2;
        N = new Vector3(LegR.x, LegR.y, LegR.z);
        vectOfCurrBone = m_BoneMapping[7].transform.position - m_BoneMapping[6].transform.position; //нога куклы
        temp2 = makeRotation(vectOfCurrBone, N, 6); //legR
        temp2.Set(temp2.x, 0.0f, temp2.z, temp2.w);
        m_BoneMapping[6].transform.localRotation = temp2;
        N = new Vector3(KneeL.x, KneeL.y, KneeL.z);
        vectOfCurrBone = m_BoneMapping[3].transform.position - m_BoneMapping[2].transform.position; //голень куклы
        temp2 = makeRotation(vectOfCurrBone, N, 2); //footL
        //temp2.Set(temp2.x, (-1)*temp2.y, temp2.z, temp2.w);
        m_BoneMapping[2].transform.localRotation = temp2;
        N = new Vector3(KneeR.x, KneeR.y, KneeR.z);
        vectOfCurrBone = m_BoneMapping[8].transform.position - m_BoneMapping[7].transform.position; //голень куклы
        temp2 = makeRotation(vectOfCurrBone, N, 7); //footR
        //temp2.Set(temp2.x, (-1)*temp2.y, temp2.z, temp2.w);
        m_BoneMapping[7].transform.localRotation = temp2;
        //--------------------------------------руки--------------------------------------------------
        N = Vector3.Cross(ShldL, HandL).normalized;
        temp = QuaternionFromMatrix(constructRotationMatrixOneAngle(angles[4], N));
        temp2.Set(temp.z, temp.x, (-1) * temp.y, temp.w);
        m_BoneMapping[15].transform.localRotation = temp2;
        N = Vector3.Cross(ShldR, HandR).normalized;
        temp = QuaternionFromMatrix(constructRotationMatrixOneAngle(angles[5], N));
        temp2.Set((-1) * temp.z, (-1) * temp.x, (-1) * temp.y, temp.w);
        m_BoneMapping[42].transform.localRotation = temp2;
        /*N = Vector3.Cross(spine, HandL);    N = N.normalized;    temp.Set((-1)*N.x * Mathf.Sin(angles[4]), N.y * Mathf.Sin(angles[4]), (-1)*N.z * Mathf.Sin(angles[4]), Mathf.Cos(angles[4]));
        m_BoneMapping[15].transform.localRotation = temp; //HandL
        N = Vector3.Cross(spine, HandR);    N = N.normalized;    temp.Set((-1)*N.x * Mathf.Sin(angles[5]), N.y * Mathf.Sin(angles[5]), (-1)*N.z * Mathf.Sin(angles[5]), Mathf.Cos(angles[5]));
        m_BoneMapping[42].transform.localRotation = temp;*/ //HandR
        /*N = Vector3.Cross(ShldL, HandL);    N = N.normalized;    temp.Set(N.x * Mathf.Sin(angles[4]), N.y * Mathf.Sin(angles[4]), N.z * Mathf.Sin(angles[4]), Mathf.Cos(angles[4]));
        m_BoneMapping[15].transform.localRotation = temp; //HandL
        N = Vector3.Cross(ShldR, HandR);    N = N.normalized;    temp.Set(N.x * Mathf.Sin(angles[5]), N.y * Mathf.Sin(angles[5]), N.z * Mathf.Sin(angles[5]), Mathf.Cos(angles[5]));
        m_BoneMapping[42].transform.localRotation = temp;*/ //HandR
        /*N = Vector3.Cross(ElbL, HandL);    N = N.normalized;    temp.Set(N.x * Mathf.Sin(angles[6]), N.y * Mathf.Sin(angles[6]), N.z * Mathf.Sin(angles[6]), Mathf.Cos(angles[6]));
        m_BoneMapping[16].transform.localRotation = temp; //ElbL
        N = Vector3.Cross(ElbR, HandR);    N = N.normalized;    temp.Set(N.x * Mathf.Sin(angles[7]), N.y * Mathf.Sin(angles[7]), N.z * Mathf.Sin(angles[7]), Mathf.Cos(angles[7]));
        m_BoneMapping[43].transform.localRotation = temp;*/ //ElbR
        N = Vector3.Cross(ElbL, HandL).normalized;
        temp = QuaternionFromMatrix(constructRotationMatrixOneAngle(angles[6], N));
        temp2.Set((-1) * temp.z, temp.x, temp.y, temp.w);
        m_BoneMapping[16].transform.localRotation = temp2;
        N = Vector3.Cross(ElbR, HandR).normalized;
        temp = QuaternionFromMatrix(constructRotationMatrixOneAngle(angles[7], N));
        temp2.Set(temp.z, temp.x, temp.y, temp.w);
        m_BoneMapping[43].transform.localRotation = temp2;
        //-------------------------------------голова--------------------------------------------------
        N = Vector3.Cross(spine, Neck).normalized; temp.Set(N.x * Mathf.Sin(angles[8]), (-1) * Vector3.up.y * Mathf.Sin(angles[10]), N.z * Mathf.Sin(angles[8]), Mathf.Cos(angles[8]));
        N = Vector3.up; temp2.Set(N.x * Mathf.Sin(angles[10]), N.y * Mathf.Sin(angles[10]), N.z * Mathf.Sin(angles[10]), Mathf.Cos(angles[10]));
        m_BoneMapping[38].transform.localRotation = temp2 * temp; //Neck
        //-------------------------------------бедра--------------------------------------------------
        N = Vector3.Cross(eyeX, hipL); N = N.normalized; temp.Set(0.0f, N.y * Mathf.Sin(angles[9]), 0.0f, Mathf.Cos(angles[9]));
        m_BoneMapping[0].transform.localRotation = temp; //hipL
    }

    public void ApplyBodyPose2(dataJSON human)
    {
        Quaternion temp2 = new Quaternion();
        Vector3 WaistP = (Vector3)human.Waist;
        Vector3 CenterP = (Vector3)human.Center;
        Vector3 LegLP = (Vector3)human.LegL;
        Vector3 LegRP = (Vector3)human.LegR;
        Vector3 KneeLP = (Vector3)human.KneeL;
        Vector3 KneeRP = (Vector3)human.KneeR;
        Vector3 FootLP = (Vector3)human.FootL;
        Vector3 FootRP = (Vector3)human.FootR;
        Vector3 HandLP = (Vector3)human.HandL;
        Vector3 HandRP = (Vector3)human.HandR;
        Vector3 ElbLP = (Vector3)human.ElbL;
        Vector3 ElbRP = (Vector3)human.ElbR;
        Vector3 ShldLP = (Vector3)human.ShoulderL;
        Vector3 ShldRP = (Vector3)human.ShoulderR;
        Vector3 NeckP = (Vector3)human.Neck;
        Vector3 HeadP = (Vector3)human.Head;
        Vector3 ChestP = (Vector3)human.Chest;
        Vector3 LegL = LegLP - KneeLP; LegL = LegL.normalized;
        Vector3 LegR = LegRP - KneeRP; LegR = LegR.normalized;
        Vector3 spine = CenterP - WaistP; spine = spine.normalized;
        Vector3 hipL = WaistP - LegLP; hipL = hipL.normalized;
        Vector3 KneeL = FootLP - KneeLP; KneeL = KneeL.normalized;
        Vector3 KneeR = FootRP - KneeRP; KneeR = KneeR.normalized;
        Vector3 HandL = ShldLP - ElbLP; HandL = HandL.normalized;
        Vector3 HandR = ShldRP - ElbRP; HandR = HandR.normalized;
        Vector3 ElbL = HandLP - ElbLP; ElbL = ElbL.normalized;
        Vector3 ElbR = HandRP - ElbRP; ElbR = ElbR.normalized;
        Vector3 Neck = NeckP - ChestP; Neck = Neck.normalized;
        Vector3 ShldL = ShldLP - ChestP; ShldL = ShldL.normalized;
        Vector3 ShldR = ShldRP - ChestP; ShldR = ShldR.normalized;
        Vector3 Head = HeadP - NeckP; Head = Head.normalized;
        Vector3 eyeX = new Vector3(1.0f, 0.0f, 0.0f);
        rotateEyeVector(ref eyeX, -16.0f);
        float[] angles = new float[11];
        angles[0] = (Vector3.Angle(LegL, spine) - 180) * Mathf.Deg2Rad / 2;
        angles[1] = (Vector3.Angle(LegR, spine) - 180) * Mathf.Deg2Rad / 2;
        angles[2] = (Vector3.Angle(LegL, KneeL) - 180) * Mathf.Deg2Rad / 2;
        angles[3] = (Vector3.Angle(LegR, KneeR) - 180) * Mathf.Deg2Rad / 2;
        /*angles[4] = (Vector3.Angle(spine, HandL) - 90) * Mathf.Deg2Rad;
        angles[5] = (Vector3.Angle(spine, HandR) - 90) * Mathf.Deg2Rad;*/
        angles[4] = (Vector3.Angle(ShldL, HandL) - 180) * Mathf.Deg2Rad;
        angles[5] = (Vector3.Angle(ShldR, HandR) - 180) * Mathf.Deg2Rad;
        angles[6] = (Vector3.Angle(ElbL, HandL) - 150) * Mathf.Deg2Rad / 2;
        angles[7] = (Vector3.Angle(ElbR, HandR) - 150) * Mathf.Deg2Rad / 2;
        angles[8] = (Vector3.Angle(spine, Neck) - 51) * Mathf.Deg2Rad / 2;
        angles[9] = Vector3.Angle(eyeX, hipL) * Mathf.Deg2Rad / 2;
        angles[10] = (Vector3.Angle(Head, Neck) - 47) * Mathf.Deg2Rad / 2;
        //-------------------------------------голова--------------------------------------------------
        N = Vector3.Cross(spine, Neck).normalized; temp.Set(N.x * Mathf.Sin(angles[8]), (-1) * Vector3.up.y * Mathf.Sin(angles[10]), N.z * Mathf.Sin(angles[8]), Mathf.Cos(angles[8]));
        N = Vector3.up; temp2.Set(N.x * Mathf.Sin(angles[10]), N.y * Mathf.Sin(angles[10]), N.z * Mathf.Sin(angles[10]), Mathf.Cos(angles[10]));
        m_BoneMapping[38].transform.localRotation = temp2 * temp; //Neck
        //-------------------------------------бедра--------------------------------------------------
        N = Vector3.Cross(eyeX, hipL); N = N.normalized; temp.Set(0.0f, N.y * Mathf.Sin(angles[9]), 0.0f, Mathf.Cos(angles[9]));
        m_BoneMapping[0].transform.localRotation = temp; //hipL
        //-------------------------------------ноги----------------------------------------------------
        //с правой ногой все правильно
        Vector3 vectOfCurrBone;
        N = new Vector3(LegL.x, LegL.y, LegL.z);
        vectOfCurrBone = m_BoneMapping[2].transform.position - m_BoneMapping[1].transform.position; //нога куклы
        temp2 = makeRotation(vectOfCurrBone, N, 1); //legL
        temp2.Set(temp2.x, 0.0f, temp2.z, temp2.w);
        m_BoneMapping[1].transform.localRotation = temp2;
        N = new Vector3(LegR.x, LegR.y, LegR.z);
        vectOfCurrBone = m_BoneMapping[7].transform.position - m_BoneMapping[6].transform.position; //нога куклы
        temp2 = makeRotation(vectOfCurrBone, N, 6); //legR
        temp2.Set(temp2.x, 0.0f, temp2.z, temp2.w);
        m_BoneMapping[6].transform.localRotation = temp2;
        N = new Vector3(KneeL.x, KneeL.y, KneeL.z);
        vectOfCurrBone = m_BoneMapping[3].transform.position - m_BoneMapping[2].transform.position; //голень куклы
        temp2 = makeRotation(vectOfCurrBone, N, 2); //footL
        //temp2.Set(temp2.x, (-1)*temp2.y, temp2.z, temp2.w);
        m_BoneMapping[2].transform.localRotation = temp2;
        N = new Vector3(KneeR.x, KneeR.y, KneeR.z);
        vectOfCurrBone = m_BoneMapping[8].transform.position - m_BoneMapping[7].transform.position; //голень куклы
        temp2 = makeRotation(vectOfCurrBone, N, 7); //footR
        //temp2.Set(temp2.x, (-1)*temp2.y, temp2.z, temp2.w);
        m_BoneMapping[7].transform.localRotation = temp2;
        //--------------------------------------руки--------------------------------------------------
        vectOfCurrBone = m_BoneMapping[16].transform.position - m_BoneMapping[15].transform.position;
        temp = makeRotation(vectOfCurrBone, HandL, 15);
        //temp2.Set(temp.z, temp.x, (-1)*temp.y, temp.w);
        m_BoneMapping[15].transform.localRotation = temp;

        vectOfCurrBone = m_BoneMapping[43].transform.position - m_BoneMapping[42].transform.position;
        temp = makeRotation(vectOfCurrBone, HandR, 42);
        //temp2.Set((-1)*temp.z, (-1)*temp.x, (-1)*temp.y, temp.w);
        m_BoneMapping[42].transform.localRotation = temp;

        vectOfCurrBone = m_BoneMapping[17].transform.position - m_BoneMapping[16].transform.position;
        temp = makeRotation(vectOfCurrBone, ElbL, 42);
        //temp2.Set((-1)*temp.z, temp.x, temp.y, temp.w);
        m_BoneMapping[16].transform.localRotation = temp;

        vectOfCurrBone = m_BoneMapping[44].transform.position - m_BoneMapping[43].transform.position;
        temp = makeRotation(vectOfCurrBone, ElbR, 43);
        //temp2.Set(temp.z, temp.x, temp.y, temp.w);
        m_BoneMapping[43].transform.localRotation = temp;
    }

    public void ApplyBodyPose3(dataJSON human, Vector3 N_Lhand, Vector3 N_Rhand)
    {
        Quaternion temp2 = new Quaternion();
        Vector3 WaistP = (Vector3)human.Waist;
        Vector3 CenterP = (Vector3)human.Center;
        Vector3 LegLP = (Vector3)human.LegL;
        Vector3 LegRP = (Vector3)human.LegR;
        Vector3 KneeLP = (Vector3)human.KneeL;
        Vector3 KneeRP = (Vector3)human.KneeR;
        Vector3 FootLP = (Vector3)human.FootL;
        Vector3 FootRP = (Vector3)human.FootR;
        Vector3 HandLP = (Vector3)human.HandL;
        Vector3 HandRP = (Vector3)human.HandR;
        Vector3 ElbLP = (Vector3)human.ElbL;
        Vector3 ElbRP = (Vector3)human.ElbR;
        Vector3 ShldLP = (Vector3)human.ShoulderL;
        Vector3 ShldRP = (Vector3)human.ShoulderR;
        Vector3 NeckP = (Vector3)human.Neck;
        Vector3 HeadP = (Vector3)human.Head;
        Vector3 ChestP = (Vector3)human.Chest;
        Vector3 LegL = LegLP - KneeLP; LegL = LegL.normalized;
        Vector3 LegR = LegRP - KneeRP; LegR = LegR.normalized;
        Vector3 spine = CenterP - WaistP; spine = spine.normalized;
        Vector3 hipL = WaistP - LegLP; hipL = hipL.normalized;
        Vector3 KneeL = FootLP - KneeLP; KneeL = KneeL.normalized;
        Vector3 KneeR = FootRP - KneeRP; KneeR = KneeR.normalized;
        Vector3 HandL = ShldLP - ElbLP; HandL = HandL.normalized;
        Vector3 HandR = ShldRP - ElbRP; HandR = HandR.normalized;
        Vector3 ElbL = HandLP - ElbLP; ElbL = ElbL.normalized;
        Vector3 ElbR = HandRP - ElbRP; ElbR = ElbR.normalized;
        Vector3 Neck = NeckP - ChestP; Neck = Neck.normalized;
        Vector3 ShldL = ShldLP - ChestP; ShldL = ShldL.normalized;
        Vector3 ShldR = ShldRP - ChestP; ShldR = ShldR.normalized;
        Vector3 Head = HeadP - NeckP; Head = Head.normalized;
        Vector3 eyeX = new Vector3(1.0f, 0.0f, 0.0f);
        rotateEyeVector(ref eyeX, -16.0f);
        float[] angles = new float[11];
        angles[0] = (Vector3.Angle(LegL, spine) - 180) * Mathf.Deg2Rad / 2;
        angles[1] = (Vector3.Angle(LegR, spine) - 180) * Mathf.Deg2Rad / 2;
        angles[2] = (Vector3.Angle(LegL, KneeL) - 180) * Mathf.Deg2Rad / 2;
        angles[3] = (Vector3.Angle(LegR, KneeR) - 180) * Mathf.Deg2Rad / 2;
        /*angles[4] = (Vector3.Angle(spine, HandL) - 90) * Mathf.Deg2Rad;
        angles[5] = (Vector3.Angle(spine, HandR) - 90) * Mathf.Deg2Rad;*/
        angles[4] = (Vector3.Angle(ShldL, HandL) - 180) * Mathf.Deg2Rad;
        angles[5] = (Vector3.Angle(ShldR, HandR) - 180) * Mathf.Deg2Rad;
        angles[6] = (Vector3.Angle(ElbL, HandL) - 150) * Mathf.Deg2Rad / 2;
        angles[7] = (Vector3.Angle(ElbR, HandR) - 150) * Mathf.Deg2Rad / 2;
        angles[8] = (Vector3.Angle(spine, Neck) - 51) * Mathf.Deg2Rad / 2;
        angles[9] = Vector3.Angle(eyeX, hipL) * Mathf.Deg2Rad / 2;
        angles[10] = (Vector3.Angle(Head, Neck) - 47) * Mathf.Deg2Rad / 2;
        //-------------------------------------голова--------------------------------------------------
        N = Vector3.Cross(spine, Neck).normalized; temp.Set(N.x * Mathf.Sin(angles[8]), (-1) * Vector3.up.y * Mathf.Sin(angles[10]), N.z * Mathf.Sin(angles[8]), Mathf.Cos(angles[8]));
        N = Vector3.up; temp2.Set(N.x * Mathf.Sin(angles[10]), N.y * Mathf.Sin(angles[10]), N.z * Mathf.Sin(angles[10]), Mathf.Cos(angles[10]));
        m_BoneMapping[38].transform.localRotation = temp2 * temp; //Neck
        //-------------------------------------бедра--------------------------------------------------
        N = Vector3.Cross(eyeX, hipL); N = N.normalized; temp.Set(0.0f, 0.0f, N.y * Mathf.Sin(angles[9]), Mathf.Cos(angles[9]));
        m_BoneMapping[0].transform.localRotation = temp; //hipL
        //-------------------------------------ноги----------------------------------------------------
        Vector3 vectOfCurrBone;
        N = new Vector3(LegL.x, LegL.y, LegL.z);
        vectOfCurrBone = m_BoneMapping[2].transform.position - m_BoneMapping[1].transform.position; //нога куклы
        temp2 = Quaternion.LookRotation(N, (-1) * Vector3.forward); //legL
        //temp2.Set(temp2.x, 0.0f, temp2.z, temp2.w);
        m_BoneMapping[1].transform.rotation = temp2;
        N = new Vector3(LegR.x, LegR.y, LegR.z);
        vectOfCurrBone = m_BoneMapping[7].transform.position - m_BoneMapping[6].transform.position; //нога куклы
        temp2 = Quaternion.LookRotation(N, (-1) * Vector3.forward); //legR
        //temp2.Set(temp2.x, 0.0f, temp2.z, temp2.w);
        m_BoneMapping[6].transform.rotation = temp2;
        N = new Vector3((-1) * KneeL.x, (-1) * KneeL.y, (-1) * KneeL.z);
        vectOfCurrBone = m_BoneMapping[3].transform.position - m_BoneMapping[2].transform.position; //голень куклы
        //temp2 = Quaternion.LookRotation(N, Vector3.up); //footL
        temp2 = Quaternion.LookRotation(m_BoneMapping[1].transform.InverseTransformVector(N), (-1) * Vector3.forward);
        m_BoneMapping[2].transform.localRotation = temp2;
        N = new Vector3((-1) * KneeR.x, (-1) * KneeR.y, (-1) * KneeR.z);
        vectOfCurrBone = m_BoneMapping[8].transform.position - m_BoneMapping[7].transform.position; //голень куклы
        //temp2 = Quaternion.LookRotation(N, Vector3.up); //footR
        temp2 = Quaternion.LookRotation(m_BoneMapping[6].transform.InverseTransformVector(N), (-1) * Vector3.forward);
        m_BoneMapping[7].transform.localRotation = temp2;
        //--------------------------------------руки--------------------------------------------------
        vectOfCurrBone = m_BoneMapping[16].transform.position - m_BoneMapping[15].transform.position;
        N = new Vector3(HandL.x, HandL.y, HandL.z);
        temp = Quaternion.LookRotation(N, Vector3.up);
        //temp2.Set(temp.z, temp.x, (-1)*temp.y, temp.w);
        //temp2.Set(temp.x, temp.y, (-1)*temp.z, temp.w);
        m_BoneMapping[15].transform.rotation = temp;

        vectOfCurrBone = m_BoneMapping[17].transform.position - m_BoneMapping[16].transform.position;
        N = new Vector3((-1) * ElbL.x, (-1) * ElbL.y, (-1) * ElbL.z);
        //temp = Quaternion.LookRotation(N, Vector3.up);
        temp = Quaternion.LookRotation(m_BoneMapping[15].transform.InverseTransformVector(N), (-1) * m_BoneMapping[15].transform.InverseTransformVector(N_Lhand));
        //temp2.Set((-1)*temp.z, temp.x, temp.y, temp.w);
        m_BoneMapping[16].transform.localRotation = temp;

        vectOfCurrBone = m_BoneMapping[43].transform.position - m_BoneMapping[42].transform.position;
        N = new Vector3(HandR.x, HandR.y, HandR.z);
        temp = Quaternion.LookRotation(N, Vector3.up);
        //temp2.Set((-1)*temp.z, (-1)*temp.x, (-1)*temp.y, temp.w);
        //temp2.Set(temp.x, temp.y, (-1)*temp.z, temp.w);
        m_BoneMapping[42].transform.rotation = temp;

        vectOfCurrBone = m_BoneMapping[44].transform.position - m_BoneMapping[43].transform.position;
        N = new Vector3((-1) * ElbR.x, (-1) * ElbR.y, (-1) * ElbR.z);
        //temp = Quaternion.LookRotation(N, Vector3.up);
        temp = Quaternion.LookRotation(m_BoneMapping[42].transform.InverseTransformVector(N), m_BoneMapping[42].transform.InverseTransformVector(N_Rhand));
        //temp2.Set(temp.z, temp.x, temp.y, temp.w);
        m_BoneMapping[43].transform.localRotation = temp;
    }

    private Quaternion makeRotation(Vector3 current, Vector3 rotated, int idOfBone)
    {
        return Quaternion.FromToRotation(m_BoneMapping[idOfBone].transform.InverseTransformVector(current), rotated);
        //m_BoneMapping[idOfBone].transform.localRotation = calc;
    }

    void ProcessJoint(Transform joint)
    {
        int index = GetJointIndex(joint.name);
        if (index >= 0 && index < k_NumSkeletonJoints)
        {
            m_BoneMapping[index] = joint;
        }
        else
        {
            Debug.LogWarning($"{joint.name} was not found.");
        }
    }

    // Returns the integer value corresponding to the JointIndices enum value
    // passed in as a string.
    int GetJointIndex(string jointName)
    {
        if (jointName.Contains("Character1"))
        {
            jointName = jointName.Replace("Character1_", "");
        }
        JointIndices val;
        if (Enum.TryParse(jointName, out val))
        {
            return (int)val;
        }
        return -1;
    }

    public dataJSON processPose(PointsJSON pose)
    {
        dataJSON newHuman = new dataJSON();
        newHuman.Center.x = pose.p0[0]; newHuman.Center.y = pose.p0[1]; newHuman.Center.z = pose.p0[2]; newHuman.Center.w = pose.p0[3];
        newHuman.Chest.x = pose.p1[0]; newHuman.Chest.y = pose.p1[1]; newHuman.Chest.z = pose.p1[2]; newHuman.Chest.w = pose.p1[3];
        newHuman.Neck.x = pose.p2[0]; newHuman.Neck.y = pose.p2[1]; newHuman.Neck.z = pose.p2[2]; newHuman.Neck.w = pose.p2[3];
        newHuman.Head.x = pose.p3[0]; newHuman.Head.y = pose.p3[1]; newHuman.Head.z = pose.p3[2]; newHuman.Head.w = pose.p3[3];
        //Debug.Log("p3: " + newHuman.Head.x + " " + newHuman.Head.y + " " + newHuman.Head.z);
        newHuman.ShoulderL.x = pose.p4[0]; newHuman.ShoulderL.y = pose.p4[1]; newHuman.ShoulderL.z = pose.p4[2]; newHuman.ShoulderL.w = pose.p4[3];
        newHuman.ShoulderR.x = pose.p5[0]; newHuman.ShoulderR.y = pose.p5[1]; newHuman.ShoulderR.z = pose.p5[2]; newHuman.ShoulderR.w = pose.p5[3];
        newHuman.ElbL.x = pose.p6[0]; newHuman.ElbL.y = pose.p6[1]; newHuman.ElbL.z = pose.p6[2]; newHuman.ElbL.w = pose.p6[3];
        newHuman.ElbR.x = pose.p7[0]; newHuman.ElbR.y = pose.p7[1]; newHuman.ElbR.z = pose.p7[2]; newHuman.ElbR.w = pose.p7[3];
        newHuman.HandL.x = pose.p8[0]; newHuman.HandL.y = pose.p8[1]; newHuman.HandL.z = pose.p8[2]; newHuman.HandL.w = pose.p8[3];
        newHuman.HandR.x = pose.p9[0]; newHuman.HandR.y = pose.p9[1]; newHuman.HandR.z = pose.p9[2]; newHuman.HandR.w = pose.p9[3];
        newHuman.LegL.x = pose.p10[0]; newHuman.LegL.y = pose.p10[1]; newHuman.LegL.z = pose.p10[2]; newHuman.LegL.w = pose.p10[3];
        newHuman.LegR.x = pose.p11[0]; newHuman.LegR.y = pose.p11[1]; newHuman.LegR.z = pose.p11[2]; newHuman.LegR.w = pose.p11[3];
        newHuman.KneeL.x = pose.p12[0]; newHuman.KneeL.y = pose.p12[1]; newHuman.KneeL.z = pose.p12[2]; newHuman.KneeL.w = pose.p12[3];
        newHuman.KneeR.x = pose.p13[0]; newHuman.KneeR.y = pose.p13[1]; newHuman.KneeR.z = pose.p13[2]; newHuman.KneeR.w = pose.p13[3];
        newHuman.FootL.x = pose.p14[0]; newHuman.FootL.y = pose.p14[1]; newHuman.FootL.z = pose.p14[2]; newHuman.FootL.w = pose.p14[3];
        newHuman.FootR.x = pose.p15[0]; newHuman.FootR.y = pose.p15[1]; newHuman.FootR.z = pose.p15[2]; newHuman.FootR.w = pose.p15[3];
        newHuman.FootFingerL.w = 0; newHuman.FootFingerR.w = 0; newHuman.ThumbL.w = 0; newHuman.SmallL.w = 0; newHuman.ThumbR.w = 0; newHuman.SmallR.w = 0;
        /*newHuman.FootFingerL.x = pose.p16[0]; newHuman.FootFingerL.y = pose.p16[1]; newHuman.FootFingerL.z = pose.p16[2]; 
        newHuman.FootFingerR.x = pose.p17[0]; newHuman.FootFingerR.y = pose.p17[1]; newHuman.FootFingerR.z = pose.p17[2]; 
        newHuman.ThumbL.x = pose.p18[0]; newHuman.ThumbL.y = pose.p18[1]; newHuman.ThumbL.z = pose.p18[2]; 
        newHuman.SmallL.x = pose.p19[0]; newHuman.SmallL.y = pose.p19[1]; newHuman.SmallL.z = pose.p19[2]; 
        newHuman.ThumbR.x = pose.p20[0]; newHuman.ThumbR.y = pose.p20[1]; newHuman.ThumbR.z = pose.p20[2]; 
        newHuman.SmallR.x = pose.p21[0]; newHuman.SmallR.y = pose.p21[1]; newHuman.SmallR.z = pose.p21[2]; */
        newHuman.Waist.x = pose.p22[0]; newHuman.Waist.y = pose.p22[1]; newHuman.Waist.z = pose.p22[2]; newHuman.Waist.w = pose.p22[3];
        return newHuman;
    }





    private Quaternion QuaternionFromMatrix(Matrix4x4 m/*, int extraSign*/)
    { // так как unity делает последнюю компоненту кватерниона отрицательной при угле > 0 используем дополнительную переменную extraSign < -1 при 180 < угле < 360
        Quaternion q = new Quaternion();
        q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
        q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
        q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
        q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
        q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
        q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
        q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
        /*q.w = Mathf.Sqrt(1 + m[0,0] + m[1,1] + m[2,2]) / 2; 
        q.x = (m[2,1] - m[1,2]) / (4*q.w);
        q.y = (m[0,2] - m[2,0]) / (4*q.w);
        q.z = (m[1,0] - m[0,1]) / (4*q.w);*/
        return q;
    }


    private Matrix4x4 constructRotationMatrixOneAngle(float a, Vector3 N)
    {
        var matrix = Matrix4x4.zero;
        matrix[0, 0] = (1 - Mathf.Cos(a)) * N.x * N.x + Mathf.Cos(a); matrix[0, 1] = (1 - Mathf.Cos(a)) * N.x * N.y - Mathf.Sin(a) * N.z; matrix[0, 2] = (1 - Mathf.Cos(a)) * N.x * N.z + Mathf.Sin(a) * N.y;
        matrix[1, 0] = (1 - Mathf.Cos(a)) * N.x * N.y + Mathf.Sin(a) * N.z; matrix[1, 1] = (1 - Mathf.Cos(a)) * N.y * N.y + Mathf.Cos(a); matrix[1, 2] = (1 - Mathf.Cos(a)) * N.y * N.z - Mathf.Sin(a) * N.x;
        matrix[2, 0] = (1 - Mathf.Cos(a)) * N.x * N.z - Mathf.Sin(a) * N.y; matrix[2, 1] = (1 - Mathf.Cos(a)) * N.y * N.z + Mathf.Sin(a) * N.x; matrix[2, 2] = (1 - Mathf.Cos(a)) * N.z * N.z + Mathf.Cos(a);
        return matrix;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeSkeletonJoints();
        Vector3 spine = new Vector3(0, 0, 0);
        Vector3 spine1 = new Vector3(0, 0, 0);
        Vector3 spine2 = new Vector3(0, 0, 0);
        Vector3 head = new Vector3(0, 0, 0);
        Vector3 shdL = new Vector3(0, 0, 0);
        Vector3 shdR = new Vector3(0, 0, 0);
        Vector3 elbL = new Vector3(0, 0, 0);
        Vector3 elbR = new Vector3(0, 0, 0);
        Vector3 handL = new Vector3(0, 0, 0);
        Vector3 handR = new Vector3(0, 0, 0);
        Vector3 hipL = new Vector3(0, 0, 0);
        Vector3 hipR = new Vector3(0, 0, 0);
        Vector3 legL = new Vector3(0, 0, 0);
        Vector3 legR = new Vector3(0, 0, 0);
        Vector3 footL = new Vector3(0, 0, 0);
        Vector3 footR = new Vector3(0, 0, 0);
        Vector3 N = new Vector3(0, 0, 0);
        Quaternion temp = new Quaternion(0, 0, 0, 0);
        poseReceiver = GetComponent<PoseReceiver>();
        if (poseReceiver == null)
        {
            throw new Exception("No pose receiver is attached");
        }
        //socket.Connect(ipe);
    }

    // Update is called once per frame
    void Update()
    {
  
        JSONpose currPose = poseReceiver.GetPose();
        
        if (currPose == null)
        {
            Debug.Log("Current pose is null.");
            return;
        }

        if (!performSkeletonTransform)
            return;

        Vector3 bodyPos = new Vector3();
        Vector3 whereToSetBody = new Vector3();
        Vector2 center2D = new Vector2();
        float scaleZMoves = 0.5f;
        if (currPose.humans.Count != 0)
        {

            Vector3 N_Lhand;
            Vector3 N_Rhand;
            Vector3 LhandTumb = new Vector3(currPose.humans[0].human_2d.p18[0], currPose.humans[0].human_2d.p18[1], 0.0f);
            Vector3 LhandPin = new Vector3(currPose.humans[0].human_2d.p19[0], currPose.humans[0].human_2d.p19[1], 0.0f);
            Vector3 Lhand = new Vector3(currPose.humans[0].human_2d.p8[0], currPose.humans[0].human_2d.p8[1], 0.0f);
            Vector3 RhandTumb = new Vector3(currPose.humans[0].human_2d.p20[0], currPose.humans[0].human_2d.p20[1], 0.0f);
            Vector3 RhandPin = new Vector3(currPose.humans[0].human_2d.p21[0], currPose.humans[0].human_2d.p21[1], 0.0f);
            Vector3 Rhand = new Vector3(currPose.humans[0].human_2d.p9[0], currPose.humans[0].human_2d.p9[1], 0.0f);
            Vector3 LhandV1 = LhandTumb - Lhand; LhandV1 = LhandV1.normalized;
            Vector3 LhandV2 = LhandPin - Lhand; LhandV2 = LhandV2.normalized;
            Vector3 RhandV1 = RhandTumb - Rhand; RhandV1 = RhandV1.normalized;
            Vector3 RhandV2 = RhandPin - Rhand; RhandV2 = RhandV2.normalized;
            N_Lhand = Vector3.Cross(LhandV1, LhandV2).normalized;
            N_Rhand = Vector3.Cross(RhandV1, RhandV2).normalized;

            dataJSON human = processPose(currPose.humans[0].human_3d);

            ApplyBodyPose3(human, N_Lhand, N_Rhand);

            bodyPos.Set(currPose.humans[0].human_2d.p0[0], currPose.humans[0].human_2d.p0[1], 0.0f);
            center2D.Set(currPose.humans[0].human_2d.p1[0] - currPose.humans[0].human_2d.p0[0], currPose.humans[0].human_2d.p1[1] - currPose.humans[0].human_2d.p0[1]);
            scaleZMoves = (0.5f - center2D.sqrMagnitude / Mathf.Sqrt(640 * 640 + 480 * 480)) * 0.2f;
            bodyPos.x /= 640; bodyPos.y /= 480; bodyPos.y = 1 - Mathf.Sqrt(bodyPos.y);
            //Debug.Log("X is " + bodyPos.x + "Y is " + bodyPos.y);
            whereToSetBody = Camera.main.ScreenToWorldPoint(new Vector3(bodyPos.x * Camera.main.pixelRect.width, bodyPos.y * Camera.main.pixelRect.height, Camera.main.nearClipPlane + 4 + scaleZMoves));
            m_BoneMapping[0].transform.position = GetComponent<Transform>().position;
            //Debug.Log("Update");


        }
    }

}
