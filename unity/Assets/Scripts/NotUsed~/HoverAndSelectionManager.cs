using UnityEngine;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

//@author: Anna Wolf, last changed: 09.09.2021 //Markus, last changed: 07.12.2021
/* This scripts manages the outline of objects
 * If they are touched by the raycast, the outline becomes white
 * after confirming the choice the outline color changes to green
 * and the change menü is open
 * 
 */

public class HoverAndSelectionManager : MonoBehaviour, IMixedRealityInputHandler
{
    //public Outline LastSelectedTarget { get => _lastSelectedTarget; set => _lastSelectedTarget = value; }

    //[SerializeField]
    //private Configuration config;

    /// <summary>
    /// grabbables
    /// </summary>
    [SerializeField]
    private LayerMask hoverLayerMask;

    //different Method to handle changeMode
    //private LayerMask noCollisionLayerMask = 0;
    //Debug.Log("noCollisionLayerMask: " + Convert.ToString(noCollisionLayerMask, 2).PadLeft(32, '0'));

    //private CubeManipulation _cubeManipulation;
    private LaserPointerCustom _laserPointerCustom;

    private Outline _lastOutlinedTarget = null;
    private Outline _lastSelectedTarget = null;
    private bool isControllerJustPressed = false;
    private bool isControllerHold = false;

    private void Awake()
    {
        //_cubeManipulation = CubeManipulation.Instance;
    /*    _laserPointerCustom = LaserPointerCustom.Instance;

        //if (_cubeManipulation == null)
        //    Debug.LogError("The CubeManipulation instance isn`t existing yet or will never");

        if (_laserPointerCustom == null)
            Debug.LogError("The LaserPointerCustom instance isn`t existing yet or will never");
            */
    }

    private void OnEnable()
    {
    /*
        //_cubeManipulation.AddListenerToManipulationEnded(OnManipulationEnded);

        //GlobalEventsManager.AddListenerToSimulationModeChanged(OnSimulationModeChanged);

        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler>(this);
      */
    }

    //private void OnSimulationModeChanged(SimulationMode newSimulationMode, SimulationMode lastSimulationMode)
    //{
    //    if(lastSimulationMode == SimulationMode.CHANGE && newSimulationMode != SimulationMode.CHANGE)
    //        transformManipulationPanel.StopManipulation();
    //}

    private void OnDisable()
    {
        //_cubeManipulation.RemoveListenerToManipulationEnded(OnManipulationEnded);
        //GlobalEventsManager.RemoveListenerSimulationModeChanged(OnSimulationModeChanged);

        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler>(this);
    }

    void Update()
    {
        //allows only 1 selection //resets via callback: transformManipulationPanel.AddListenerToManipulationEnded(OnManipulationEnded); 
         if (_lastSelectedTarget != null)
        {
            if (isControllerJustPressed)
                isControllerJustPressed = false;
            
            return;
        }

        //if (config.CurrentSimulationMode != SimulationMode.CHANGE)
        //    return;

        if (_laserPointerCustom.TryGetFirstLaserPointerRaycastCollision(out Outline target, hoverLayerMask))
        {
            if (_lastOutlinedTarget != target)
            {
                if (_lastOutlinedTarget != null)
                    OnHoverExit(_lastOutlinedTarget);

                OnHoverEnter(target);
                _lastOutlinedTarget = target;

                if (isControllerJustPressed)
                    isControllerJustPressed = false;
            }
        }
        else
        {
            if (_lastOutlinedTarget != null)
            {
                OnHoverExit(_lastOutlinedTarget);
                _lastOutlinedTarget = null;

                if (isControllerJustPressed)
                    isControllerJustPressed = false;
            }
        }

        if(_lastOutlinedTarget != null)
        {
            if (isControllerJustPressed)
            {
                isControllerJustPressed = false;

                //TODO: should not be necessary if the target has no collider anymore anyways!
                /*if (!_lastOutlinedTarget.GetComponent<Placeable>().IsSelectedForManipulationSequence)
                {
                    //if panel is active because of spawning!
                    //if (_cubeManipulation.TransformToManipulate != null)
                      //  return;

                    //fixes that you can select and be stuck in an placed object from beginning which you are not allowed to manipulate
                    if (!_lastOutlinedTarget.GetComponent<Placeable>().isActiveAndEnabled)
                        return;

                    OnSelected(_lastOutlinedTarget);
                    //requires this component!
                    _lastOutlinedTarget.GetComponent<Placeable>().InitPlaceableSequence();
                    

                    //@ANNA
                    //START SYNC
                    //_lastOutlinedTarget.gameObject.GetComponent<PhotonView>().enabled = true; 

                    //design choice
                    //transformManipulationPanel.SetManipulationPanel(_lastOutlinedTarget.GetComponent<Placeable>());
                }*/
            }
        }
    }

    private void OnManipulationEnded()
    {
        ResetTarget();
    }

    private void ResetTarget()
    {
        //@ANNA
        //END SYNC
        //_lastOutlinedTarget.gameObject.GetComponent<PhotonView>().enabled = false;
        _lastSelectedTarget = null;
    }

    public void OnHoverEnter(Outline outline)
    {
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = Color.white;
        outline.OutlineWidth = 2f;
        outline.enabled = true;
    }

    public void OnHoverExit(Outline outline)
    {
        outline.enabled = false;
    }

    public void OnSelected(Outline outline)
    {
        //takeover the owenership of the photon object
        PhotonView photonView = outline.GetComponentInParent<PhotonView>();
        if (photonView.IsMine != true)
        {
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
        }
        
        //change outline
        outline.enabled = true; 
        outline.OutlineColor = Color.green;
        _lastSelectedTarget = outline;
    }

    public void OnInputDown(InputEventData eventData)
    {
        //this input action is the input action for controller click :) - found out by breakpoint at this event - if it some day changes
        if (eventData.Handedness == Handedness.Right && eventData.InputSource.SourceType == InputSourceType.Controller && eventData.MixedRealityInputAction.Id == 1)
        {
            isControllerJustPressed = true;
            isControllerHold = true;
            //Debug.Log("Triggered OnInputDown");
        }
    }

    public void OnInputUp(InputEventData eventData)
    {
        //this input action is the input action for controller click :) - found out by breakpoint at this event - if it some day changes
        if (eventData.Handedness == Handedness.Right && eventData.InputSource.SourceType == InputSourceType.Controller && eventData.MixedRealityInputAction.Id == 1)
        { 
            isControllerHold = false;
            //Debug.Log("Triggered OnInputUp");
        }
    }
}