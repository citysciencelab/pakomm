using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class XR_Menu_Main_DrawColor : XRBaseInteractable
{

    public Color _color; 
    
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

        Brush.BrushManager.lineColor = _color; 

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
