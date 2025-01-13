using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Normal.Realtime;
using SimpleGraphQL;
using TMPro;
using TouchScript.Behaviors.Cursors;
using TouchScript.Examples.CameraControl;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using TouchScript.InputSources;
using TouchScript.Layers;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Management;
using UnityEngine.InputSystem;
using UnityEngine.UI.Extensions;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class Manager : MonoBehaviour
{
    public static Manager GameManager;
    public string passwort = "pw";
    public bool locked;
    public TMP_InputField pwInput;
    public List<int> idsForMerger;
    public GameObject Annotation;
    public GameObject AnnotationData;
    public GameObject AnnotationMarker;
    public GameObject AnnotationItem;
    public bool adminMode = false;
    public bool RoomConnected = false;
    public bool DebugEnabled;
    public bool ARModeIOS;
    public GameObject ImmersalSDK;
    public GameObject AR_Session;
    public GameObject AR_Button;
    public ARCameraManager AR_CameraManager;
    public ARCameraBackground AR_CameraBackground;
    public bool VRMode;
    public bool VRMode_Simulator;
    public bool TUIOMode;
    public bool Drawing_Enabled;
    public bool Eraser_Enabled;
    public bool AnnotationsVisible = true;
    public bool camControllers_Enabled = true;
    public TMP_InputField _inputSessionName;
    public TMP_InputField _searchSessionName;
    public GameObject TouchIOSMenu;
    public GameObject AnnotationsList;
    public Vector3 pos12;
    public Vector3 pos28;
    public Vector3 pos112;
    public Vector3 startPosTouchDevices;
    public Vector3 startPosVR;
    public Vector3 startPosAR;
    public GameObject SafeZoneGO;
    public GameObject VRMenuHolder;
    public GameObject otherMenu;
    public XRUIInputModule _XruiInputModule;
    public GameObject MRTK_XR_RIG;
    public Vector3 MRTK_XR_RIG_Camera_Offset_Position = new Vector3(0f, 100f, 0f);
    public Vector3 MRTK_XR_RIG_Camera_Offset_Position_AR = new Vector3(0f, 1.36144f, 0f);
    public Quaternion MRTK_XR_RIG_Camera_Offset_Rotation = Quaternion.Euler(90f, 0f, 0f);
    public Quaternion MRTK_XR_RIG_Camera_Offset_Rotation_AR = Quaternion.Euler(0, 0f, 0f);
    public GameObject MRTK_XR_RIG_Camera_Offset;
    public GameObject MRTK_XR_RIG_Camera;
    public GameObject GO_XROrigin;
    public GameObject LeftHand;
    public GameObject RightHand;
    public GameObject Pivot;
    public Camera CamFly;
    public Camera CamStreet;
    public GameObject MainDisplayMenu;
    public GameObject SecondaryDisplayMenu;
    public GameObject DrawModusBorder;
    public GameObject EraserModusBorder;
    public GameObject TouchManager;
    public GameObject VRCam;
    public GameObject HandsManager;
    public GameObject Cursors;
    public GameObject FlickMenu;
    public GameObject EventSessionLoader;
    public GameObject Card;
    public GameObject eventSessionLoaderContent;
    public HorizontalScrollSnap EventSession_Horizontal_Snap;
    public GameObject PlacementMenu;
    public GameObject uiHelperGameObject;
    public GameObject uiPointer;
    //public ScaleManipulationPanel ScaleManipulationPanel;
    //public LaserPointer laserPointer;
    public GameObject EndPoint;
    public GameObject plane;
    public CamController _CamController;
    public bool newSession = false;
    public Button guiButton;
    public GameObject UIPlacement;
    public GameObject XRSimulator;
    public GameObject LastSelectedCategory;
    public GameObject CamFlyGo;
    public GameObject CamStreetGo;
    public Camera SecondScreenStartCam;
    public GameObject _drawPointer;
    public ObjectTwoCamera _objectTwoCamera;
    public ObjectThreeCamera _objectThreeCamera;
    public GameObject _rightControllerPokeInteractorPoint;
    public GameObject _rightControllerPokeInteractorCylinder;
    public GameObject _rightControllerPen;
    public GameObject _rightControllerRayCaster;
    public Vector3 preARPosition;
    public GameObject[] armodeDisable;
    public RealtimeAvatarManager _avatarManager;
    public Camera ScreenshotCam;



    private void Awake()
    {
        Debug.unityLogger.logEnabled = DebugEnabled;

        if (GameManager != null)
        {
            GameObject.Destroy(GameManager);
        }
        else
        {
            GameManager = this;
        }

        if (VRMode)
        {
            Debug.Log("AWAKE GameManager");
            TouchManager.SetActive(false);
            MainDisplayMenu.SetActive(false);
            SecondaryDisplayMenu.SetActive(false);
            SecondScreenStartCam.enabled = false;
            Cursors.SetActive(false);
        }

        //DontDestroyOnLoad(this);

    }



    void Start()
    {
        // Display.displays[0] is the primary, default display and is always ON, so start at index 1.
        // Check if additional displays are available and activate each.

#if UNITY_IOS
        Debug.Log("RUNNING ON IOS");
        AR_Button.SetActive(true);

#endif
        //if (XRGeneralSettings.Instance.Manager.activeLoader != null) {
        // XR device detected/loaded
        //    Debug.LogWarning("Ich bin im VR Mode");
        //   VRMode = true;
        // }
        if (!VRMode)
        {
            //
            //Disable XRSimulator
            //

            //Instantiate(TouchManager);
            _XruiInputModule.enabled = false;
            SafeZoneGO.SetActive(false);



            //
            //Displays Check and Activation
            //

            Debug.Log("displays connected: " + Display.displays.Length);
            for (int i = 1; i < Display.displays.Length; i++)
            {
                Display.displays[i].Activate();
            }

            //
            //Cam Adjustment for Touch Devices
            //

            MRTK_XR_RIG.gameObject.transform.SetParent(Pivot.transform);
            MRTK_XR_RIG_Camera_Offset.transform.position = MRTK_XR_RIG_Camera_Offset_Position;
            MRTK_XR_RIG_Camera_Offset.transform.rotation = MRTK_XR_RIG_Camera_Offset_Rotation;
            MRTK_XR_RIG_Camera.GetOrAddComponent<StandardLayer>();
            MRTK_XR_RIG_Camera.GetOrAddComponent<FullscreenLayer>();
            MRTK_XR_RIG_Camera.GetComponent<Camera>().farClipPlane = 1000f;
            LeftHand.SetActive(false);
            RightHand.SetActive(false);


            EventSessionLoader.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            EventSessionLoader.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
            EventSession_Horizontal_Snap.gameObject.GetComponent<RectTransform>().position = new Vector3(0, -174f, 0);


            Pivot.transform.position = startPosTouchDevices;

            //
            //Disable all XROrigin Stuff to be Performant on Mobile Devices
            //

            MonoBehaviour[] behavs = GO_XROrigin.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour c in behavs)
            {
                c.enabled = false;
            }

            GO_XROrigin.GetComponent<CharacterController>().enabled = false;
            SecondScreenStartCam.enabled = true;


        }

        if (VRMode)
        {

            Brush.BrushManager.SetBrushWitdh(0);
            Debug.LogError("ICH WURDE AUSGEFÜHRT UND " + Brush.BrushManager._brushStrokeWidth);

            if (VRMode_Simulator)
            {
                GameObject xrSim = Instantiate(XRSimulator);
                xrSim.SetActive(true);

            }

            MRTK_XR_RIG.transform.position = startPosVR;
            AnnotationsList.SetActive(false);



            //StartCoroutine(StartXR());



            //EventSessionLoader.SetActive(false);
            //EventSessionLoader.SetActive(true);
            //EventSessionLoader.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            //EventSessionLoader.GetComponent<Canvas>().worldCamera = Camera.main;

            EventSessionLoader.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            EventSessionLoader.transform.position = new Vector3(MRTK_XR_RIG.transform.position.x, MRTK_XR_RIG.transform.position.y + 1.5f, MRTK_XR_RIG.transform.position.z + 1.5f);
            EventSessionLoader.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
            SafeZoneGO.SetActive(true);
            //Vector3 rotat = new Vector3(0f, 0f, 0f);
            //EventSessionLoader.transform.rotation = Quaternion.Euler(rotat);



            //CamFly.enabled = false;
            //CamStreet.enabled = false;
            //_mrtkInput = new MRTKDefaultInputActions();
            //_mrtkInput.MRTKLeftHand.Enable();
            //_mrtkInput.MRTKLeftHand.Activate.performed += LeftHandTriggerPressed;
            //TouchManager.SetActive(false);

        }



        DgraphQuery.DQ.createEventSessionLoader();

    }

    public void ActivateAR()
    {
        ARModeIOS = !ARModeIOS;
        Debug.Log("Clicked AR BUTTON  " + ARModeIOS);
        if (ARModeIOS)
        {
            preARPosition = Pivot.transform.position;
            MRTK_XR_RIG.transform.SetParent(null);
            MRTK_XR_RIG.transform.position = startPosAR;
            MRTK_XR_RIG_Camera_Offset.transform.localPosition = MRTK_XR_RIG_Camera_Offset_Position_AR;
            MRTK_XR_RIG_Camera_Offset.transform.localRotation = MRTK_XR_RIG_Camera_Offset_Rotation_AR;
            GO_XROrigin.GetComponent<XROrigin>().enabled = true;

            /// Tag objects with ARModeDisabled that should be hidden in the AR mode
            armodeDisable = GameObject.FindGameObjectsWithTag("ARModeDisabled");

            /// Deactivate Terrain in AR mode
            //var terr = FindFirstObjectByType<Terrain>().enabled = false; 


            foreach (GameObject go in armodeDisable)
            {
                go.SetActive(false);
            }

            //MRTK_XR_RIG.transform.position = startPos; 
            StartCoroutine(StartXRCoroutine());
        }
        else
        {
            StopXR();

            foreach (GameObject go in armodeDisable)
            {
                go.SetActive(true);
            }
            var terr = FindFirstObjectByType<Terrain>().enabled = true;

        }
    }

    void StopXR()
    {

        ImmersalSDK.SetActive(false);
        Debug.Log("Stopping XR...");
        XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        Debug.Log("XR stopped completely.");

        MRTK_XR_RIG_Camera_Offset.transform.localPosition = MRTK_XR_RIG_Camera_Offset_Position_AR;
        MRTK_XR_RIG_Camera_Offset.transform.localPosition = MRTK_XR_RIG_Camera_Offset_Position;
        MRTK_XR_RIG_Camera_Offset.transform.localRotation = MRTK_XR_RIG_Camera_Offset_Rotation_AR;
        MRTK_XR_RIG_Camera_Offset.transform.localRotation = MRTK_XR_RIG_Camera_Offset_Rotation;

        //MRTK_XR_RIG_Camera.GetOrAddComponent<StandardLayer>();
        //MRTK_XR_RIG_Camera.GetOrAddComponent<FullscreenLayer>();
        MRTK_XR_RIG_Camera.GetComponent<Camera>().farClipPlane = 1000f;
        LeftHand.SetActive(false);
        RightHand.SetActive(false);

        GO_XROrigin.GetComponent<XROrigin>().enabled = false;
        MRTK_XR_RIG.gameObject.transform.SetParent(Pivot.transform);
        GO_XROrigin.transform.localPosition = Vector3.zero;
        GO_XROrigin.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);


        Pivot.transform.position = preARPosition;

        MonoBehaviour[] behavs = GO_XROrigin.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour c in behavs)
        {
            c.enabled = false;
        }

        AR_CameraManager.enabled = false;
        AR_CameraBackground.enabled = false;
        AR_Session.SetActive(false);
        Debug.Log(" Deactived AR...... ");

    }

    public IEnumerator StartXRCoroutine()
    {

        Debug.Log("Initializing XR...");
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("Initializing XR Failed. Check Editor or Player log for details.");
        }
        else
        {

            Debug.Log("Starting XR...");
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            AR_Session.SetActive(true);

        }

        AR_CameraBackground = Camera.main.GetComponent<ARCameraBackground>();
        AR_CameraBackground.enabled = true;
        AR_CameraManager = Camera.main.GetComponent<ARCameraManager>();
        AR_CameraManager.enabled = true;
        ImmersalSDK.SetActive(true);
    }

    private void LeftHandTriggerPressed(InputAction.CallbackContext obj)
    {
        Debug.Log("LeftHand Trigger Pressed");
    }


    public void ScreenShot()
    {

        Debug.Log("Screenshot taken.");

//#if !UNITY_EDITOR
//		    string url = Application.dataPath + "/" + DgraphQuery.DQ.activeSessionNumber.ToString() + ".png";
//#endif 
//#if UNITY_EDITOR 
//        string url = Path.GetDirectoryName(Application.dataPath) + "/" + DgraphQuery.DQ.activeSessionNumber.ToString() + ".png";
//#endif
        //ScreenCapture.CaptureScreenshot(DgraphQuery.DQ.activeSessionNumber.ToString() + ".png");
        ScreenshotCam.enabled = true;
        RenderTexture screenTexture = new RenderTexture(Screen.width, Screen.height, 16);
        ScreenshotCam.targetTexture = screenTexture;
        RenderTexture.active = screenTexture;
        ScreenshotCam.Render();
        Texture2D renderedTexture = new Texture2D(Screen.width, Screen.height);
        renderedTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        RenderTexture.active = null;
        //byte[] byteArray = renderedTexture.EncodeToPNG();
        //File.WriteAllBytes(url, byteArray);
        var screenshotPose = new Pose(ScreenshotCam.transform.position, ScreenshotCam.transform.rotation);
        ScreenshotCam.enabled = false;


        Debug.Log("Firebase: Uploading Screenshot ...");
        StartCoroutine(ScreenshotHelper.shared.SaveScreenshot(renderedTexture, DgraphQuery.DQ.activeSessionNumber, screenshotPose));


        //StartCoroutine(DgraphQuery.DQ.delayedShare(DgraphQuery.DQ.activeSessionNumber.ToString()));
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        Debug.LogWarning("SZENE GELADEN ------ Deaktiver Startup Cam ");
        SecondScreenStartCam.enabled = false;


        //Debug.Log("Additive Szene Geladen " + scene.name);
        if (scene.buildIndex == 1 && scene.isLoaded)
        {
            Debug.LogError("HARKORSTRASSE GELADN");

            if (!VRMode)
            {

                Debug.Log("Fertig mit dem Laden");
                GameObject[] Hmmm = scene.GetRootGameObjects();
                var _lands = Hmmm[0];
                ScreenTransformGesture _scaleGesture = _lands.AddComponent<ScreenTransformGesture>();
                ScreenTransformGesture _moveGesture = _lands.AddComponent<ScreenTransformGesture>();
                LongPressGesture _longPressGesture = _lands.AddComponent<LongPressGesture>();
                _moveGesture.MinPointers = 1;
                _moveGesture.MaxPointers = 1;
                _scaleGesture.MinPointers = 2;
                _scaleGesture.MaxPointers = 2;
                _longPressGesture.TimeToPress = 1;
                CamController _newCam = _lands.AddComponent<CamController>();
                _newCam.PanFingerMoveGesture = _moveGesture;
                _newCam.RotZoomManipulationGesture = _scaleGesture;
                _newCam.LongPressGestureTerrain = _longPressGesture;
                _newCam.registerGestures();
                _newCam.PanSpeed = 0.1f;
                _newCam.layerToCheck = "Landscape";
                _newCam.RotationSpeed = 1f;
                _newCam.ZoomSpeed = 10f;

                Debug.Log("Fixed");

            }

            if (VRMode)
            {
                TeleportationArea _teleportationArea =
                    scene.GetRootGameObjects()[0].gameObject.AddComponent<TeleportationArea>();
                _teleportationArea.matchDirectionalInput = true;
                _teleportationArea.interactionLayers = InteractionLayerMask.GetMask("Teleport");
            }

            //DgraphQuery.DQ.Subscribe();
        }

    }
    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (VRMode)
        {
            //StopXR();

        }

        //_mrtkInput.MRTKLeftHand.Activate.performed -= LeftHandTriggerPressed;

    }

    public void checkPW()
    {
        if (adminMode)
        {
            pwInput.text = "";
            adminMode = false;
            GameObject[] deletes = GameObject.FindGameObjectsWithTag("Delete");
            foreach (GameObject del in deletes)
            {
                del.GetComponent<Button>().enabled = false;
                Debug.Log("FOUND" + del.gameObject.name);
            }
            GameObject[] toggles = GameObject.FindGameObjectsWithTag("Toggle");
            foreach (GameObject tog in toggles)
            {
                Toggle togg = tog.GetComponent<Toggle>();
                togg.interactable = false;
                Debug.Log("FOUND" + togg.gameObject.name);
                togg.onValueChanged.AddListener(delegate
                {
                    miniTest(togg);

                });
            }
            GameObject[] locks = GameObject.FindGameObjectsWithTag("Locked");
            foreach (GameObject loc in locks)
            {
                Toggle togg = loc.GetComponent<Toggle>();
                togg.interactable = false;
                Debug.Log("FOUND Locked" + togg.gameObject.name);
                togg.onValueChanged.AddListener(delegate
                {
                    miniTestLock(togg);

                });
            }
        }


        else
        {
            if (pwInput.text.IsNullOrEmpty())
            {
                return;
            }

            string _tempPW = pwInput.text;
            if (_tempPW.Equals(passwort))
            {
                adminMode = true;
                Debug.Log("AdminMode Detected");
                GameObject[] deletes = GameObject.FindGameObjectsWithTag("Delete");
                foreach (GameObject del in deletes)
                {
                    del.GetComponent<Button>().enabled = true;
                    Debug.Log("FOUND" + del.gameObject.name);
                }
                GameObject[] toggles = GameObject.FindGameObjectsWithTag("Toggle");
                foreach (GameObject tog in toggles)
                {
                    Toggle togg = tog.GetComponent<Toggle>();
                    togg.interactable = true;
                    Debug.Log("FOUND" + togg.gameObject.name);
                    togg.onValueChanged.AddListener(delegate
                    {
                        miniTest(togg);

                    });
                }
                GameObject[] locks = GameObject.FindGameObjectsWithTag("Locked");
                foreach (GameObject loc in locks)
                {
                    Toggle togg = loc.GetComponent<Toggle>();
                    togg.interactable = true;
                    Debug.Log("FOUND Locked" + togg.gameObject.name);
                    togg.onValueChanged.AddListener(delegate
                    {
                        miniTestLock(togg);

                    });
                }
            }
            else
            {
                Debug.Log("Wrong Password");
            }
        }



    }

    private void miniTest(Toggle togg)
    {
        Debug.Log("togg" + togg.gameObject.name);
        if (togg.isOn)
        {
            idsForMerger.Add(int.Parse(togg.gameObject.name));
        }
        else
        {
            idsForMerger.Remove(int.Parse(togg.gameObject.name));
        }
        Debug.Log(idsForMerger.Count);
        GameObject mergerButton = GameObject.Find("MergerButton");
        if (idsForMerger.Count <= 1)
        {

            mergerButton.GetComponent<Button>().interactable = false;

        }
        else
        {
            mergerButton.GetComponent<Button>().interactable = true;
        }
    }

    private void miniTestLock(Toggle togg)
    {
        Debug.Log("togg" + togg.gameObject.name);
        int sessionnumber = int.Parse(togg.gameObject.name);
        if (togg.isOn)
        {
            DgraphQuery.DQ.lockUnlockSessionByNumber(sessionnumber, true);
        }
        else
        {
            DgraphQuery.DQ.lockUnlockSessionByNumber(sessionnumber, false);
        }

    }


    public void DeleteSessionWithNumber(int session, GameObject Modal)
    {
        DgraphQuery.DQ.deleteSessionByNumber(session);
        Modal.SetActive(false);
        Debug.Log("Modal for Session Deleted");
    }


    public void restartGame()
    {
        Debug.Log("Restarted Game");
        if (_avatarManager.avatars.Count <= 1)
        {
            DgraphQuery.DQ.lockUnlockSessionByNumber(DgraphQuery.DQ.activeSessionNumber, true);
            Debug.Log("Locked");
        }
        SceneManager.LoadScene(0);
        RoomConnected = false;
    }

    public void loadScene(string name)
    {
        SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        EventSessionLoader.SetActive(false);
    }

    public void activateCamFly()
    {

        CamFly.enabled = true;
        CamStreet.enabled = false;

    }

    public void activateCamStreet()
    {

        CamFly.enabled = false;
        CamStreet.enabled = true;

    }

    public void ShowVRMenuToggle()
    {
        VRMenuHolder.SetActive(!VRMenuHolder.activeSelf);
        otherMenu.SetActive(!otherMenu.activeSelf);
    }

    public void ActivateDrawMode()
    {
        Debug.Log("Enabled DrawMode");
        Drawing_Enabled = !Drawing_Enabled;


    }

    public void StartPos()
    {
        if (!VRMode)
        {
            Debug.Log("TELEPORT ME TO 12");
            Pivot.transform.position = startPosTouchDevices;
        }

        if (VRMode)
        {
            GO_XROrigin.transform.position = startPosVR;
            Debug.Log("TELEPORTIERT");
        }

    }

    public void Schnack12()
    {
        if (!VRMode)
        {
            Debug.Log("TELEPORT ME TO 12");
            Pivot.transform.position = pos12;
        }

        if (VRMode)
        {
            GO_XROrigin.transform.position = pos12;
            Debug.Log("TELEPORTIERT");
        }

    }

    public void Schnack28()
    {

        if (!VRMode)
        {
            Debug.Log("TELEPORT ME TO 12");
            Pivot.transform.position = pos28;
            Debug.Log("TELEPORTIERT");


        }

        if (VRMode)
        {
            GO_XROrigin.transform.position = pos28 + new Vector3(0f, 0f, 0f);
            Debug.Log("TELEPORTIERT");

        }
    }

    public void Schnack112()
    {
        if (!VRMode)
        {
            Debug.Log("TELEPORT ME TO 12");
            Pivot.transform.position = pos112;
        }

        if (VRMode)
        {
            GO_XROrigin.transform.position = pos112 + new Vector3(0f, 0f, 0f);
        }
    }

    public void backToZero()
    {

        if (!VRMode)
        {
            Debug.Log("TELEPORT ME TO ZERO");
            Pivot.transform.position = startPosTouchDevices;
            Debug.Log("TELEPORTIERT");


        }

        if (VRMode)
        {
            GO_XROrigin.transform.position = startPosVR + new Vector3(0f, 0f, 0f);
            Debug.Log("TELEPORTIERT");

        }
    }

    public void CamControllerToggle()
    {
        camControllers_Enabled = !camControllers_Enabled;

        if (camControllers_Enabled)
        {
            /*for (int i = 0; i < _CamController.Count; ++i)
            {
                _CamController[i].enabled = true;
            }*/

            _CamController.enabled = true;


        }

        if (!camControllers_Enabled)
        {
            /*for (int i = 0; i < _CamController.Count; ++i)
            {
                _CamController[i].enabled = false;
            }*/

            _CamController.enabled = false;
        }

    }


    public void DrawingModeToggle()
    {
        Drawing_Enabled = !Drawing_Enabled;

        if (!VRMode)
        {

            if (Drawing_Enabled)
            {
                if (Eraser_Enabled)
                {
                    EraserModeToggle();
                }

                if (!AnnotationsVisible)
                {
                    AnnotationVisibleToggle();
                }

                DrawModusBorder.SetActive(true);

                _CamController.enabled = false;
                /*
                for (int i = 0; i < _CamController.Count; ++i)
                {
                    _CamController[i].enabled = false;
                }
                */

            }
            else
            {
                DrawModusBorder.SetActive(false);
                _CamController.enabled = true;

                /*
                for (int i = 0; i < _CamController.Count; ++i)
                {
                    _CamController[i].enabled = true;
                }
                */
            }
        }

        if (VRMode)
        {
            if (Drawing_Enabled)
            {
                if (!AnnotationsVisible)
                {
                    AnnotationVisibleToggle();
                }

                _drawPointer.GetComponent<Renderer>().material.color = Color.cyan;
                _rightControllerPokeInteractorCylinder.GetComponent<MeshRenderer>().enabled = false;
                _rightControllerPokeInteractorPoint.GetComponent<MeshRenderer>().enabled = false;
                _rightControllerPen.GetComponent<MeshRenderer>().enabled = true;
                _rightControllerPen.GetComponent<MeshRenderer>().material.color = Color.yellow;
                _rightControllerRayCaster.SetActive(false);


            }
            else
            {
                _drawPointer.GetComponent<Renderer>().material.color = Color.white;
                _rightControllerPokeInteractorCylinder.GetComponent<MeshRenderer>().enabled = true;
                _rightControllerPokeInteractorPoint.GetComponent<MeshRenderer>().enabled = true;
                _rightControllerPen.GetComponent<MeshRenderer>().enabled = false;
                _rightControllerRayCaster.SetActive(true);
            }
        }

    }

    public void EraserModeToggle()
    {
        Eraser_Enabled = !Eraser_Enabled;

        if (!VRMode)
        {
            if (Eraser_Enabled)
            {
                EraserModusBorder.SetActive(true);

                if (Drawing_Enabled)
                {
                    DrawingModeToggle();
                }
                Debug.Log("Eraser Getoggelt OHNE VR");
            }
            else
            {
                EraserModusBorder.SetActive(false);
            }

        }


    }


    public void loadSceneFromDatabase(int eventNr, bool locked)
    {
        Debug.Log("BUTTON ÜBERGABE " + eventNr);
        DgraphQuery.DQ.loadSession(eventNr);
        loadScene("Harkortstrasse");
        Manager.GameManager.locked = locked;

        //NetworkManager.NetM.ConnectToServer();
        //NetworkManager.NetM.roomId = eventNr;
        DisableStartMenu();

    }


    public void createNewSession()
    {
        if (!_inputSessionName.text.IsNullOrEmpty())
        {
            DgraphQuery.DQ.createNewSession(_inputSessionName.text);
            //MARKS PARKS LADEN
            loadScene("Harkortstrasse");
            newSession = true;
            //NetworkManager.NetM.ConnectToServer();
            //NetworkManager.NetM.roomId = DgraphQuery.DQ.newestSessionNumber;
            DisableStartMenu();
        }

    }

    public void searchSessionByTerm()
    {
        if (!_searchSessionName.text.IsNullOrEmpty())
        {
            foreach (Transform trans in eventSessionLoaderContent.transform)
            {
                Destroy(trans.gameObject);

            }

            DgraphQuery.DQ.createEventSessionLoaderBySearch(_searchSessionName.text);
        }
        else
        {
            foreach (Transform trans in eventSessionLoaderContent.transform)
            {
                Destroy(trans.gameObject);
                DgraphQuery.DQ.createEventSessionLoader();
            }
        }

    }

    public void mergeSessions()
    {


        if (!_inputSessionName.text.IsNullOrEmpty())
        {
            foreach (Transform trans in eventSessionLoaderContent.transform)
            {
                Destroy(trans.gameObject);

            }
            DgraphQuery.DQ.mergeSession(_inputSessionName.text);

        }
        else
        {
            Debug.Log("ICH HABE NOCH KEINEN NAMEN");
        }
    }

    public void FlickMenuUp()
    {
        Debug.Log("Flick Up");
    }

    public void FlickMenuDown()
    {
        Debug.Log("Flick Down");
    }

    public void DisableStartMenu()
    {
        MainDisplayMenu.SetActive(false);
        SecondaryDisplayMenu.SetActive(false);
        EventSessionLoader.SetActive(false);
    }

    /*   private void OnApplicationQuit()
       {

           if (PhotonNetwork.CurrentRoom != null)
           {
               PhotonNetwork.LeaveRoom();

           }
           PhotonNetwork.Disconnect();
           Debug.Log("DISCONNECTED");



       }*/


    public void toggle()
    {
        UIPlacement.SetActive(!UIPlacement.activeSelf);

    }

    public void DrawingEnabled()
    {
        if (!Drawing_Enabled)
        {
            Drawing_Enabled = true;
        }
    }

    public void DrawingDisabled()
    {
        Drawing_Enabled = false;
    }


    IEnumerator StartXR()
    {
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("Initializing XR Failed. Check Editor or Player log for details.");
        }
        else
        {
            Debug.Log("Starting XR...");
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            yield return null;
        }
    }



    public void generateCams()
    {
        Debug.LogWarning("Reached GenerateCams");

        Realtime.InstantiateOptions options = new Realtime.InstantiateOptions();
        options.ownedByClient = false;
        options.preventOwnershipTakeover = false;
        options.useInstance = NetworkManager.NetM._Realtime;
        options.destroyWhenOwnerLeaves = false;
        options.destroyWhenLastClientLeaves = true;


        CamFlyGo = Realtime.Instantiate("CamFlyGo", options);
        CamStreetGo = Realtime.Instantiate("CamStreetGo", options);
        CamStreetGo.GetComponent<RealtimeView>().RequestOwnership();
        CamStreetGo.GetComponent<RealtimeTransform>().RequestOwnership();
        CamFlyGo.GetComponent<RealtimeView>().RequestOwnership();
        CamFlyGo.GetComponent<RealtimeTransform>().RequestOwnership();
        CamStreetGo.transform.position = new Vector3(MRTK_XR_RIG.transform.position.x - 3.281349f, MRTK_XR_RIG.transform.position.y, MRTK_XR_RIG.transform.position.z - 8.04937f);
        CamFlyGo.transform.position = new Vector3(MRTK_XR_RIG.transform.position.x, MRTK_XR_RIG.transform.position.y + 70f, MRTK_XR_RIG.transform.position.z);
        _objectTwoCamera.enabled = true;
        _objectThreeCamera.enabled = true;

    }

    public void AnnotationVisibleToggle()
    {
        AnnotationsVisible = !AnnotationsVisible;


        if (VRMode)
        {
            if (AnnotationsVisible)
            {
                var newMask = Camera.main.cullingMask |= (1 << 11);
                var rayMask = _rightControllerRayCaster.GetComponent<XRRayInteractor>().raycastMask |= (1 << 11);
                Camera.main.cullingMask = newMask;
                _rightControllerRayCaster.GetComponent<XRRayInteractor>().raycastMask = rayMask;

            }

            if (!AnnotationsVisible)
            {
                var newMask = Camera.main.cullingMask & ~(1 << 11);
                var rayMask = _rightControllerRayCaster.GetComponent<XRRayInteractor>().raycastMask & ~(1 << 11);
                Camera.main.cullingMask = newMask;
                _rightControllerRayCaster.GetComponent<XRRayInteractor>().raycastMask = rayMask;

            }

        }

    }

    public void copySession(int everyNumber)
    {
        if (!_inputSessionName.text.IsNullOrEmpty())
        {
            DgraphQuery.DQ.copySession(_inputSessionName.text, everyNumber);
            foreach (Transform trans in eventSessionLoaderContent.transform)
            {
                Destroy(trans.gameObject);
                Debug.Log("what");
            }

        }
    }

    public void OnApplicationQuit()
    {
        Debug.Log("Funktioniere ich auch im Editor?");
        Debug.Log(" COUNT" + _avatarManager.avatars.Count);
        if (_avatarManager.avatars.Count <= 1)
        {
            DgraphQuery.DQ.lockUnlockSessionByNumber(DgraphQuery.DQ.activeSessionNumber, true);
            Debug.Log("Locked");
        }


        if (ARModeIOS)
        {
            StopXR();
        }
    }
}