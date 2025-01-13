/*using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CubeManipulation : MonoBehaviour
{
    // Singleton!
    private static CubeManipulation _instance;
    public static CubeManipulation Instance { get { return _instance; } }

    public Transform TransformToManipulate { get => _transformToManipulate; set => _transformToManipulate = value; }

    public delegate void ManipulationEnded();


    //MENU FUNCTIONS AT MANIPULATION CUBE? //MANIPULATION SEQUENCE?! 
    //placement has to be selected! othervise this mode is skipped! when you select an already placed Object
    //maybe its no ui button --> maybe its a physicall hitable button & pressable --> some mixed reality button if that exists?
    //[SerializeField]
    //private Button saveAndExitButton = null;
    //[SerializeField]
    //private Button deleteAndExitButton = null;
    //reset to cached from Beginning after Manipulation started
    //[SerializeField]
    //private Button resetTransform = null;
    //[SerializeField]
    //private Button copyObject = null;
    //[SerializeField]
    //private Button replaceObject = null;

    //alles funktionen die egal wie selbst wenn sich das mit dem manipulation cube ändern sollte lohnen - weiter verwendet werden
    //die Funktionen? lieber unterm Cube! anstatt kontextsensitive im rumfliege menü & einem zweiten Menü

    //nur beim Spawnen Placebar? sonst auch wie cube bewegung?

    //the actual interactable thing
    [SerializeField]
    private GameObject manipulationCubeObj;

    private BoundsControl _cubeBoundsControl;

    private Transform _transformToManipulate = null;
    private event ManipulationEnded manipulationEndedEvent;

    [SerializeField]
    AnimationCurve rescalingCubeAcceleration;

    //Initialisation
    private bool _isRotatingStarted = false;
    private bool _isScalingStarted = false;
    private Quaternion _startingCubeRotation;
    private Vector3 _startingObjScale;

    //Starting Values Cached
    private Vector3 _positionCached;
    private Vector3 _positionCubeCached;
    private Quaternion _rotationCached;
    private Quaternion _rotationCubeCached;
    private Vector3 _scaleCached;
    private Vector3 _scaleCubeCached;
    private float _scaleCubeOffset;

    private void Awake()
    {
        // Singleton!
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        SetManipulationCube(false);
        _cubeBoundsControl = manipulationCubeObj.GetComponent<BoundsControl>();
    }

    //TODO: Button Event! - Delete button
    private void OnDelete()
    {
        //@ ANNA
        //TODO: 
        //PhotonNetwork.Destroy()
        Destroy(_transformToManipulate.gameObject);

        OnStopManipulation();
    }

    public void AddListenerToManipulationEnded(ManipulationEnded call)
    {
        manipulationEndedEvent += call;
    }

    public void RemoveListenerToManipulationEnded(ManipulationEnded call)
    {
        manipulationEndedEvent += call;
    }

    //TODO: Button Event! - Save button
    public void OnStopManipulation()
    {
        SetManipulationCube(false);

        if (_transformToManipulate != null)
        {
            _transformToManipulate.GetComponent<Placeable>().IsSelectedForManipulationSequence = false;
            _transformToManipulate.GetComponent<Outline>().enabled = false; //to "ResetTarget in Hovermanager Soon!"
            _transformToManipulate = null;
        }

        //to reset hover managers selectionTarget
        //TODO: for placeable to stop copying the transform
        manipulationEndedEvent?.Invoke();
    }
    
    public void InitManipulationCubeWithTarget(Transform targetTransform)
    {
        _transformToManipulate = targetTransform;
        SetManipulationCube(true); //needed?

        //TODO:
        //Sphere collision check --> enough space to be spawned ?! --> left or right around the object? whereever you can spawn it? or on the person?

        //for the placement / correct alignment function
        //Vor Target Transform --> mit der gleichen Orientierung / cube scale initialisieren! / oder durch 10 teilen? oder anders ummappen!
        float offsetDist = 1f;
        //float lowerHeight = 0.75f;
        float lowerHeight = 0.1f;
        Vector3 offsetDir = (targetTransform.position - Camera.main.transform.position).normalized;
        Vector3 spawnPos = Camera.main.transform.position + offsetDir * offsetDist;
        spawnPos = new Vector3(spawnPos.x, spawnPos.y - lowerHeight, spawnPos.z);
        manipulationCubeObj.transform.position = spawnPos;

        //_cubeBoundsControl.RotateLerpTime //does this somehow matter when trying to copy?
        //TODO: Remove Listener aswell! --> in OnDisable for safety + in when complete manipulation sequence ended! for sure
        //just because the cube gets rotated when the user moves it around for better interaction!
        _cubeBoundsControl.RotateStarted.AddListener(OnRotationStarted);
        _cubeBoundsControl.RotateStopped.AddListener(OnRotationStopped);
        _cubeBoundsControl.ScaleStarted.AddListener(OnScaleStarted);
        _cubeBoundsControl.ScaleStopped.AddListener(OnScaleStopped);

        //For a Reset Function
        _positionCached = _transformToManipulate.position;
        _rotationCached = _transformToManipulate.rotation;
        _scaleCached = _transformToManipulate.localScale;
        

        //TODO: just reset it to fix values? or do this in awake! - even better
        _positionCubeCached = manipulationCubeObj.transform.position;
        _rotationCubeCached = manipulationCubeObj.transform.rotation;
        _scaleCubeCached = manipulationCubeObj.transform.localScale;
        
        //_scaleCubeOffset = 1f - _scaleCubeCached.x;
    }

    private void OnScaleStopped()
    {
        _isScalingStarted = false;
        StartCoroutine(ResetCubeScale());
    }

    private void OnScaleStarted()
    {
        _isScalingStarted = true;
        _startingObjScale = _transformToManipulate.localScale;
    }

    private void OnRotationStarted()
    {
        _isRotatingStarted = true;
        //maybe you should save the rotation of the object instead
        _startingCubeRotation = manipulationCubeObj.transform.rotation; 
    }

    private void OnRotationStopped()
    {
        _isRotatingStarted = false;
    }

    internal void SetManipulationCube(bool isActive)
    {
        manipulationCubeObj.SetActive(isActive);
    }

    internal void CopyTransform(Transform targetTransform)
    {
        //SCALING
        if(_isScalingStarted)
        {
            //targetTransform.localScale = Vector3.Scale(_scaleCached, manipulationCubeObj.transform.localScale + Vector3.one * _scaleCubeOffset);
            targetTransform.localScale = Vector3.Scale(_startingObjScale, manipulationCubeObj.transform.localScale);
        }

        //COULD RESET AND WORK THE SAME WAY AS SCALE DOES!
        //ROTATING
        if (_isRotatingStarted)
        {
            //multiplying the Inverse stands for subtracting --> A * B * iB = A //note that matrix multiplication is not commutative property //Kommutativgesetz
            //https://forum.unity.com/threads/subtracting-quaternions.317649/
            targetTransform.rotation = manipulationCubeObj.transform.rotation manipulationCubeObj.transform.rotation * Quaternion.Inverse(_startingCubeRotation); //how to scale?
        }
    }
  
    //StopAllCoroutines();
    private IEnumerator ResetCubeScale()
    {
        float timeRunning = 0f;
        bool isRunning = true;

        while (isRunning)
        {
            timeRunning += Time.deltaTime;
            manipulationCubeObj.transform.localScale = Vector3.Lerp(manipulationCubeObj.transform.localScale, Vector3.one, rescalingCubeAcceleration.Evaluate(timeRunning));

            if (manipulationCubeObj.transform.localScale == Vector3.one)
                isRunning = false;

            yield return null;
        }
    }
}
*/