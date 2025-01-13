using System;
using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using UnityEditor;
using UnityEngine;

public class LongTapAddAnnotation : MonoBehaviour
{
    public LongPressGesture _longPressGesture;

    // Start is called before the first frame update

    void Awake()
    {
        _longPressGesture = GetComponent<LongPressGesture>();
    }
    private void Start()
    {
        
        if (_longPressGesture != null)
        {
            _longPressGesture.LongPressed += LongPressGestureTerrainOnLongPressed;
        }

    }
    
    private void OnDisable()
    {


        _longPressGesture.LongPressed -= LongPressGestureTerrainOnLongPressed;

    }
    
    private void LongPressGestureTerrainOnLongPressed(object sender, EventArgs e)
    {
            Debug.Log(e);
            Debug.Log("ICH WURDE AUSGEFÜHRT LONGPRESS AUf Object " + this.gameObject.GetComponent<DatabaseSyncNormal>().id + "          " + this.gameObject.name);
            
            AnnotationManager.AMG.SetAndShowInputModal(_longPressGesture.ScreenPosition, this.gameObject);
    }

}
