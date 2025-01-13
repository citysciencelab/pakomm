using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TransformManipulationPanel : MonoBehaviour
{
    public Transform TransformToManipulate { get => transformToManipulate; set => transformToManipulate = value; }

    public delegate void ManipulationEnded();

    [SerializeField]
    private float distFromFace = 4f;

    [SerializeField]
    private Canvas parentCanvas = null;

    [SerializeField]
    private AnimationCurve animationCurve = null;

    [SerializeField]
    private Toggle translationToggle = null;

    [SerializeField]
    private Toggle rotationToggle = null;

    [SerializeField]
    private Toggle scaleToggle = null;

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

    private Transform transformToManipulate = null;
    private event ManipulationEnded manipulationEndedEvent;

    private void Awake()
    {
        saveAndExitButton.onClick.AddListener(StopManipulation);

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
    }

    public void AddListenerToManipulationEnded(ManipulationEnded call)
    {
        manipulationEndedEvent += call;
    }

    public void RemoveListenerToManipulationEnded(ManipulationEnded call)
    {
        manipulationEndedEvent += call;
    }

    internal void StopManipulation()
    {
        gameObject.SetActive(false);

        if (transformToManipulate != null)
        {
            //transformToManipulate.GetComponent<Placeable>().IsSelectedForManipulationSequence = false;
            //transformToManipulate.GetComponent<Outline>().enabled = false;

            //TODO:
            //should be pooled in the future //on the gameobject// if it stays via design choices
            Destroy(transformToManipulate.gameObject.GetComponent<ShowTransformAxis>());
        }

        //to reset hover managers selectionTarget
        manipulationEndedEvent?.Invoke();
    }

    internal void SetManipulationPanel(Placeable placeable)
    {
        //placeable.IsSelectedForManipulationSequence = true;
        transformToManipulate = placeable.transform;
        gameObject.SetActive(true);

        //Place infront of User
        Vector3 dir = transformToManipulate.position - Camera.main.transform.position;
        dir.Normalize();
        transform.position = Camera.main.transform.position + dir * distFromFace; //offset nach unten? als serializefield

        //show XYZ Axis
        //TODO:
        //should be pooled in the future //on the gameobject// if it stays via design choices
        transformToManipulate.gameObject.AddComponent<ShowTransformAxis>();
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
            float value = Time.deltaTime * animationCurve.Evaluate(timeRunning/2);
            
            //tanslation values are doubled
            if(translationToggle.isOn)
                transformToManipulate.Translate(axis * value*2, Space.Self);

            //rotation values are trippled
            if (rotationToggle.isOn)
                transformToManipulate.Rotate(axis * value*30, Space.Self);

            //scaling speed is halfed for better controll
            if (scaleToggle.isOn)
                transformToManipulate.localScale = transformToManipulate.localScale + axis * value/2;

            yield return null;
        }
    }

    private void Update()
    {
        parentCanvas.transform.LookAt(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z));
        parentCanvas.transform.Rotate(Vector3.up, 180f);
    }
}
