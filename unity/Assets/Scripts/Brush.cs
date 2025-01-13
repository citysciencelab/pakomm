using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Normal.Realtime;
using System;
using TouchScript.Behaviors;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Pointer = TouchScript.Pointers.Pointer;

public class Brush : MonoBehaviour
{

    public static Brush BrushManager;

    // Prefab to instantiate when we draw a new brush stroke
    [SerializeField] private GameObject _brushStrokePrefab = null;
    [SerializeField] private Realtime _realtime;

    public InputActionProperty _buttonPressTrigger;
    public InputActionProperty _controllerPosition;
    public InputActionProperty _controllerRotation;
    public InputActionProperty _isTracked;
    public GameObject _pokePoint;
    public GameObject brushStrokeGameObject;

    // Used to keep track of the current brush tip position and the actively drawing brush stroke
    private Vector3 _handPosition;
    private Quaternion _handRotation;
    private BrushStroke _activeBrushStroke;

    private bool triggerPressed;
    private bool handIsTracking;
    public ScreenTransformGesture _drawingTouch;
    public Vector3 movedByVec;
    public Vector3 TuioPositionWithZTemp;
    public Vector3 touchPositionWithZTemp;
    public float _brushStrokeWidth = 0.2f;
    public Color lineColor = Color.green;
    public Realtime.InstantiateOptions _InstantiateOptions;


    private void Awake()
    {

        if (BrushManager != null)
        {
            GameObject.Destroy(BrushManager);
        }
        else
        {
            BrushManager = this;
        }


        _InstantiateOptions = new Realtime.InstantiateOptions()
        {
            useInstance = _realtime,
            ownedByClient = true,
            destroyWhenOwnerLeaves = false,
            destroyWhenLastClientLeaves = true,
            preventOwnershipTakeover = false

        };

    }

