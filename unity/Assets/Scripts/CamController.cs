/*
 * @author Valentin Simonov / http://va.lent.in/
 */

//using CityGen3D;
//using CityGen3D.EditorExtension;

using System;
using TouchScript.Examples.CameraControl;
using TouchScript.Gestures;
using UnityEngine;
using TouchScript.Gestures.TransformGestures;
using TouchScript.Hit;
using TouchScript.Pointers;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.SceneManagement;



public class CamController : MonoBehaviour
{
    public ScreenTransformGesture PanFingerMoveGesture;
    public ScreenTransformGesture RotZoomManipulationGesture;
    public LongPressGesture LongPressGestureTerrain; 
    public GameObject myPrefabToSpawn;
    private Vector3 pos;
    public Transform pivot;
    public float RotationSpeed = 2f;
    public float ZoomSpeed = 1f;
    public float PanSpeed = 2f;
    public float camYpos; 

    private GameObject tempPrefabTransform;

    //private MapCoord mpCord;
    //private GeoCoord geoCord;
    public string layerToCheck = "Landscape";
    private GameObject DatabaseObjects;

    private void Awake()
    {
        pivot = Manager.GameManager.Pivot.transform;
       
    }

    void Update()
    {

       

    }

    private void OnEnable()
    {
        Manager.GameManager._CamController = this; 

        if (PanFingerMoveGesture != null)
        {
            PanFingerMoveGesture.Transformed += oneFingerTransformHandler;
        }
        if(RotZoomManipulationGesture != null)
        {
            RotZoomManipulationGesture.Transformed += manipulationTransformedHandler;

        }

        if (LongPressGestureTerrain != null)
        {
            LongPressGestureTerrain.LongPressed += LongPressGestureTerrainOnLongPressed;
        }
        
    }

    private void LongPressGestureTerrainOnLongPressed(object sender, EventArgs e)
    {
        
        if (LongPressGestureTerrain.ActivePointers[0].Type == Pointer.PointerType.Touch || 
            LongPressGestureTerrain.ActivePointers[0].Type == Pointer.PointerType.Mouse)
        {
            Debug.Log(e);
            Debug.Log("ICH WURDE AUSGEFÜHRT LONGPRESS");
            Debug.Log(LongPressGestureTerrain.ActivePointers[0].Position);
            
            pos = LongPressGestureTerrain.GetScreenPositionHitData().RaycastHit.point;
            DgraphQuery.DQ.addAnnotationObject(DgraphQuery.DQ.activeSessionNumber, pos.x, pos.y,
                pos.z, LongPressGestureTerrain.ActivePointers[0].Position);
            
            Debug.Log("Unity World Position Completed :  " + pos.x + "   " + pos.y + "    " + pos.z);
        }
    }


    public void registerGestures()
    {
        PanFingerMoveGesture.Transformed += oneFingerTransformHandler;
        RotZoomManipulationGesture.Transformed += manipulationTransformedHandler;
        LongPressGestureTerrain.LongPressed += LongPressGestureTerrainOnLongPressed;
    }

    private void OnDestroy()
    {
        PanFingerMoveGesture.Transformed -= oneFingerTransformHandler;
        RotZoomManipulationGesture.Transformed -= manipulationTransformedHandler;
        LongPressGestureTerrain.LongPressed -= LongPressGestureTerrainOnLongPressed;
        

    }

   
    private void manipulationTransformedHandler(object sender, System.EventArgs e)
    {
        /*var rotation = Quaternion.Euler(RotZoomManipulationGesture.DeltaPosition.y/Screen.height*RotationSpeed,
            -RotZoomManipulationGesture.DeltaPosition.x/Screen.width*RotationSpeed,
            RotZoomManipulationGesture.DeltaRotation);
        pivot.localRotation *= rotation;
        
        */
        if (RotZoomManipulationGesture.ActivePointers[0].Type == Pointer.PointerType.Object)
        {
            Debug.Log("ICH IGNORIERE OBJEKT FÜR INPUT");
            return;
        }

        if (RotZoomManipulationGesture.ActivePointers[0].Type == Pointer.PointerType.Touch ||
            RotZoomManipulationGesture.ActivePointers[0].Type == Pointer.PointerType.Mouse)
        {
             camYpos = pivot.position.y;
             camYpos += -(RotZoomManipulationGesture.DeltaScale - 1f) * ZoomSpeed;
             camYpos = Mathf.Clamp(camYpos, -180,200f);

             pivot.position = new Vector3(pivot.position.x, camYpos, pivot.position.z); 
        }
    }


    private void oneFingerTransformHandler(object sender, System.EventArgs e)
    {

        if (pivot.transform.position.y > -50f)
        {
            PanSpeed = 0.1f; 
        }

        if (pivot.transform.position.y < - 120f)
        {
            PanSpeed = 0.05f; 
        }

        if (pivot.transform.position.y < -150f)
        {
            PanSpeed = 0.01f; 
        }
        //if (PanFingerMoveGesture.ActivePointers[0].Type == Pointer.PointerType.Object)
        //{
        //    return; 
        //}
        //if (PanFingerMoveGesture.ActivePointers[0].Type == Pointer.PointerType.Touch || PanFingerMoveGesture.ActivePointers[0].Type == Pointer.PointerType.Mouse || PanFingerMoveGesture.ActivePointers[0].Type == Pointer.PointerType.Pen || PanFingerMoveGesture.ActivePointers[0].Type == Pointer.PointerType.Unknown)
        //{
        if (!Manager.GameManager.Drawing_Enabled)
        {
            if (Manager.GameManager.camControllers_Enabled)
            {  
                
                Vector3 delt = new Vector3(PanFingerMoveGesture.DeltaPosition.x, PanFingerMoveGesture.DeltaPosition.z,
                    PanFingerMoveGesture.DeltaPosition.y);
                pivot.localPosition -= delt * PanSpeed;
                Debug.Log("Drawing Disabled - So i can translate");
                
            }
          
        }

        if (Manager.GameManager.Drawing_Enabled)
        {
            if (PanFingerMoveGesture.ActivePointers[0].Type == Pointer.PointerType.Touch ||
                PanFingerMoveGesture.ActivePointers[0].Type == Pointer.PointerType.Mouse)

            {
                Brush.BrushManager._drawingTouch = PanFingerMoveGesture;
                //Debug.Log("TUIO DRAWING MODE DETECTION" + PanFingerMoveGesture.ActivePointers[0].Position.x + "        " +PanFingerMoveGesture.ActivePointers[0].Position.y);
            }

            else
            {
                Debug.Log("OBJECT CURSORS DETECTED AND IGNORED");
            }
        }

        //}



        //}



    }
}
