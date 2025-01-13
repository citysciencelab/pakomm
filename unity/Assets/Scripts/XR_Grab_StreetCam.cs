using System;
using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit;


public class XR_Grab_StreetCam : XRBaseInteractable
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
        initialScale = transform.localScale; 
        Debug.Log("Ich bin Start vom XR GRAB PAKOMM Street");
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
        Debug.Log("ICH HOVERE ÜBER NEM GESPAWNTES ITEM MIT " + args.interactorObject.transform.gameObject.name);
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        if (Manager.GameManager.Drawing_Enabled)
        {
            return; 
        }
        base.OnHoverExited(args);
        Debug.Log("Ich hoVERE nicht mehr über das Object" + args.interactableObject.transform.gameObject.name);
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
            
            _gameObject.GetComponent<Collider>().enabled = false; 
            _xrRayInteractor = args.interactorObject.transform.gameObject.GetComponent<XRRayInteractor>();  
            selected = true;
            Debug.Log("ich wurde selected und dem Ray zugewiesen" + args.interactableObject.transform.gameObject.name + " " + 
                      "   mit     " + args.interactorObject.transform.gameObject.name );

        }
       
        
    }
    
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
     
        base.OnSelectExited(args);
        
        

        _realtimeView.ClearOwnership();
        _realtimeTransform.ClearOwnership();
        
        Debug.Log("Ich wurde losgelassen" + args.interactableObject.transform.gameObject.name);
        selected = false;
        _gameObject.GetComponent<Collider>().enabled = true; 
        _xrRayInteractor.raycastMask = LayerMask.GetMask("Landscape", "Default", "Grabbable", "Menu", "Teleport", "Annotation");
   
        
        Debug.Log("Objekt wurde geupdated");
        
       

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
                            _gameObject.transform.position = hit.point;    
                        }
  
                }

                _gameObject.transform.Rotate( 0f, _rightThumbstick.action.ReadValue<Vector2>().x, 0.0f, Space.Self);
             
                


                if (_rightButtonPrimary.action.triggered)
                {
                    Debug.Log("XR_Grab_Pakomm Primary Button");
                  
                }
                
                
                
                break;
        }
     


       
        //Debug.Log("HITTE " + hit.transform.gameObject.name);
        
        
    }


    
    
   
    
}
