using Normal.Realtime;
using TouchScript.Gestures;
using UnityEngine;
using TouchScript.Gestures.TransformGestures;
using TouchScript.Hit;
using TouchScript.Pointers;
using UnityEditor;



public class Translate : MonoBehaviour
{
    public ScreenTransformGesture pressGesture;
    public GameObject myPrefabToSpawn;
    private Vector3 pos;
    private GameObject tempPrefabTransform;
    //private MapCoord mpCord;
    //private GeoCoord geoCord;
    public string layerToCheck = "Landscape";
    public DatabaseSyncNormal tempDatabaseID;
    private Vector3 targetPosition, targetScale;
    private Quaternion targetRotation;
    private TransformGesture.TransformType transformMask;

    private void Awake()
    {
        tempDatabaseID = GetComponent<DatabaseSyncNormal>();

#if UNITY_STANDALONE || UNITY_IOS
        //GetComponent<ButtonScript>().enabled = false;
        pressGesture = GetComponent<ScreenTransformGesture>();
        tempPrefabTransform = this.gameObject;

#endif

#if UNITY_ANDROID

        GetComponent<ScreenTransformGesture>().enabled = false;

#endif

    }

    void Update()
    {
        var mask = pressGesture.TransformMask;
        if ((mask & TransformGesture.TransformType.Scaling) != 0) targetScale *= pressGesture.DeltaScale;
        if ((mask & TransformGesture.TransformType.Rotation) != 0)
            targetRotation = Quaternion.AngleAxis(pressGesture.DeltaRotation, pressGesture.RotationAxis) * targetRotation;
        if ((mask & TransformGesture.TransformType.Translation) != 0) targetPosition += pressGesture.DeltaPosition;
        transformMask |= mask;
    }

    private void OnEnable()
    {
        pressGesture.TransformStarted += startedTransform;
        pressGesture.Transformed += updatedTransformer;
        pressGesture.TransformCompleted += completedTransform;

    }

    private void OnDisable()
    {
        pressGesture.TransformStarted -= startedTransform;
        pressGesture.Transformed -= updatedTransformer;
        pressGesture.TransformCompleted -= completedTransform;


    }
    private void completedTransform(object sender, System.EventArgs e)
    {

        //tempPrefabTransform.GetComponent<RealtimeTransform>().ClearOwnership();
        //tempPrefabTransform.GetComponent<RealtimeView>().ClearOwnership();


            Debug.Log("Objekt wurde erzeugt");
            DgraphQuery.DQ.updatePlacedObjects(tempDatabaseID.id, transform.position.x, transform.position.y, transform.position.z, transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z, transform.localScale.x, transform.localScale.y, transform.localScale.z);
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

    }

    private void startedTransform(object sender, System.EventArgs e)
    {


    }

    private void updatedTransformer(object sender, System.EventArgs e)
    {
        // Debug.Log(" Ich bin X hier  " + pressGesture.ScreenPosition.x + "und Y hier: " + pressGesture.ScreenPosition.y);
        if (pressGesture.GetScreenPositionHitData().RaycastHit.collider.gameObject.layer == 15)
        {

            pos = pressGesture.GetScreenPositionHitData().RaycastHit.point;
            tempPrefabTransform.transform.position = pos;
        }
        tempPrefabTransform.GetComponent<RealtimeView>().RequestOwnership();
        tempPrefabTransform.GetComponent<RealtimeTransform>().RequestOwnership();

        Debug.Log(tempPrefabTransform.transform.position.y);

        //pos.y = Generator.Instance.GetAltitudeAtWorldPosition(pos);
        //Debug.Log(" Press Gesture " + pressGesture.ScreenPosition.x + "       "  + pressGesture.ScreenPosition.y);

    }





}
