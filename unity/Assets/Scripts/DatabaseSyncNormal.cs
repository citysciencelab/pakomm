using System;
using System.Linq;
using System.Numerics;
using Normal.Realtime;
using TMPro;
using TouchScript.Behaviors;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.XR.Interaction.Toolkit;
using Vector3 = UnityEngine.Vector3;

public class DatabaseSyncNormal : RealtimeComponent<DatabaseNormalId>
{
    public string id => model.id;
    public string visualId;
    public bool isScalable = false;
    public bool hasMarker => model.marker;
    public bool visualMarker; 
    private void Start()
    {
        // Steuerung für Multitouch und Ipad
        if (!Manager.GameManager.VRMode)
        {
            var _tmpLongTapGes = gameObject.AddComponent<LongPressGesture>();
            var _tmpLongTapAnnotation = gameObject.AddComponent<LongTapAddAnnotation>(); 
            _tmpLongTapGes.TimeToPress = 1f;
            _tmpLongTapAnnotation._longPressGesture = _tmpLongTapGes; 
            
            if (!Manager.GameManager.locked)
            {
                var _tmpGes = gameObject.AddComponent<TapGesture>();
                var _tmpDel = gameObject.AddComponent<TapDelete>();
               
                _tmpDel.tapGesture = _tmpGes;
                _tmpGes.NumberOfTapsRequired = 2;
                _tmpGes.TimeLimit = 1f; 
                var _tmpTranGes = gameObject.AddComponent<TransformGesture>();
                var _tmpTrans = gameObject.AddComponent<Transformer>();
            }

        }

        if (Manager.GameManager.VRMode)
        {
            if (!Manager.GameManager.locked)
            {
                var _tmpXRGrabPakomm = gameObject.GetOrAddComponent<XR_Grab_Pakomm>();
                var _tint = gameObject.AddComponent<XRTintInteractableVisualPaKOMM>();
                _tint.tintColor = Color.cyan;
            }
          
        }
        
       

    }

    protected override void OnRealtimeModelReplaced(DatabaseNormalId previousModel, DatabaseNormalId currentModel)
    {

        if (realtimeView.isSceneView)
        {
            Destroy(this);
            return;
        }

        if (previousModel != null)
        {
            // Unregister from events
            previousModel.idDidChange -= IdDidChange;
            previousModel.markerDidChange -= MarkerDidChange; 
        }

        if (currentModel != null)
        {
            if (currentModel.isFreshModel)
            {
                if (GetComponent<RealtimeTransform>().isOwnedLocallyInHierarchy)
                {

                    model.id = "";
                    visualId = model.id;
                    visualMarker = model.marker; 

                }
            }

            UpdateID();
            UpdateMarker();

            currentModel.idDidChange += IdDidChange;
            currentModel.markerDidChange += MarkerDidChange; 
        }
    }

    private void MarkerDidChange(DatabaseNormalId databaseNormalId, bool value)
    {
        UpdateMarker(); 
    }


    private void IdDidChange(DatabaseNormalId model, string id)
    {
        UpdateID();
        Debug.Log(" Update EVENT DID ID CHANGE");
    }

    private void UpdateID()
    {
        // Update the UI
        visualId = model.id;
        Debug.Log( id + "ID CHANGED!?!?!??!");

    }
    private void UpdateMarker()
    {
        // Update the UI
        visualMarker = model.marker;
        if (visualMarker)
        {
            GameObject Marker = Instantiate(Manager.GameManager.AnnotationMarker, transform.position, UnityEngine.Quaternion.identity, transform);
            if (!Manager.GameManager.VRMode)
            {
                Marker.transform.rotation = UnityEngine.Quaternion.Euler(90f, 0f, 0f);
                Marker.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
            
        }
        Debug.Log( id + "ID CHANGED!?!?!??!");

    }
    public void DeleteObject()
    {
        Debug.Log("Delete Object reached");
        if (id != "" || id != null || id != " ")
        {
           
            DgraphQuery.DQ.deleteObjectById(this.gameObject, model.id);
            Debug.Log("Requested DELETE WITH ID " + model.id);
        }
        
        else
        {
            Debug.Log("NO ID CANT DELETE WITHOUT BREAKING DATABASE SYNC");
        }
    }

    public void SetId(string id)
    {
        model.id = id;
    }

    public void SetMarker(bool _hasmarker)
    {
        model.marker = _hasmarker; 
    }

}
