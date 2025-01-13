using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public static class Extensions
{
    
    private static int layerMask = 1 << 15;
    private static int layerMask2 = 1 << 30;
    private static  int final = layerMask | layerMask2;
    
   public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        if(gameObject.TryGetComponent<T>(out T t))
        {
            return t;
        }
        else
        {
            return gameObject.AddComponent<T>();
        }
    }

    // Get Mouse Position in World with Z = 0f
    public static Vector3 GetMouseWorldPosition() {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }


    public static Vector3 GetMouseWorldPositionWithZ() {
        return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }

    public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera) {
        return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
    }

    public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera) {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }

    public static Vector3 GetDirToMouse(Vector3 fromPosition) {
        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        return (mouseWorldPosition - fromPosition).normalized;
    }

    public static bool IsPointerOverUI() {
        if (EventSystem.current.IsPointerOverGameObject()) {
            return true;
        } else {
            PointerEventData pe = new PointerEventData(EventSystem.current);
            pe.position =  Input.mousePosition;
            List<RaycastResult> hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll( pe, hits );
            return hits.Count > 0;
        }
    }

    public static Vector3 GetRaycastedPosition()
    {
     
        RaycastHit hit;
        Ray rr = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(rr, out hit, Mathf.Infinity, final);
        hit.point = new Vector3 (hit.point.x, hit.point.y + 0.2f, hit.point.z);
        return hit.point;
        

    }

    public static Vector3 GetRaycastedTouchPosition()
    {
     
        RaycastHit hit;
        Ray rr = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        Physics.Raycast(rr, out hit, Mathf.Infinity, final);
        hit.point = new Vector3 (hit.point.x, hit.point.y + 0.2f, hit.point.z);
        return hit.point;


    }
    
    public static Vector3 GetRaycastedTuioPosition(Vector3 _vector)
    {
       
        RaycastHit hit;
        Ray rr = Camera.main.ScreenPointToRay(_vector);
        Physics.Raycast(rr, out hit, Mathf.Infinity,  final);
        hit.point = new Vector3 (hit.point.x, hit.point.y + 0.1f, hit.point.z);
        return hit.point;

      
    }


}
