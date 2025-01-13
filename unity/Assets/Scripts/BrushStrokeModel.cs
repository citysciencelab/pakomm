using UnityEngine;
using Normal.Realtime.Serialization;
using Normal.Realtime;

[RealtimeModel]
public partial class BrushStrokeModel {
    [RealtimeProperty(1, true)]
    private RealtimeArray<RibbonPointModel> _ribbonPoints;

    [RealtimeProperty(2, true)]
    private Vector3 _brushTipPosition;

    [RealtimeProperty(3, true)]
    private Quaternion _brushTipRotation;

    [RealtimeProperty(4, true)]
    private bool _brushStrokeFinalized;

    [RealtimeProperty(5, true)] private float _brushStrokeWidh;
    [RealtimeProperty(6, true)] private Color _brushStrokeColor;
    
}


