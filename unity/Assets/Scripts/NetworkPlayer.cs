using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
//using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Tutorial by Valem: https://www.youtube.com/watch?v=KHWuTBmT1oI 
/// adapted by Anna Wolf to fit Hand Gesture instead of controller 12.10.2020
/// adapted for MRTK 12.04.2022
/// This script synchronizes the player's movement with its Avataer Representation on networked devices
/// </summary>
public class NetworkPlayer : MonoBehaviour
{
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    public Transform player;
    //private PhotonView photonView;
    //private OVRPlayerController rig;
    //private OVRCameraRig rig;

    private Transform headRig;
    private Transform leftHandRig;
    private Transform rightHandRig;

    // Start is called before the first frame update
    void Start()
    {
        //photonView = GetComponent<PhotonView>();
        
        #if UNITY_ANDROID
        /* rig = FindObjectOfType<OVRCameraRig>();
        if(rig != null) { 
            Debug.Log("NetworkPlayer Rig : " + rig);

            //MRTK upgrade
            headRig = rig.transform.Find("TrackingSpace/CenterEyeAnchor");
            leftHandRig = rig.transform.Find("TrackingSpace/LeftHandAnchor");
            rightHandRig = rig.transform.Find("TrackingSpace/RightHandAnchor");
        }
        else
        {
            Debug.Log("Error: No rig found! Please ensure, that a fitting hmd is connected.");
        }
        */

       /* if (photonView.IsMine)
        {
            foreach(var item in GetComponentsInChildren<Renderer>())
            {
                item.enabled = false;
            }
        }
*/ 
        #endif
    }

    // Update is called once per frame
    /*void Update()
    {
        if (photonView.IsMine)
        {
            MapPosition(head, XRNode.Head);
            MapPosition(leftHand, XRNode.LeftHand);
            MapPosition(rightHand, XRNode.RightHand);
            //MapPosition(player, rig.transform);   
        }
    }*/

    void Update()
    {
        //#if UNITY_ANDROID
        /*if (photonView.IsMine)
        {
            //disabled and changed to the Start function, because otherwise the animoation does not synchronize
            head.gameObject.SetActive(false);
            leftHand.gameObject.SetActive(false);
            rightHand.gameObject.SetActive(false);
            MapPosition(head, headRig);
            MapPosition(leftHand, leftHandRig);
            MapPosition(rightHand, rightHandRig);
        }*/
        //#endif
    }

    /*void MapPosition(Transform target, XRNode node)
    {
        InputDevices.GetDeviceAtXRNode(node).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position);
        InputDevices.GetDeviceAtXRNode(node).TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation);

        target.position = position;
        target.rotation = rotation;
    }*/
   

    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }
}
