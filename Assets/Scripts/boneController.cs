using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boneController : MonoBehaviour
{
    // 3D joint skeleton
    enum JointIndices
    {
        Invalid = -1,
        Hips = 0, // parent: Root [0]
        LeftUpLeg = 1, // parent: Hips [1]
        LeftLeg = 2, // parent: LeftUpLeg [2]
        LeftFoot = 3, // parent: LeftLeg [3]
        LeftToeBase = 4, // parent: LeftFoot [4]
        LeftToe_End = 5, // parent: LeftToes [5]
        RightUpLeg = 6, // parent: Hips [1]
        RightLeg = 7, // parent: RightUpLeg [7]
        RightFoot = 8, // parent: RightLeg [8]
        RightToeBase = 9, // parent: RightFoot [9]
        RightToe_End = 10, // parent: RightToes [10]

    }
    const int k_NumSkeletonJoints = 12;

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

    public void ApplyBodyPose()
    {   
        Vector3 some = new Vector3(1, 1, 1);

        for (int i = 0; i < k_NumSkeletonJoints; ++i)
        {
            
            var bone = m_BoneMapping[i];
            if (bone != null)
            {
                bone.transform.localPosition += some;
                bone.transform.localRotation *= Quaternion.Euler(10, 10, 10);
            }
        }
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
        JointIndices val;
        if (System.Enum.TryParse("mixamorig9:"+jointName, out val))
        {
            return (int)val;
        }
        return -1;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeSkeletonJoints();
        //ApplyBodyPose();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
