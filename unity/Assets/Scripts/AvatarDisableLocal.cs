using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal;
using Normal.Realtime;

public class AvatarDisableLocal : MonoBehaviour
{
    [SerializeField] private GameObject _XRLeftController;
    [SerializeField] private GameObject _XRRightController;
    [SerializeField] private GameObject _XRHeadModel;
    [SerializeField] private GameObject _XRMouthMesh;
    private Realtime _realtime;
    private RealtimeTransform _rtTransform; 

// Start is called before the first frame update
    void Awake()
    {
       

    }

  

    void Start()
    {
        _rtTransform = GetComponent<RealtimeTransform>();
        if (_rtTransform.isOwnedLocallySelf)
        {
            _XRLeftController.SetActive(false);
            _XRRightController.SetActive(false);
            _XRHeadModel.GetComponent<MeshRenderer>().enabled = false; 
            _XRMouthMesh.GetComponent<MeshRenderer>().enabled = false; 
            Debug.LogError("deactivated double Controllers for Performance");
        }
        
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
