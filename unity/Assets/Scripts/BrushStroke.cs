using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Normal.Realtime;
using Normal.Realtime.Serialization;
using TMPro;
using TouchScript.Behaviors;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;

public class BrushStroke : RealtimeComponent<BrushStrokeModel> {
    [SerializeField]
    private BrushStrokeMesh _mesh = null;
    
    // Smoothing
    private Vector3    _ribbonEndPosition;
    private Quaternion _ribbonEndRotation = Quaternion.identity;

    // Mesh
    private Vector3    _previousRibbonPointPosition;
    private Quaternion _previousRibbonPointRotation = Quaternion.identity;

    public BoxCollider _boxCollider;
    public bool refreshed = false;
    public float _brushWidth = 0.2f;
    public Color _color = Color.blue;
    private XR_Grab_Annotation _tmpXRGrabPakomm;
    public List<DgraphQuery.RibbonPoint> _ribbons = new List<DgraphQuery.RibbonPoint>();

    private void Start()
    {
      

    }
    
 

   private void OnEnable()
    {
       
    }

    // Unity Events
    private void Update() {
        // Animate the end of the ribbon towards the brush tip
        AnimateLastRibbonPointTowardsBrushTipPosition();

        // Add a ribbon segment if the end of the ribbon has moved far enough
        AddRibbonPointIfNeeded();
    }

    // Interface
    public void BeginBrushStrokeWithBrushTipPoint(Vector3 position, Quaternion rotation) {
        // Update the model
        model.brushTipPosition = position;
        model.brushTipRotation = rotation;

        // Update last ribbon point to match brush tip position & rotation
        _ribbonEndPosition = position;
        _ribbonEndRotation = rotation;
        _mesh._brushStrokeWidth = model.brushStrokeWidh; 
        Debug.Log("Funzt das hier? ");
        
        _mesh.UpdateLastRibbonPoint(_ribbonEndPosition, _ribbonEndRotation);
    }

    public void MoveBrushTipToPoint(Vector3 position, Quaternion rotation) {
        model.brushTipPosition = position;
        model.brushTipRotation = rotation;
       
        
    }

    public void EndBrushStrokeWithBrushTipPoint(Vector3 position, Quaternion rotation) {
        // Add a final ribbon point and mark the stroke as finalized
        AddRibbonPoint(position, rotation);

        if (_mesh._mesh.vertices.Length > 3)
        {
            
            model.brushStrokeFinalized = true;

        }
        else
        {
            Realtime.Destroy(this.gameObject);
        }
       
    }


    // Ribbon drawing
    private void AddRibbonPointIfNeeded() {

        // If the brush stroke is finalized, stop trying to add points to it.
        if (model.brushStrokeFinalized)
        {
            if (!refreshed)
            {
                Destroy(gameObject.GetComponent<BoxCollider>());
                Debug.Log("Destroyed Box Collider on Fresh Annotation");
                _boxCollider = gameObject.AddComponent<BoxCollider>();
                Debug.Log("Added new Box Collider on Fresh Annotation");
                
                // Steuerung für Multitouch und Ipad
                if (!Manager.GameManager.VRMode)
                {
                    var _tmpGes = gameObject.GetOrAddComponent<TapGesture>();
                    var _tmpDel = gameObject.GetOrAddComponent<TapDelete>();
                    _tmpDel.tapGesture = _tmpGes;
                    _tmpGes.NumberOfTapsRequired = 2;
                    var _tmpTranGes = gameObject.GetOrAddComponent<TransformGesture>();
                    var _tmpTrans = gameObject.GetOrAddComponent<Transformer>();
                }

                if (Manager.GameManager.VRMode)
                {
                    _tmpXRGrabPakomm = gameObject.GetOrAddComponent<XR_Grab_Annotation>();
                    if (_tmpXRGrabPakomm.colliders.Count > 0)
                    {
                        _tmpXRGrabPakomm.colliders.Clear();
                    }
                    //var _tint = gameObject.AddComponent<XRTintInteractableVisual>();
                    //_tint.tintColor = Color.cyan;
                    var _tintTest = gameObject.GetOrAddComponent<XRTintAnnotation>();
                    _tintTest.tintColor = Color.cyan;

                    Debug.Log("Created VR MODE Addons on Brush");
                    _tmpXRGrabPakomm.colliders.Add(_boxCollider);
                }
            }

            refreshed = true; 
            return;
            
        }
        
        if (!realtimeView.isOwnedLocallySelf)
        {
            return;
        }
        
       
           
        if (Vector3.Distance(_ribbonEndPosition, _previousRibbonPointPosition) >= 0.01f ||
            Quaternion.Angle(_ribbonEndRotation, _previousRibbonPointRotation) >= 10.0f) {

            // Add ribbon point model to ribbon points array. This will fire the RibbonPointAdded event to update the mesh.
            AddRibbonPoint(_ribbonEndPosition, _ribbonEndRotation);
            //AddRibbonPointTemp(_ribbonEndPosition, _ribbonEndRotation);
            

            // Store the ribbon point position & rotation for the next time we do this calculation
            _previousRibbonPointPosition = _ribbonEndPosition;
            _previousRibbonPointRotation = _ribbonEndRotation;
        }
    }

