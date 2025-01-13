using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class genericTeleport : XRBaseInteractable
{

    public bool _is12;
    public bool _is28;
    public bool _is112;
    public bool _isZeroPos; 
    
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        Debug.Log("ICH HOVERE ÜBER CATEGORY ITEM MIT " + args.interactorObject.transform.gameObject.name);
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        Debug.Log("ICH HOVERE ÜBER CATEGORY ITEM NICHT MEHR " + args.interactableObject.transform.gameObject.name);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        Debug.Log("CATEGORY OBJECT AUSGEWÄHLT " + args.interactableObject.transform.gameObject.name + "    mit     " + args.interactorObject.transform.gameObject.name );

        if (_is12)
        {
            Manager.GameManager.Schnack12();
        }
        
        if (_is28)
        {
            Manager.GameManager.Schnack28();
        }
        
        if (_is112)
        {
            Manager.GameManager.Schnack112();
        }
        if (_isZeroPos)
        {
            Manager.GameManager.backToZero();
        }
      

    }
    
     
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
     
        base.OnSelectExited(args);

        Debug.Log("CATEGORY OBJECT NICHT MEHR AUSGEWÄHLT");
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);
        Debug.Log("Activated Category Object");
        
    }

}