using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//For touchtable: 
//using TouchScript.Gestures;
//using Unity.VisualScripting;

//using Microsoft.MixedReality.Toolkit;
//using Microsoft.MixedReality.Toolkit.Input;
//using Microsoft.MixedReality.Toolkit.Utilities;

    
public class ManipulatableObject : MonoBehaviour
{
    private GameObject obj;
    private float timeStamp = 0f;
    [SerializeField]
    private bool networkMode = true;
    //PhotonView photonView;
    [SerializeField]
    private int yOffset = 0;
    private LaserPointerCustom _laserPointerCustom; 

    //private int databaseID = 0; 

    //
    private void Awake()
    {
        //Hier die ben�tigten Scripte zuweisen, die aktuell noch manuell zugewiesen wurden und die notwendigen Einstellungen vornehmen?
        //Photon View
        //photonView = this.gameObject.AddComponent<PhotonView>();
        //Photon Transform View
        //MRTK BoundsControl
        //MRTK Object Manipulation + dazugeh�rende Objekte
        
    }


    // Start is called before the first frame update
    void Start()
    {
        obj = this.gameObject;
        _laserPointerCustom = LaserPointerCustom.Instance;
    }

    // Update is called once per frame
    void Update()
    {

       /*  if (_laserPointerCustom.TryGetLaserPointerPlaceableHitPosition(out Vector3 hitPos))
        {
            transform.position = hitPos + transform.up * yOffset;
        }
        */ 

    }

  

    public void takeOwnership()
    {
        //TODO: CHANGE TO NORMCORE

        /*if (photonView.IsMine != true)
        {
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
        }*/
        //Debug.Log("Ownership �bertragen");
    }
    
    //TODO Where should the new object be placed at the first time? 2m in front of the player?
    //siehe ButtonScript.spawnPrefab()
    /*public void addObject(GameObject prefab)
    {
        if (networkMode)
        {
            //to spawn networked objects, use PhotonNetwork.Instantiate(), put Prefabs in Ressources Folder and add their name
            obj = PhotonNetwork.Instantiate(prefab.name, transform.position, Quaternion.identity);
        }
        else
        {
            obj = Instantiate(prefab, transform.position, Quaternion.identity);
        }
        addObjectToDatabase(obj);
    } */
    
    private void addObjectToDatabase(GameObject prefab)
    {
        //DgraphQuery.DQ.addPlacedObjects(pointer.transform, DgraphQuery.DQ.activeSessionNumber, pos.x, pos.y, pos.z, prefab.transform.rotation.eulerAngles.x, prefab.transform.rotation.eulerAngles.y, prefab.transform.rotation.eulerAngles.z, scale.x, scale.y, scale.z, prefab.name);
    }

    public void WriteChangesToDatabase()
    {
        //DgraphQuery.DQ.updatePlacedObjects(obj.GetComponent<DatabaseID>().id, obj.transform.position.x, obj.transform.position.y, obj.transform.position.z, obj.transform.rotation.eulerAngles.x, obj.transform.rotation.eulerAngles.y, obj.transform.rotation.eulerAngles.z, obj.transform.localScale.x, obj.transform.localScale.y, obj.transform.localScale.z);
        timeStamp = Time.time;
    }

    private void OnDeleteObject()
    {
        //PhotonNetwork.Destroy(obj);
        //obj.GetComponent<DatabaseID>().DeleteObject();
    }

    private void AdaptObjectToTerrainHeight()
    {
        //transform.position = hitPos + transform.up * yOffset
        
    }



}
