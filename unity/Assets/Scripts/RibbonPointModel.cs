using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using Normal.Realtime.Serialization;


[RealtimeModel]
public partial class RibbonPointModel {
    [RealtimeProperty(1, true)]
    private Vector3    _position;

    [RealtimeProperty(2, true)]
    private Quaternion _rotation = Quaternion.identity;
}


