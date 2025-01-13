using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;


public class ThumbstickChecker : InputSystemGlobalHandlerListener, IMixedRealityInputHandler<Vector2>
{
    public MixedRealityInputAction moveAction;
    public float speed = 1.0f;

    public void OnInputChanged(InputEventData<Vector2> eventData)
    {
        if (eventData.MixedRealityInputAction == moveAction)
        {
            Vector3 localDelta = speed * (Vector3)eventData.InputData;
            transform.position = transform.position + transform.rotation * localDelta;
        }
    }

    protected override void RegisterHandlers()
    {
        throw new System.NotImplementedException();
    }

    protected override void UnregisterHandlers()
    {
        throw new System.NotImplementedException();
    }
}

    
