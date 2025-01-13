using UnityEngine;

/// <summary>
/// rework as a new instantiated Pointer in mrtk settings --> with inheritance! and stuff :D
/// </summary>
public class LaserPointerCustom : MonoBehaviour
{
    // Singleton!
    private static LaserPointerCustom _instance;
    public static LaserPointerCustom Instance { get { return _instance; } }

    public GameObject IndicatorEndPointGameObj { get => indicatorEndPointGameObj; /*set => indicatorEndPointGameObj = value;*/}

    private GameObject indicatorEndPointGameObj;

    private Vector3 _forward = Vector3.zero;
    private Vector3 _startPoint = Vector3.zero;
    private Vector3 _indicatorEndPoint = Vector3.zero;
    private bool isActiveShellPointer = false;

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

        //PointerUtils.SetMotionControllerRayPointerBehavior(PointerBehavior.AlwaysOff, Handedness.Left);
        indicatorEndPointGameObj = new GameObject();
        indicatorEndPointGameObj.transform.localScale = indicatorEndPointGameObj.transform.localScale * 3;
    }

    ////how can i sub an event?! instead of querying every time!
    //private void OnEnable()
    //{
    //    CoreServices.InputSystem.InputEnabled += OnInputEnabled;
    //    CoreServices.InputSystem.InputDisabled += OnInputDisabled;
    //}

    //private void OnDisable()
    //{
    //    CoreServices.InputSystem.InputEnabled -= OnInputEnabled;
    //    CoreServices.InputSystem.InputDisabled -= OnInputDisabled;
    //}

    /* void Update()
    {
        indicatorEndPointGameObj.transform.position = _indicatorEndPoint;

        foreach (IMixedRealityInputSource source in CoreServices.InputSystem.DetectedInputSources)
        {
            if (source.SourceType == InputSourceType.Controller)
            {
                if(source.Pointers[0].Controller.ControllerHandedness.IsMatch(Handedness.Right))
                {
                    for (int i = 0; i < source.Pointers.Length; i++)
                    {
                        if (source.Pointers[i] is ShellHandRayPointer)
                        {
                            if (!source.Pointers[i].IsActive)
                            {
                                isActiveShellPointer = source.Pointers[i].IsActive;
                                return;
                            }

                            //At Start Null & the first Trigger activation too!
                            //if it is teleporting it has no result, since not active!
                            //if(source.Pointers[i].Result == null)
                            //source.Pointers[i].Position
                            //_indicatorEndPoint = source.Pointers[i].Result.Details.Point

                            _startPoint = (source.Pointers[i] as ShellHandRayPointer).LineBase.FirstPoint;
                            _indicatorEndPoint = (source.Pointers[i] as ShellHandRayPointer).BaseCursor.Position;
                            _forward = (_indicatorEndPoint - _startPoint).normalized;

                            //Debug.DrawLine(_startPoint, _indicatorEndPoint, Color.blue, 0.3f);

                            //var hitObject = p.Result.Details.Object;
                        }
                    }
                }


                //ShellHandRayPointer shellHandRayPointer in )

                //IMixedRealityController mixedRealityController = source;


                //if (controller.ControllerHandedness.IsMatch(Handedness.Right))
                //{
                //    Debug.Log("rightController");
                //}

                //foreach (var p in source.Pointers)
                //{
                //    p.Controller.ControllerHandedness.IsRight

                //    //if (p is IMixedRealityNearPointer)
                //    //{
                //    //    // Ignore near pointers, we only want the rays
                //    //    continue;
                //    //}
                //    if (p.Result != null)
                //    {
                //        //der: https://docs.microsoft.com/en-us/dotnet/api/microsoft.mixedreality.toolkit.input.shellhandraypointer?view=mixed-reality-toolkit-unity-2020-dotnet-2.7.0
                //        //oder direkt den controller?
                //        if (p is IMixedRealityPrimaryPointerSelector)
                //        {
                //            var startPoint = p.Position;
                //            var endPoint = p.Result.Details.Point;
                //            var hitObject = p.Result.Details.Object;
                //            if (hitObject)
                //            {
                //                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //                sphere.transform.localScale = Vector3.one * 0.01f;
                //                sphere.transform.position = endPoint;
                //            }
                //        }
                //        Debug.Log("not primarypointerselector");
                //    }
                //}
            }
        }
    }*/

    /// <summary>
    /// does only work if shellpointer is active;
    /// does only raycast as far as GlobalPintingExtent is set!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collision"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    /* public bool TryGetFirstLaserPointerRaycastCollision<T>(out T collision, int layerMask) where T : UnityEngine.Component
    {
        bool isHit = false;
        collision = null;

        if (isActiveShellPointer)
        {
            Debug.Log("isActiveShellPointer: " + isActiveShellPointer);
            return isHit;
        }

        RaycastHit hit;

        if (Physics.Raycast(_startPoint, _forward, out hit, CoreServices.InputSystem.FocusProvider.GlobalPointingExtent, layerMask)) //Mathf.Infinity ////found this in mixed reality toolkit profiles! 
        {
            collision = hit.transform.GetComponent<T>();

            if (collision != null)
                isHit = true;
        }

        return isHit;
    }*/ 

    /* public bool TryGetLaserPointerPlaceableHitPosition(out Vector3 environmentCollision)
    {
        //Bit shift the index of the layer (5) to get a bit mask and invert it
        //This casts rays against every colliders except in layers --> 
        int layerMask = ~((1 << 5) | (1 << 0) | (1 << 8)); // everything except ui(5) & Default (which ovr rig has/ had) & Grabbables(just the spawning one would be cool/ collider off until its placed?! 

        //Also useable in the future? to be more precise
        //int layerMask = 1 << 9; // layer 9: environment
        //nice log!
        //Debug.Log(Convert.ToString(layerMask, 2).PadLeft(32, '0'));

        bool isHit = false;
        environmentCollision = Vector3.zero;

        if (isActiveShellPointer)
            return isHit;

        RaycastHit hit;

        //WEIL FOCUS irgendwie hängen bleibt!
        //Vector3.Distance(_startPoint,_indicatorEndPoint)+0.1f
        if (Physics.Raycast(_startPoint, _forward, out hit, CoreServices.InputSystem.FocusProvider.GlobalPointingExtent, layerMask)) //Mathf.Infinity ////found this in mixed reality toolkit profiles! 
        {
            //TODO:
            //Design? --> Pointer doesnt update his last pos… --> placeable will stand infront of "Structure"
            if (hit.collider.gameObject.layer == 11)
            {
                return isHit;
            }

            //Debug.Log("hit.collider.gameObject.layer: " + hit.collider.gameObject.layer);

            if (hit.point != null)
            {
                environmentCollision = hit.point;
                isHit = true;
            }
        }

        return isHit;
    }
    */
}