    public void AddRibbonPoint(Vector3 position, Quaternion rotation) {
        // Create the ribbon point
        RibbonPointModel ribbonPoint = new RibbonPointModel();
        ribbonPoint.position = position;
        ribbonPoint.rotation = rotation;
        model.ribbonPoints.Add(ribbonPoint);
        
        // Update the mesh
        _mesh.InsertRibbonPoint(position, rotation);
        
        

    }

    public void myNewAddRibbonPoint(Vector3 position, Quaternion rotation)
    {
        RibbonPointModel ribbonPoint = new RibbonPointModel();
        ribbonPoint.position = position;
        ribbonPoint.rotation = rotation;
        model.ribbonPoints.Add(ribbonPoint);
        
        // Update the mesh
        _mesh.InsertRibbonPoint(position, rotation);
    }
    
    public void ResetCollidersAndSetFinalized()
    {
        if (!refreshed)
        {
            Destroy(gameObject.GetComponent<BoxCollider>());
            Debug.Log("Destroyed Box Collider on Fresh Annotation");
            _boxCollider = gameObject.AddComponent<BoxCollider>();
            Debug.Log("Added new Box Collider on Fresh Annotation");
                
            // Steuerung für Multitouch und Ipad
            if (!Manager.GameManager.VRMode)
            {
                var _tmpGes = gameObject.GetOrAddComponent<TapGesture>();
                var _tmpDel = gameObject.GetOrAddComponent<TapDelete>();
                _tmpDel.tapGesture = _tmpGes;
                _tmpGes.NumberOfTapsRequired = 2;
                var _tmpTranGes = gameObject.GetOrAddComponent<TransformGesture>();
                var _tmpTrans = gameObject.GetOrAddComponent<Transformer>();
            }

            if (Manager.GameManager.VRMode)
            {
                _tmpXRGrabPakomm = gameObject.GetOrAddComponent<XR_Grab_Annotation>();
                if (_tmpXRGrabPakomm.colliders.Count > 0)
                {
                    _tmpXRGrabPakomm.colliders.Clear();
                }
                //var _tint = gameObject.AddComponent<XRTintInteractableVisual>();
                //_tint.tintColor = Color.cyan;
                var _tintTest = gameObject.GetOrAddComponent<XRTintAnnotation>();
                _tintTest.tintColor = Color.cyan;

                Debug.Log("Created VR MODE Addons on Brush");
                _tmpXRGrabPakomm.colliders.Add(_boxCollider);
            }
        }

        refreshed = true; 
        SetModelFinalized();
    }

    public int GetRibbonsPointsCount()
    {
        return model.ribbonPoints.Count;
    }
    
