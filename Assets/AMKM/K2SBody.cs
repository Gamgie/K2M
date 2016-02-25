using UnityEngine;
using System.Collections;
using Kinect = Windows.Kinect;
using System.Collections.Generic;

public class K2SBody : MonoBehaviour {

    public GameObject jointPrefab;
    public GameObject bonePrefab;

    [Range(.1f, 10f)]
    public float jointScale = 1;

    [Range(.1f,10f)]
    public float boneScale = 1;

    public bool boneIsCentered;

    private Transform[] joints;
    private Transform[] bones;

    int numJoints = 25;
    int numBones = 24;

    private Dictionary<Kinect.JointType, Kinect.JointType> boneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.Head ,Kinect.JointType.Neck },
        { Kinect.JointType.Neck , Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder , Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineBase }
       

    };

    // Use this for initialization
    void Awake () {
        joints = new Transform[25];
        bones = new Transform[24];

        for(int i=0;i< numJoints;i++)
        {
            joints[i] = GameObject.Instantiate(jointPrefab).transform;
            joints[i].parent = transform;
        }

        for(int i=0;i< numBones;i++)
        {
            bones[i] = GameObject.Instantiate(bonePrefab).transform;
            bones[i].parent = transform;
        }
	}

    void Start()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        for (int i = 0; i < numJoints; i++)
        {
            joints[i].rotation = Quaternion.identity; //force
        }

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void updateBody(Kinect.Body body)
    {
        for (int i = 0; i < numJoints; i++)
        {

            Kinect.Joint joint = body.Joints[(Kinect.JointType)i];
            
            Transform jt = joints[i];
            jt.localPosition = new Vector3(-joint.Position.X, joint.Position.Y, joint.Position.Z);
            jt.localScale = new Vector3(jointScale, jointScale, jointScale);
        }


        for (int i = 0; i < numBones; i++)
        {

            Kinect.JointType t1 = (Kinect.JointType)(i + 1); //start after spineBase
            Kinect.JointType t2 = boneMap[t1];

            Transform jt1 = joints[(int)t1];
            Transform jt2 = joints[(int)t2];

            Transform bone = bones[i];

            if (boneIsCentered)
            {
                bone.localPosition = Vector3.Lerp(jt1.localPosition, jt2.localPosition, .5f);
            }
            else
            {
                bone.localPosition = jt2.localPosition;
            }

            bone.LookAt(jt1);

            float dist = Vector3.Distance(jt1.position, jt2.position);
            bone.localScale = new Vector3(boneScale, boneScale, dist* boneScale);
        }
    }
}
