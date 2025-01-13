using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Based on Valems Tutorial https://www.youtube.com/watch?v=Wk2_MtYSPaM 

[System.Serializable]
public class VRMapDeviceToAvatar
{
    public Transform vrTarget;
    public Transform rigTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;

    public void Map()
    {
        rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }
}



public class VRRigForAvatar : MonoBehaviour
{
    public VRMapDeviceToAvatar head;
    public VRMapDeviceToAvatar leftHand;
    public VRMapDeviceToAvatar rightHand;


    public Transform headConstraint;
    public Vector3 headBodyOffset;
    public float turnSmoothness =1f;

    // Start is called before the first frame update
    void Start()
    {
        headBodyOffset = transform.position - headConstraint.position;
    }

    void FixedUpdate()
    {
        transform.position = headConstraint.position + headBodyOffset;
        transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(headConstraint.up, Vector3.up).normalized, Time.deltaTime * turnSmoothness); //to rotate only in y-axis

        //Mapping the head and hands positions to the VR Device
        head.Map();
        leftHand.Map();
        rightHand.Map();
    }
}