    private void Start()
    {

    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    public void SetBrushWitdh(int _brushWidth)
    {
        if (_brushWidth == 0)
        {
            if (!Manager.GameManager.VRMode)
            {
                _brushStrokeWidth = 0.2f;
            }
            else
            {
                _brushStrokeWidth = 0.05f;
            }

        }

        if (_brushWidth == 1)
        {
            if (!Manager.GameManager.VRMode)
            {
                _brushStrokeWidth = 0.4f;
            }
            else
            {
                _brushStrokeWidth = 0.1f;
            }
        }

        if (_brushWidth == 2)
        {
            if (!Manager.GameManager.VRMode)
            {
                _brushStrokeWidth = 2f;
            }
            else
            {
                _brushStrokeWidth = 0.2f;
            }

        }

    }

    public void SetBrushColor(int _brushFloat)
    {
        if (_brushFloat == 0)
        {
            lineColor = Color.green;
        }

        if (_brushFloat == 1)
        {
            lineColor = Color.blue;
        }

        if (_brushFloat == 2)
        {
            lineColor = Color.yellow;
        }

        if (_brushFloat == 3)
        {
            lineColor = Color.red;
        }
    }


    private void oneFingerDrawing(object sender, System.EventArgs e)
    {
        Debug.Log("TESTING BRUSH MANIPULATION");
    }


    private void Update()
    {
        if (!_realtime.connected)
        {
            return;
        }

        if (!Manager.GameManager.Drawing_Enabled)
        {
            return; 
        }

        // Start by figuring out which hand we're tracking

        //XRNode node    = _hand == Hand.LeftHand ? XRNode.LeftHand : XRNode.RightHand;
        //string trigger = _hand == Hand.LeftHand ? "Left Trigger" : "Right Trigger";

        // Get the position & rotation of the hand
        //bool handIsTracking = UpdatePose(node, ref _handPosition, ref _handRotation);

        // Figure out if the trigger is pressed or not

        if (!Manager.GameManager.VRMode)
        {
            if (!Manager.GameManager.TUIOMode)
            {

                if (Input.GetMouseButton(0))
                {
                    triggerPressed = true; 
                }

                if (Input.GetMouseButtonUp(0))
                {
                    triggerPressed = false;
                }



                // If the trigger is pressed and we haven't created a new brush stroke to draw, create one!
                if (triggerPressed && _activeBrushStroke == null)
                {
                    var mousePositionWithZTemp = Extensions.GetRaycastedPosition();
                    var rot = quaternion.LookRotation(Camera.main.transform.position, Vector3.forward);
                    Debug.Log(mousePositionWithZTemp);

                    // Instantiate a copy of the Brush Stroke prefab.
                    brushStrokeGameObject = Realtime.Instantiate(_brushStrokePrefab.name, _InstantiateOptions);
                    brushStrokeGameObject.GetComponent<RealtimeView>().RequestOwnership();
                    brushStrokeGameObject.GetComponent<RealtimeTransform>().RequestOwnership();
                    movedByVec = brushStrokeGameObject.transform.position - mousePositionWithZTemp;
                    brushStrokeGameObject.transform.position = mousePositionWithZTemp;
                    brushStrokeGameObject.GetComponent<BrushStrokeMesh>()._brushStrokeWidth = _brushStrokeWidth;
                    brushStrokeGameObject.GetComponent<Renderer>().material.color = lineColor;
                    brushStrokeGameObject.GetComponent<BrushStroke>()._color = lineColor;

                    
                    //FIX this for Pivot? 

                    Debug.Log("FROM MOMENT PASTED " + mousePositionWithZTemp.x + "    " + mousePositionWithZTemp.y +
                              "   " +
                              mousePositionWithZTemp.z);

                
                    // Grab the BrushStroke component from it
                    _activeBrushStroke = brushStrokeGameObject.GetComponent<BrushStroke>();

                    // Tell the BrushStroke to begin drawing at the current brush position
                    _activeBrushStroke.BeginBrushStrokeWithBrushTipPoint(mousePositionWithZTemp + movedByVec, rot);
                }

                // If the trigger is pressed, and we have a brush stroke, move the brush stroke to the new brush tip position
                if (triggerPressed && _activeBrushStroke != null)
                {
                    var rot = quaternion.LookRotation(Camera.main.transform.position, Vector3.down);
                    var mousePositionWithZTemp = Extensions.GetRaycastedPosition();
                    Debug.Log("FROM HOLDING " + mousePositionWithZTemp.x + "    " + mousePositionWithZTemp.y + "   " +
                              mousePositionWithZTemp.z);
                    _activeBrushStroke.MoveBrushTipToPoint(mousePositionWithZTemp + movedByVec, rot);

                }

                // If the trigger is no longer pressed, and we still have an active brush stroke, mark it as finished and clear it.
                if (!triggerPressed && _activeBrushStroke != null)
                {
                    var rot = quaternion.LookRotation(Camera.main.transform.position, Vector3.down);
                    var mousePositionWithZTemp = Extensions.GetRaycastedPosition();
                    _activeBrushStroke.EndBrushStrokeWithBrushTipPoint(mousePositionWithZTemp + movedByVec,
                        rot);
                    _activeBrushStroke = null;
                    
                }
            }



            /*
            #if UNITY_IOS || UNITY_ANDROID
    
                if (Input.touchCount > 0)
                {
                    triggerPressed = true;
                    Touch touch = Input.GetTouch(0);
    
                }
                else
                {
                    triggerPressed = false;
                }
    
                // If the trigger is pressed and we haven't created a new brush stroke to draw, create one!
                if (triggerPressed && _activeBrushStroke == null)
                {
                    // Instantiate a copy of the Brush Stroke prefab
                    var mousePositionWithZTemp = Extensions.GetRaycastedTouchPosition();
                    Debug.Log(mousePositionWithZTemp);
    
                    // Instantiate a copy of the Brush Stroke prefab.
                    brushStrokeGameObject = Realtime.Instantiate(_brushStrokePrefab.name, _InstantiateOptions);
                    brushStrokeGameObject.GetComponent<RealtimeView>().RequestOwnership();
                    brushStrokeGameObject.GetComponent<RealtimeTransform>().RequestOwnership();
                    movedByVec = brushStrokeGameObject.transform.position - mousePositionWithZTemp;
                    brushStrokeGameObject.transform.position = touchPositionWithZTemp;
                    brushStrokeGameObject.GetComponent<BrushStrokeMesh>()._brushStrokeWidth = _brushStrokeWidth;
                    brushStrokeGameObject.GetComponent<Renderer>().material.color = lineColor;
                    brushStrokeGameObject.GetComponent<BrushStroke>()._color = lineColor;
    
    
                    // Grab the BrushStroke component from it
                    _activeBrushStroke = brushStrokeGameObject.GetComponent<BrushStroke>();
    
                    // Tell the BrushStroke to begin drawing at the current brush position
                    _activeBrushStroke.BeginBrushStrokeWithBrushTipPoint(mousePositionWithZTemp + movedByVec,
                        Quaternion.identity);
                }
    
                // If the trigger is pressed, and we have a brush stroke, move the brush stroke to the new brush tip position
                if (triggerPressed)
                {
                    var mousePositionWithZTemp = Extensions.GetRaycastedPosition();
                    _activeBrushStroke.MoveBrushTipToPoint(mousePositionWithZTemp + movedByVec, Quaternion.identity);
                }
    
                // If the trigger is no longer pressed, and we still have an active brush stroke, mark it as finished and clear it.
                if (!triggerPressed && _activeBrushStroke != null)
                {
                    var mousePositionWithZTemp = Extensions.GetRaycastedPosition();
                    _activeBrushStroke.EndBrushStrokeWithBrushTipPoint(mousePositionWithZTemp + movedByVec,
                        Quaternion.identity);
    
                    _activeBrushStroke = null;
                }
    #endif
    */

            if (Manager.GameManager.TUIOMode)
            {
                if (_drawingTouch != null)
                {
                    if (_drawingTouch.ActivePointers.Count > 0)
                    {
                        if (_drawingTouch.ActivePointers[0].Type == Pointer.PointerType.Touch ||
                            _drawingTouch.ActivePointers[0].Type == Pointer.PointerType.Mouse)
                        {
                            TuioPositionWithZTemp = Extensions.GetRaycastedTuioPosition(_drawingTouch.ScreenPosition);
                            Debug.Log(" Pointertype " + _drawingTouch.ActivePointers[0].Type);
                            triggerPressed = true;
                            Debug.Log("TUIO DRAWIN ENABLED");
                        }
                    }
                    else
                    {
                        triggerPressed = false;
                        Debug.Log("Nothing Registered");
                    }
                }

                // If the trigger is pressed and we haven't created a new brush stroke to draw, create one!
                if (triggerPressed && _activeBrushStroke == null)
                {
                    
                    // Instantiate a copy of the Brush Stroke prefab.
                    brushStrokeGameObject = Realtime.Instantiate(_brushStrokePrefab.name, _InstantiateOptions);
                    brushStrokeGameObject.GetComponent<RealtimeView>().RequestOwnership();
                    brushStrokeGameObject.GetComponent<RealtimeTransform>().RequestOwnership();
                    movedByVec = brushStrokeGameObject.transform.position - TuioPositionWithZTemp;
                    brushStrokeGameObject.transform.position = TuioPositionWithZTemp;
                    brushStrokeGameObject.GetComponent<BrushStrokeMesh>()._brushStrokeWidth = _brushStrokeWidth;
                    brushStrokeGameObject.GetComponent<Renderer>().material.color = lineColor;
                    brushStrokeGameObject.GetComponent<BrushStroke>()._color = lineColor;

                    //FIX this for Pivot? 

                    Debug.Log(TuioPositionWithZTemp.x + "    " + TuioPositionWithZTemp.y + "   " +
                              TuioPositionWithZTemp.z);

                    
                    // Grab the BrushStroke component from it
                    _activeBrushStroke = brushStrokeGameObject.GetComponent<BrushStroke>();
                    var rot = quaternion.LookRotation(Camera.main.transform.position, Vector3.down);

                    // Tell the BrushStroke to begin drawing at the current brush position
                    _activeBrushStroke.BeginBrushStrokeWithBrushTipPoint(TuioPositionWithZTemp + movedByVec,
                        rot);

                    Vector3 temp = TuioPositionWithZTemp + movedByVec;

                  
                }

                // If the trigger is pressed, and we have a brush stroke, move the brush stroke to the new brush tip position
                if (triggerPressed && _activeBrushStroke != null)
                {
                    if (_drawingTouch.ActivePointers != null)
                    {
                        if (_drawingTouch.ScreenPosition.x != null && _drawingTouch.ScreenPosition.y != null)
                        {
                            var rot = quaternion.LookRotation(Camera.main.transform.position, Vector3.down);

                            _activeBrushStroke.MoveBrushTipToPoint(TuioPositionWithZTemp + movedByVec,
                                rot);
                            
                            Vector3 temp = TuioPositionWithZTemp + movedByVec;

                        
                        }

                    }

                }


                // If the trigger is no longer pressed, and we still have an active brush stroke, mark it as finished and clear it.
                if (!triggerPressed && _activeBrushStroke != null)
                {
                    var rot = quaternion.LookRotation(Camera.main.transform.position, Vector3.down);

                    _activeBrushStroke.EndBrushStrokeWithBrushTipPoint(TuioPositionWithZTemp + movedByVec,
                        rot);
                    
                    Vector3 temp = TuioPositionWithZTemp + movedByVec;
                    DgraphQuery.DQ.addBrushAnnotation(_activeBrushStroke.gameObject,  DgraphQuery.DQ.activeSessionNumber, _activeBrushStroke.transform.position.x, _activeBrushStroke.transform.position.y, _activeBrushStroke.transform.position.z, _activeBrushStroke.transform.rotation.eulerAngles.x, _activeBrushStroke.transform.rotation.eulerAngles.y, _activeBrushStroke.transform.rotation.eulerAngles.z, _activeBrushStroke.transform.localScale.x,_activeBrushStroke.transform.localScale.y, _activeBrushStroke.transform.localScale.z, lineColor.r, lineColor.g, lineColor.b, lineColor.a, _brushStrokeWidth,  _brushStrokePrefab.name  );
    
                  
                    Debug.Log("ENDE BRUSH");
                    _drawingTouch = null;
                    _activeBrushStroke = null;

                }

            }


        }


        if (Manager.GameManager.VRMode)
        {


            if (_buttonPressTrigger.action.triggered)
                {
                    Debug.Log("ich drücke die Rechte Trigger taste" + " POSITION VECTOR " +
                              _controllerPosition.action.ReadValue<Vector3>() + " Rotation  " +
                              _controllerRotation.action.ReadValue<Quaternion>());
                }

                if (_isTracked.action.IsPressed())
                {

                    handIsTracking = true;
                }

                // Figure out if the trigger is pressed or not
                bool triggerPressed = _buttonPressTrigger.action.IsPressed();

                // If we lose tracking, stop drawing
                if (!handIsTracking)
                {
                    triggerPressed = false;
                }
                  

                // If the trigger is pressed and we haven't created a new brush stroke to draw, create one!
                if (triggerPressed && _activeBrushStroke == null)
                {

                    var mousePositionWithZTemp = _pokePoint.transform.position;
                    // Instantiate a copy of the Brush Stroke prefab, set it to be owned by us.
                    brushStrokeGameObject = Realtime.Instantiate(_brushStrokePrefab.name, _InstantiateOptions);
                    brushStrokeGameObject.GetComponent<RealtimeView>().RequestOwnership();
                    brushStrokeGameObject.GetComponent<RealtimeTransform>().RequestOwnership();

                    movedByVec = brushStrokeGameObject.transform.position - mousePositionWithZTemp;
                    brushStrokeGameObject.transform.position = mousePositionWithZTemp;
                    brushStrokeGameObject.GetComponent<BrushStroke>()._brushWidth = _brushStrokeWidth;
                    brushStrokeGameObject.GetComponent<BrushStroke>()._color = lineColor;
                    brushStrokeGameObject.GetComponent<BrushStrokeMesh>()._brushStrokeWidth = _brushStrokeWidth;
                    brushStrokeGameObject.GetComponent<Renderer>().material.color = lineColor;






// Grab the BrushStroke component from it
                    _activeBrushStroke = brushStrokeGameObject.GetComponent<BrushStroke>();

                    // Tell the BrushStroke to begin drawing at the current brush position
                    _activeBrushStroke.BeginBrushStrokeWithBrushTipPoint(_pokePoint.transform.position + movedByVec,
                        _pokePoint.transform.rotation);
                }

                // If the trigger is pressed, and we have a brush stroke, move the brush stroke to the new brush tip position
                if (triggerPressed && _activeBrushStroke != null)
                    _activeBrushStroke.MoveBrushTipToPoint(_pokePoint.transform.position + movedByVec,
                        _pokePoint.transform.rotation);

                // If the trigger is no longer pressed, and we still have an active brush stroke, mark it as finished and clear it.
                if (!triggerPressed && _activeBrushStroke != null)
                {
                    _activeBrushStroke.EndBrushStrokeWithBrushTipPoint(_pokePoint.transform.position + movedByVec,
                        _pokePoint.transform.rotation);
                    
                    
                    DgraphQuery.DQ.addBrushAnnotation(_activeBrushStroke.gameObject,  DgraphQuery.DQ.activeSessionNumber, _activeBrushStroke.transform.position.x, _activeBrushStroke.transform.position.y, _activeBrushStroke.transform.position.z, _activeBrushStroke.transform.rotation.eulerAngles.x, _activeBrushStroke.transform.rotation.eulerAngles.y, _activeBrushStroke.transform.rotation.eulerAngles.z, _activeBrushStroke.transform.localScale.x,_activeBrushStroke.transform.localScale.y, _activeBrushStroke.transform.localScale.z, lineColor.r, lineColor.g, lineColor.b, lineColor.a, _brushStrokeWidth,  _brushStrokePrefab.name  );

                    _activeBrushStroke = null;
                }
        }
    }
}






//// Utility

    // Given an XRNode, get the current position & rotation. If it's not tracking, don't modify the position & rotation.
   /* private static bool UpdatePose(XRNode node, ref Vector3 position, ref Quaternion rotation) {
        List<XRNodeState> nodeStates = new List<XRNodeState>();
        InputTracking.GetNodeStates(nodeStates);

        foreach (XRNodeState nodeState in nodeStates) {
            if (nodeState.nodeType == node) {
                Vector3    nodePosition;
                Quaternion nodeRotation;
                bool gotPosition = nodeState.TryGetPosition(out nodePosition);
                bool gotRotation = nodeState.TryGetRotation(out nodeRotation);

                if (gotPosition)
                    position = nodePosition;
                if (gotRotation)
                    rotation = nodeRotation;

                return gotPosition;
            }
        }

        return false;
    }*/

