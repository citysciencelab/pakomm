using System;
using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using UnityEditor;
using UnityEngine;

public class TapDelete : MonoBehaviour
{
    public TapGesture tapGesture;

    // Start is called before the first frame update

    void Awake()
    {
        tapGesture = GetComponent<TapGesture>();


    }
    private void Start()
    {


            tapGesture.Tapped += tappedHandler;


    }
    private void OnDisable()
    {


            tapGesture.Tapped -= tappedHandler;

    }

    private void tappedHandler(object sender, EventArgs e)
    {
      
        if (gameObject.tag == "Annotation") 
            
        {
            if (Manager.GameManager.Eraser_Enabled)
            {
                if (GetComponent<DatabaseSyncNormal>() != null)
                {
                    GetComponent<DatabaseSyncNormal>().DeleteObject();
                }
                
              
            }
           
        }
        else
        {
            GetComponent<DatabaseSyncNormal>().DeleteObject();

        }
    }




}
