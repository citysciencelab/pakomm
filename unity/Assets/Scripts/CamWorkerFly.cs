using System;
using System.Numerics;
using Normal.Realtime;
using TMPro;
using TouchScript.Behaviors;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Vector3 = UnityEngine.Vector3;

public class CamWorkerFly : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Manager.GameManager.CamFlyGo = this.gameObject;
        Manager.GameManager.CamFly = this.gameObject.GetComponentInChildren<Camera>();
        // Steuerung für Multitouch und Ipad
        if (!Manager.GameManager.VRMode)
        {
            var _tmpGes = gameObject.AddComponent<TapGesture>();
            var _tmpDel = gameObject.AddComponent<TapActivateFly>();
            _tmpDel.tapGesture = _tmpGes;
            _tmpGes.NumberOfTapsRequired = 2;
            
            var _tmpTranGes = gameObject.AddComponent<TransformGesture>();  
            var _tmpTrans = gameObject.AddComponent<Transformer>();
            
                
        }
            
            
        

        if (Manager.GameManager.VRMode)
        {
            
            Camera _cam = GetComponentInChildren<Camera>();
            _cam.enabled = false;
            
            var _tmpXRGrabPakomm = gameObject.GetOrAddComponent<XR_Grab_FlyCam>();
            var _tint = gameObject.AddComponent<XRTintInteractableVisual>();
            _tint.tintColor = Color.cyan;
            

        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
