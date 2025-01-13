using System.Collections;
using System.Collections.Generic;
using Normal.Realtime;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class XRMenuItem : XRBaseInteractable
{

    public GameObject _prefab;
    private GameObject tempObject; 
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (Manager.GameManager.Drawing_Enabled)
        {
            return;
        }

        if (Manager.GameManager.locked)
        {
            return;
        }
        base.OnHoverEntered(args);
        args.interactableObject.transform.position = this.gameObject.transform.position; 
        Debug.Log("ICH HOVERE ÜBER NEM MENU ITEM MIT " + args.interactorObject.transform.gameObject.name);
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        if (Manager.GameManager.Drawing_Enabled)
        {
            return;
        }
        if (Manager.GameManager.locked)
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
        if (Manager.GameManager.locked)
        {
            return;
        }
        base.OnSelectEntered(args);
        Debug.Log("ich wurde selected und erzeugt " + args.interactableObject.transform.gameObject.name + "    mit     " + args.interactorObject.transform.gameObject.name );
        InstantiateAndSelectInteractable(args);

    }
    
     
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
      
        base.OnSelectExited(args);
        
        DgraphQuery.DQ.addPlacedObjects(tempObject, DgraphQuery.DQ.activeSessionNumber, tempObject.transform.position.x, tempObject.transform.position.y,
            tempObject.transform.position.z, tempObject.transform.rotation.eulerAngles.x , tempObject.transform.rotation.eulerAngles.y, tempObject.transform.rotation.eulerAngles.z, 
            tempObject.transform.localScale.x, tempObject.transform.localScale.y, tempObject.transform.localScale.z,
            _prefab.name);
        
        Debug.Log("Objekt wurde erzeugt und ist BEENDET? ");
    }

    void InstantiateAndSelectInteractable(SelectEnterEventArgs args)
    {
        
        Realtime.InstantiateOptions options = new Realtime.InstantiateOptions();
        options.ownedByClient = false;
        options.preventOwnershipTakeover = false;
        options.useInstance = NetworkManager.NetM._Realtime;
        options.destroyWhenOwnerLeaves = false;
        options.destroyWhenLastClientLeaves = true;

        tempObject = Realtime.Instantiate(_prefab.name,
            args.interactableObject.transform.position,
            args.interactableObject.transform.rotation, options);
        XR_Grab_Pakomm _inter = tempObject.AddComponent<XR_Grab_Pakomm>();
       
        
       //GameObject _aff =  Instantiate(Resources.Load(("Interaction Affordance")) as GameObject, tempObject.transform); 
       args.manager.SelectEnter((IXRSelectInteractor)args.interactorObject, _inter);

    }
    
   
    
}
