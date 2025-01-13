using System;
using System.Collections;
using System.Collections.Generic;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using UnityEditor;
using UnityEngine;

public class TapActivateFly : MonoBehaviour
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

        Manager.GameManager.activateCamFly();
    }




}