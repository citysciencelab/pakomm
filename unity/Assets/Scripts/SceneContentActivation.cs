using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*



public class SceneContentActivation : MonoBehaviour
{
    public static SceneContentActivation sceneContentManager; //Singleton
    [SerializeField]
    private GameObject movableObjectParent;
    //private ShellHandRayPointer _pointer;
    private Placeable[] placeables;
    [SerializeField]
    private GameObject nearMenu;
    [SerializeField]
    private GameObject wallMenu;

    private void Awake()
    {
        if (sceneContentManager != null)
        {
            GameObject.Destroy(sceneContentManager);
        }
        else
        {
            sceneContentManager = this;
        }
        DontDestroyOnLoad(this);
    }

    //void Start()
    public void ActivateContent() 
    {
        //CoreServices.TeleportSystem.Enable();
        Debug.Log("[PaKOMM] SceneContentActivation - TeleportSystem enabled.");

        /* 
        if (_pointer == null)
        {
            _pointer = GetTheRightPointer();
        }
        placeables = movableObjectParent.GetComponentsInChildren<Placeable>();
        foreach (Placeable p in placeables){
            p.isSceneObject = true; 
            p.AddAllNecessaryComponents();
            p.ActivateComponents();
        }
    
       

        
    }

    public GameObject GetNearMenu() { return nearMenu; }
    public GameObject GetWallMenu() { return wallMenu; }

    /*
     * Searches for the HandShellRayPointer (MRTK) of the right hand and stores it  
     */
    
    /* 
    public ShellHandRayPointer GetTheRightPointer()
    {
        ShellHandRayPointer pointer = null;
        foreach (var source in CoreServices.InputSystem.DetectedInputSources)
        {
            foreach (var p in source.Pointers)
            {
                if (p is IMixedRealityNearPointer)
                {
                    // Ignore near pointers, we only want the rays
                    continue;
                }
                if (p is ShellHandRayPointer)
                {
                    //Debug.Log("ShellHandRayPointer was found: " + p);
                    pointer = (ShellHandRayPointer)p;
                    if (pointer.Handedness.IsRight())
                    {
                        _pointer = pointer;
                        return _pointer;
                    }

                }
            }
        }
        Debug.Log("[PaKOMM] No pointer found. Return null.");
        return pointer;
    }
    


    
    {
        return _pointer;
    }
    
}

*/