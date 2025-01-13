using Normal.Realtime;
using Normal.Realtime.Serialization;
using UnityEngine;

[RealtimeModel]
public partial class DatabaseNormalId
{
    [RealtimeProperty(1, true, true)]
    private string _id;
    [RealtimeProperty(2, true, true)]
    private bool _marker;
}


