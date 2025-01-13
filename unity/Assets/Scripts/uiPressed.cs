using Normal.Realtime;
using TouchScript.Gestures;
using UnityEngine;
using TouchScript.Gestures.TransformGestures;
using TouchScript.Hit;
using TouchScript.Pointers;
using UnityEditor;
using UnityEngine.SceneManagement;

public class uiPressed : MonoBehaviour
{
    public ScreenTransformGesture pressGesture;
    public GameObject myPrefabToSpawn;
    private Vector3 pos;
    private GameObject tempPrefabTransform;
    //private MapCoord mpCord;
    //private GeoCoord geoCord;
    public string layerToCheck = "Landscape";
    private GameObject DatabaseObjects;
    
    private void Awake()
    {
            
    }

    void Update()
    {

    }

    private void Start()
    {
        pressGesture.TransformStarted += startedTransform;
        pressGesture.Transformed += updatedTransformer;
        pressGesture.TransformCompleted += completedTransform;

    }

    private void OnDestroy()
    {
        pressGesture.TransformStarted -= startedTransform;
        pressGesture.Transformed -= updatedTransformer;
        pressGesture.TransformCompleted -= completedTransform;
    }
    
    
    
    private void completedTransform(object sender, System.EventArgs e)
    {

        
            Debug.Log("Objekt wurde erzeugt");
            DgraphQuery.DQ.addPlacedObjects(tempPrefabTransform, DgraphQuery.DQ.activeSessionNumber, pos.x, pos.y,
                pos.z, 0f, 0f, 0f, tempPrefabTransform.transform.localScale.x,
                tempPrefabTransform.transform.localScale.y, tempPrefabTransform.transform.localScale.z,
                myPrefabToSpawn.name);
            Debug.Log("Unity World Position Completed :  " + pos.x + "   " + pos.y + "    " + pos.z);




        Debug.Log(" KOMME ICH HIER AN? ");
        //pos = pressGesture.GetScreenPositionHitData().Point;

        //pos.y = Generator.Instance.GetAltitudeAtWorldPosition(pos);
        //Debug.Log(" Press Gesture " + pressGesture.ScreenPosition.x + "       "  + pressGesture.ScreenPosition.y);
        //Debug.Log("Unity World Position STARTED :  " + pos.x + "   " + pos.y + "    " + pos.z);
        //  Debug.Log("Latitude : " + geoCord.latitude);
        //  Debug.Log("Longitude : " + geoCord.longitude);

    }


    private void pressTransformer(object sender, System.EventArgs e)
    {
        // Debug.Log(" Ich bin X hier  " + pressGesture.ScreenPosition.x + "und Y hier: " + pressGesture.ScreenPosition.y);
        pos = pressGesture.GetScreenPositionHitData().RaycastHit.point;
        tempPrefabTransform.transform.position = pos;
        
     /*   if (Map.Instance != null)
        {
            geoCord = Map.Instance.data.origin.GetLocation(pos.x,pos.z);

        }
        */
        //pos.y = Generator.Instance.GetAltitudeAtWorldPosition(pos);
        //Debug.Log(" Press Gesture " + pressGesture.ScreenPosition.x + "       "  + pressGesture.ScreenPosition.y);
        //   Debug.Log("Unity World Position :  " + pos.x + "   " + pos.y + "    " + pos.z);
        //  Debug.Log("Latitude : " + geoCord.latitude);
        //  Debug.Log("Longitude : " + geoCord.longitude);

    }

    private void startedTransform(object sender, System.EventArgs e)
    {
        

            //DatabaseObjects = GameObject.Find(DgraphQuery.DQ.activeSessionNumber.ToSafeString());
            // Debug.Log(" Ich bin X hier  " + pressGesture.ScreenPosition.x + "und Y hier: " + pressGesture.ScreenPosition.y);
            pos = pressGesture.GetScreenPositionHitData().RaycastHit.point;
            Realtime.InstantiateOptions options = new Realtime.InstantiateOptions();
            options.ownedByClient = true;
            options.preventOwnershipTakeover = true;
            options.useInstance = NetworkManager.NetM._Realtime;
            options.destroyWhenOwnerLeaves = true;
            options.destroyWhenLastClientLeaves = true;
            tempPrefabTransform = Realtime.Instantiate(myPrefabToSpawn.name, options);
            Destroy(GetComponent<DatabaseSyncNormal>());



            //pos.y = Generator.Instance.GetAltitudeAtWorldPosition(pos);
            //Debug.Log(" Press Gesture " + pressGesture.ScreenPosition.x + "       "  + pressGesture.ScreenPosition.y);
            //Debug.Log("Unity World Position STARTED :  " + pos.x + "   " + pos.y + "    " + pos.z);
            //  Debug.Log("Latitude : " + geoCord.latitude);
            //  Debug.Log("Longitude : " + geoCord.longitude);


    }

    private void updatedTransformer(object sender, System.EventArgs e)
    {
        Vector3 targetPosition = new Vector3();
        int layerMask = 1 << 15;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(pressGesture.ScreenPosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity,  layerMask))
        {
            

            if (gameObject.tag != "Camera")
            {
                //Debug.Log("ich treffe Landscape");
                //Debug.Log(hit.point);

                pos = hit.point; 
                tempPrefabTransform.transform.position = hit.point;
                //targetPosition.y = hit.point.y;
            }


        }
        // Debug.Log(" Ich bin X hier  " + pressGesture.ScreenPosition.x + "und Y hier: " + pressGesture.ScreenPosition.y);
        //HitData data = pressGesture.GetScreenPositionHitData();
        //pos = pressGesture.GetScreenPositionHitData().RaycastHit.point;
        //Debug.LogWarning("Name of the Layer " + data.Target.gameObject.layer);
        tempPrefabTransform.GetComponent<RealtimeView>().RequestOwnership();
        tempPrefabTransform.GetComponent<RealtimeTransform>().RequestOwnership();
        
        //Debug.LogError("PosX" + pos.x + " PosY " + pos.y + " PosZ " + pos.z);


        //pos.y = Generator.Instance.GetAltitudeAtWorldPosition(pos);
        //Debug.Log(" Press Gesture " + pressGesture.ScreenPosition.x + "       "  + pressGesture.ScreenPosition.y);

    }
}