    public RealtimeArray<RibbonPointModel> GetRibbon()
    {
        return model.ribbonPoints;
    }
    
    public Vector3 GetRibbonsPointPosition(int index)
    {
        return model.ribbonPoints[index].position;

    }
    public Quaternion GetRibbonsPointRotation(int index)
    {
        return model.ribbonPoints[index].rotation;
    }

    public void SetModelFinalized()
    {
        model.brushStrokeFinalized = true; 
    }

    public Color GetModelColor()
    {
        return model.brushStrokeColor; 
    }

    public float GetModelBrushWidth()
    {
        return model.brushStrokeWidh;
    }

    public void SetModelColor(Color color)
    {
        model.brushStrokeColor = color; 
    }

    public void SetBrushStrokeWidth(float width)
    {
        model.brushStrokeWidh = width; 
    }

    private void RibbonPointAdded(RealtimeArray<RibbonPointModel> ribbonPoints, RibbonPointModel ribbonPoint, bool remote)
    {
        _mesh.InsertRibbonPoint(ribbonPoint.position, ribbonPoint.rotation);
    }
    // Brush tip + smoothing
    private void AnimateLastRibbonPointTowardsBrushTipPosition() {
        // If the brush stroke is finalized, skip the brush tip mesh, and stop animating the brush tip.
        if (model.brushStrokeFinalized) {
            _mesh.skipLastRibbonPoint = true;
            return;
        }

        Vector3    brushTipPosition = model.brushTipPosition;
        Quaternion brushTipRotation = model.brushTipRotation;

        // If the end of the ribbon has reached the brush tip position, we can bail early.
        if (Vector3.Distance(_ribbonEndPosition, brushTipPosition) <= 0.0001f &&
            Quaternion.Angle(_ribbonEndRotation, brushTipRotation) <= 0.01f) {
            return;
        }

        // Move the end of the ribbon towards the brush tip position
        _ribbonEndPosition =     Vector3.Lerp(_ribbonEndPosition, brushTipPosition, 25.0f * Time.deltaTime);
        _ribbonEndRotation = Quaternion.Slerp(_ribbonEndRotation, brushTipRotation, 25.0f * Time.deltaTime);

        // Update the end of the ribbon mesh
        _mesh.UpdateLastRibbonPoint(_ribbonEndPosition, _ribbonEndRotation);
    }

    

    protected override void OnRealtimeModelReplaced(BrushStrokeModel previousModel, BrushStrokeModel currentModel) {
        // Clear Mesh
        _mesh.ClearRibbon();

        if (previousModel != null) {
            // Unregister from events
            previousModel.ribbonPoints.modelAdded -= RibbonPointAdded;
        }

        if (currentModel != null)
        {
            if (currentModel.isFreshModel)
            {
                
                model.brushStrokeColor = Brush.BrushManager.lineColor;
                model.brushStrokeWidh = Brush.BrushManager._brushStrokeWidth;

            }
            
            _mesh._brushStrokeWidth = model.brushStrokeWidh;
            GetComponent<Renderer>().material.color = model.brushStrokeColor;
            //Brush.BrushManager.lineColor = model.brushStrokeColor; 
            // Replace ribbon mesh
            foreach (RibbonPointModel ribbonPoint in currentModel.ribbonPoints)
            {
                _mesh.InsertRibbonPoint(ribbonPoint.position, ribbonPoint.rotation);
            }
                
            
            // Update last ribbon point to match brush tip position & rotation
            _ribbonEndPosition = model.brushTipPosition;
            _ribbonEndRotation = model.brushTipRotation;
            _mesh.UpdateLastRibbonPoint(model.brushTipPosition, model.brushTipRotation);

            // Turn off the last ribbon point if this brush stroke is finalized
            _mesh.skipLastRibbonPoint = model.brushStrokeFinalized;
            

            // Let us know when a new ribbon point is added to the mesh
            currentModel.ribbonPoints.modelAdded += RibbonPointAdded;
        }
    }
}

