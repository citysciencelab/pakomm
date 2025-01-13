using Normal.Realtime;
using Normal.Realtime.Serialization;
using UnityEngine;

[RealtimeModel]
public partial class DatabaseAnnotationModel
{
    [RealtimeProperty(1, true, true)]
    private string _id;

    [RealtimeProperty(2, true, true)]
    private string _content;
    
    [RealtimeProperty(3, true, true)]
    private int _voteUp;
    [RealtimeProperty(4, true, true)]
    private int _voteDown; 
    [RealtimeProperty(5, true, true)]
    private string _parentObjectId;
}


