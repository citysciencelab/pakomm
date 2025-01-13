using System;
using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;


public class XR_Grab_Annotation : XRBaseInteractable
{
    private bool selected = false;
    private bool deleted = false;
    public GameObject _gameObject; 
    private XRRayInteractor _xrRayInteractor;
    public InputActionProperty _rightThumbstick;
    public InputActionProperty _rightButtonPrimary;
    public InputActionProperty _rightButtonSecondary;
    public Vector3 initialScale; 
    public float _minScale = 0.5f;
    public float _maxScale = 5f;
    private RealtimeView _realtimeView;
    private RealtimeTransform _realtimeTransform;


    private void Start()
    {
        Debug.Log("ich bin hier im Annotation Script Setup");
        initialScale = transform.localScale; 
        Debug.Log("Ich bin Start vom XR GRAB ANNOTATION");
        _rightThumbstick = Manager.GameManager.RightHand.GetComponent<ActionBasedController>().directionalAnchorRotationAction;
        _rightButtonPrimary = Manager.GameManager.RightHand.GetComponent<InputTest>()._buttonPressPrimary;
        _rightButtonSecondary = Manager.GameManager.RightHand.GetComponent<InputTest>()._buttonPressSecondary;
        _realtimeView = GetComponent<RealtimeView>();
        _realtimeTransform = GetComponent<RealtimeTransform>();
       

    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (Manager.GameManager.Drawing_Enabled)
        {
            return;
        }
        
        base.OnHoverEntered(args);
        //args.interactableObject.transform.position = this.gameObject.transform.position;
        Debug.Log("ICH HOVERE ÜBER NEM ANNOTATION " + args.interactorObject.transform.gameObject.name);
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        if (Manager.GameManager.Drawing_Enabled)
        {
            return;
        }
        base.OnHoverExited(args);
        Debug.Log("Ich hoVERE nicht MEHR ÜBER ANNOTATION" + args.interactableObject.transform.gameObject.name);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (Manager.GameManager.Drawing_Enabled)
        {
            return; 
        }
        
        base.OnSelectEntered(args);

        _gameObject = args.interactableObject.transform.gameObject;
        
        _realtimeView = GetComponent<RealtimeView>();
        _realtimeTransform = GetComponent<RealtimeTransform>();
        _realtimeView.RequestOwnership();
        _realtimeTransform.RequestOwnership();
        
        if (this._gameObject != null)
        {
            
            _gameObject.GetComponent<BoxCollider>().enabled = false; 
            _xrRayInteractor = args.interactorObject.transform.gameObject.GetComponent<XRRayInteractor>();  
            selected = true;
            Debug.Log("ich wurde selected und dem Ray zugewiesen" + args.interactableObject.transform.gameObject.name + " " + 
                      "   mit     " + args.interactorObject.transform.gameObject.name );

        }
       
        
    }
    
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        
        if (Manager.GameManager.Drawing_Enabled)
        {
            return;
        }
        base.OnSelectExited(args);

        _realtimeView.ClearOwnership();
        _realtimeTransform.ClearOwnership();
        
        Debug.Log("Ich wurde losgelassen" + args.interactableObject.transform.gameObject.name);
        selected = false;
        _gameObject.GetComponent<BoxCollider>().enabled = true;
        DgraphQuery.DQ.updateBrushAnnotation(_gameObject.GetComponent<DatabaseSyncNormal>().id, _gameObject.transform.position.x, _gameObject.transform.position.y , _gameObject.transform.position.z, _gameObject.transform.rotation.eulerAngles.x, _gameObject.transform.rotation.eulerAngles.y, _gameObject.transform.rotation.eulerAngles.z, _gameObject.transform.localScale.x, _gameObject.transform.localScale.y, _gameObject.transform.localScale.z  );
        Debug.Log("ich wurde bewegt losgelassen und geupdated ");
        _xrRayInteractor.raycastMask = LayerMask.GetMask("Default", "Grabbable", "Menu", "Annotation");

    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (!selected)
        {
            return; 
        }
        
        _realtimeView.RequestOwnership();
        _realtimeTransform.RequestOwnership();
        _xrRayInteractor.raycastMask = LayerMask.GetMask("Landscape");
        
        switch (updatePhase)
        {

            case XRInteractionUpdateOrder.UpdatePhase.Dynamic:
            case XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender:

                _xrRayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit);
                    
                if (hit.transform != null)
                {
            
                        Debug.Log("Nullcheck durchlaufen");
                        
                        if (hit.transform.gameObject.layer == 15)
                        {
                            Debug.Log("Layercheck durchlaufen");
                            _gameObject.transform.position = new Vector3(hit.point.x, _gameObject.transform.position.y, hit.point.z);
                        }
  
                }
                
                if (_rightThumbstick.action.ReadValue<Vector2>().x > 0.5f && _rightThumbstick.action.ReadValue<Vector2>().x > _rightThumbstick.action.ReadValue<Vector2>().y)
                {
                    _gameObject.transform.Rotate( 0f, _rightThumbstick.action.ReadValue<Vector2>().x, 0.0f, Space.Self);
                }
                
                if (_rightThumbstick.action.ReadValue<Vector2>().x < -0.5f &&  _rightThumbstick.action.ReadValue<Vector2>().x < _rightThumbstick.action.ReadValue<Vector2>().y)
                {
                    _gameObject.transform.Rotate( 0f, _rightThumbstick.action.ReadValue<Vector2>().x, 0.0f, Space.Self);
                }

                if (_rightThumbstick.action.ReadValue<Vector2>().y > 0.5f && _rightThumbstick.action.ReadValue<Vector2>().y > _rightThumbstick.action.ReadValue<Vector2>().x)
                {
                    _gameObject.transform.localScale += new Vector3(_rightThumbstick.action.ReadValue<Vector2>().y / 20,
                        _rightThumbstick.action.ReadValue<Vector2>().y / 20f,
                        _rightThumbstick.action.ReadValue<Vector2>().y / 20f);
                }
                
                if (_rightThumbstick.action.ReadValue<Vector2>().y < -0.5f && _rightThumbstick.action.ReadValue<Vector2>().y < _rightThumbstick.action.ReadValue<Vector2>().x)
                {
                    _gameObject.transform.localScale += new Vector3(_rightThumbstick.action.ReadValue<Vector2>().y / 20,
                        _rightThumbstick.action.ReadValue<Vector2>().y / 20f,
                        _rightThumbstick.action.ReadValue<Vector2>().y / 20f );
                        
                }

                _gameObject.transform.localScale =
                    new Vector3(Mathf.Clamp(_gameObject.transform.localScale.x, initialScale.x* 0.25f, initialScale.x * 4),
                        Mathf.Clamp(_gameObject.transform.localScale.y, initialScale.y*0.25f, initialScale.y*4),
                        Mathf.Clamp(_gameObject.transform.localScale.z, initialScale.z*0.25f, initialScale.z*4));
                    
                Debug.Log("_rightThumbstick + "  + _rightThumbstick.action?.ReadValue<Vector2>().x);
                //this.gameObject.transform.RotateAround(gameObject.transform.position, new Vector3(0, 0, 1),_rightThumbstick.action?.ReadValue<Vector2>().x ); 


                if (_rightButtonPrimary.action.triggered)
                {

                    if (gameObject.layer == 11)
                    {
                        Debug.Log("DESTROYING ANNOTATION ");
                        DgraphQuery.DQ.deleteObjectById(_gameObject, _gameObject.GetComponent<DatabaseSyncNormal>().id);
                    }
                   
                }
                
                
                
                break;
        }
        
        
        
    }


    
    
   
    
}
