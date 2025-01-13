using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScaleManipulationPanelOld : MonoBehaviour
{
    /*public Transform TransformToManipulate { get => transformToManipulate; set => transformToManipulate = value; }

    public delegate void ManipulationEnded();

    [SerializeField]
    private AnimationCurve animationCurve = null;

    [SerializeField]
    private Button plusAllAxisButton = null;

    [SerializeField]
    private Button minusAllAxisButton = null;

    [SerializeField]
    private Button plusXAxisButton = null;
    [SerializeField]
    private Button minusXAxisButton = null;

    [SerializeField]
    private Button plusYAxisButton = null;
    [SerializeField]
    private Button minusYAxisButton = null;

    [SerializeField]
    private Button plusZAxisButton = null;
    [SerializeField]
    private Button minusZAxisButton = null;

    [SerializeField]
    private Button saveAndExitButton = null;

    [SerializeField]
    private Button deleteAndExitButton = null;

    private Transform transformToManipulate = null;
    private event ManipulationEnded manipulationEndedEvent;

    private void Awake()
    {
        saveAndExitButton.onClick.AddListener(OnStopManipulation);
        deleteAndExitButton.onClick.AddListener(OnDelete);

        //X-AXIS-Button
        EventTrigger minusXAxisButtonEventTrigger = minusXAxisButton.GetComponent<EventTrigger>();
        AddEventTriggerListener(minusXAxisButtonEventTrigger, EventTriggerType.PointerDown, function => { OnPointerDown(Vector3.left); }); //Vector3 direction hier direkt rein + rest 0,0 --> damit multiply! nurnoch ob translation oder rot oder 
        AddEventTriggerListener(minusXAxisButtonEventTrigger, EventTriggerType.PointerUp, function => { OnPointerUp(); });

        EventTrigger plusXAxisButtonEventTrigger = plusXAxisButton.GetComponent<EventTrigger>();
        AddEventTriggerListener(plusXAxisButtonEventTrigger, EventTriggerType.PointerDown, function => { OnPointerDown(Vector3.right); });
        AddEventTriggerListener(plusXAxisButtonEventTrigger, EventTriggerType.PointerUp, function => { OnPointerUp(); });

        //Y-AXIS-Button
        EventTrigger minusYAxisButtonEventTrigger = minusYAxisButton.GetComponent<EventTrigger>();
        AddEventTriggerListener(minusYAxisButtonEventTrigger, EventTriggerType.PointerDown, function => { OnPointerDown(Vector3.down); });
        AddEventTriggerListener(minusYAxisButtonEventTrigger, EventTriggerType.PointerUp, function => { OnPointerUp(); });

        EventTrigger plusYAxisButtonEventTrigger = plusYAxisButton.GetComponent<EventTrigger>();
        AddEventTriggerListener(plusYAxisButtonEventTrigger, EventTriggerType.PointerDown, function => { OnPointerDown(Vector3.up); });
        AddEventTriggerListener(plusYAxisButtonEventTrigger, EventTriggerType.PointerUp, function => { OnPointerUp(); });

        //Z-AXIS-Button
        EventTrigger minusZAxisButtonEventTrigger = minusZAxisButton.GetComponent<EventTrigger>();
        AddEventTriggerListener(minusZAxisButtonEventTrigger, EventTriggerType.PointerDown, function => { OnPointerDown(Vector3.back); });
        AddEventTriggerListener(minusZAxisButtonEventTrigger, EventTriggerType.PointerUp, function => { OnPointerUp(); });

        EventTrigger plusZAxisButtonEventTrigger = plusZAxisButton.GetComponent<EventTrigger>();
        AddEventTriggerListener(plusZAxisButtonEventTrigger, EventTriggerType.PointerDown, function => { OnPointerDown(Vector3.forward); });
        AddEventTriggerListener(plusZAxisButtonEventTrigger, EventTriggerType.PointerUp, function => { OnPointerUp(); });

        //ALL-AXIS-Button
        EventTrigger minusAllAxisButtonEventTrigger = minusAllAxisButton.GetComponent<EventTrigger>();
        AddEventTriggerListener(minusAllAxisButtonEventTrigger, EventTriggerType.PointerDown, function => { OnPointerDown(Vector3.one * -1); });
        AddEventTriggerListener(minusAllAxisButtonEventTrigger, EventTriggerType.PointerUp, function => { OnPointerUp(); });

        EventTrigger plusAllAxisButtonEventTrigger = plusAllAxisButton.GetComponent<EventTrigger>();
        AddEventTriggerListener(plusAllAxisButtonEventTrigger, EventTriggerType.PointerDown, function => { OnPointerDown(Vector3.one); });
        AddEventTriggerListener(plusAllAxisButtonEventTrigger, EventTriggerType.PointerUp, function => { OnPointerUp(); });
    }

    private void OnDelete()
    {
        //@ ANNA
        //TODO: 
        //PhotonNetwork.Destroy()
        
        transformToManipulate.GetComponent<DatabaseID>().DeleteObject();
        
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

    internal void OnStopManipulation()
    {
        DgraphQuery.DQ.updatePlacedObjects(transformToManipulate.GetComponent<DatabaseID>().id, transformToManipulate.position.x, transformToManipulate.position.y, transformToManipulate.position.z, transformToManipulate.rotation.eulerAngles.x, transformToManipulate.rotation.eulerAngles.y, transformToManipulate.rotation.eulerAngles.z, transformToManipulate.localScale.x, transformToManipulate.localScale.y, transformToManipulate.localScale.z);
        gameObject.SetActive(false);

        if (transformToManipulate != null)
        {
            transformToManipulate.GetComponent<Placeable>().IsChangedViaCanvas = false;
            transformToManipulate.GetComponent<Outline>().enabled = false;
            transformToManipulate = null;
        }
        
       

        //to reset hover managers selectionTarget
        manipulationEndedEvent?.Invoke();
    }

    public void SetManipulationPanel(Placeable placeable)
    {
        placeable.IsChangedViaCanvas = true;
        transformToManipulate = placeable.transform;
    }

    internal void SetManipulationPanelActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    private void AddEventTriggerListener(EventTrigger trigger, EventTriggerType eventType, System.Action<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener(new UnityEngine.Events.UnityAction<BaseEventData>(callback));
        trigger.triggers.Add(entry);
    }

    private void OnPointerDown(Vector3 axis)
    {
        StartCoroutine(ChangeTransformValue(axis));
    }

    private void OnPointerUp()
    {
        StopAllCoroutines();
    }

    private IEnumerator ChangeTransformValue(Vector3 axis)
    {
        float timeRunning = 0f;

        while (true)
        {
            timeRunning += Time.deltaTime;
            //animation curve values takes 2 sec to be on fullspeed
            float value = Time.deltaTime * animationCurve.Evaluate(timeRunning / 2);

            transformToManipulate.localScale = transformToManipulate.localScale + axis * value / 2;

            yield return null;
        }
    }*/
}
